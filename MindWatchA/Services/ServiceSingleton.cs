﻿using System;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Runtime;
using Android.Widget;
using MindWatchA.Tooling;
using MindWidgetA;
using MindWidgetA.StateMachine;
using MindWidgetA.UI;
using Rollbar;

namespace MindWatchA.Services
{
    public class ServiceSingleton
    {

        public Context Context { get; private set; }
        public Context ApplicationContext { get; private set; }
        public ComponentName Widget { get; private set; }
        public int[] Ids { get; private set; }
        public RemoteViews RemoteViews { get; private set; }
        public AbstractUI Ui { get; private set; }

        private static ServiceSingleton instance;

        public static ServiceSingleton Instance(Context context)
        {

            if (instance == null)
            {
                instance = new ServiceSingleton(context);
            }

            if (context != instance.Context)
            {
                Logger.Error("Context does not equals the old one");
                Console.WriteLine("context does not equals the old one");
            }
            return instance;
        }


        private ServiceSingleton(Context context)
        {
            Init(context);
            RollbarLocator.RollbarInstance.Configure(new RollbarConfig("904e61ade59142b5bb6784b35767c269"));
            RollbarLocator.RollbarInstance.Info("Mindwidget - Widget wurde gestartet");
            RollbarHelper.RegisterForGlobalExceptionHandling();

            AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                var newExc = new ApplicationException("AndroidEnvironment_UnhandledExceptionRaiser", args.Exception);
                RollbarLocator.RollbarInstance.AsBlockingLogger(TimeSpan.FromSeconds(10)).Critical(newExc);
            };
        }

        internal void Logout()
        {
            Ui.InformAboutLogout();
        }

        internal void Login()
        {
            Ui.InformAboutLogin();
        }

        public void ButtonClicked(string buttonAction)
        {
            Console.WriteLine("Button clicked");
            Console.WriteLine(buttonAction);
            switch (buttonAction)
            {
                case MainWidget.INFO_BTN_CLICKED: Ui.InfoButton.Clicked(this, null); break;
                case MainWidget.HAPPY_BTN_CLICKED: Ui.HappyButton.Clicked(this, null); break;
                case MainWidget.NEUTRAL_BTN_CLICKED: Ui.NeutralButton.Clicked(this, null); break;
                case MainWidget.SAD_BTN_CLICKED: Ui.SadButton.Clicked(this, null); break;
                case MainWidget.BACK_BTN_CLICKED: Ui.BackButton.Clicked(this, null); break;
                case MainWidget.OK_BTN_CLICKED: Ui.OkButton.Clicked(this, null); break;
                case MainWidget.NO_BTN_CLICKED: Ui.NoButton.Clicked(this, null); break;
                case MainWidget.LATER_BTN_CLICKED: Ui.LaterButton.Clicked(this, null); break;
            }
        }

        private void Init(Context context)
        {
            this.Context = context;
            this.ApplicationContext = context.ApplicationContext;
            RemoteViews = new RemoteViews(context.PackageName, Resource.Layout.widget_main);
            Widget = new ComponentName(context, Java.Lang.Class.FromType(typeof(MainWidget))); //  new ComponentName(context, typeof(Widget));

            Console.WriteLine("Creating Abstract UI");
            var appWidgetManager = AppWidgetManager.GetInstance(context);

            Ui = AbstractUI.Instance;
            Ui.SetBaseData(RemoteViews, Widget, appWidgetManager);
            Ui.FinishedRegistration();

            Console.WriteLine("Base data set");
        }


    }
}
