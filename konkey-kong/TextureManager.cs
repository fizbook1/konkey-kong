using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    public class TextureManager
    {
        public Texture2D health, ghost, point, blank, wallSheet, pakeman, pakemandeath, pakemanAngy, ghost2, ghost3, ghostscared
            , bottomMenu, orange, strawberry, cherry, button, powerupWall, powerupGhost, smallbutton, logo, sliderbar, slidercursor, sliderfill;

        public void Load(ContentManager Content)
        {
            logo = Content.Load<Texture2D>(@"gui\title");
            orange = Content.Load<Texture2D>(@"world\orange");
            strawberry = Content.Load<Texture2D>(@"world\stawbebbi");
            cherry = Content.Load<Texture2D>(@"world\cherry");
            bottomMenu = Content.Load<Texture2D>(@"gui\bottommenu");
            button = Content.Load<Texture2D>(@"gui\button");
            smallbutton = Content.Load<Texture2D>(@"gui\smallbutton");
            health = Content.Load<Texture2D>(@"gui\health");
            point = Content.Load<Texture2D>(@"world\points");
            powerupGhost = Content.Load<Texture2D>(@"world\powerup1");
            powerupWall = Content.Load<Texture2D>(@"world\powerup2");
            ghost = Content.Load<Texture2D>(@"characters\ghost");
            ghost2 = Content.Load<Texture2D>(@"characters\ghost2");
            ghost3 = Content.Load<Texture2D>(@"characters\ghost3");
            ghostscared = Content.Load<Texture2D>(@"characters\ghostscared");
            blank = Content.Load<Texture2D>(@"blanktile");
            wallSheet = Content.Load<Texture2D>(@"world\tilewall2");
            pakeman = Content.Load<Texture2D>(@"characters\pakeman");
            pakemandeath = Content.Load<Texture2D>(@"characters\pakemanDeath");
            pakemanAngy = Content.Load<Texture2D>(@"characters\pakemanAngy");
            sliderbar = Content.Load<Texture2D>(@"gui\sliderbar");
            sliderfill = Content.Load<Texture2D>(@"gui\sliderfill");
            slidercursor = Content.Load<Texture2D>(@"gui\slidercursor");
        }
    }
}
