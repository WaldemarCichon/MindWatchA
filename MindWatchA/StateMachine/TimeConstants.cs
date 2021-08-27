using System;
namespace MindWidgetA.StateMachine
{
    public class TimeConstants
    {
        public static bool is_test = true;


        public static int MAIN_UNIT = is_test ? 10 : 60 * 60;
        public static int START = 8;
        public static int MIDDAY = 13;
        public static int END = 21;
        public static int DURATION_BAD = (int) 3 * MAIN_UNIT / 2;
        public static int DURATION_MIDDLE = MAIN_UNIT * 2;
        public static int DURATION_GOOD = 3 * MAIN_UNIT;
        public static int WAIT_TIME = is_test ? 20 : 60 * 20;

        public static int SecondsToNextGreeting => is_test ? 60 * 5 : (int) (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, MIDDAY, 0, 0) - DateTime.Now).TotalSeconds;

        private TimeConstants()
        {
        }
    }
}
