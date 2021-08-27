using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using MindWidgetA.StateMachine;
using Selftastic_WS_Test.API;
using Android.Content;
using System.Threading.Tasks;
using Selftastic_WS_Test.Models.Single;

namespace MindWatchA
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity Instance { get; private set; }
        public static MainActivity PreviousInstance { get; private set; }

        private User user;

        public FloatingActionButton happyButton;
        public FloatingActionButton neutralButton;
        public FloatingActionButton sadButton;
        public FloatingActionButton okButton;
        public FloatingActionButton laterButton;
        public FloatingActionButton noButton;
        public FloatingActionButton syncButton;
        public FloatingActionButton logoutButton;
        public Android.Widget.ImageView backgroundImageView;
        public Android.Widget.Button confirmButton;
        public Android.Widget.ImageView backButton;
        private Android.Widget.ListView mainListView;
        public FloatingActionButton infoButton;
        public Android.Widget.TimePicker laterTimePicker;


        [Obsolete]
        private void removeStatusBar()
        {
            View decorView = Window.DecorView;
            var uiOptions = SystemUiFlags.Fullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Immersive;
            decorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            user = User.Instance;
            if (user.user_id == null)
            {
                PreviousInstance = this;
                StartActivity(typeof(LoginActivity));
            }
            TimeConstants.is_test = user.test_mode;

#pragma warning disable CS0612 // Type or member is obsolete
            removeStatusBar();
#pragma warning restore CS0612 // Type or member is obsolete

            /*
            var controller = Window.InsetsController;
            controller.Hide(WindowInsets.Type.NavigationBars());
            */

            /*
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            */

            var ui = AbstractUI.Instance;
            happyButton = FindViewById<FloatingActionButton>(Resource.Id.happy);
            ui.HappyButton.Register(happyButton);

            neutralButton = FindViewById<FloatingActionButton>(Resource.Id.neutral);
            ui.NeutralButton.Register(neutralButton);

            sadButton = FindViewById<FloatingActionButton>(Resource.Id.sad);
            ui.SadButton.Register(sadButton);

            infoButton = FindViewById<FloatingActionButton>(Resource.Id.info);
            ui.InfoButton.Register(infoButton);

            okButton = FindViewById<FloatingActionButton>(Resource.Id.ok);
            ui.OkButton.Register(okButton);

            noButton = FindViewById<FloatingActionButton>(Resource.Id.no);
            ui.NoButton.Register(noButton);

            laterButton = FindViewById<FloatingActionButton>(Resource.Id.later);
            ui.LaterButton.Register(laterButton);

            backButton = FindViewById<Android.Widget.ImageView>(Resource.Id.back);
            ui.BackButton.Register(backButton);

            //mainTextView = FindViewById<Android.Widget.TextView>(Resource.Id.mainText);
            //ui.MainText.Register(mainTextView);

            backgroundImageView = FindViewById<Android.Widget.ImageView>(Resource.Id.backgroundImage);
            ui.Background.Register(backgroundImageView);

            mainListView = FindViewById<Android.Widget.ListView>(Resource.Id.mainList);
            ui.MainText.Register(mainListView, this);

            laterTimePicker = FindViewById<Android.Widget.TimePicker>(Resource.Id.laterTimePicker);
            laterTimePicker.SetIs24HourView(Java.Lang.Boolean.True);
            ui.LaterTimePicker.Register(laterTimePicker);

            syncButton = FindViewById<FloatingActionButton>(Resource.Id.sync);
            ui.SyncButton.Register(syncButton);

            logoutButton = FindViewById<FloatingActionButton>(Resource.Id.logout);
            ui.LogoutButton.Register(logoutButton);

            ui.FinishedRegistration();

            // Task.Run(async() => await ApiCall.Instance.TestLogin());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
