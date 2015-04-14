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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeapMotionDriverWindow.Menu
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : MenuTemplate
    {
        public MainMenu() : base()
        {
            InitializeComponent();
            for (int i = 0; i < menuContainer.Children.Count; i++)
            {                
                this.menuButtonList.Add(menuContainer.Children[i] as Menu.MenuButton);
                this.menuButtonList[i].ButtonIndex = i;
            }
            ArrangeCircleMenu();
        }
    }
}
