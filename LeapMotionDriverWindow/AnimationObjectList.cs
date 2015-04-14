using ATAnimation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace LeapMotionDriverWindow
{
    public class AnimationObjectList
    {
        private static AnimationObjectList defaultInstance = new AnimationObjectList();
        private DirectoryInfo objectsDirectory = new DirectoryInfo("AnimationObjects");
        private Dictionary<string, PowerfulGif> objectSet = new Dictionary<string, PowerfulGif>();

        public static Dictionary<string, PowerfulGif> Default
        {
            get
            {
                return defaultInstance.objectSet;
            }
        }

        public static void Init()
        {
            defaultInstance.StaticInitial();
        }
        public AnimationObjectList()
        {
            Initilize();
        }
        private void Initilize()
        {
            InitialObjectStoryboard();
        }
        private void InitialObjectStoryboard()
        {
            GetSetFromDirectory(objectsDirectory, objectSet);
        }
        private void GetSetFromDirectory(DirectoryInfo objectDirectory, Dictionary<string, PowerfulGif> gifSet)
        {
            var directoryOfObject = objectDirectory.GetDirectories();
            for (int i = 0; i < directoryOfObject.Length; i++)
            {
                gifSet.Add(directoryOfObject[i].Name, GetStoryboard(directoryOfObject[i]));
            }
        }
        private PowerfulGif GetStoryboard(DirectoryInfo dir)
        {
            if (dir == null)
                throw new FileNotFoundException("Animation source not found!");
            PowerfulGif sbn = PowerfulGif.GetStoryboardFromDirectoryMMF(dir, 100);
            return sbn;
        }
        private void StaticInitial()
        {
        }
    }
}
