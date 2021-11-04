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
    public enum PickupType {Points = 0, PowerupEatWall = 1, PowerupEatGhost = 2}

    public class Pickup : BaseObject
    {
        public int value = 10;
        public PickupType type = PickupType.Points;
        protected Rectangle srcRec = new Rectangle(0, 0, 8, 8);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, srcRec, Color.White);
        }

        public Pickup(Vector2 pos, Texture2D tex, Rectangle size) : base(pos, tex, size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
        }
    }

    public class BigPickup : Pickup
    {
        public void BigPickupCreation()
        {
            srcRec.Width = 32;
            srcRec.Height = 32;
        }
        public BigPickup(Vector2 pos, Texture2D tex, Rectangle size) : base(pos, tex, size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
        }
    }

    public class Powerup : Pickup
    {
        private double frameTimer = 56;
        private int frame = 0;
        public void Anim(double time)
        {
            srcRec.Width = 32;
            srcRec.Height = 32;
            frameTimer -= time;
                if (frameTimer <= 0 && frame < 8) 
                {
                    frameTimer = 56;
                    frame++;
                    if (frame > 7) { frame = 0; }
                }
            srcRec.X = frame * size.Width;
        }
        public Powerup(Vector2 pos, Texture2D tex, Rectangle size, PickupType type) : base(pos, tex, size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
            this.type = type;
        }
    }

    public class Button : BaseObject
    {
        string printedString;
        public Button(Vector2 pos, Texture2D tex, Rectangle size, string printedString) : base(pos, tex, size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
            this.printedString = printedString;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            Vector2 adjustedPos = pos;
            
            adjustedPos.Y = pos.Y + (size.Height /2 ) - spriteFont.MeasureString(printedString).Y / 2;
            adjustedPos.X = pos.X + (size.Width / 2) - spriteFont.MeasureString(printedString).X / 2;

            spriteBatch.Draw(tex, pos, Color.White);
            spriteBatch.DrawString(spriteFont, printedString, adjustedPos, Color.White);
        }

        public bool Update(Vector2 mousePos)
        {
            if (size.Contains(mousePos))
            {
                return true;
            } else { return false; }
        }
    }

    class HealthBar
    {
        public Vector2 pos;
        public Texture2D tex;
        public bool lost;
        private double frameTimer;
        private int frame;
        Rectangle srcRec = new Rectangle(0, 0, 32, 32);

        public void Draw(SpriteBatch spriteBatch, double time)
        {

            if (lost)
            {
                frame = 1;
            }
            else { frame = 0; }
            srcRec.X = frame * 50;


            spriteBatch.Draw(tex, pos, srcRec, Color.White);
        }

        public HealthBar()
        {
        }
        public HealthBar(Vector2 pos, Texture2D tex, bool lost) { this.pos = pos; this.tex = tex; this.lost = lost; }
    }

    public class PointString
    {
        public double duration = 3000;
        public int value;
        public Vector2 pos;
        private Vector2 speed = new Vector2(0, -1);
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, ($"+{value}"), pos, Color.White);
        }

        public void PointStringAnim(double time)
        {
            duration -= time;
            pos += speed;
        }

        public PointString(int value, Vector2 pos)
        {
            this.value = value;
            this.pos = pos;
        }

        public PointString()
        {
        }
    }
}
