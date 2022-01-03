
using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;

using MindWidgetA.StateMachine;

namespace MindWatchA
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private EditText mailAddress;
        private EditText password;
        private Button loginButton;
        private Button signUpButton;
        private CheckBox testModeCheckBox;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_login);
            // Create your application here
            View decorView = Window.DecorView;
            var uiOptions = SystemUiFlags.Fullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Immersive;
#pragma warning disable CS0618 // Type or member is obsolete
            decorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
#pragma warning restore CS0618 // Type or member is obsolete
            mailAddress = FindViewById<EditText>(Resource.Id.mailAddressEditText);
            password = FindViewById<EditText>(Resource.Id.passwordEditText);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            signUpButton = FindViewById<Button>(Resource.Id.signUpButton);
            testModeCheckBox = FindViewById<CheckBox>(Resource.Id.testModeCheckBox);
            loginButton.Click += loginClicked;
            signUpButton.Click += signUpClicked;

        }

        private void signUpClicked(object sender, EventArgs args)
        {
            StartActivity(typeof(SignUpActivity));
        }

        private async void loginClicked(object sender, EventArgs args)
        {
            if (mailAddress.Text == "" || password.Text == "")
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Kann nicht einloggen");
                alert.SetMessage("Mail-Adresse oder Passwort ist leer");
                Dialog dialog = null;
                alert.SetNeutralButton("OK", (sender, eventArgs) => { dialog.Dismiss(); });
                (dialog = alert.Create()).Show();
                return;
            }
            var userId = await ApiCall.Instance.Login(mailAddress.Text, password.Text);
            if (userId == "")
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Kann nicht einloggen");
                alert.SetMessage("Passwort oder Mail-Adresse sind falsch/nicht existent");
                Dialog dialog = null;
                alert.SetNeutralButton("OK", (sender, eventArgs) => { dialog.Dismiss(); });
                (dialog = alert.Create()).Show();
                return;
            }
            var user = User.Instance;
            user.email = mailAddress.Text;
            //user.name = mailAddress.Text;
            user.user_id = userId;
            user.test_mode = testModeCheckBox.Checked;
            user.Persist();
            AbstractUI.Instance.Login();
            StartActivity(typeof(MainActivity));
        }
    }

    
}
