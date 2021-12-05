
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
            var type = this.Intent.Extras.GetString("type");
            SetContentView(Resource.Layout.acitivity_element_explanation);
            // Create your application here
            var element_title = FindViewById<TextView>(Resource.Id.element_title);
            var element_desription = FindViewById<TextView>(Resource.Id.element_description);
            var element_type = FindViewById<TextView>(Resource.Id.element_type);
            if (description == null || description == "")
            {
                description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";
            }
            element_type.Text = type;
            element_title.Text = title;
            element_desription.Text = description;
        }
    }
}
