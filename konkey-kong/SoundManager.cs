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
    public class SoundManager
    {
        public AudioSlider musicSlider;
        public AudioSlider soundSlider;
        private float musicVolume = 0.5F;
        private float soundVolume = 0.5F;
        SoundEffect musicHighscore, musicLevel1, musicLevel2, musicLevel3, musicTitle, musicLevelEdit, musicNextLevel;
        SoundEffect pickup, powerup, ghostDeath, pakemanDeath, button, crunch;
        public SoundEffectInstance pickupInst, powerupInst, ghostDeathInst, pakemanDeathInst, buttonInst, crunchInst;
        private SoundEffectInstance musicHighscoreInst, musicLevel1Inst, musicLevel2Inst, musicLevel3Inst, musicTitleInst, musicLevelEditInst, musicNextLevelInst;
        private List<SoundEffectInstance> music = new List<SoundEffectInstance>();
        private List<SoundEffectInstance> sound = new List<SoundEffectInstance>();
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
            crunch = Content.Load<SoundEffect>(@"audio\crunch");
            pakemanDeath = Content.Load<SoundEffect>(@"audio\pakemanDead");
            ghostDeath = Content.Load<SoundEffect>(@"audio\ghostDead");
            powerup = Content.Load<SoundEffect>(@"audio\powerup");
            button = Content.Load<SoundEffect>(@"audio\button");

            musicHighscoreInst = musicHighscore.CreateInstance();
            musicLevel1Inst = musicLevel1.CreateInstance();
            musicLevel2Inst = musicLevel2.CreateInstance();
            musicLevel3Inst = musicLevel3.CreateInstance();
            musicTitleInst = musicTitle.CreateInstance();
            musicLevelEditInst = musicLevelEdit.CreateInstance();
            musicNextLevelInst = musicNextLevel.CreateInstance();

            music.Add(musicHighscoreInst);
            music.Add(musicLevel1Inst);
            music.Add(musicLevel2Inst);
            music.Add(musicLevel3Inst);
            music.Add(musicLevel1Inst);
            music.Add(musicTitleInst);
            music.Add(musicLevelEditInst);
            music.Add(musicNextLevelInst);

            pickupInst = pickup.CreateInstance();
            powerupInst = powerup.CreateInstance();
            ghostDeathInst = ghostDeath.CreateInstance();
            pakemanDeathInst = pakemanDeath.CreateInstance();
            buttonInst = button.CreateInstance();
            crunchInst = crunch.CreateInstance();

            sound.Add(pickupInst);
            sound.Add(powerupInst);
            sound.Add(ghostDeathInst);
            sound.Add(pakemanDeathInst);
            sound.Add(buttonInst);
            sound.Add(crunchInst);

            foreach(SoundEffectInstance s in sound)
            {
                s.Volume = soundVolume;
            }
            foreach(SoundEffectInstance m in music)
            {
                m.IsLooped = true;
                m.Volume = musicVolume;
            }
        }

        public void Music(GameState gamestate, int level)
        {
            if(gamestate == GameState.Title)
            {
                musicTitleInst.Play();
                
            } else { musicTitleInst.Pause(); }

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
                else { musicLevel1Inst.Pause(); }

                if (level == 1)
                {
                    musicLevel2Inst.Play();
                    
                }
                else { musicLevel2Inst.Pause(); }

                if (level == 2)
                {
                    musicLevel3Inst.Play();
                    
                }
                else { musicLevel3Inst.Pause(); }
            }
            else 
            {
                musicLevel1Inst.Pause();
                musicLevel2Inst.Pause();
                musicLevel3Inst.Pause(); 
            }

        }

        public void Reset()
        {
            foreach (SoundEffectInstance m in music)
            {
                m.Volume = 0;
                m.Play();
                m.Stop();
                m.Volume = musicVolume;
            }
        }

        public void Update(Vector2 mousePos, bool mousePressed)
        {
            if(mousePressed)
            {
                musicSlider.Move(mousePos);
                soundSlider.Move(mousePos);
            }

            musicVolume = (float)musicSlider.value;
            soundVolume = (float)soundSlider.value;

            foreach (SoundEffectInstance s in sound)
            {
                s.Volume = soundVolume;
            }
            foreach (SoundEffectInstance m in music)
            {
                m.Volume = musicVolume;
            }
        }
    }
    public class AudioSlider
    {
        Texture2D cursorTex, fillTex, barTex;
        Rectangle size = new Rectangle(0, 0, 256, 52);
        Rectangle srcRec = new Rectangle(0, 0, 128, 52);
        Vector2 pos;
        Vector2 cursorPos = new Vector2(124, 5);
        public double value;
        public AudioSlider(Texture2D cursorTex, Texture2D fillTex, Texture2D barTex, Vector2 pos)
        {
            this.cursorTex = cursorTex;
            this.fillTex = fillTex;
            this.barTex = barTex;
            this.pos = pos;

            size.X = (int)pos.X;
            size.Y = (int)pos.Y;
        }
        public void Move(Vector2 mousePos)
        {
            if (mousePos.X > pos.X + (cursorTex.Width/2)-2 && mousePos.X < pos.X + size.Width - 21 && size.Contains(mousePos))
            {
                cursorPos.X = mousePos.X - pos.X -21;
                
                value = (cursorPos.X) / 246; 
                if (value < 0) { value = 0; }
                srcRec.Width = (int)(cursorPos.X + 21);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(barTex, pos, Color.White);
            spriteBatch.Draw(fillTex, pos, srcRec, Color.White);
            spriteBatch.Draw(cursorTex, cursorPos + pos, Color.White);
        }
    }
}
