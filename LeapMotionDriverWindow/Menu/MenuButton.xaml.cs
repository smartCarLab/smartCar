using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeapMotionDriverWindow.Menu
{
    /// <summary>
    /// Interaction logic for MenuButton.xaml
    /// </summary>
    public partial class MenuButton : Grid
    {
        public int ButtonIndex = -1;
        public MenuTemplate menuParent = null;
        private int moveIndex = -1;
        private bool isSelected = false;        
        private double bigSize = EnvironmentSetting.Default["bigSize"];
        private double mediumSize = EnvironmentSetting.Default["mediumSize"];
        private double smallSize = EnvironmentSetting.Default["smallSize"];

        public MenuTemplate MenuParent
        {
            get
            {
                if (menuParent == null)
                    menuParent = ((this.Parent as Grid).Parent as MenuTemplate);
                return menuParent;
            }
        }
        public int SelectIndex
        {
            set
            {
                moveIndex = (ButtonIndex - value < 0) ? ButtonIndex - value + (this.Parent as Grid).Children.Count : ButtonIndex - value;
                Panel.SetZIndex(this, (int)(MenuParent.CircleMenuPosition[moveIndex].Y));
            }
        }
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (value)
                {

                    this.Opacity = 1.0;
                    scaleTransform.ScaleX = bigSize;
                    scaleTransform.ScaleY = bigSize;
                    var shiftX = MenuParent.CircleMenuPosition[0].X - this.Margin.Left;
                    var shiftY = MenuParent.CircleMenuPosition[0].Y - this.Margin.Top;
                    var translateAnimationX = new DoubleAnimation(translate.X, shiftX, new Duration(TimeSpan.FromMilliseconds(200)));
                    var translateAnimationY = new DoubleAnimation(translate.Y, shiftY, new Duration(TimeSpan.FromMilliseconds(200)));
                    var scaleAnimation = new DoubleAnimation(mediumSize, bigSize, new Duration(TimeSpan.FromMilliseconds(200)));
                    translate.BeginAnimation(TranslateTransform.XProperty, translateAnimationX);
                    translate.BeginAnimation(TranslateTransform.YProperty, translateAnimationY);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                }
                else
                {                    
                    this.Opacity = 0.3;
                    var shiftX = MenuParent.CircleMenuPosition[moveIndex].X - this.Margin.Left;
                    var shiftY = MenuParent.CircleMenuPosition[moveIndex].Y - this.Margin.Top;                    
                    var translateAnimationX = new DoubleAnimation(translate.X, shiftX, new Duration(TimeSpan.FromMilliseconds(200)));
                    var translateAnimationY = new DoubleAnimation(translate.Y, shiftY, new Duration(TimeSpan.FromMilliseconds(200)));                    
                    translate.BeginAnimation(TranslateTransform.XProperty, translateAnimationX);
                    translate.BeginAnimation(TranslateTransform.YProperty, translateAnimationY);
                    DoubleAnimation scaleAnimation;
                    if (moveIndex == 1 || moveIndex == MenuParent.CircleMenuPosition.Length - 1)
                    {
                        scaleAnimation = new DoubleAnimation(scaleTransform.ScaleX, mediumSize, new Duration(TimeSpan.FromMilliseconds(200)));
                    }
                    else
                    {
                        scaleAnimation = new DoubleAnimation(scaleTransform.ScaleX,smallSize, new Duration(TimeSpan.FromMilliseconds(200)));
                    }
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
                }
                isSelected = value;
            }
        }

        public MenuButton()
        {
            InitializeComponent();
        }
    }
}
