using Leap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for DockMainScene.xaml
    /// </summary>
    public partial class DockMainScene : Grid
    {
        private Controller leapController;
        private LeapListener leapListener;
        private MenuWindow menuWindow = new MenuWindow();
        private Stopwatch menuWatcher = new Stopwatch();
        private Stopwatch swipeWatcher = new Stopwatch();
        private Uri videoSourceUri;
        private bool isVideoDragMode = false;
        private double windowDefaultWidth = EnvironmentSetting.Default["windowDefaultWidth"];
        private double windowDefaultHeight = EnvironmentSetting.Default["windowDefaultHeight"];
        private int closeWindowDelay = EnvironmentSetting.Default["closeWindowDelay"];
        private int swipeDetectionDelay = EnvironmentSetting.Default["swipeDetectionDelay"];
        private float gestureSwipeMinLength = EnvironmentSetting.Default["gestureSwipeMinLength"];
        private float gestureSwipeMinVelocity = EnvironmentSetting.Default["gestureSwipeMinVelocity"];

        private bool IsMenuShow
        {
            get
            {
                return menuWindow.Visibility==Visibility.Visible;
            }            
        }

        public DockMainScene()
        {
            InitializeComponent();
            videoSourceUri = new Uri(new DirectoryInfo("Video").GetFiles().Where(video => EnvironmentSetting.VideoFileFilter.Contains(video.Extension.ToLower())).Select(video => video.FullName).FirstOrDefault(), UriKind.RelativeOrAbsolute);
            this.mainMediaElement.Source = videoSourceUri;
            this.mainMediaElement.Stretch = Stretch.Fill;
            menuWindow.OnMenuChanged += menuWindow_OnMenuChanged;
            menuWindow.OnButtonSelected += menuWindow_OnButtonSelected;
            leapController = new Controller();
            leapListener = new LeapListener();            
            leapController.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
            leapController.AddListener(leapListener);
            leapController.Config.SetFloat("Gesture.Swipe.MinLength", gestureSwipeMinLength);
            leapController.Config.SetFloat("Gesture.Swipe.MinVelocity", gestureSwipeMinVelocity);
            leapController.Config.Save();
            leapListener.OnPalmOpened += leapListener_OnPalmOpened;
            leapListener.OnSwiped += leapListener_OnSwiped;
            leapListener.OnGriped += leapListener_OnGriped;
            leapListener.OnGripMove += leapListener_OnGripMove;
            ThreadPool.QueueUserWorkItem(new WaitCallback(MenuWatcherLoop));
        }
        private void menuWindow_OnMenuChanged(object sender, EventArgs e)
        {
            if (!(sender is Menu.VideoMenu))
            {
                if (isVideoDragMode) isVideoDragMode = false;
            }
        }
        private void leapListener_OnGripMove(System.Windows.Vector position)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!isVideoDragMode) return;
                int step = (int)this.Width / 3;
                this.mainMediaElement.Width = windowDefaultWidth;
                this.mainMediaElement.Height = windowDefaultHeight;
                if (position.X >= 0 && position.X < step)
                {
                    this.mainMediaElement.Margin = new Thickness(step / 2 - this.mainMediaElement.Width / 2, this.workView.Height / 2 - this.mainMediaElement.Height / 2, 0, 0);
                }
                else if (position.X >= step && position.X < 2 * step)
                {
                    this.mainMediaElement.Margin = new Thickness(step + step / 2 - this.mainMediaElement.Width / 2, this.workView.Height / 2 - this.mainMediaElement.Height / 2, 0, 0);
                }
                else if (position.X >= 2 * step && position.X <= 3 * step)
                    this.mainMediaElement.Margin = new Thickness(2 * step + step / 2 - this.mainMediaElement.Width / 2, this.workView.Height / 2 - this.mainMediaElement.Height / 2, 0, 0);                
            }));
        }
        public void FullScreenGoogleMap()
        {
            if (!(this.statusView.route.Parent == this.statusView.statusContainer)) return;
            if (this.statusView.statusContainer.Children.Contains(this.statusView.route))
            {
                if (this.mainMediaElement.Visibility == Visibility.Visible)
                    HideVideo();
                this.statusView.statusContainer.Children.Remove(this.statusView.route);
                this.statusView.route.Width = this.workView.Width;
                this.statusView.route.Height = this.workView.Height;
                Grid.SetColumnSpan(googleMapView, 3);
                this.googleMapView.Children.Add(this.statusView.route);
                this.statusView.ReFlash();
            }
        }
        public void HideGoogleMap()
        {
            if (!(this.statusView.route.Parent == this.googleMapView)) return;
            if (this.googleMapView.Children.Contains(this.statusView.route))
            {
                this.googleMapView.Children.Remove(this.statusView.route);
                this.statusView.route.Width = 350;
                this.statusView.route.Height = this.statusView.statusContainer.Height;
                this.statusView.statusContainer.Children.Add(this.statusView.route);
                this.statusView.ReFlash();
            }
        }
        public void FullScreenVideo()
        {
            isVideoDragMode = false;                        
            if (this.googleMapView.Children.Contains(this.statusView.route)) HideGoogleMap();
            this.mainMediaElement.Margin = new Thickness(0, 0, 0, 0);
            this.mainMediaElement.Width = this.workView.Width;
            this.mainMediaElement.Height = this.workView.Height;            
            this.mainMediaElement.Position = TimeSpan.Zero;
            this.mainMediaElement.Play();
            this.mainMediaElement.Visibility = Visibility.Visible;
        }
        public void HideVideo()
        {
            isVideoDragMode = false;
            if (this.mainMediaElement.Visibility == Visibility.Hidden) return;
            this.mainMediaElement.Visibility = Visibility.Hidden;
            this.mainMediaElement.Stop();
        }
        public void StartVideoDragMode()
        {
            HideGoogleMap();            
            isVideoDragMode = true;
            this.mainMediaElement.Width = windowDefaultWidth;
            this.mainMediaElement.Height = windowDefaultHeight;
            int step = (int)this.Width / 3;
            this.mainMediaElement.Margin = new Thickness(step + step / 2 - this.mainMediaElement.Width / 2, this.workView.Height / 2 - this.mainMediaElement.Height / 2, 0, 0);
            this.mainMediaElement.Position = TimeSpan.Zero;
            this.mainMediaElement.Play();
            this.mainMediaElement.Visibility = Visibility.Visible;
        }
        private void menuWindow_OnButtonSelected(string command)
        {
            var commandMethod = typeof(DockMainScene).GetMethod(command);
            if (commandMethod != null)
                commandMethod.Invoke(this, new object[] { });
        }
        private void leapListener_OnGriped(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                menuWindow.Execute();
            }));
        }
        private void leapListener_OnSwiped(System.Windows.Vector direction)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {                
                if (!IsMenuShow) return;
                if (swipeWatcher.IsRunning && swipeWatcher.ElapsedMilliseconds < swipeDetectionDelay) return;
                int tmpIndex = menuWindow.SelectionIndex;

                if(direction.X>0)
                    menuWindow.SelectionIndex++;
                else
                    menuWindow.SelectionIndex--;

                if (tmpIndex != menuWindow.SelectionIndex)
                    swipeWatcher.Restart();
                menuWatcher.Restart();
            }));
        }
        private void leapListener_OnPalmOpened(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                menuWatcher.Restart();
                if (!IsMenuShow)
                {
                    menuWindow.SetContent(new Menu.MainMenu());
                    this.Opacity = 0.8;
                    try
                    {
                        menuWindow.ShowDialog();
                    }
                    catch { }
                    this.Opacity = 1.0;
                }
            }));
        }
        private void MenuWatcherLoop(object o)
        {
            while (true)
            {
                Thread.Sleep(50);
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (menuWatcher.IsRunning && menuWatcher.ElapsedMilliseconds > closeWindowDelay && IsMenuShow)
                    {
                        menuWatcher.Stop();
                        if (isVideoDragMode) isVideoDragMode = false;
                        menuWindow.Close();
                    }
                }));
            }
        }
        private void mainMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.mainMediaElement.Position = TimeSpan.Zero;
            this.mainMediaElement.Play();
        }
    }
}
