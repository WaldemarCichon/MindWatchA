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
using MindWatchA.UI.Fragments;
using Google.Android.Material.BottomNavigation;
using FloatingActionButton = Google.Android.Material.FloatingActionButton.FloatingActionButton;

namespace MindWatchA
{
    // for icons https://stackoverflow.com/questions/37945767/how-to-change-application-icon-in-xamarin-forms
    [Activity(Label = "@string/app_name", Icon="@drawable/logo", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
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
        public Android.Widget.TextView mainTextView;
        public Android.Widget.ImageView backgroundImageView;
        public Android.Widget.Button confirmButton;
        public Android.Widget.ImageView backButton;
        private Android.Widget.ListView mainListView;
        public FloatingActionButton infoButton;


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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTIwMTY3QDMxMzkyZTMzMmUzMFNUQjdXNVY5R3FJRDUrcnpZbDhaRTQxaloyMDZYMy9FL25FcE9uUDI5S2M9");
            Instance = this;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.bottom_navigation);
            user = User.Instance;
            if (user.user_id == null)
            {
                PreviousInstance = this;
                StartActivity(typeof(LoginActivity));
            }
            var bottomnavigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            bottomnavigation.ItemSelected += (sender, args) => LoadFragment(args.Item.ItemId);
            LoadFragment(Resource.Id.navigation_statistics);
            // bottomnavigation.Selected

#pragma warning disable CS0612 // Type or member is obsolete
            //removeStatusBar();


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

        void LoadFragment(int id)
        {
            AndroidX.Fragment.App.Fragment  fragment = null;
            
            switch (id)
            {
                case Resource.Id.navigation_statistics:
                    fragment = StatisticFragment.Instance;
                    break;
                case Resource.Id.navigation_settings:
                    fragment = SettingsFragment.Instance;
                    break;
                case Resource.Id.navigation_saved_items:
                    fragment = SavedElementsFragment.Instance;
                    break;
            }

            if (fragment == null) {
                return;
            }

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }
    }
}
