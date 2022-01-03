
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
using MindWatchA.Tooling;
using MindWidgetA.Tooling;

namespace MindWatchA.UI.Fragments
{
    public class BadgesFragment : Fragment
    {
        private static readonly IFormatProvider formatProvider = CultureInfo.CreateSpecificCulture("de-DE");

        private static Fragment instance;

        public static Fragment Instance => instance == null ? instance = new BadgesFragment() : instance;
        private LinearLayout mainLayout;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            mainLayout = view.FindViewById<LinearLayout>(Resource.Id.main_layout);
            
            var badgesView = new BadgesViewCreator(view.Context, Statistics.Badges).View;
            mainLayout.AddView(badgesView);
        }

        protected void saveButtonClicked(object sender, EventArgs args)
        {

        }

        protected void logoutButtonClicked(object sender, EventArgs args)
        {
            User.Remove();
            Intent intent = new Intent(this.Activity, typeof(LoginActivity));
            
            StartActivity(intent);
        }

        protected void FocusChangeListener(object sender, FocusChangeEventArgs eventArgs)
        {

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
            return inflater.Inflate(Resource.Layout.badge_fragment, container, false); 
        }

        public static void Clear()
        {
            instance = null;
        }
    }
}
