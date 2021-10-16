using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;

namespace konkey_kong
{
    public enum PickupType {Points = 0, Hammer = 1}

    public class Switch
    {
        public Vector2 pos;
        public Texture2D tex;
        public Rectangle srcRec = new Rectangle(0, 0, 40, 40);
        public Rectangle size = new Rectangle(0, 0, 40, 40);
        private int frame = 0;
        private float frameTimer;
        public bool toggled = false;
        public void Activated(float time)
        {
            frameTimer -= time;
            if (frameTimer <= 0 && frame < 2)
            {
                frameTimer = 120;
                frame++;
            }
            srcRec.X = frame * 40;
            if (frame == 2) { toggled = true; }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(tex, pos, srcRec, Color.White);
        }
        public Switch (Vector2 pos, Texture2D tex, Rectangle size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
        }

    }
    public class Hammer
    {
        public Vector2 pos;
        public Texture2D tex;

        public Rectangle srcRec = new Rectangle(0,0,100,40);
        public Rectangle size = new Rectangle(0,0, 120, 40);
        public float duration = 0;
        private double frameTimer;
        private int frame;  
        public void Animation(double time)
        {
            duration -= (float)time;
            size.X = (int)pos.X;
            size.Y = (int)pos.Y;
            frameTimer -= time;

            if (frameTimer <= 0 && frame < 5)
            {
                frameTimer = 40;
                frame++;
                if (frame > 4) { frame = 0; }
            }
            srcRec.Y = frame * 40;
        }

        public void Draw(SpriteBatch spritebatch) 
        {
            

            spritebatch.Draw(tex, pos, srcRec, Color.White);
        }

        public Hammer(Vector2 pos, Texture2D tex, Rectangle size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
        }

    }
    public class Pickup
    {
        public Vector2 pos;
        public Texture2D tex;
        public int value;
        private double frameTimer;
        private int frame;
        public Rectangle size;
        public PickupType type;
        Rectangle srcRec = new Rectangle(0, 0, 40, 40);

        public void Draw(SpriteBatch spriteBatch, double time)
        {
            if(type == PickupType.Points) { 
            frameTimer -= time;
            if (frameTimer <= 0 && frame < 5)
            {
                frameTimer = 90;
                frame++;
                if (frame > 4) { frame = 0; }
            }
            srcRec.X = frame * 40;
            }

            spriteBatch.Draw(tex, pos, srcRec, Color.White);
        }

        public Pickup(Vector2 pos, Texture2D tex, int value, Rectangle size, PickupType type)
        {
            this.pos = pos;
            this.tex = tex;
            this.value = value;
            this.size = size;
            this.type = type;
        }

        public Pickup()
        {
        }
    }

    class HealthBar
    {
        public Vector2 pos;
        public Texture2D tex;
        public bool lost;
        private double frameTimer;
        private int frame;
        Rectangle srcRec = new Rectangle(0, 0, 50, 50);

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

        public static void PointStringAnim(PointString ps, double time)
        {
            ps.duration -= time;
            ps.pos += ps.speed;
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
