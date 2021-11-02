using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;

namespace pakeman
{
    public enum TileType { Standard = 0, Wall = 1, Gate = 2, GhostSpawn = 3, PowerUpSpawn = 4, PacmanSpawn = 5 }
    public class Tile : BaseObject
    {
        public TileType type;
        private Rectangle srcrec = new Rectangle(0,0,32,32);
        private SpriteEffects SpriteFx;
        private float rotation;
        public int posX;
        public int posY;
        public void DetermineWallTex(Tile t1, Tile t2, Tile t3, Tile t4, Tile t5, Tile t6, Tile t7, Tile t8 )
        {
            bool topLeftCorner = false;
            bool topRightCorner = false;
            bool botLeftCorner = false;
            bool botRightCorner = false;

            bool leftWall = false;
            bool topWall = false;
            bool rightWall = false;
            bool botWall = false;

            /*
            t1 t2 t3
            t4    t5
            t6 t7 t8
            */

            if (t1.type == TileType.Wall) { topLeftCorner = true; }  else { topLeftCorner = false; }
            if (t2.type == TileType.Wall) { topWall = true; }        else { topWall = false; }
            if (t3.type == TileType.Wall) { topRightCorner = true; } else { topRightCorner = false; }
            if (t4.type == TileType.Wall) { leftWall = true; }       else { leftWall = false; }
            if (t5.type == TileType.Wall) { rightWall = true; }      else { rightWall = false; }
            if (t6.type == TileType.Wall) { botLeftCorner = true; }  else { botLeftCorner = false; }
            if (t7.type == TileType.Wall) { botWall = true; }        else { botWall = false; }
            if (t8.type == TileType.Wall) { botRightCorner = true; } else { topRightCorner = false; }
            
            srcrec.X = 32; srcrec.Y = 96;

            if (topLeftCorner && topRightCorner && botLeftCorner && botRightCorner && topWall && leftWall && rightWall && botWall ) 
            { 
                srcrec.X = 32; srcrec.Y = 64; 
                SpriteFx = SpriteEffects.None;
                rotation = MathHelper.ToRadians(0);
            }
            //single walls
            { 
                if (!topWall && leftWall && rightWall && botWall) 
                { 
                    srcrec.X = 64; srcrec.Y = 64; 
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(0);
                }
                if (topWall && !leftWall && rightWall && botWall) 
                { 
                    srcrec.X = 64; srcrec.Y = 64;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(270);
                }
                if (topWall && leftWall && rightWall && !botWall)
                {
                    srcrec.X = 64; srcrec.Y = 64;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(180);
                }
                if (topWall && leftWall && !rightWall && botWall)
                {
                    srcrec.X = 64; srcrec.Y = 64;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
            }
            //double walls
            {
                if (topWall && !leftWall && !rightWall && botWall)
                {
                    srcrec.X = 64; srcrec.Y = 32;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
                if (!topWall && leftWall && rightWall && !botWall)
                {
                    srcrec.X = 64; srcrec.Y = 32;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(0);
                }
            }
            //outer corners
            {
                if (!topLeftCorner && botRightCorner && !topWall && !leftWall && rightWall && botWall)
                {
                    srcrec.X = 32; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(0);
                }
                if (!topRightCorner && botLeftCorner && !topWall && leftWall && !rightWall && botWall)
                {
                    srcrec.X = 32; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
                if (topLeftCorner && !botRightCorner && topWall && leftWall && !rightWall && !botWall)
                {
                    srcrec.X = 32; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(180);
                }
                if ( topRightCorner && !botLeftCorner  && topWall && !leftWall && rightWall && !botWall)
                {
                    srcrec.X = 32; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(270);
                }

            }
            //inner corners
            {
                if (!topLeftCorner && topRightCorner && botLeftCorner && botRightCorner && topWall && leftWall && rightWall && botWall)
                {
                    srcrec.X = 0; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(0);
                }
                if (topLeftCorner && !topRightCorner && botLeftCorner && botRightCorner && topWall && leftWall && rightWall && botWall)
                {
                    srcrec.X = 0; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
                if (topLeftCorner && topRightCorner && !botLeftCorner && botRightCorner && topWall && leftWall && rightWall && botWall)
                {
                    srcrec.X = 0; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(180);
                }
                if (topLeftCorner && topRightCorner && botLeftCorner && !botRightCorner && topWall && leftWall && rightWall && botWall)
                {
                    srcrec.X = 0; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(270);
                }

            }
            //double corners
            {
                if (!topLeftCorner && !botRightCorner && !topWall && !leftWall && rightWall && botWall)
                {
                    srcrec.X = 32; srcrec.Y = 32;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(0);
                }
                if (!topRightCorner && !botLeftCorner && !topWall && leftWall && !rightWall && botWall)
                {
                    srcrec.X = 32; srcrec.Y = 32;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
                if (!topLeftCorner && !botRightCorner && topWall && leftWall && !rightWall && !botWall)
                {
                    srcrec.X = 32; srcrec.Y = 32;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(180);
                }
                if (!topRightCorner && !botLeftCorner && topWall && !leftWall && rightWall && !botWall)
                {
                    srcrec.X = 32; srcrec.Y = 32;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(270);
                }
            }
            //end caps
            {
                if (topWall && !leftWall && !rightWall && !botWall)
                {
                    srcrec.X = 64; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(180);
                }
                if (!topWall && leftWall && !rightWall && !botWall)
                {
                    srcrec.X = 64; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
                if (!topWall && !leftWall && rightWall && !botWall)
                {
                    srcrec.X = 64; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(270);
                }
                if (!topWall && !leftWall && !rightWall && botWall)
                {
                    srcrec.X = 64; srcrec.Y = 0;
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(0);
                }
            }
            //inner T-junction

            //outer t-junction

            //miscellaneous

            //srcrec.X = 64; srcrec.Y = 128;
        }

        public void Draw(SpriteBatch spritebatch/* SpriteFont font*/)
        {
            Vector2 adjustedPos = new Vector2(pos.X + 16, pos.Y + 16);
            spritebatch.Draw(tex, adjustedPos, srcrec, Color.White, rotation, new Vector2(16, 16), 1, SpriteFx, 1);
            //spritebatch.DrawString(font, posX.ToString(), new Vector2(pos.X, pos.Y), Color.White);
            //spritebatch.DrawString(font, posY.ToString(), new Vector2(pos.X, pos.Y+12), Color.White);
        }

        public Tile(Vector2 pos, Rectangle size, TileType type, Texture2D tex, int posX, int posY) : base(pos, tex, size)
        {
            this.pos = pos;
            this.size = size;
            this.type = type;
            this.tex = tex;
            this.posX = posX;
            this.posY = posY;
        }
    }
}
