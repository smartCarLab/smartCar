using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeapMotionDriverWindow
{
    /// <summary>
    /// Interaction logic for DockView.xaml
    /// </summary>
    public partial class DockView : Grid
    {
        public DockView()
        {
            InitializeComponent();
            this.topAnimation.SetGif(AnimationObjectList.Default["Top"]);
            this.topAnimation.SetRepeatBehavior(System.Windows.Media.Animation.RepeatBehavior.Forever);
            this.topAnimation.Start();
            this.topAnimation.SetSpeed(EnvironmentSetting.Default["topAnimationSpeed"]);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var iconPaths = new DirectoryInfo("Images/DockIcon").GetFiles().Where(p=>EnvironmentSetting.imgExtensionFilter.Contains(p.Extension.ToLower())).Select(p=>p.FullName).ToArray();
                if (iconPaths != null)
                {
                    for (int i = 0; i < iconPaths.Length; i++)
                    {
                        var cell = new Grid() { Width = this.Height, Height = this.Height, Background = new ImageBrush(new BitmapImage(new Uri(iconPaths[i], UriKind.RelativeOrAbsolute))) { Stretch = Stretch.Uniform } };
                        DockPanel.SetDock(cell, Dock.Left);
                        this.dockIconContainer.Children.Add(cell);
                    }
                }
            }
        }
    }
}
