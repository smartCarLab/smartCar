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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeapMotionDriverWindow
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        public delegate void CommandHandler(string command);
        public event CommandHandler OnButtonSelected;
        public event EventHandler OnMenuChanged;
        private List<Menu.MenuButton> menuButtonList = new List<Menu.MenuButton>();
        private int selectionIndex;

        public int SelectionIndex
        {
            get
            {
                return selectionIndex;
            }
            set
            {
                if (value < 0) value = menuButtonList.Count - 1;
                if (value > menuButtonList.Count - 1) value = 0;
                selectionIndex = value;                
                foreach (var button in menuButtonList)
                {
                    if (button == menuButtonList[selectionIndex]) continue;
                    button.SelectIndex = selectionIndex;
                    button.IsSelected = false;
                }                
                menuButtonList[selectionIndex].IsSelected = true;                
            }
        }        

        public MenuWindow()
        {            
            InitializeComponent();
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - this.Width / 2;
            this.Topmost = true;
            this.Top = 350.0 / 768.0 * System.Windows.SystemParameters.PrimaryScreenHeight;
        }
        public void SetContent(Menu.MenuTemplate menu)
        {
            if(this.OnMenuChanged!=null)
                this.OnMenuChanged(menu, EventArgs.Empty);
            this.container.Children.Clear();
            this.container.Children.Add(menu);            
            this.menuButtonList = menu.menuButtonList;
            if(this.menuButtonList.Count>1)
                SelectionIndex = this.menuButtonList.Count / 2;            
        }
        public void Execute()
        {
            if (this.container.Children.Count>0&&this.container.Children[0].GetType() == typeof(Menu.MainMenu))
            {
                if(menuButtonList[0].IsSelected)
                    SetContent(new Menu.GMapMenu());
                else if(menuButtonList[1].IsSelected)
                    SetContent(new Menu.VideoMenu());
            }
            else if (this.container.Children.Count > 0 && this.container.Children[0].GetType() == typeof(Menu.GMapMenu))
            {
                if (menuButtonList[0].IsSelected)
                {
                    if (OnButtonSelected != null)
                    {                        
                        OnButtonSelected("FullScreenGoogleMap");
                    }
                }
                else if (menuButtonList[1].IsSelected)
                {
                    if (OnButtonSelected != null)
                    {
                        OnButtonSelected("HideGoogleMap");
                    }                    
                }
                else if (menuButtonList[2].IsSelected)
                {
                    SetContent(new Menu.MainMenu());
                }
            }
            else if (this.container.Children.Count > 0 && this.container.Children[0].GetType() == typeof(Menu.VideoMenu))
            {
                if (menuButtonList[0].IsSelected)
                {
                    if (OnButtonSelected != null)
                    {
                        OnButtonSelected("FullScreenVideo");
                    }
                }
                else if (menuButtonList[1].IsSelected)
                {
                    if (OnButtonSelected != null)
                    {
                        OnButtonSelected("StartVideoDragMode");
                    }
                }  
                else if (menuButtonList[2].IsSelected)
                {
                    if (OnButtonSelected != null)
                    {
                        OnButtonSelected("HideVideo");
                    }
                }
                else if (menuButtonList[3].IsSelected)
                {
                    SetContent(new Menu.MainMenu());
                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }        
    }
}
