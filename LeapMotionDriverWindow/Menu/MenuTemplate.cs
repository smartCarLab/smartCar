using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LeapMotionDriverWindow.Menu
{
    public class MenuTemplate : Grid
    {
        public List<Menu.MenuButton> menuButtonList = new List<Menu.MenuButton>();
        public Point[] CircleMenuPosition;

        public MenuTemplate()
        {            
        }
        public void ArrangeCircleMenu()
        {
            var width = 1000;
            var height = 500;
            var buttonWidth = 200;
            var buttonHeight = 200;
            CircleMenuPosition = new Point[menuButtonList.Count];
            CircleMenuPosition[0] = new Point(width / 2 - buttonWidth / 2, height - buttonHeight);
            var step = 360.0 / (double)menuButtonList.Count;
            for (int i = 0; i < menuButtonList.Count; i++)
            {
                CircleMenuPosition[i] = new Point(370 + 300 * Math.Cos((step * i + 90) * Math.PI / 180), 50 + 100 * Math.Sin((step * i+90) * Math.PI / 180));
                menuButtonList[i].HorizontalAlignment = HorizontalAlignment.Left;
                menuButtonList[i].VerticalAlignment = System.Windows.VerticalAlignment.Top;
                menuButtonList[i].Margin = new Thickness(CircleMenuPosition[i].X, CircleMenuPosition[i].Y, 0, 0);
            }
        }
    }
}
