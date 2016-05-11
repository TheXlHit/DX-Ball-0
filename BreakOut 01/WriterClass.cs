using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace BreackOutLevelEditor
{
    public static class WriterClass
    {
        private const int HeaderOld = 0x4946;
        private const int HeaderNew = 0x26E4946;
        private const int HeaderNew2 = 0x1F04946;
        private const int HeaderNew3 = 0x3774946;
        private const int HeaderNew4 = 0x3F14946;
        private const int HeaderNew5 = 0x1C94946;

        private const int HeaderScores = 0x39C4946;

        public static Level[] ReadFromFile(string file, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphic)
        {
            Level[] temp = new Level[0];
            BinaryReader br = new BinaryReader(new FileStream(file, FileMode.Open));
            int Header = br.ReadInt32();
            if (Header == HeaderNew || Header == HeaderOld || Header == HeaderNew2 || Header == HeaderNew3 || Header == HeaderNew4 || Header == HeaderNew5)
            {
                int l = br.ReadInt32();
                for (int i = 0; i < l; i++)
                {
                    Array.Resize(ref temp, temp.Length + 1);
                    int f = temp.Length - 1;
                    temp[f] = new Level(br.ReadString(), br.ReadInt32());
                    temp[f].RatingFSK18 = br.ReadBoolean();

                    if (Header == HeaderNew3 || Header == HeaderNew4 || Header == HeaderNew5)
                    {
                        temp[f].Creator = br.ReadString();
                    }

                    if (Header == HeaderNew || Header == HeaderNew2 || Header == HeaderNew3 || Header == HeaderNew4 || Header == HeaderNew5)
                    {
                        int le = br.ReadInt32();
                        using (MemoryStream ms = new MemoryStream(br.ReadBytes(le)))
                        {
                            temp[f].MiniMap = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(graphic, ms);
                        }
                    }
                    int ll = br.ReadInt32();

                    for (int ii = 0; ii < ll; ii++)
                    {
                        GameObject g = new GameObject();
                        g.Location = new System.Drawing.Point(br.ReadInt32(), br.ReadInt32());
                        g.objType = br.ReadString();
                        if (Header == HeaderNew2 || Header == HeaderNew3)
                        {
                            Array.Resize(ref g.CustomColor, 1);
                            g.CustomColor = new Color[] { Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte()) };
                        }
                        if (Header == HeaderNew4)
                        {
                            Array.Resize(ref g.CustomColor, 4);

                            Color c1 = Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());
                            Color c2 = Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());
                            Color c3 = Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());
                            Color c4 = Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());

                            g.CustomColor = new Color[] {
                                c1, c2, c3 ,c4
                            };
                        }
                        if (Header == HeaderNew5)
                        {
                            Color[] col = new Color[br.ReadInt32()];
                            for (int Colori = 0; Colori < col.Length; Colori++)
                            {
                                byte cr = br.ReadByte();
                                byte cg = br.ReadByte();
                                byte cb = br.ReadByte();
                                col[Colori] = Color.FromArgb(255, cr, cg, cb);
                            }
                            g.CustomColor = col;
                        }

                        temp[f].Objects.Add(g);
                    }
                }
            }

            br.Close();
            return temp;
        }

        public static ScoreList ReadScoresFromFile(string file)
        {
            BinaryReader br = new BinaryReader(new FileStream(file, FileMode.Open));
            ScoreList list = new ScoreList();

            if (br.ReadInt32() == HeaderScores)
            {
                int count = br.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    Score s = new Score();
                    s.User = br.ReadString();
                    s.Date = br.ReadInt64();
                    s.HighScore = br.ReadInt32();
                    list.AddScore(s, false);
                }
            }

            br.Close();
            return list;
        }

        public static void WriteScoresToFile(ScoreList list, string file)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(file, FileMode.Create));

            bw.Write(HeaderScores);
            bw.Write(list.Count);
            foreach (Score sc in list)
            {
                bw.Write(sc.User);
                bw.Write(sc.Date);
                bw.Write(sc.HighScore);
            }
            bw.Close();
        }
    }

    public class ScoreList : List<Score>
    {
        public int MaxScores = 100;

        public ScoreList() : base()
        { }

        public void AddScore(Score score, bool Write = true)
        {
            if (this.Count >= MaxScores)
            {
                Score Lowest = GetLowestScore;
                if (score.HighScore > Lowest.HighScore)
                {

                    this.Remove(Lowest);
                    this.Add(score);
                }
            }
            else
            {
                this.Add(score);
            }

            while (Count > MaxScores)
            {
                Remove(GetLowestScore);
            }

            if (Count > 0)
            {
                if (BreakOut_01.Game1.roomMenu != null)
                {
                    BreakOut_01.Game1.roomMenu.Resort();
                }
            }

            if (Write)
            {
                WriterClass.WriteScoresToFile(this, "Scores_0.bin");
            }
        }

        internal List<Score> GetOrdered()
        {
            return this.OrderByDescending(o => o.HighScore).ToList();
        }

        public Score GetLowestScore
        {
            get
            {
                Score s = GetHighestScore;
                foreach (Score sc in this)
                {
                    if (sc.HighScore < s.HighScore)
                    {
                        s = sc;
                    }
                }
                return s;
            }
        }

        public Score GetHighestScore
        {
            get
            {
                Score s = new Score();
                foreach (Score sc in this)
                {
                    if (sc.HighScore > s.HighScore)
                    {
                        s = sc;
                    }
                }
                return s;
            }
        }

        public bool IsInRanking(int Score)
        {
            if (GetLowestScore.HighScore < Score || this.Count != MaxScores)
            { return true; }
            return false;
        }
    }
}
