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
        public Texture2D health, ghost, point, pickup, blank, wallSheet, pakeman, pakemandeath, ghost2, ghost3, ghostscared
            , bottomMenu, orange, strawberry, cherry, button, powerupWall, powerupGhost, smallbutton, logo;

        public void Load(ContentManager Content)
        {
            logo = Content.Load<Texture2D>(@"title");
            orange = Content.Load<Texture2D>(@"orange");
            strawberry = Content.Load<Texture2D>(@"stawbebbi");
            cherry = Content.Load<Texture2D>(@"cherry");
            bottomMenu = Content.Load<Texture2D>(@"bottommenu");
            button = Content.Load<Texture2D>(@"button");
            smallbutton = Content.Load<Texture2D>(@"smallbutton");
            health = Content.Load<Texture2D>(@"health");
            point = Content.Load<Texture2D>(@"points");
            powerupGhost = Content.Load<Texture2D>(@"powerup1");
            powerupWall = Content.Load<Texture2D>(@"powerup2");
            pickup = Content.Load<Texture2D>(@"pickup");
            ghost = Content.Load<Texture2D>(@"ghost");
            ghost2 = Content.Load<Texture2D>(@"ghost2");
            ghost3 = Content.Load<Texture2D>(@"ghost3");
            ghostscared = Content.Load<Texture2D>(@"ghostscared");
            blank = Content.Load<Texture2D>(@"blanktile");
            wallSheet = Content.Load<Texture2D>(@"tilewall2");
            pakeman = Content.Load<Texture2D>(@"pakeman");
            pakemandeath = Content.Load<Texture2D>(@"pakemanDeath");
        }




    }
}
