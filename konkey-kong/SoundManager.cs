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
        private const float V = 0.17F;
        SoundEffect musicHighscore, musicLevel1, musicLevel2, musicLevel3, musicTitle, musicLevelEdit, musicNextLevel;
        SoundEffect pickup, powerup, ghostDeath, pakemanDeath, button;
        SoundEffectInstance pickupInst, powerupInst, ghostDeathInst, pakemanDeathInst, buttonInst;
        SoundEffectInstance musicHighscoreInst, musicLevel1Inst, musicLevel2Inst, musicLevel3Inst, musicTitleInst, musicLevelEditInst, musicNextLevelInst;
        public void Load(ContentManager Content)
        {

            musicHighscore = Content.Load<SoundEffect>(@"audio\music\Canon_In_D_For_8_Bit_Synths-Highscore");
            musicLevel1 = Content.Load<SoundEffect>(@"audio\music\Newer_Wave-LevelEdit");
            musicLevel2 = Content.Load<SoundEffect>(@"audio\music\Voxel_Revolution-Level2");
            musicLevel3 = Content.Load<SoundEffect>(@"audio\music\Neon_Laser_Horizon-Level3");
            musicTitle = Content.Load<SoundEffect>(@"audio\music\Space_Jazz-Titlemusic");
            musicLevelEdit = Content.Load<SoundEffect>(@"audio\music\Ethernight_Club-Level1");
            musicNextLevel = Content.Load<SoundEffect>(@"audio\music\Ether_Vox-NextLevel");

            pickup = Content.Load<SoundEffect>(@"audio\pickups");
            pakemanDeath = Content.Load<SoundEffect>(@"audio\pakemanDead");
            ghostDeath = Content.Load<SoundEffect>(@"audio\ghostDead");
            powerup = Content.Load<SoundEffect>(@"audio\powerup");
            button = Content.Load<SoundEffect>(@"audio\buton");
            //Content.Load<SoundEffect>(@"audio\music1");

            musicHighscoreInst = musicHighscore.CreateInstance();
            musicLevel1Inst = musicLevel1.CreateInstance();
            musicLevel2Inst = musicLevel2.CreateInstance();
            musicLevel3Inst = musicLevel3.CreateInstance();
            musicTitleInst = musicTitle.CreateInstance();
            musicLevelEditInst = musicLevelEdit.CreateInstance();
            musicNextLevelInst = musicNextLevel.CreateInstance();

            pickupInst = pickup.CreateInstance();
            powerupInst = powerup.CreateInstance();
            ghostDeathInst = ghostDeath.CreateInstance();
            pakemanDeathInst = pakemanDeath.CreateInstance();

            musicTitleInst.IsLooped = true;
            musicTitleInst.Volume = 0.6F;
            musicLevelEditInst.IsLooped = true;
            musicLevelEditInst.Volume = V;
            musicNextLevelInst.IsLooped = true;
            musicNextLevelInst.Volume = V;
            musicHighscoreInst.IsLooped = true;
            musicHighscoreInst.Volume = V;
            musicLevel1Inst.IsLooped = true;
            musicLevel1Inst.Volume = V;
            musicLevel2Inst.IsLooped = true;
            musicLevel2Inst.Volume = V;
            musicLevel3Inst.IsLooped = true;
            musicLevel3Inst.Volume = V;
        }

        public void Music(GameState gamestate, int level)
        {
            if(gamestate == GameState.Title)
            {
                musicTitleInst.Play();
                
            } else { musicTitleInst.Stop(); }

            if (gamestate == GameState.LevelEditor)
            {
                musicLevelEditInst.Play();
                
            }
            else { musicLevelEditInst.Stop(); }

            if (gamestate == GameState.Menu)
            {
                musicNextLevelInst.Play();
                
            }
            else { musicNextLevelInst.Stop(); }

            if (gamestate == GameState.GameOver)
            {
                musicHighscoreInst.Play();
                
            }
            else { musicHighscoreInst.Stop(); }

            if (gamestate == GameState.InGame)
            {
                if (level == 0)
                {
                    musicLevel1Inst.Play();
                    
                }
                else { musicLevel1Inst.Stop(); }

                if (level == 1)
                {
                    musicLevel2Inst.Play();
                    
                }
                else { musicLevel2Inst.Stop(); }

                if (level == 2)
                {
                    musicLevel3Inst.Play();
                    
                }
                else { musicLevel3Inst.Stop(); }
            }
            else 
            {
                musicLevel1Inst.Stop();
                musicLevel2Inst.Stop();
                musicLevel3Inst.Stop(); 
            }

        }
    }
}
