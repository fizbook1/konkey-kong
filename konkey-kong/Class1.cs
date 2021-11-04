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
using static pakeman.Game1;

namespace pakeman
{
    class SoundManager
    {
        SoundEffect music, pakemanMove, powerup, ghostDeath, pakemanDeath;
        public void Load(ContentManager Content)
        {
            music = Content.Load<SoundEffect>(@"audio\music2");
            pakemanMove = Content.Load<SoundEffect>(@"audio\pickup");
            pakemanDeath = Content.Load<SoundEffect>(@"audio\pakemanDead");
            ghostDeath = Content.Load<SoundEffect>(@"audio\pickup");
            powerup = Content.Load<SoundEffect>(@"audio\powerup");
            //Content.Load<SoundEffect>(@"audio\music1");
        }
    }

}
