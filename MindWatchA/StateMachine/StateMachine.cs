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

namespace MindWidgetA.StateMachine
{
    public class StateMachine
    {
        private static Timer timer;
        private static States LastQuestionTaskState = States.TASK;
        private const int DoNotChangeTimer = -1;
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

        public Statistics StatisticsProxy
        {
            get
            {
                return statistics;
            }

            set
            {
                statistics = value;
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

        public static States CurrentState { get; set; }
        public static StateMap StateMap;
        private static AbstractUI UI { get; set; }
        private static Statistics statistics;
        private static int lastAffirmationDuration;
        private static bool laterChosen = false;
        private static int currentGreetingsTimerDuration;
        private static DateTime nextGreetingAt;
        private static DateTime timerStopsAt;
        private static Timer greetingsTimer;

        private StateTransitions GreetingsStateTransitions = new StateTransitions(States.GREETINGS).
            Add(new Transition(States.AFFIRMATION, Events.HappyButtonPressed, () => { return true; }, () => { statistics.IncrementMindState(Events.HappyButtonPressed); sendAnswerAsync(MoodAnswerKind.good); setAffirmationState(TimeConstants.DURATION_GOOD); })).
            Add(new Transition(States.AFFIRMATION, Events.NeutralButtonPressed, () => { return true; }, () => { statistics.IncrementMindState(Events.NeutralButtonPressed); sendAnswerAsync(MoodAnswerKind.neutral);  setAffirmationState(TimeConstants.DURATION_MIDDLE); })).
            Add(new Transition(States.AFFIRMATION, Events.SadButtonPressed, () => { return true; }, () => { statistics.IncrementMindState(Events.SadButtonPressed); sendAnswerAsync(MoodAnswerKind.bad); setAffirmationState(TimeConstants.DURATION_BAD); }));
        private StateTransitions AffirmationStateTransistions = new StateTransitions(States.AFFIRMATION).
            Add(new Transition(States.QUESTION, Events.TimeEllapsed, () => { return LastQuestionTaskState == States.TASK; }, () => { setQuestionState(); })).
            Add(new Transition(States.TASK, Events.TimeEllapsed, () => { return LastQuestionTaskState == States.QUESTION; }, () => { setTaskState(); })).
            Add(new Transition(States.INFO, Events.InfoButtonPressed, () => { return true; }, () => { setInfoState(); }));
        private StateTransitions TaskStateTransistions = new StateTransitions(States.TASK).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => { return true; }, () => { setTaskAnswer(true); })).
            Add(new Transition(States.AFFIRMATION, Events.NoButtonPressed, () => { return true; }, () => { setTaskAnswer(false); })).
            Add(new Transition(States.CHOOSE_LATER, Events.ChooseLaterPressed, () => { return true; }, setChooseLaterState));
        private StateTransitions QuestionStateTransitions = new StateTransitions(States.QUESTION).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => { return true; }, () => { setQuestionAnswer(true); })).
            Add(new Transition(States.AFFIRMATION, Events.NoButtonPressed, () => { return true; }, () => { setQuestionAnswer(false); })).
            Add(new Transition(States.CHOOSE_LATER, Events.ChooseLaterPressed, () => { return true; }, setChooseLaterState));
        private StateTransitions InfoStateTransisions = new StateTransitions(States.INFO).
            Add(new Transition(States.AFFIRMATION, Events.BackButtonPressed, () => { return true; }, () => { setAffirmationState(DoNotChangeTimer); })).
            Add(new Transition(States.QUESTION, Events.TimeEllapsed, () => LastQuestionTaskState == States.TASK ^ laterChosen, () => { setQuestionState(); })).
            Add(new Transition(States.TASK, Events.TimeEllapsed, () => LastQuestionTaskState == States.QUESTION ^ laterChosen, () => { setTaskState(); })).
            Add(new Transition(States.INFO, Events.SyncButtonPressed, () => true, synchronize)).
            Add(new Transition(States.NONE, Events.LogoutButtonPressed, () => true, logout));
        private StateTransitions LaterChosenTransisions = new StateTransitions(States.CHOOSE_LATER).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => true, () => setAffirmationState(UseChosenDuration))).
            Add(new Transition(States.QUESTION, Events.NoButtonPressed, () => LastQuestionTaskState == States.QUESTION, () => setQuestionState())).
            Add(new Transition(States.TASK, Events.NoButtonPressed, () => LastQuestionTaskState == States.TASK, () => setTaskState()));



        public StateMachine() { }

        public StateMachine(AbstractUI ui)
        {
            CurrentState = States.GREETINGS;
            statistics = new Statistics();
            createTimers();
            InitStateMap();
            UI = ui;
        }

        private bool True()
        {
            return true;
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
            
        }

        private void createTimers()
        {
            timer = new Timer();
            timer.Elapsed += (object o, ElapsedEventArgs a) => { Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => PushEvent(Events.TimeEllapsed)); timer.Interval = 0; };
            greetingsTimer = new Timer();
            greetingsTimer.Elapsed += (object sender, ElapsedEventArgs a) => { Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => setGreetingsState()); greetingsTimer.Interval = 0; };
        }

        private static void setTimer(int seconds)
        {
            currentGreetingsTimerDuration = seconds;
            timerStopsAt = DateTime.Now + new TimeSpan(((long)seconds) * 10000000L);
            if (timer.Interval>0)
            {
                timer.Stop();
            }
            timer.AutoReset = false;
            timer.Interval = seconds * 1000;
            timer.Start();
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

        private static int chosenLaterDuration => (int)(UI.LaterTimePicker.ChosenTime - DateTime.Now).TotalSeconds;

        private static void setGreetingsState()
        {
            UI.HappyButton.Visibility = Android.Views.ViewStates.Visible;
            UI.NeutralButton.Visibility = Android.Views.ViewStates.Visible;
            UI.SadButton.Visibility = Android.Views.ViewStates.Visible;
            UI.BackButton.Visibility = Android.Views.ViewStates.Gone;
            UI.InfoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.Background.SetImageResource(Resource.Drawable.gemuetszustand);
            sendAnswerAsync(Selftastic_WS_Test.Enums.MoodAnswerKind.shown);
            setGreetingsTimer(TimeConstants.SecondsToNextGreeting);
        }

        private static void setAffirmationState(int seconds) 
        {
            if (seconds > 0)
            {
                setTimer(seconds);
                lastAffirmationDuration = seconds;
            } else
            {
                if (seconds == UseChosenDuration)
                {
                    setTimer(chosenLaterDuration);
                    laterChosen = true;
                }
            }

            UI.HappyButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NeutralButton.Visibility = Android.Views.ViewStates.Gone;
            UI.SadButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Gone;
            UI.NoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.BackButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LaterButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LaterTimePicker.Visibility = Android.Views.ViewStates.Gone;
            UI.InfoButton.Visibility = Android.Views.ViewStates.Visible;
            UI.MainText.Visibility = Android.Views.ViewStates.Visible;
            UI.Background.SetImageResource(Resource.Drawable.affirmations);
            UI.MainText.Text = Affirmations.Instance.NewRandom.Text;
            UI.SyncButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LogoutButton.Visibility = Android.Views.ViewStates.Gone;
            Affirmations.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.shown);
        }


        private static void setQuestionTaskState(string random, int backgroundPicId)
        {
            UI.InfoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Visible;
            UI.NoButton.Visibility = Android.Views.ViewStates.Visible;
            UI.LaterButton.Visibility = Android.Views.ViewStates.Visible;
            UI.BackButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LaterTimePicker.Visibility = Android.Views.ViewStates.Gone;
            UI.SyncButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LogoutButton.Visibility = Android.Views.ViewStates.Gone;
            // UI.backgroundImageView.SetImageResource(backgroundPicId);
            UI.MainText.Text = random;
            laterChosen = false;
        }

        private static void setQuestionState()
        {
            var question = LastQuestionTaskState == States.QUESTION ? UI.MainText.Text : Questions.Instance.NewRandom.Text;
            setQuestionTaskState(question, Resource.Drawable.questions);
            LastQuestionTaskState = States.QUESTION;
            UI.Background.SetImageResource(Resource.Drawable.questions);
            Questions.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.shown);
        }

        private static void setTaskState()
        {
            var task = LastQuestionTaskState == States.TASK ? UI.MainText.Text : Tasks.Instance.NewRandom.Text;
            setQuestionTaskState(task, Resource.Drawable.tasks);
            LastQuestionTaskState = States.TASK;
            UI.Background.SetImageResource(Resource.Drawable.tasks);
            Tasks.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.shown);
        }

        private static void setInfoState()
        {
            UI.MainText.Text = statistics.InfoText;
            UI.InfoButton.Visibility = Android.Views.ViewStates.Gone;
            UI.BackButton.Visibility = Android.Views.ViewStates.Visible;
            UI.SyncButton.Visibility = Android.Views.ViewStates.Visible;
            UI.LogoutButton.Visibility = Android.Views.ViewStates.Visible;

            UI.Background.SetImageResource(Resource.Drawable.hg4);
        }

        private static void setTaskAnswer(bool answer)
        {
            statistics.IncrementTask(answer);
            setAffirmationState(currentGreetingsTimerDuration);
            Tasks.Instance.SendAnswer(answer ? Selftastic_WS_Test.Enums.AnswerKind.accepted : Selftastic_WS_Test.Enums.AnswerKind.declined);
        }

        private static void setQuestionAnswer(bool answer)
        {
            statistics.IncrementQuestion(answer);
            setAffirmationState(currentGreetingsTimerDuration);
            Questions.Instance.SendAnswer(answer ? Selftastic_WS_Test.Enums.AnswerKind.accepted : Selftastic_WS_Test.Enums.AnswerKind.declined);
        }

        private static void setChooseLaterState()
        {
            UI.MainText.Visibility = Android.Views.ViewStates.Gone;
            UI.OkButton.Visibility = Android.Views.ViewStates.Visible;
            UI.NoButton.Visibility = Android.Views.ViewStates.Visible;
            UI.LaterButton.Visibility = Android.Views.ViewStates.Gone;
            UI.LaterTimePicker.Visibility = Android.Views.ViewStates.Visible;
            if (LastQuestionTaskState == States.QUESTION)
            {
                Questions.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.later);
            } else
            {
                Questions.Instance.SendAnswer(Selftastic_WS_Test.Enums.AnswerKind.later);
            }
        }

        private static void synchronize()
        {

        }

        private static void logout()
        {
            var prefix = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            File.Delete(Path.Combine(prefix, "user.json"));
            File.Delete(Path.Combine(prefix, "task.json"));
            File.Delete(Path.Combine(prefix, "question.json"));
            File.Delete(Path.Combine(prefix, "affirmation.json"));
            File.Delete(Path.Combine(prefix, "statemachine.json"));
            MainActivity.Instance.FinishAndRemoveTask();
            if (MainActivity.PreviousInstance != null)
            {
                MainActivity.PreviousInstance.FinishAndRemoveTask();
            }
        }

        public void PushEvent(Events _event)
        {
            var newState = StateMap[CurrentState].PushEvent(_event);
            if (newState != States.NONE)
            {
                CurrentState = newState;
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
            if (File.Exists(path))
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
