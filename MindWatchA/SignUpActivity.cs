
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Selftastic_WS_Test.API;
using Selftastic_WS_Test.Models.Single;

namespace MindWatchA
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {
        EditText firstNameEditText;
        EditText lastNameEditText;
        EditText mailAddressEditText;
        EditText birthDateEditText;
        EditText password1EditText;
        EditText password2EditText;
        RadioGroup genderRadioGroup;
        Button createUserButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.sign_up_activity);
            firstNameEditText = FindViewById<EditText>(Resource.Id.first_name);
            lastNameEditText = FindViewById<EditText>(Resource.Id.last_name);
            mailAddressEditText = FindViewById<EditText>(Resource.Id.mail_address);
            birthDateEditText = FindViewById<EditText>(Resource.Id.birtday);
            genderRadioGroup = FindViewById<RadioGroup>(Resource.Id.gender_radio_group);
            password1EditText = FindViewById<EditText>(Resource.Id.password1);
            password2EditText = FindViewById<EditText>(Resource.Id.password2);
            createUserButton = FindViewById<Button>(Resource.Id.create_user);
            createUserButton.Click += createUserButtonClicked;
            // Create your application here
        }

        private async void createUserButtonClicked(object sender, EventArgs args)
        {
            if (password1EditText.Text != password2EditText.Text)
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Passwort");
                alert.SetMessage("Passwort und Wiederholung stimmen nicht überein");
                Dialog dialog = null;
                alert.SetNeutralButton("OK", (sender, eventArgs) => { dialog.Dismiss(); });
                (dialog = alert.Create()).Show();
                return;
            }
            var user = User.Instance;
            user.first_name = firstNameEditText.Text;
            user.last_name = lastNameEditText.Text;
            user.gender = "none";
            switch (genderRadioGroup.CheckedRadioButtonId)
            {
                case Resource.Id.male: user.gender = "male"; break;
                case Resource.Id.female: user.gender = "female"; break;
                case Resource.Id.divers: user.gender = "divers"; break;
                case Resource.Id.none: user.gender = "none";break;

            }
            user.gender = Enums.Gender.male.ToString();
            user.password = password1EditText.Text;
            await ApiCall.AdminInstance.PostUser(user);
            user.Persist();
            StartActivity(typeof(MainActivity));
        }
    }
}
