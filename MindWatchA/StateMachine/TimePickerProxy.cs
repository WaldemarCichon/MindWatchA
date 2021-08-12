using System;
using Android.Views;
using Android.Widget;

namespace MindWidgetA.StateMachine
{
    public class TimePickerProxy
    {
        private StateMachine stateMachine;
        private TimePicker timePicker;
        private ViewStates _visibility;

        public TimePickerProxy(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Register(TimePicker timePicker)
        {
            this.timePicker = timePicker;
        }

        public DateTime ChosenTime
        {
            get
            {
                DateTime now = DateTime.Now;
                DateTime chosenTime = new DateTime(now.Year, now.Month, now.Day, timePicker.Hour, timePicker.Minute, 0);
                return chosenTime;
            }

            set
            {
                timePicker.Hour = value.Hour;
                timePicker.Minute = value.Minute;
            }
        }

        public void Reset()
        {
            ChosenTime = DateTime.Now;
        }

        public ViewStates Visibility
        {
            get
            {
                return _visibility;
            }

            set
            {
                _visibility = value;
                timePicker.Visibility = _visibility;
            }
        }

    }
}
