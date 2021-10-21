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
    public class DummyTile
    {
        public Vector2 pos;
        public Vector2 speed;
        public Texture2D tex;
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(tex, pos, Color.White);
        }

        public void Fall()
        {
            if (speed.Y < 4) { speed.Y *= (float)1.1; }
            pos += speed;
        }

        public DummyTile(Vector2 pos, Vector2 speed, TileType type, Texture2D tex)
        {
            this.pos = pos;
            this.speed = speed;
            this.tex = tex;
        }

        public DummyTile()
        {
        }
    }

    public enum TileType { Standard = 1, Ladder = 2, Blank = 3 }
    public class Tile
    {
        public Vector2 pos;
        public Rectangle size;
        public TileType type;
        public Texture2D tex;

        public void Draw(SpriteBatch spritebatch, SpriteFont font)
        {

            spritebatch.Draw(tex, pos, Color.White);
            //spritebatch.DrawString(font, number1.ToString(), new Vector2(pos.X, pos.Y), Color.White);
            //spritebatch.DrawString(font, number2.ToString(), new Vector2(pos.X, pos.Y+12), Color.White);
        }

        public Tile(Vector2 pos, Rectangle size, TileType type, Texture2D tex)
        {
            this.pos = pos;
            this.size = size;
            this.type = type;
            this.tex = tex;
        }

        public Tile()
        {
        }
    }
}
