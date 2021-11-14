using System;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using MindWatchA;
using MindWidgetA.StateMachine.RemoteComponents;

namespace MindWidgetA.StateMachine
{
    public class AbstractUI
    {
        public static AbstractUI Instance { get; } = new AbstractUI();
        //public static AbstractUI Instance1 { get; } = new AbstractUI();

        public ButtonProxy HappyButton { get; }
        public ButtonProxy NeutralButton { get; }
        public ButtonProxy SadButton { get; }
        public ButtonProxy InfoButton { get; }
        public ButtonProxy BackButton { get; }
        public ButtonProxy OkButton { get; }
        public ButtonProxy LaterButton { get; }
        public ButtonProxy NoButton { get; }
        public ButtonProxy LogoutButton { get; }
        public ButtonProxy SyncButton { get; }

        public TextViewProxy MainText { get; }

        public ImageViewProxy Background { get; }

        private RemoteImageView backgroundImage;
        private RemoteTextView mainText;
        private RemoteButton happyButton;
        private RemoteButton neutralButton;
        private RemoteButton sadButton;
        private RemoteButton infoButton;
        private RemoteButton okButton;
        private RemoteButton noButton;
        private RemoteButton backButton;
        private StateMachine stateMachine;

        private AbstractUI()
        {
            Console.WriteLine("Abstract UI - in constructor");
            stateMachine = new StateMachine(this, null);
            Console.WriteLine("Statemachine created");
            HappyButton = new ButtonProxy(stateMachine, Events.HappyButtonPressed);
            NeutralButton = new ButtonProxy(stateMachine, Events.NeutralButtonPressed);
            SadButton = new ButtonProxy(stateMachine, Events.SadButtonPressed);
            InfoButton = new ButtonProxy(stateMachine, Events.InfoButtonPressed);
            BackButton = new ButtonProxy(stateMachine, Events.BackButtonPressed);
            LaterButton = new ButtonProxy(stateMachine, Events.ChooseLaterPressed);
            OkButton = new ButtonProxy(stateMachine, Events.YesButtonPressed);
            NoButton = new ButtonProxy(stateMachine, Events.NoButtonPressed);
            MainText = new TextViewProxy();
            Background = new ImageViewProxy();
            SyncButton = new ButtonProxy(stateMachine, Events.SyncButtonPressed);
            LogoutButton = new ButtonProxy(stateMachine, Events.LogoutButtonPressed);

            Console.WriteLine("Proxies created");

            backgroundImage = new RemoteImageView(Resource.Id.backgroundImage);
            mainText = new RemoteTextView(Resource.Id.mainText);
            happyButton = new RemoteButton(Resource.Id.happy_widget);
            neutralButton = new RemoteButton(Resource.Id.neutral_widget);
            sadButton = new RemoteButton(Resource.Id.sad_widget);
            infoButton = new RemoteButton(Resource.Id.info_widget);
            okButton = new RemoteButton(Resource.Id.ok_widget);
            noButton = new RemoteButton(Resource.Id.no_widget);
            backButton = new RemoteButton(Resource.Id.back_widget);

            Console.WriteLine("Abstract UI created");
        }

        internal void SetBaseData(RemoteViews remoteViews, ComponentName widget, AppWidgetManager appWidgetManager)
        {
            StateMachine.RemoteViews = remoteViews;
            backgroundImage.SetBaseData(remoteViews, widget, appWidgetManager);
            mainText.SetBaseData(remoteViews, widget, appWidgetManager);
            happyButton.SetBaseData(remoteViews, widget, appWidgetManager);
            neutralButton.SetBaseData(remoteViews, widget, appWidgetManager);
            sadButton.SetBaseData(remoteViews, widget, appWidgetManager);
            infoButton.SetBaseData(remoteViews, widget, appWidgetManager);
            okButton.SetBaseData(remoteViews, widget, appWidgetManager);
            noButton.SetBaseData(remoteViews, widget, appWidgetManager);
            backButton.SetBaseData(remoteViews, widget, appWidgetManager);
            HappyButton.Register(happyButton);
            NeutralButton.Register(neutralButton);
            SadButton.Register(sadButton);
            InfoButton.Register(infoButton);
            OkButton.Register(okButton);
            NoButton.Register(noButton);
            BackButton.Register(backButton);
            Background.Register(backgroundImage);
            MainText.Register(mainText);

            MainText.Text = "Barbara";
        }

        internal void FinishedRegistration()
        {
            stateMachine.Recall();
        }
    }
}
