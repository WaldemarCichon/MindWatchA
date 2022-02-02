
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
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
        private CheckBox confirmTermsOfServicesCheckox;
        private CheckBox confirmPrivacyPolicyCheckbox;
        private TextView privacyPolicyLinkTextView;
        private TextView termsOfServicesLinkTextView;

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
            createUserButton.Enabled = false;
            createUserButton.Click += createUserButtonClicked;
            confirmTermsOfServicesCheckox = FindViewById<CheckBox>(Resource.Id.confirm_terms_of_services_checkbox);
            confirmPrivacyPolicyCheckbox = FindViewById<CheckBox>(Resource.Id.confirm_privacy_policy_checkbox);
            privacyPolicyLinkTextView = FindViewById<TextView>(Resource.Id.privacy_policy_link_textview);
            termsOfServicesLinkTextView = FindViewById<TextView>(Resource.Id.terms_of_serices_link_textview);
            privacyPolicyLinkTextView.MovementMethod = (LinkMovementMethod.Instance);
            termsOfServicesLinkTextView.MovementMethod = (LinkMovementMethod.Instance);
            confirmPrivacyPolicyCheckbox.CheckedChange += checkboxesCheckedChanged;
            confirmTermsOfServicesCheckox.CheckedChange += checkboxesCheckedChanged;
            // Create your application here
        }

        private void checkboxesCheckedChanged (object sender, EventArgs args)
        {
            createUserButton.Enabled = confirmPrivacyPolicyCheckbox.Checked && confirmTermsOfServicesCheckox.Checked;
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
                case Resource.Id.none: user.gender = "none"; break;

            }
            try
            {
                birthDateEditText.Text = birthDateEditText.Text.Replace(",", ".");
                DateTime.ParseExact(birthDateEditText.Text, "dd.MM.yyyy", CultureInfo.GetCultureInfo("de-DE"));
            }
            catch (Exception)
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Geburtsdatum");
                alert.SetMessage("Das Geburtsdatum entspricht nicht dem Muster tt.mm.jjjj");
                Dialog dialog = null;
                alert.SetNeutralButton("OK", (sender, eventArgs) => { dialog.Dismiss(); });
                (dialog = alert.Create()).Show();
                return;
            }
            user.password = password1EditText.Text;
            user.email = mailAddressEditText.Text;
            user.accepted_gdpr = DateTime.Now;
            user.accepted_tac = DateTime.Now;
            var answer = await ApiCall.AdminInstance.PostUser(user);
            if (answer)
            {
                FinishAffinity();
            }
            else
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Email-Adresse bereits vorhanden");
                alert.SetMessage("Die von Ihnen eingegebene Email-Adresse\nexistiert bereits im System.\nWahlweise bitte einloggen mit der passenden Adresse oder\neine andere Adresse benutzen.");
                Dialog dialog = null;
                alert.SetNeutralButton("OK", (sender, eventArgs) => { dialog.Dismiss(); });
                (dialog = alert.Create()).Show();
                return;
            }
        }
    }
}
