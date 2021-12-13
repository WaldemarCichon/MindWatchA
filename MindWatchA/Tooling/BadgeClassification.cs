using System;
namespace MindWatchA.Tooling
{
    public class BadgeClassification
    {
        public static BadgeClassification[] badgeClassifications =
        {
            new BadgeClassification(500, "Selfastic-Rookie", Resource.Drawable.badge500),
            new BadgeClassification(1000, "Mindset-Rookie", Resource.Drawable.badge1000),
            new BadgeClassification(2000, "Mindset-Student", Resource.Drawable.badge2000),
            new BadgeClassification(3000, "Mindeset-Professional", Resource.Drawable.badge3000),
            new BadgeClassification(4000, "Mindset-Master", Resource.Drawable.badge4000),
            new BadgeClassification(6000, "Zen-Rookie", Resource.Drawable.badge6000),
            new BadgeClassification(8000,"Zen-Sudent", Resource.Drawable.badge8000),
            new BadgeClassification(10000, "Zen-Professional", Resource.Drawable.badge10000),
            new BadgeClassification(12000, "Zen-Master", Resource.Drawable.badge12000)
        };

        public int MinimumPoints { get; set; }
        public string Description { get; set; }
        public int ImageResource { get; set; }

        public BadgeClassification(int minimumPoints, string description, int imageResource)
        {
            MinimumPoints = minimumPoints;
            Description = description;
            ImageResource = imageResource;
        }
        
    }
}
