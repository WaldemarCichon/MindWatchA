
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

namespace MindWatchA.UI
{
    [Activity(Label = "ElementExplanationActivity")]
    public class ElementExplanationActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var title = this.Intent.Extras.GetString("title");
            var description = this.Intent.Extras.GetString("description");
            SetContentView(Resource.Layout.acitivity_element_explanation);
            // Create your application here
            var element_title = FindViewById<TextView>(Resource.Id.element_title);
            var element_desription = FindViewById<TextView>(Resource.Id.element_description);
            element_title.Text = title;
            element_desription.Text = description;
        }
    }
}
