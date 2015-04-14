using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for StatusView.xaml
    /// </summary>
    public partial class StatusView : Grid
    {
        public Awesomium.Windows.Controls.WebControl GMDisplay = new Awesomium.Windows.Controls.WebControl();   

        public StatusView()
        {
            InitializeComponent();
            GMDisplay.Cursor = Cursors.None;
            this.statusAnimaiton.SetGif(AnimationObjectList.Default["Speed"]);
            this.statusAnimaiton.SetRepeatBehavior(System.Windows.Media.Animation.RepeatBehavior.Forever);
            this.statusAnimaiton.Start();
            this.statusAnimaiton.SetSpeed(5);
            Init();
            Loop();
        }
        public void ReFlash()
        {
            this.GMDisplay.Reload(false);
        }
        private void Loop()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var now = DateTime.Now.ToString("yyyyMMddHHmmss");
                        this.SetNumberImage(Y1, now, 0);
                        this.SetNumberImage(Y2, now, 1);
                        this.SetNumberImage(Y3, now, 2);
                        this.SetNumberImage(Y4, now, 3);
                        this.SetNumberImage(M1, now, 4);
                        this.SetNumberImage(M2, now, 5);
                        this.SetNumberImage(D1, now, 6);
                        this.SetNumberImage(D2, now, 7);
                        this.SetNumberImage(H1, now, 8);
                        this.SetNumberImage(H2, now, 9);
                        this.SetNumberImage(m1, now, 10);
                        this.SetNumberImage(m2, now, 11);
                        this.SetNumberImage(s1, now, 12);
                        this.SetNumberImage(s2, now, 13);
                    }));
                }
            }));
        }
        private void SetNumberImage(Grid numberPanel, string now, int index)
        {
            numberPanel.Background = new ImageBrush(new BitmapImage(new Uri("Images/Number/" + now.Substring(index, 1) + ".png", UriKind.RelativeOrAbsolute))) { Stretch = Stretch.Uniform };
        }
        private void Init()
        {            
            this.GMDisplay.LoadingFrameComplete += GMDisplay_LoadingFrameComplete;
            this.route.Children.Add(this.GMDisplay);
            Panel.SetZIndex(GMDisplay, 100);
            GMDisplay.Visibility = Visibility.Hidden;
            LoadPath("route.html");
        }
        private void GMDisplay_LoadingFrameComplete(object sender, Awesomium.Core.FrameEventArgs e)
        {
            if (this.GMDisplay.Visibility == Visibility.Hidden) this.GMDisplay.Visibility = Visibility.Visible;
        }
        private void LoadPath(string s)
        {
            string path = System.IO.Path.GetFullPath(s);
            char[] dst = new char[path.Length - 2];
            path.CopyTo(2, dst, 0, path.Length - 2);
            path = new string(dst);
            path = "file://127.0.0.1\\c$" + path;
            GMDisplay.Source = new Uri(path, UriKind.Absolute);
        }
    }
}
