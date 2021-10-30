using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace pakeman
{
    public class BaseObject
        {
        public Vector2 pos;
        public Texture2D tex;
        public Rectangle size;

        public BaseObject(Vector2 pos, Texture2D tex, Rectangle size)
        {
            this.pos = pos;
            this.tex = tex;
            this.size = size;
        }
    }
}
