using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MindWatchA.Models.Single;

namespace MindWatchA.UI.Adapters
{
    public class SavedElementsAdapter : ArrayAdapter<SavedTaskQuestion>
    {
        public SavedElementsAdapter(Context context, int layout) : base(context, layout) { }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = null;
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                view = inflater.Inflate(Resource.Layout.saved_items_list_element, parent, false);
            } else
            {
                view = convertView;
            }

            var itemDescription = view.FindViewById<TextView>(Resource.Id.item_description);
            itemDescription.Text = GetItem(position).Text;
            view.SetBackgroundColor(GetItem(position).IsQuestion ?  Color.Bisque : Color.LavenderBlush);
            return view;
        }
    }
}
