using MindWidgetA;
using System;
using System.Collections.Generic;
using System.Timers;
using Android.App;
using MindWidgetA.Tooling;
using MindWatchA;
using Selftastic_WS_Test.Models.Collections;
using Selftastic_WS_Test.Models.Single;
using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Enums;
using System.IO;
using System.Text.Json;
using System.Text;
using Android.OS;
using Java.Interop;
using Android.Appwidget;
using MindWidgetA.UI;
using Android.Content;
using Android.Widget;
using MindWatchA.Models.Collections;
using MindWatchA.StateMachine;
using MindWatchA.Tooling;

namespace MindWidgetA.StateMachine
{
    

    public class StateMachine
    {
        public class OnAlarmListenerImpl : Java.Lang.Object, AlarmManager.IOnAlarmListener
        {
            public OnAlarmListenerImpl(): base()
            {

            }

            public void OnAlarm()
            {
                Instance.onAlarm();
            }
        }

        private static StateMachine Instance { get; set; }
        private static Timer timer;
        private static States LastQuestionTaskState = States.TASK;
        private const int DoNotChangeTimer = -1;
        private const int UseLastAffirmationDuration = -1;
        private const int UseChosenDuration = -2;

        public States CurrentStateProxy { get
            {
                return CurrentState;
            }
            set
            {
                CurrentState = value;
            }
        }

        public DateTime TimerStopsAtProxy
        {
            get
            {
                return timerStopsAt;
            }
            set
            {
                timerStopsAt = value;
            }
        }

        public States LastQuestionTaskStateProxy
        {
            get
            {
                return LastQuestionTaskState;
            }

            set
            {
                LastQuestionTaskState = value;
            }
        }

        public static Context Context = Application.Context;

