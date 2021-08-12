using MindWidgetA;
using System;
using System.Collections.Generic;
using System.Timers;
using Android.App;
using MindWidgetA.Tooling;
using MindWatchA;
using Selftastic_WS_Test.Models.Collections;

namespace MindWidgetA.StateMachine
{
    public class StateMachine
    {
        private static Timer timer;
        private static States LastQuestionTaskState = States.TASK;
        private const int DoNotChangeTimer = -1;
        private const int UseChosenDuration = -2;

        public static States CurrentState { get; set; }
        public static StateMap StateMap;
        private static AbstractUI UI { get; set; }
        private static Statistics statistics;
        private static int lastAffirmationDuration;
        private static bool laterChosen = false;

        private StateTransitions GreetingsStateTransitions = new StateTransitions(States.GREETINGS).
            Add(new Transition(States.AFFIRMATION, Events.HappyButtonPressed, () => { return true; }, () => { statistics.IncrementMindState(Events.HappyButtonPressed) ; setAffirmationState(TimeConstants.DURATION_GOOD); })).
            Add(new Transition(States.AFFIRMATION, Events.NeutralButtonPressed, () => { return true; }, () => { statistics.IncrementMindState(Events.NeutralButtonPressed) ; setAffirmationState(TimeConstants.DURATION_MIDDLE); })).
            Add(new Transition(States.AFFIRMATION, Events.SadButtonPressed, () => { return true; }, () => { statistics.IncrementMindState(Events.SadButtonPressed); setAffirmationState(TimeConstants.DURATION_BAD); }));
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
            Add(new Transition(States.QUESTION, Events.TimeEllapsed, () => LastQuestionTaskState == States.TASK ^ laterChosen , () => { setQuestionState(); })).
            Add(new Transition(States.TASK, Events.TimeEllapsed, () => LastQuestionTaskState == States.QUESTION ^ laterChosen, () => { setTaskState(); }));
        private StateTransitions LaterChosenTransisions = new StateTransitions(States.CHOOSE_LATER).
            Add(new Transition(States.AFFIRMATION, Events.YesButtonPressed, () => true, () => setAffirmationState(UseChosenDuration))).
            Add(new Transition(States.QUESTION, Events.NoButtonPressed, () => LastQuestionTaskState == States.QUESTION, () => setQuestionState())).
            Add(new Transition(States.TASK, Events.NoButtonPressed, () => LastQuestionTaskState == States.TASK, () => setTaskState()));

        private static int currentTimerDuration;

        public StateMachine(AbstractUI ui)
        {
            CurrentState = States.GREETINGS;
            UI = ui;
            createTimer();
            InitStateMap();
            statistics = new Statistics();
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

        private void createTimer()
        {
            timer = new Timer();
            timer.Elapsed += (object o, ElapsedEventArgs a) => { Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => PushEvent(Events.TimeEllapsed)); timer.Interval = 0; };
        }

        private static void setTimer(int seconds)
        {
            currentTimerDuration = seconds;
            if (timer.Interval>0)
            {
                timer.Stop();
            }
            timer.AutoReset = false;
            timer.Interval = seconds * 1000;
            timer.Start();
        }

        private static int chosenLaterDuration => (int)(UI.LaterTimePicker.ChosenTime - DateTime.Now).TotalSeconds;

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
            UI.Background.SetImageResource(Resource.Drawable.hg4);
        }

        private static void setTaskAnswer(bool answer)
        {
            statistics.IncrementTask(answer);
            setAffirmationState(currentTimerDuration);
            Tasks.Instance.SendAnswer(answer ? Selftastic_WS_Test.Enums.AnswerKind.accepted : Selftastic_WS_Test.Enums.AnswerKind.declined);
        }

        private static void setQuestionAnswer(bool answer)
        {
            statistics.IncrementQuestion(answer);
            setAffirmationState(currentTimerDuration);
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

        public void PushEvent(Events _event)
        {
            var newState = StateMap[CurrentState].PushEvent(_event);
            if (newState != States.NONE)
            {
                CurrentState = newState;
            }
        }

    }
}
