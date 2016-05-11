using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BreakOut_01.Elements
{
    public class HitObject
    {
        public Texture2D Texture;
        public Texture2D TextureX2;
        public Texture2D ImageTexture2D;
        public Vector2 Position;
        public Vector2 Size;
        public string objType = "";
        ContentManager Content;
        GraphicsDevice graphic;
        float BlockSite = 4;

        Color[] CustomColor = new Color[] { Color.White };
        Color AverageColor = Color.White;

        public HitObject(string objType, Texture2D Texture, Texture2D TextureX2, Vector2 Pos, Vector2 Size, ContentManager Content, GraphicsDevice graphic, Color[] CustomColor)
        {
            this.Texture = Texture;
            this.TextureX2 = TextureX2;
            this.Position = Pos;
            this.Size = Size;
            this.objType = objType;
            this.Content = Content;
            this.graphic = graphic;
            this.CustomColor = CustomColor;

            BlockSite = (float)Math.Sqrt(CustomColor.Length);
            AverageColor = CutColor(CustomColor);
            ImageTexture2D = GenerateImage(CustomColor, graphic);

        }

        public void Draw(SpriteBatch batch)
        {
            #region Generate Advanced Block Image

            if (ImageTexture2D == null)
            {
                ImageTexture2D = GenerateImage(CustomColor, graphic);
            }

            #endregion

            #region DrawBlock

            int PosX = (int)(Position.X);
            int PosY = (int)(Position.Y);
            int SizeX = (int)(Size.X + .5f);
            int SizeY = (int)(Size.Y + 1f);

            #region Draw Special Block

            if (objType == "sp_obj_01" && Game1.ShowBlockImage)
            {
                if (Game1.ShowAdvancedBlockImage)
                {
                    switch (CustomColor.Length)
                    {
                        case 1:
                            {
                                batch.Draw(Texture, new Rectangle(PosX, PosY, SizeX, SizeY), AverageColor);
                            }
                            break;
                        default:
                            {
                                batch.Draw(ImageTexture2D, new Rectangle(PosX, PosY, SizeX, SizeY), Color.White);
                            }
                            break;
                    }
                }
                else
                {
                    batch.Draw(Texture, new Rectangle(PosX, PosY, SizeX, SizeY), AverageColor);
                }
            }

            #endregion

            #region Draw Block Normal

            else
            {
                batch.Draw(Texture, new Rectangle(PosX, PosY, SizeX, SizeY), Color.White);
            }

            #endregion

            #endregion

            #region Draw Hitbox

            if (Game1.ShowHitbox)
            {
                Game1.DrawRectangle(batch, Game1.HitLighting, new Rectangle(PosX, PosY, SizeX, SizeY), Color.White);
            }

            #endregion
        }

        public HitObjectOption Destroy()
        {
            switch (objType)
            {
                case "Type02":
                    {
                        return new HitObjectOption(this, true);
                    }
                case "Type07":
                    {
                        objType = "Type08";
                        Texture = Content.Load<Texture2D>(objType);
                        return null;
                    }
                case "Type09":
                    {
                        return null;
                    }
                default:
                    {
                        return new HitObjectOption(this, false);
                    }
            }
        }

        private Color CutColor(Color[] col)
        {
            int cR = 0;
            int cG = 0;
            int cB = 0;

            foreach (Color c in col)
            {
                cR += c.R;
                cG += c.G;
                cB += c.B;
            }

            return new Color(cR / col.Length, cG / col.Length, cB / col.Length, 255);
        }

        private Texture2D GenerateImage(Color[] col, GraphicsDevice graphics)
        {
            if (graphics != null)
            {
                bool isHD = IsHD(graphics);
                float SizeX = isHD ? 84f : 42f;
                float SizeY = isHD ? 40f : 20f;

                RenderTarget2D render = new RenderTarget2D(graphics, (int)SizeX, (int)SizeY);
                SpriteBatch batch = new SpriteBatch(graphics);

                graphics.SetRenderTarget(render);

                batch.Begin();

                int i = 0;
                float XCount = SizeX / BlockSite;
                float YCount = SizeY / BlockSite;

                for (double by = 0; by < SizeY; by += YCount)
                {
                    for (double bx = 0; bx < SizeX; bx += XCount)
                    {
                        if (i < col.Length)
                        {
                            batch.Draw(isHD ? TextureX2 : Texture,
                                new Rectangle((int)bx, (int)by, (int)(SizeX / BlockSite + 1f), (int)(SizeY / BlockSite + 1f)),
                                new Rectangle((int)bx, (int)by, (int)((SizeX / BlockSite + 1f)), (int)(SizeY / BlockSite + 1f)),
                                col[i++]);
                        }
                    }
                }

                batch.End();

                graphic.SetRenderTarget(null);

                return render;
            }
            return null;
        }

        public bool IsHD(GraphicsDevice graphic)
        {
            if (graphic.Viewport.Width >= 1600 && graphic.Viewport.Height >= 900)
            {
                return true;
            }
            return false;
        }
    }


    public class HitObjectOption
    {
        public HitObject obj;
        public bool IsExplosive = false;

        public HitObjectOption(HitObject obj, bool isExp)
        {
            this.obj = obj;
            this.IsExplosive = isExp;
        }
    }
}
