using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LeapMotionDriverWindow
{
    class EnvironmentSetting : LocalSetting.SettingListBase
    {
        private static EnvironmentSetting defaultInstance = new EnvironmentSetting();
        public static string[] VideoFileFilter = new string[] { ".mp4", ".wmv", ".mov", ".avi", ".mpg" };
        public static string[] SoundFileFilter = new string[] { ".mp3", ".wav" };
        public static string[] ImageFileFilter = new string[] { ".jpg", ".bmp", ".png" };
        public static Random r = new Random(DateTime.Now.Millisecond);

        public static EnvironmentSetting Default
        {
            get
            {
                return defaultInstance;
            }
        }

        protected override void BuildSettingList()
        {
            AddingSetting("swipeDetectionDelay", 700);
            AddingSetting("closeWindowDelay", 1000);
            AddingSetting("topAnimationSpeed", 0.2);
            AddingSetting("windowDefaultWidth", 640);
            AddingSetting("windowDefaultHeight", 480);
            AddingSetting("bigSize", 2.0);
            AddingSetting("mediumSize", 1.0);
            AddingSetting("smallSize", 0.5);
            AddingSetting("gestureSwipeMinLength", 150);
            AddingSetting("gestureSwipeMinVelocity", 1000);
            base.InitSettingList();
        }
    }
}