        public  static States CurrentState { get; set; }
        public  static StateMap StateMap;
        private static AbstractUI UI { get; set; }
        public  static RemoteViews RemoteViews { get; set; }
        private static int lastAffirmationDuration;
        private static bool laterChosen = false;
        private static int currentGreetingsTimerDuration;
        private static DateTime nextGreetingAt;
        private static DateTime timerStopsAt;
        private static Timer greetingsTimer;
        private static Vibrator vibrator;
        private static AlarmManager alarmManager;
        private AlarmManager greetingsAlarmManager;
        private StateTransitions GreetingsStateTransitions = new StateTransitions(States.GREETINGS).
            Add(new Transition(States.AFFIRMATION, Events.HappyButtonPressed, () => { return true; }, () => { Statistics.IncrementMindState(Events.HappyButtonPressed); sendAnswerAsync(MoodAnswerKind.good); setAffirmationState(TimeConstants.DURATION_GOOD); })).
            Add(new Transition(States.AFFIRMATION, Events.NeutralButtonPressed, () => { return true; }, () => { Statistics.IncrementMindState(Events.NeutralButtonPressed); sendAnswerAsync(MoodAnswerKind.neutral); setAffirmationState(TimeConstants.DURATION_MIDDLE); })).
            Add(new Transition(States.AFFIRMATION, Events.SadButtonPressed, () => { return true; }, () => { Statistics.IncrementMindState(Events.SadButtonPressed); sendAnswerAsync(MoodAnswerKind.bad); setAffirmationState(TimeConstants.DURATION_BAD); })).
            Add(new Transition(States.NOT_LOGGED_IN, Events.NotLoggedIn, () => { return true; }, logout)).
            Add(new Transition(States.NOT_LOGGED_IN, Events.LogoutButtonPressed, () => { return true; }, logout)).
            Add(new Transition(States.GREETINGS, Events.MiddayReached, () => { return false; }, () => { setGreetingsState(); })); 
        private StateTransitions AffirmationStateTransistions = new StateTransitions(States.AFFIRMATION).
            Add(new Transition(States.QUESTION, Events.TimeEllapsed, () => { return LastQuestionTaskState == States.TASK; }, () => { setQuestionState(); })).
            Add(new Transition(States.TASK, Events.TimeEllapsed, () => { return LastQuestionTaskState == States.QUESTION; }, () => { setTaskState(); })).
            Add(new Transition(States.INFO, Events.InfoButtonPressed, () => { return true; }, () => { setInfoState(); })).
            Add(new Transition(States.GREETINGS, Events.MiddayReached, () => { return true; }, () => { setGreetingsState(); })) ;
        private StateTransitions TaskStateTransistions = new StateTransitions(States.TASK).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => { return true; }, () => { setTaskAnswer(true); })).
            Add(new Transition(States.AFFIRMATION, Events.NoButtonPressed, () => { return true; }, () => { setTaskAnswer(false); })).
            Add(new Transition(States.AFFIRMATION, Events.ChooseLaterPressed, () => { return true; }, saveTaskQuestions)).
            Add(new Transition(States.GREETINGS, Events.MiddayReached, () => { return true; }, () => { setGreetingsState(); }));
        private StateTransitions QuestionStateTransitions = new StateTransitions(States.QUESTION).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => { return true; }, () => { setQuestionAnswer(true); })).
            Add(new Transition(States.AFFIRMATION, Events.NoButtonPressed, () => { return true; }, () => { setQuestionAnswer(false); })).
            Add(new Transition(States.AFFIRMATION, Events.ChooseLaterPressed, () => { return true; }, saveTaskQuestions)).
            Add(new Transition(States.GREETINGS, Events.MiddayReached, () => { return true; }, () => { setGreetingsState(); }));
        private StateTransitions InfoStateTransisions = new StateTransitions(States.INFO).
            Add(new Transition(States.AFFIRMATION, Events.BackButtonPressed, () => { return true; }, () => { setAffirmationState(DoNotChangeTimer); })).
            Add(new Transition(States.QUESTION, Events.TimeEllapsed, () => LastQuestionTaskState == States.TASK /* ^ laterChosen*/, () => { setQuestionState(); })).
            Add(new Transition(States.TASK, Events.TimeEllapsed, () => LastQuestionTaskState == States.QUESTION /*^ laterChosen*/, () => { setTaskState(); })).
            Add(new Transition(States.INFO, Events.SyncButtonPressed, () => true, synchronize)).
            Add(new Transition(States.NONE, Events.LogoutButtonPressed, () => true, logout));
        private StateTransitions LaterChosenTransisions = new StateTransitions(States.CHOOSE_LATER).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => true, () => setAffirmationState(UseChosenDuration))).
            Add(new Transition(States.QUESTION, Events.NoButtonPressed, () => LastQuestionTaskState == States.QUESTION, () => setQuestionState())).
            Add(new Transition(States.TASK, Events.NoButtonPressed, () => LastQuestionTaskState == States.TASK, () => setTaskState()));
        private StateTransitions NotLoggedInStateTransitions = new StateTransitions(States.NOT_LOGGED_IN).
            Add(new Transition(States.GREETINGS, Events.LoggedIn, () => true, login)).
            Add(new Transition(States.NOT_LOGGED_IN, Events.LogoutButtonPressed, () => { return true; }, logout)).
            Add(new Transition(States.NOT_LOGGED_IN, Events.MiddayReached, () => { return false; }, () => { }));


        public StateMachine() { Instance = this; }

        public StateMachine(AbstractUI ui, RemoteViews remoteViews): this()
        {
            CurrentState = States.GREETINGS;
            createTimers();
            InitStateMap();
            UI = ui;
            RemoteViews = remoteViews;
            vibrator = (Vibrator) Context.GetSystemService(Android.Content.Context.VibratorService);
            // activityManager = (ActivityManager) Context.GetSystemService(Android.Content.Context.ActivityService);
        }

        private bool True()
        {
            return true;
        }

        private static void vibrate()
        {
            
            vibrator.Vibrate(500);
        }

        private void InitStateMap()
        {
            StateMap = new StateMap();
            StateMap.Add(GreetingsStateTransitions);
            StateMap.Add(AffirmationStateTransistions);
            StateMap.Add(QuestionStateTransitions);
            StateMap.Add(TaskStateTransistions);
            StateMap.Add(InfoStateTransisions);
            StateMap.Add(LaterChosenTransisions);
            StateMap.Add(NotLoggedInStateTransitions);
        }


        //TODO Probably implement greetings-alarm for a specified time.
        //TODO Store Settings in (static) memory 
        private void createTimers()
        {
            timer = new Timer();
            timer.Elapsed += (object o, ElapsedEventArgs a) => { Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => PushEvent(Events.TimeEllapsed)); timer.Interval = 0; };
            greetingsTimer = new Timer();
            greetingsTimer.Elapsed += (object sender, ElapsedEventArgs a) => { Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => setGreetingsState()); greetingsTimer.Interval = 0; };
            alarmManager = (AlarmManager) Context.GetSystemService(Context.AlarmService);
            greetingsAlarmManager = (AlarmManager)Context.GetSystemService(Context.AlarmService);
            var greetingIntent = new Intent(Context, typeof(AlarmReceiver));
            var alarmIntent = PendingIntent.GetBroadcast(Context, 0, greetingIntent, 0);
            greetingsAlarmManager.SetInexactRepeating(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + 10000, 60 * 1000, alarmIntent);
        }

        private static void setTimer(int seconds)
        {
            currentGreetingsTimerDuration = seconds;
            timerStopsAt = DateTime.Now + new TimeSpan(((long)seconds) * 10000000L);
            if (timer.Interval > 0)
            {
                timer.Stop();
            }
            /*
            timer.AutoReset = false;
            timer.Interval = seconds * 1000;
            timer.Start();
            */
            alarmManager.Set(AlarmType.RtcWakeup, seconds * 1000, "alarm", new OnAlarmListenerImpl(), null);
        }

        private void onAlarm()
        {
            PowerManager pm = (PowerManager)Context.GetSystemService(Android.Content.Context.PowerService);
            PowerManager.WakeLock wl = pm.NewWakeLock(WakeLockFlags.ScreenDim 
                                                 | WakeLockFlags.OnAfterRelease,
                                                 "wakeup");
            wl.Acquire();
            // ... do work...
            //show toask
       
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => Instance.PushEvent(Events.TimeEllapsed));
            wl.Release();
        }

        private static void setGreetingsTimer(int seconds)
        {
            currentGreetingsTimerDuration = seconds;
            nextGreetingAt = DateTime.Now + new TimeSpan(((long)seconds) * 10000000L);
            if (greetingsTimer.Interval > 0)
            {
                greetingsTimer.Stop();
            }
            greetingsTimer.AutoReset = false;
            greetingsTimer.Interval = seconds * 1000;
            greetingsTimer.Start();
        }

        private static void sendAnswerAsync(MoodAnswerKind answer)
        {
            System.Threading.Thread t = new System.Threading.Thread(
                async () => await ApiCall.Instance.SendAnswer(Mood.Default, answer));
            t.Start();
        }

        // private static int chosenLaterDuration => (int)(UI.LaterTimePicker.ChosenTime - DateTime.Now).TotalSeconds;

        private static void setGreetingsState()
        {
            UI.HappyButton.Visibility = Android.Views.ViewStates.Visible;
            UI.NeutralButton.Visibility = Android.Views.ViewStates.Visible;
            UI.SadButton.Visibility = Android.Views.ViewStates.Visible;
            UI.BackButton.Visibility = Android.Views.ViewStates.Gone;
            UI.InfoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LaterButton.Visibility = Android.Views.ViewStates.Gone;
            UI.PointsAmountText.Visibility = Android.Views.ViewStates.Gone;
            UI.MoneyIcon.Visibility = Android.Views.ViewStates.Gone;
            UI.Background.SetImageResource(Resource.Drawable.mindset_background);
            UI.MainText.Text = User.Instance.fresh ? "Bitte vor Benutzung anmelden" : "Guten Morgen\n\nWie fühlst Du dich heute, mein lieber";
            sendAnswerAsync(Selftastic_WS_Test.Enums.MoodAnswerKind.shown);
            setGreetingsTimer(TimeConstants.SecondsToNextGreeting);
            Console.WriteLine("SetGreetingsState finished");
        }

        private static void setAffirmationState(int seconds) 
        {
            if (seconds > 0)
            {
                vibrate();
                setTimer(seconds);
                lastAffirmationDuration = seconds;
            } else
            {
                if (seconds == UseLastAffirmationDuration)
                // if (seconds == UseChosenDuration)
                {
                    
                    vibrate();
                    setTimer(lastAffirmationDuration);
                    laterChosen = true;
                }
            }

            UI.HappyButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NeutralButton.Visibility = Android.Views.ViewStates.Gone;
            UI.SadButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.BackButton.Visibility = Android.Views.ViewStates.Gone;
            // UI.InfoButton.Visibility = Android.Views.ViewStates.Visible;
            UI.MainText.Visibility = Android.Views.ViewStates.Visible;
            UI.Background.SetImageResource(Resource.Drawable.affirmation_background);
            UI.MainText.Text = Affirmations.Instance.NewRandom.Output;
            UI.SyncButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LogoutButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LaterButton.Visibility = Android.Views.ViewStates.Gone;
            UI.PointsAmountText.Visibility = Android.Views.ViewStates.Gone;
            UI.MoneyIcon.Visibility = Android.Views.ViewStates.Gone;
            Affirmations.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.shown);
        }

        private static void setQuestionTaskState(string random = null, int backgroundPicId = -1)
        {
            UI.InfoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Visible;
            UI.NoButton.Visibility = Android.Views.ViewStates.Visible;
            UI.LaterButton.Visibility = Android.Views.ViewStates.Visible;
            UI.PointsAmountText.Visibility = Android.Views.ViewStates.Visible;
            UI.MoneyIcon.Visibility = Android.Views.ViewStates.Visible;
            UI.BackButton.Visibility = Android.Views.ViewStates.Gone;
            UI.SyncButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LogoutButton.Visibility = Android.Views.ViewStates.Gone;
            // UI.backgroundImageView.SetImageResource(backgroundPicId);
            if (random != null)
            {
                UI.MainText.Text = random;
            }
            UI.PointsAmountText.Text = Statistics.Points.ToString("#,##0");
            laterChosen = false;
        }

        private static void setQuestionState()
        {
            vibrate();
            var question = LastQuestionTaskState == States.QUESTION ? UI.MainText.Text : Questions.Instance.NewRandom.Output;
            setQuestionTaskState(question, Resource.Drawable.questions);
            LastQuestionTaskState = States.QUESTION;
            UI.Background.SetImageResource(Resource.Drawable.question_backgoune);
            Questions.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.shown);
        }

        private static void setTaskState()
        {
            vibrate();
            var task = LastQuestionTaskState == States.TASK ? UI.MainText.Text : Tasks.Instance.NewRandom.Output;
            setQuestionTaskState(task, Resource.Drawable.tasks);
            LastQuestionTaskState = States.TASK;
            UI.Background.SetImageResource(Resource.Drawable.aufgabe_hintergrund);
            Tasks.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.shown);
        }

        private static void setInfoState()
        {
            UI.MainText.Text = Statistics.Global.InfoText;
            UI.InfoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.BackButton.Visibility = Android.Views.ViewStates.Visible;
            UI.SyncButton.Visibility = Android.Views.ViewStates.Visible;
            UI.LogoutButton.Visibility = Android.Views.ViewStates.Visible;

            UI.Background.SetImageResource(Resource.Drawable.hg4);
        }

        private static void setTaskAnswer(bool answer)
        {
            Statistics.IncrementTask(answer);
            setAffirmationState(currentGreetingsTimerDuration);
            Tasks.Instance.SendAnswer(answer ? Selftastic_WS_Test.Enums.AnswerKind.accepted : Selftastic_WS_Test.Enums.AnswerKind.declined);
        }

        private static void setQuestionAnswer(bool answer)
        {
            Statistics.IncrementQuestion(answer);
            setAffirmationState(currentGreetingsTimerDuration);
            Questions.Instance.SendAnswer(answer ? Selftastic_WS_Test.Enums.AnswerKind.accepted : Selftastic_WS_Test.Enums.AnswerKind.declined);
        }

        private static void saveTaskQuestions()
        {
            if (LastQuestionTaskState == States.QUESTION)
            {
                SavedTaskQuestions.Instance.Add(Questions.Instance.Last);
                SavedTaskQuestions.Instance.Persist();
                Statistics.IncrementLater(true);
                Questions.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.later);
            } else
            {
                SavedTaskQuestions.Instance.Add(Tasks.Instance.Last);
                SavedTaskQuestions.Instance.Persist();
                Statistics.IncrementLater(false);
                Tasks.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.later);
            }
           
            setAffirmationState(UseLastAffirmationDuration);
        }

        private static void synchronize()
        {

        }

        private static void login()
        {
            User.Instance.Reload();
            CurrentState = States.GREETINGS;
            Instance.Persist();
            setGreetingsState();
        }

        private static void logout()
        {
            CurrentState = States.NOT_LOGGED_IN;
            Instance.Persist();
            var prefix = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            File.Delete(Path.Combine(prefix, "user.json"));
            File.Delete(Path.Combine(prefix, "task.json"));
            File.Delete(Path.Combine(prefix, "question.json"));
            File.Delete(Path.Combine(prefix, "affirmation.json"));
            File.Delete(Path.Combine(prefix, "statemachine.json"));
            User.Instance.Reload();
            Statistics.Clear();
            if (MainActivity.Instance != null)
            {
                MainActivity.Instance?.FinishAndRemoveTask();
            }
            if (MainActivity.PreviousInstance != null)
            {
                MainActivity.PreviousInstance.FinishAndRemoveTask();
            }
            UI.MainText.Text = "Benutzer nicht eingeloggt.\nBitte in der App einloggen.";
            UI.HappyButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NeutralButton.Visibility = Android.Views.ViewStates.Gone;
            UI.SadButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NoButton.Visibility = Android.Views.ViewStates.Gone;
            /*
            var appWidgetManager = AppWidgetManager.GetInstance(Context.ApplicationContext);
            ComponentName widget = new ComponentName(Context, Java.Lang.Class.FromType(typeof(MainWidget)));
            appWidgetManager.UpdateAppWidget(widget, RemoteViews);
            var ids = appWidgetManager.GetAppWidgetIds(widget);
            var updateIntent = new Intent();
            updateIntent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            updateIntent.PutExtra("IDS", ids);
            updateIntent.PutExtra("DATA", "");
            Context.SendBroadcast(updateIntent);*/
        }

        public void setState(States newState)
        {
            switch (newState)
            {
                case States.GREETINGS: setGreetingsState(); break;
                case States.AFFIRMATION: setAffirmationState(UseChosenDuration); break;
                case States.QUESTION:
                case States.TASK: setQuestionTaskState(); break;
            }
        }

        private void logIn()
        {
            if (User.Instance.fresh)
            {
                logout();
                return;
            }
            setGreetingsState();
        }

        public void PushEvent(Events _event)
        {
            if (_event == Events.LoggedIn)
            {
                Logger.Info($"LoggedIn: User = {User.Instance} currentState = {CurrentState.ToString()}, Event = {_event.ToString()}");
                logIn();
                return;
            }
            if (User.Instance.fresh || _event == Events.LogoutButtonPressed || _event == Events.NotLoggedIn)
            {
                Logger.Info($"logging out: User = {User.Instance}  currentState = {CurrentState.ToString()}, Event = {_event.ToString()}");
                logout();
                return;
            }
            
            var newState = StateMap[CurrentState].PushEvent(_event);
            if (newState == States.NONE)
            {
                Logger.Error($"newState = NONE, currentState = {CurrentState.ToString()}, Event = {_event.ToString()}");
                Console.WriteLine("New State was NONE");
                newState = CurrentState; // States.GREETINGS;
                setState(newState);
            }
            if (newState != States.NONE)
            {
                CurrentState = newState;
                AppWidgetManager man = AppWidgetManager.GetInstance(Context);
                ComponentName widget = new ComponentName(Context, Java.Lang.Class.FromType(typeof(MainWidget)));
                var ids = man.GetAppWidgetIds(widget);
                Console.WriteLine($"{ids}, {ids.Length}");
                var updateIntent = new Intent();
                updateIntent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
                updateIntent.PutExtra("IDS", ids);
                updateIntent.PutExtra("DATA", "");
                Context.SendBroadcast(updateIntent);
                // RemoteViews remoteViews = new RemoteViews(Context.PackageName, Resource.Layout.widget_main);
                Console.WriteLine("Statemachine is updating remote views", RemoteViews);
                var appWidgetManager = AppWidgetManager.GetInstance(Context.ApplicationContext);
                if (Xamarin.Essentials.MainThread.IsMainThread)
                {
                    appWidgetManager.UpdateAppWidget(widget, RemoteViews);
                }
                else
                {
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => appWidgetManager.UpdateAppWidget(widget, RemoteViews));
                }
                this.Persist();
            }
        }

        private void trySetAffirmationState()
        {
            var difference = DateTime.Now - timerStopsAt;
            if (difference.Ticks > 0)
            {
                setAffirmationState((int)difference.TotalSeconds);
            } else
            {
                if (LastQuestionTaskState == States.QUESTION)
                {
                    setTaskState();
                } else
                {
                    setQuestionState();
                }
            }
        }

        // TODO save and use last Affirmation, Question or even Task; use timer for greetings, 
        public void Recall()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "statemachine.json");
            if (File.Exists(path) && false)
            {
                {
                    using FileStream fileStream = File.OpenRead(path);
                    var bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes);
                    var content = Encoding.ASCII.GetString(bytes);
                    if (bytes.Length > 0)
                    {
                        var instance = JsonSerializer.Deserialize<StateMachine>(content);
                        instance = null;
                        switch (CurrentState)
                        {
                            case States.INFO:
                            case States.AFFIRMATION: trySetAffirmationState(); break;
                            case States.CHOOSE_LATER:
                                if (LastQuestionTaskState == States.QUESTION)
                                {
                                    setQuestionState();
                                }
                                else
                                {
                                    setTaskState();
                                }
                                return;
                            case States.QUESTION: setQuestionState(); break;
                            case States.TASK: setTaskState(); break;
                            default: setGreetingsState(); break;
                        }
                    }
                }
                Persist();
            }
        }

        public void Persist()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "statemachine.json");
            using FileStream fileStream = File.Create(path);
            var serialized = JsonSerializer.Serialize(this);
            fileStream.Write(Encoding.ASCII.GetBytes(serialized));
        }
    }
}
