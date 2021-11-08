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
    class ButtonManager
    {
        public Button start, highscore, editor, restart, nextLevel, exit, back, map1, map2, map3, quitGame, credits, settings, backtogame;
        TextureManager textures;
        
        public ButtonManager(TextureManager textures)
        {
            this.textures = textures;
        }
        public void Initialize(SoundManager sound)
        {
            settings = new Button(new Vector2(800, 652), textures.smallbutton, new Rectangle(800, 652, 128, 128), "Opts", sound);

            start = new Button(new Vector2(220, 195), textures.button, new Rectangle(220, 195, 520, 128), "Start Game", sound);
            highscore = new Button(new Vector2(220, 340), textures.button, new Rectangle(220, 340, 520, 128), "High Scores", sound);
            editor = new Button(new Vector2(220, 485), textures.button, new Rectangle(220, 485, 520, 128), "Level Editor", sound);
            quitGame = new Button(new Vector2(220, 630), textures.button, new Rectangle(220, 630, 520, 128), "Exit Game", sound);
            credits = new Button(new Vector2(20, 652), textures.smallbutton, new Rectangle(20, 652, 128, 128), "Cred", sound);

            restart = new Button(new Vector2(220, 620), textures.button, new Rectangle(220, 620, 520, 128), "Main Menu", sound);

            nextLevel = new Button(new Vector2(220, 370), textures.button, new Rectangle(220, 370, 520, 128), "Start Next Level", sound);
            backtogame = new Button(new Vector2(220, 490), textures.button, new Rectangle(220, 490, 520, 128), "Return", sound);
            exit = new Button(new Vector2(220, 635), textures.button, new Rectangle(220, 635, 520, 128), "Exit to Title", sound);

            back = new Button(new Vector2(0, 672), textures.button, new Rectangle(0, 672, 520, 128), "Save and Exit", sound);
            map1 = new Button(new Vector2(575, 672), textures.smallbutton, new Rectangle(560, 672, 128, 128), "1", sound);
            map2 = new Button(new Vector2(703, 672), textures.smallbutton, new Rectangle(688, 672, 128, 128), "2", sound);
            map3 = new Button(new Vector2(831, 672), textures.smallbutton, new Rectangle(816, 672, 128, 128), "3", sound);
        }
        public void UpdateDraw(GameState state, SpriteBatch spriteBatch, SpriteFont spriteFont, bool paused)
        {
            if(state == GameState.Title)
            {
                start.Draw(spriteBatch, spriteFont);
                highscore.Draw(spriteBatch, spriteFont);
                editor.Draw(spriteBatch, spriteFont);
                quitGame.Draw(spriteBatch, spriteFont);
                credits.Draw(spriteBatch, spriteFont);
                settings.Draw(spriteBatch, spriteFont);
            }
            if (state == GameState.Credits)
            {
                restart.Draw(spriteBatch, spriteFont);
            }
            if (state == GameState.Menu)
            {
                if(paused)
                {
                    backtogame.Draw(spriteBatch, spriteFont);
                    settings.Draw(spriteBatch, spriteFont);
                } 
                else { nextLevel.Draw(spriteBatch, spriteFont); }
                exit.Draw(spriteBatch, spriteFont);
            }
            if(state == GameState.GameOver)
            {
                restart.Draw(spriteBatch, spriteFont);
            }
            if (state == GameState.Settings)
            {
                backtogame.Draw(spriteBatch, spriteFont);
            }
            if (state == GameState.LevelEditor)
            {
                back.Draw(spriteBatch, spriteFont);
                map1.Draw(spriteBatch, spriteFont);
                map2.Draw(spriteBatch, spriteFont);
                map3.Draw(spriteBatch, spriteFont);
            }
        }
    }
}
