
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
using MindWatchA.Models.Single;
using MindWatchA.Repository;
using MindWatchA.Models.Collections;

namespace MindWatchA.UI.Fragments
{
    public class SavedElementsFragment : Fragment
    {
        public static SavedElementsFragment Instance => new SavedElementsFragment();
        private ListView savedElementsList;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            savedElementsList = view.FindViewById<ListView>(Resource.Id.saved_items_list_view);
            var adapter = new ArrayAdapter<SavedTaskQuestion>(view.Context, Android.Resource.Layout.SimpleListItem1);
            adapter.AddAll(SavedTaskQuestions.Instance.positions);
            savedElementsList.Adapter = adapter;
            savedElementsList.ItemClick += SavedElementsList_ItemClick;
        }

        private void SavedElementsList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var element = SavedTaskQuestions.Instance.positions[(int)e.Id];
            var title = element.Title;
            var description = element.Description;
            var intent = new Intent(this.Context, typeof(ElementExplanationActivity));

            intent.PutExtra("type", element.Type);
            intent.PutExtra("title", title);
            intent.PutExtra("description", description);
            StartActivity(intent);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.saved_items_list, container, false);
        }
    }
}
