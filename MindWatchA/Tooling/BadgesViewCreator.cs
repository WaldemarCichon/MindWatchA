using System;
using System.Collections.Generic;
using Android.Content;
using Android.Widget;

namespace MindWatchA.Tooling
{
    public class BadgesViewCreator
    {
        public ScrollView View { get; private set; }
        public Context Context { get; }
        public List<BadgeClassification> Badges { get; }

        public BadgesViewCreator(Context context, System.Collections.Generic.List<BadgeClassification> badges)
        {
            this.Context = context;
            this.Badges = badges;
            create();
        }

        private void create()
        {
            View = new ScrollView(Context);
            var mainLayout = new LinearLayout(Context) { Orientation = Orientation.Vertical} ;
            var currentLayout = (LinearLayout)null;
            int i = 0; 
            foreach (var badge in Badges)
            {
                if (i++ % 3 == 0)
                {
                    if (currentLayout != null)
                    {
                        mainLayout.AddView(currentLayout);
                    }
                    currentLayout = new LinearLayout(View.Context) { Orientation = Orientation.Horizontal };
                    
                }
                var layoutParams = new LinearLayout.LayoutParams(0, 600, 1.0f);
                layoutParams.TopMargin = 125;
                var singleLayout = new LinearLayout(currentLayout.Context) { Orientation = Orientation.Vertical};
                singleLayout.LayoutParameters = layoutParams;
                var badgeView = new ImageView(singleLayout.Context);
                badgeView.SetImageResource(badge.ImageResource);
                badgeView.SetForegroundGravity(Android.Views.GravityFlags.CenterHorizontal);
                var text = new TextView(singleLayout.Context) { Text = badge.Description, TextSize=12};                
                var textLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
                textLayoutParams.BottomMargin = 50;
                text.LayoutParameters = textLayoutParams;
                text.TextAlignment = Android.Views.TextAlignment.Center;
                text.SetForegroundGravity(Android.Views.GravityFlags.CenterHorizontal);
                text.Typeface = Android.Graphics.Typeface.DefaultBold;
                singleLayout.AddView(text);
                singleLayout.AddView(badgeView);
                currentLayout.AddView(singleLayout);
            }
            if (currentLayout != null)
            {
                mainLayout.AddView(currentLayout);
            }
            View.AddView(mainLayout);
        }
    }
}
