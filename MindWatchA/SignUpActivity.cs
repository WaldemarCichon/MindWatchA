
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
            createUserButton = FindViewById<Button>(Resource.Id.create_user);
            createUserButton.Click += createUserButtonClicked;
            // Create your application here
        }

        private async void createUserButtonClicked(object sender, EventArgs args)
        {
            var user = User.Instance;
            user.first_name = firstNameEditText.Text;
            user.last_name = lastNameEditText.Text;
            user.gender = Enums.Gender.male.ToString();
            user.password = "xyzt";
            await ApiCall.AdminInstance.PostUser(user);
        }
    }
}
