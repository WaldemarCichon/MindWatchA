using System;
using System.Collections.Generic;

namespace MindWatchA.Tooling
{
    public class BadgeCalculation
    {

        private BadgeCalculation()
        {
        }

        public static List<BadgeClassification> GetBadges(int points)
        {
            var badges = new List<BadgeClassification>();
            foreach (var badgeClassification in BadgeClassification.badgeClassifications)
            {
                if (points > badgeClassification.MinimumPoints)
                {
                    badges.Add(badgeClassification);
                } else
                {
                    break;
                }
            }
            return badges;
        }
    }
}
