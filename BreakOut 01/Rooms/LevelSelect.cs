using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using BreackOutLevelEditor;


namespace BreakOut_01.Rooms
{
    public class LevelSelect
    {
        ContentManager Content;
        GraphicsDevice graphic;

        Texture2D NoImg;
        Texture2D Cover;

        int PageIndex = 0;

        public Dictionary<int, List<LevelPreview>> Pages = new Dictionary<int, List<LevelPreview>>();

        public Level[] UsedLevel {
            set
            {
                Pages.Clear();
                int page = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!Pages.ContainsKey(page))
                    {
                        Pages.Add(page, new List<LevelPreview>());
                    }
                    Pages[page].Add(new LevelPreview(value[i]));
                }
            }
        }

        public LevelSelect(ContentManager Content, GraphicsDevice graphic)
        {
            this.Content = Content;
            this.graphic = graphic;
            NoImg = Content.Load<Texture2D>("NoImage");
            Cover = Content.Load<Texture2D>("Cover");
        }

        public void Draw(SpriteBatch sprite)
        {
            if (Pages.Count > 0)
            {
                List<LevelPreview> lvl = Pages[PageIndex];

                for(int i = 0; i < lvl.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            {
                                sprite.Draw(lvl[i].MiniMap == null ? NoImg : lvl[i].RatingFSK18 ? NoImg : lvl[i].MiniMap, new Rectangle(178, 88, 400, 206), Color.White);
                                sprite.Draw(Cover, new Rectangle(178, 88, 400, 206), Color.White);
                            }
                            break;
                        case 1:
                            {
                                sprite.Draw(lvl[i].MiniMap == null ? NoImg : lvl[i].RatingFSK18 ? NoImg : lvl[i].MiniMap, new Rectangle(703, 88, 400, 206), Color.White);
                                sprite.Draw(Cover, new Rectangle(703, 88, 400, 206), Color.White);
                            }
                            break;
                        case 2:
                            {
                                sprite.Draw(lvl[i].MiniMap == null ? NoImg : lvl[i].RatingFSK18 ? NoImg : lvl[i].MiniMap, new Rectangle(178, 428, 400, 206), Color.White);
                                sprite.Draw(Cover, new Rectangle(178, 428, 400, 206), Color.White);
                            }
                            break;
                        case 3:
                            {
                                sprite.Draw(lvl[i].MiniMap == null ? NoImg : lvl[i].RatingFSK18 ? NoImg : lvl[i].MiniMap, new Rectangle(703, 428, 400, 206), Color.White);
                                sprite.Draw(Cover, new Rectangle(703, 428, 400, 206), Color.White);
                            }
                            break;
                    }
                }
            }
        }
    }
}
