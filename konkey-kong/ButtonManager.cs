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
        public Button start, highscore, editor, restart, nextLevel, exit, back, map1, map2, map3;
        TextureManager textures;

        public ButtonManager(TextureManager textures)
        {
            this.textures = textures;
        }
        public void Initialize()
        {
            start = new Button(new Vector2(220, 205), textures.button, new Rectangle(220, 205, 520, 128), "Start Game");
            highscore = new Button(new Vector2(220, 370), textures.button, new Rectangle(220, 370, 520, 128), "High Scores");
            editor = new Button(new Vector2(220, 535), textures.button, new Rectangle(220, 535, 520, 128), "Level Editor");

            restart = new Button(new Vector2(220, 620), textures.button, new Rectangle(220, 620, 520, 128), "Main Menu");

            nextLevel = new Button(new Vector2(220, 370), textures.button, new Rectangle(220, 370, 520, 128), "Start Next Level");
            exit = new Button(new Vector2(220, 635), textures.button, new Rectangle(220, 635, 520, 128), "Exit Game");

            back = new Button(new Vector2(40, 672), textures.button, new Rectangle(40, 672, 520, 128), "Save and Exit");
            map1 = new Button(new Vector2(600, 672), textures.smallbutton, new Rectangle(600, 672, 128, 128), "1");
            map2 = new Button(new Vector2(728, 672), textures.smallbutton, new Rectangle(728, 672, 128, 128), "2");
            map3 = new Button(new Vector2(856, 672), textures.smallbutton, new Rectangle(856, 672, 128, 128), "3");
        }
        public void UpdateDraw(GameState state, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            if(state == GameState.Title)
            {
                start.Draw(spriteBatch, spriteFont);
                highscore.Draw(spriteBatch, spriteFont);
                editor.Draw(spriteBatch, spriteFont);
            }
            if(state == GameState.Menu)
            {
                nextLevel.Draw(spriteBatch, spriteFont);
                exit.Draw(spriteBatch, spriteFont);
            }
            if(state == GameState.GameOver)
            {
                restart.Draw(spriteBatch, spriteFont);
            }
            if(state == GameState.LevelEditor)
            {
                back.Draw(spriteBatch, spriteFont);
                map1.Draw(spriteBatch, spriteFont);
                map2.Draw(spriteBatch, spriteFont);
                map3.Draw(spriteBatch, spriteFont);
            }
        }
    }
}
