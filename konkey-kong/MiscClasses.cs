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
    public enum PickupType {Points = 0, Powerup = 1}

    public class Pickup : BaseObject
    {
        public int value = 10;
        protected Rectangle srcRec = new Rectangle(0, 0, 8, 8);

        public void Draw(SpriteBatch spriteBatch, double time)
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
        public Powerup(Vector2 pos, Texture2D tex, Rectangle size) : base(pos, tex, size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
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
                frameTimer -= time;
                if (frameTimer <= 0 && frame < 3)
                {
                    frameTimer = 56;
                    frame++;
                }
            }
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
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos, double time)
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
