using System;
namespace MindWatchA.Tooling
{
    public class BadgeClassification
    {
        public static BadgeClassification[] badgeClassifications =
        {
            new BadgeClassification(500, "Selfastic-Rookie"),
            new BadgeClassification(1000, "Mindset-Rookie"),
            new BadgeClassification(2000, "Mindset-Student"),
            new BadgeClassification(3000, "Mindeset-Professional"),
            new BadgeClassification(4000, "Mindset-Master"),
            new BadgeClassification(6000, "Zen-Rookie"),
            new BadgeClassification(8000,"Zen-Sudent"),
            new BadgeClassification(10000, "Zen-Professional"),
            new BadgeClassification(12000, "Zen-Master")
        };

        public int MinimumPoints { get; set; }
        public string Description { get; set; }
        public int imageResource { get; set; }

        public BadgeClassification(int minimumPoints, string description)
        {
            MinimumPoints = minimumPoints;
            Description = description;
            imageResource = Resource.Drawable.badge;
        }
        
    }
}
