using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace BreackOutLevelEditor
{
    public class Level
    {
        public string Name = "Lvl1";
        public List<GameObject> Objects = new List<GameObject>();
        public int Order = 0;
        public bool RatingFSK18 = false;
        public string Creator = "Unknown";
        public Texture2D MiniMap;

        public Level(string Name, int Order)
        {
            this.Name = Name;
            this.Order = Order;
        }

        public Level()
        { }

        public override string ToString()
        {
            return Name + (RatingFSK18 ? " (FSK 18)" : "");
        }
    }

    public class LevelPreview : Level
    {
        public Level lvl;
        
        public LevelPreview(Level lvl)
        {
            this.MiniMap = lvl.MiniMap;
            this.Name = lvl.Name;
            this.Objects = lvl.Objects;
            this.Order = lvl.Order;
            this.RatingFSK18 = lvl.RatingFSK18;
            this.lvl = lvl;
        }
    }

    public class GameObject
    {
        public string objType = "Default";
        public Point Location;

        #region custom Color

        public Color[] CustomColor = new Color[16];

        #endregion

        public GameObject(string type, Point Location)
        {
            this.Location = Location;
            objType = type;
        }
        public GameObject()
        {

        }
        public override string ToString()
        {
            return "obj." + objType;
        }
    }

    public class Score
    {
        public string User = "";
        public long Date = 0;
        public int HighScore = 0;

        public Score()
        { }

        public Score(int HighScore, string User, long Date)
        {
            this.HighScore = HighScore;
            this.Date = Date;
            this.User = User;
        }
    }
}
