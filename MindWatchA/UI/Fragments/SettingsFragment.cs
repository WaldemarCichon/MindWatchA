
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AndroidX.Fragment.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Views.View;
using System.Globalization;
using AndroidX.AppCompat.App;
using Selftastic_WS_Test.Models.Single;
using MindWatchA.Models.Single;
using MindWidgetA.StateMachine;

namespace MindWatchA.UI.Fragments
{
    public class SettingsFragment : Fragment
    {
        private static readonly IFormatProvider formatProvider = CultureInfo.CreateSpecificCulture("de-DE");
        private EditText nightStopFrom;
        private EditText nightStopTo;
        private CheckBox[] weeklyCheckBoxes;
        private static readonly int[] weeklyCheckBoxIds =
        {
            Resource.Id.checkBox_monday,
            Resource.Id.checkBox_tuesday,
            Resource.Id.checkBox_wednesday,
            Resource.Id.checkBox_thursday,
            Resource.Id.checkBox_fridayday,
            Resource.Id.checkBox_saturday,
            Resource.Id.checkBox_sunday
        };
        private DateTime nightStopFromTime;
        private DateTime nightStopToTime;
        private Button saveButton;
        private Button logoutButton;
        private Settings settings;
        private static SettingsFragment instance;

        public static Fragment Instance => instance == null? instance = new SettingsFragment() : instance;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            nightStopFrom = view.FindViewById<EditText>(Resource.Id.night_stop_from);
            nightStopTo = view.FindViewById<EditText>(Resource.Id.night_stop_to);
            weeklyCheckBoxes = new CheckBox[7];
            for (int i=0; i<7; i++)
            {
                weeklyCheckBoxes[i] = view.FindViewById<CheckBox>(weeklyCheckBoxIds[i]);
            }
            nightStopFrom.FocusChange += FocusChangeListener;
            nightStopTo.FocusChange += FocusChangeListener;
            saveButton = view.FindViewById<Button>(Resource.Id.save_button);
            logoutButton = view.FindViewById<Button>(Resource.Id.logout_button);
            saveButton.Click += saveButtonClicked;
            logoutButton.Click += logoutButtonClicked;
            settings = Settings.Restore();
            nightStopFromTime = settings.EndTime;
            nightStopFrom.Text = settings.EndTime.ToString("HH:mm");
            nightStopToTime = settings.StartTime;
            nightStopTo.Text = settings.StartTime.ToString("HH:mm");
            for (int i=0; i<7; i++)
            {
                weeklyCheckBoxes[i].Checked = settings.GetWeekDayValue(i);
            }
        }

        protected void saveButtonClicked(object sender, EventArgs args)
        {
            settings.StartTime = nightStopToTime;
            settings.EndTime = nightStopFromTime;
            for (int i=0; i<7; i++)
            {
                settings.SetWeekDayValue(i, weeklyCheckBoxes[i].Checked);
            }
            settings.Persist();
        }

        protected void logoutButtonClicked(object sender, EventArgs args)
        {
            User.Remove();
            AbstractUI.Instance.InformAboutLogout();
            Intent intent = new Intent(this.Activity, typeof(LoginActivity));
            
            StartActivity(intent);
        }

        protected void FocusChangeListener(object sender, FocusChangeEventArgs eventArgs)
        {
            if (eventArgs.HasFocus)
            {
                return;
            }
            var editText = sender as EditText;
            var content = editText.Text;
            DateTime result;
            if (!DateTime.TryParse(content, formatProvider, DateTimeStyles.None, out result))
            {
                DisplayAlert();
                return;
            }
            editText.Text = result.ToString("t", formatProvider);
            if (sender == nightStopFrom)
            {
                nightStopFromTime = result;
            } else
            {
                nightStopToTime = result;
            }
        }

        private void DisplayAlert()
        {
            var builder = new AlertDialog.Builder(Context);
            var alert = builder.Create();
            alert.SetTitle("Falsches Zeitformat");
            alert.SetMessage("Das angegebe Zeitformat ist falsch.\nBitte korrigieren sie es, damit er dem Beispiel 8:52 entspricht");
            alert.SetButton((int)DialogButtonType.Positive, "OK", (s, a) => { });
            alert.Show();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.settings, container, false); 
        }

        public static void Clear()
        {
            instance = null;
        }
    }
}
