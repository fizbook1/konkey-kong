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

    public class Game1 : Game
    {
        Random rnd = new Random();
        public const int TILESIZE = 32;

        public enum GameState { Title = 0, InGame = 1, GameOver = 2, Menu = 3, LevelEditor = 4, Credits = 5, Settings = 6 }

        GameState gameState = GameState.Title;

        List<Tile[,]> mapList;

        Color bgcolor = new Color(13, 4, 25);

        SoundManager soundManager;
        TextureManager textureManager;
        TileManager tileManager;
        EnemyManager enemyManager;
        ScoreManager scoreManager;
        PickupManager pickupManager;
        ButtonManager buttonManager;

        List<HealthBar> healthBarList;
        List<string> highscoreList = new List<string>();
        List<int> sortingHighscoreList = new List<int>();

        Rectangle editorNullZone;
        int currentlyEditedMap = 1;

        public static SpriteFont font;
        private SpriteFont bigFont;
        Vector2 mousePos;
        bool mouseLReady = false;
        bool mouseRReady = false;

        bool finalScoreCreated = false;
        bool paused = false;
        GameState previousState;

        Player player;

        int currentMap = 0;
        bool scoreCalculated = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 800;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>(@"font");
            bigFont = Content.Load<SpriteFont>(@"bigFont");

            textureManager = new TextureManager();
            textureManager.Load(Content);
            soundManager = new SoundManager();
            soundManager.Load(Content);

            AudioSlider music = new AudioSlider(textureManager.slidercursor, textureManager.sliderfill, textureManager.sliderbar, new Vector2(500, 400));
            AudioSlider sound = new AudioSlider(textureManager.slidercursor, textureManager.sliderfill, textureManager.sliderbar, new Vector2(200, 400));
            soundManager.musicSlider = music;
            soundManager.soundSlider = sound;

            mapList = new List<Tile[,]>();
            highscoreList = new List<string>();
            sortingHighscoreList = new List<int>();

            player = new Player(new Vector2(0, 0), new Rectangle(0, 0, 0, 0), textureManager.pakeman, 0, 0, textureManager, soundManager);

            tileManager = new TileManager(textureManager, player, soundManager);
            pickupManager = new PickupManager(textureManager, soundManager);
            scoreManager = new ScoreManager();
            enemyManager = new EnemyManager(textureManager);
            buttonManager = new ButtonManager(textureManager);

            editorNullZone = new Rectangle(0, 672, 960, 128);

            healthBarList = new List<HealthBar>();

            buttonManager.Initialize(soundManager);

            if (File.Exists("highscores.txt") == false)
            {
                File.Create("highscores.txt");
            }
            scoreManager.highscores = File.ReadAllLines("highscores.txt").ToList();

            int healthBarTempPosX = textureManager.bottomMenu.Width - 28 - textureManager.health.Width;
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(healthBarTempPosX, 737);
                HealthBar healthBarCreator = new HealthBar(pos, textureManager.health, false);
                healthBarList.Add(healthBarCreator);
                healthBarTempPosX -= textureManager.health.Width + 5;
            }

            for (int l = 0; l < 3; l++) {

                List<string> strings = new List<string>();
                StreamReader sr = new StreamReader(String.Format("map{0}.txt", l + 1));
                while (!sr.EndOfStream)
                {
                    strings.Add(sr.ReadLine());
                }
                sr.Close();

                Tile[,] tileArr = new Tile[36, 27];
                tileArr = new Tile[strings[0].Length, strings.Count];
                mapList.Add(tileArr);

                for (int i = 0; i < mapList[l].GetLength(0); i++)
                {
                    for (int j = 0; j < mapList[l].GetLength(1); j++)
                    {
                        Vector2 tempPos = new Vector2((i * TILESIZE) - 3 * TILESIZE, (j * TILESIZE) - 3 * TILESIZE);
                        Rectangle tempRect = new Rectangle((i * TILESIZE) - 3 * TILESIZE, (j * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE);
                        if (strings[j][i] == 'X')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Wall, textureManager.wallSheet, i, j);
                        }
                        if (strings[j][i] == 'o')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Standard, textureManager.blank, i, j);
                        }
                        if (strings[j][i] == 'p')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.PacmanSpawn, textureManager.blank, i, j);
                        }
                        if (strings[j][i] == 'P')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.PowerUpSpawn, textureManager.blank, i, j);
                        }
                        if (strings[j][i] == 'e')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.GhostSpawn, textureManager.blank, i, j);
                        }
                    }
                }
            }



        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released)
            {
                mouseLReady = true;
            }
            if (mouseState.RightButton == ButtonState.Released)
            {
                mouseRReady = true;
            }

            soundManager.Music(gameState, currentMap);
            mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);

            switch (gameState)
            {
                case GameState.Title:

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady) {
                        if(buttonManager.start.Update(mousePos))
                        {
                            scoreCalculated = false;
                            gameState = GameState.InGame;
                            TileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                            tileManager.Initialize(pickupManager, enemyManager);
                        }
                        if(buttonManager.highscore.Update(mousePos))
                        {
                            gameState = GameState.GameOver;
                        }
                        if (buttonManager.editor.Update(mousePos))
                        {
                            gameState = GameState.LevelEditor;
                            TileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                        }
                        if (buttonManager.quitGame.Update(mousePos))
                        {
                            this.Exit();
                        }
                        if (buttonManager.credits.Update(mousePos))
                        {
                            gameState = GameState.Credits;
                        }
                        if (buttonManager.settings.Update(mousePos))
                        {
                            gameState = GameState.Settings;
                            previousState = GameState.Title;
                        }
                        mouseLReady = false;
                        
                    }

                    break;
                case GameState.Credits:
                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        if (buttonManager.restart.Update(mousePos))
                        {
                            gameState = GameState.Title;
                        }
                        mouseLReady = false;
                    }
                    break;
                case GameState.Settings:
                    soundManager.Update(mousePos, mouseState.LeftButton == ButtonState.Pressed);
                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        if (buttonManager.backtogame.Update(mousePos))
                        {
                            gameState = previousState;
                        }
                        mouseLReady = false;
                    }
                    break;

                case GameState.InGame:

                    double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
                    double smallElapsed = gameTime.ElapsedGameTime.TotalMilliseconds;

                    if(keyboardState.IsKeyDown(Keys.Escape))
                    {
                        gameState = GameState.Menu;
                        paused = true;
                    }

                    if (pickupManager.list.Count == 0)
                    {
                        if (currentMap == mapList.Count() - 1)
                        {
                            gameState = GameState.GameOver;
                        }
                        else
                        {
                            gameState = GameState.Menu;
                            pickupManager.Reset();
                            enemyManager.Reset();
                        }
                    }

                    tileManager.Update(smallElapsed, gameState);
                    scoreManager.Update(smallElapsed);
                    pickupManager.Update(smallElapsed, player, scoreManager, tileManager);
                    enemyManager.Update(smallElapsed, player, scoreManager, tileManager);
                    player.Update(smallElapsed, tileManager);

                    int healthCheck = 0;
                    while (player.health + healthCheck <= 2 && player.health >= 0)
                    {
                        healthBarList[player.health + healthCheck].lost = true;
                        healthCheck++;
                    }
                    if (player.health < 0)
                    {
                        scoreCalculated = false;
                        gameState = GameState.GameOver;
                    }

                    break;
                case GameState.Menu:

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        if (paused)
                        {
                            if( buttonManager.backtogame.Update(mousePos))
                            {
                                gameState = GameState.InGame;
                                paused = false;
                            }
                        } else if (buttonManager.nextLevel.Update(mousePos))
                        {
                            currentMap++;
                            TileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                            tileManager.Initialize(pickupManager, enemyManager);
                            gameState = GameState.InGame;
                        }
                        if (buttonManager.settings.Update(mousePos))
                        {
                            gameState = GameState.Settings;
                            previousState = GameState.Menu;
                        }
                        if (buttonManager.exit.Update(mousePos))
                        {
                            gameState = GameState.GameOver;
                        }
                        mouseLReady = false;
                    }
                    break;
                case GameState.GameOver:

                    if(!scoreCalculated)
                    {
                        scoreManager.CalculateHighscores();
                        scoreCalculated = true;
                    }
                    
                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        if(buttonManager.restart.Update(mousePos))
                        {
                            gameState = GameState.Title;
                            scoreManager.score = 0;
                            pickupManager.Reset();
                            enemyManager.Reset();
                            foreach(HealthBar hb in healthBarList)
                            {
                                hb.lost = false;
                            }
                            mouseLReady = false;
                            player.health = 3;
                            soundManager.Reset();
                            currentMap = 0;
                            mapList.Clear();
                            for (int l = 0; l < 3; l++)
                            {

                                List<string> strings = new List<string>();
                                StreamReader sr = new StreamReader(String.Format("map{0}.txt", l + 1));
                                while (!sr.EndOfStream)
                                {
                                    strings.Add(sr.ReadLine());
                                }
                                sr.Close();

                                Tile[,] tileArr = new Tile[36, 27];
                                tileArr = new Tile[strings[0].Length, strings.Count];
                                mapList.Add(tileArr);

                                for (int i = 0; i < mapList[l].GetLength(0); i++)
                                {
                                    for (int j = 0; j < mapList[l].GetLength(1); j++)
                                    {
                                        Vector2 tempPos = new Vector2((i * TILESIZE) - 3 * TILESIZE, (j * TILESIZE) - 3 * TILESIZE);
                                        Rectangle tempRect = new Rectangle((i * TILESIZE) - 3 * TILESIZE, (j * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE);
                                        if (strings[j][i] == 'X')
                                        {
                                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Wall, textureManager.wallSheet, i, j);
                                        }
                                        if (strings[j][i] == 'o')
                                        {
                                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Standard, textureManager.blank, i, j);
                                        }
                                        if (strings[j][i] == 'p')
                                        {
                                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.PacmanSpawn, textureManager.blank, i, j);
                                        }
                                        if (strings[j][i] == 'P')
                                        {
                                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.PowerUpSpawn, textureManager.blank, i, j);
                                        }
                                        if (strings[j][i] == 'e')
                                        {
                                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.GhostSpawn, textureManager.blank, i, j);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;

                case GameState.LevelEditor:
                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady && !editorNullZone.Contains(mousePos))
                    {
                        mouseLReady = false;
                        tileManager.LevelEdit('l');
                    }
                    if (mouseState.RightButton == ButtonState.Pressed && mouseRReady && !editorNullZone.Contains(mousePos))
                    {
                        mouseRReady = false;
                        tileManager.LevelEdit('r');
                    }

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseLReady && editorNullZone.Contains(mousePos))
                    { 
                        if(buttonManager.map1.Update(mousePos))
                        {
                            currentlyEditedMap = 1;
                            TileManager.currentMap = (Tile[,])mapList[currentlyEditedMap - 1].Clone();
                        }
                        if (buttonManager.map2.Update(mousePos))
                        {
                            currentlyEditedMap = 2;
                            TileManager.currentMap = (Tile[,])mapList[currentlyEditedMap - 1].Clone();
                        }   
                        if (buttonManager.map3.Update(mousePos))
                        {
                            currentlyEditedMap = 3;
                            TileManager.currentMap = (Tile[,])mapList[currentlyEditedMap - 1].Clone();
                        }
                        if (buttonManager.back.Update(mousePos))
                        {
                            StringBuilder stringToAdd = new StringBuilder(50);
                            gameState = GameState.Title;
                            char[,] types = new char[36, 27];
                            for (int i = 0; i < TileManager.currentMap.GetLength(1); i++)
                            {
                                for (int j = 0; j < TileManager.currentMap.GetLength(0); j++)
                                {
                                    if (TileManager.currentMap[j, i].type == TileType.Wall)
                                    {
                                        types[j, i] = 'X';
                                    }
                                    if (TileManager.currentMap[j, i].type == TileType.Standard)
                                    {
                                        types[j, i] = 'o';
                                    }
                                    if (TileManager.currentMap[j, i].type == TileType.PacmanSpawn)
                                    {
                                        types[j, i] = 'p';
                                    }
                                    if (TileManager.currentMap[j, i].type == TileType.PowerUpSpawn)
                                    {
                                        types[j, i] = 'P';
                                    }
                                    if (TileManager.currentMap[j, i].type == TileType.GhostSpawn)
                                    {
                                        types[j, i] = 'e';
                                    }
                                }
                            }
                            List<string> listToWrite = new List<string>();
                            for (int j = 0; j < types.GetLength(1); j++)
                            {
                                for (int i = 0; i < types.GetLength(0); i++)
                                {
                                    if (types[i, j] != null)
                                    { stringToAdd.Append(types[i, j]); }
                                } //this won't work probably but I'm too tired
                                listToWrite.Add(stringToAdd.ToString());
                                stringToAdd.Clear();
                            }

                            File.WriteAllLines(String.Format("map{0}.txt", currentlyEditedMap), listToWrite, Encoding.UTF8);
                        }
                        mouseLReady = false;
                    }


                    tileManager.Update(0, gameState);

                    break;
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(bgcolor);
            _spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Title:

                    _spriteBatch.Draw(textureManager.logo, new Vector2(140, 35), Color.White);

                    break;

                case GameState.Settings: 

                    _spriteBatch.DrawString(bigFont, "Sound Volume", new Vector2(180, 350), Color.White);
                    soundManager.soundSlider.Draw(_spriteBatch);
                    _spriteBatch.DrawString(bigFont, "Music Volume", new Vector2(510, 350), Color.White);
                    soundManager.musicSlider.Draw(_spriteBatch);

                    break;

                case GameState.Credits:

                    Vector2 adjustedPos2 = new Vector2();

                    adjustedPos2.Y = (Window.ClientBounds.Height / 2 - font.MeasureString(scoreManager.ToString()).Y / 2) - 200;
                    adjustedPos2.X = (Window.ClientBounds.Width / 2 - font.MeasureString(scoreManager.ToString()).X / 2) - 40;

                    _spriteBatch.DrawString(font, @"Ethernight Club Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/", new Vector2((Window.ClientBounds.Width / 2) - 200, 40), Color.White);

                    _spriteBatch.DrawString(font, @"Space Jazz Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/", new Vector2((Window.ClientBounds.Width / 2) - 200, 120), Color.White);

                    _spriteBatch.DrawString(font, @"Canon In D For 8 Bit Synths Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/" , new Vector2((Window.ClientBounds.Width/2) - 200, 200), Color.White);

                    _spriteBatch.DrawString(font, @"Ether Vox Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/", new Vector2((Window.ClientBounds.Width / 2) - 200, 280), Color.White);

                    _spriteBatch.DrawString(font, @"Neon Laser Horizon Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/", new Vector2((Window.ClientBounds.Width / 2) - 200, 360), Color.White);

                    _spriteBatch.DrawString(font, @"Newer Wave Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/", new Vector2((Window.ClientBounds.Width / 2) - 200, 440), Color.White);

                    _spriteBatch.DrawString(font, @"Voxel Revolution Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0 License
http://creativecommons.org/licenses/by/3.0/" , new Vector2((Window.ClientBounds.Width / 2) - 200, 520), Color.White);

                    break;
                case GameState.InGame:


                    tileManager.UpdateDraw(_spriteBatch, gameState);
                    pickupManager.UpdateDraw(_spriteBatch);
                    enemyManager.UpdateDraw(_spriteBatch);
                    scoreManager.UpdateDraw(_spriteBatch, font);
                    player.Draw(_spriteBatch);

                    _spriteBatch.Draw(textureManager.bottomMenu, new Vector2(0, 672), Color.White);

                    _spriteBatch.DrawString(bigFont, String.Format("Level {0}", currentMap + 1), new Vector2(400, 702), Color.White);
                    
                    _spriteBatch.DrawString(bigFont, "Score", new Vector2(15, 687), Color.White);
                    _spriteBatch.DrawString(bigFont, scoreManager.score.ToString(), new Vector2(15, 737), Color.White);
                    _spriteBatch.DrawString(bigFont, "Lives", new Vector2(815, 687), Color.White);

                    
                    foreach (HealthBar h in healthBarList)
                    {
                        h.Draw(_spriteBatch, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }


                    break;
                case GameState.Menu:

                    Vector2 adjustedPos = new Vector2();

                    adjustedPos.Y = (Window.ClientBounds.Height/2 - bigFont.MeasureString(scoreManager.ToString()).Y / 2) -200;
                    adjustedPos.X = Window.ClientBounds.Width/2 - bigFont.MeasureString(scoreManager.ToString()).X / 2;

                    _spriteBatch.DrawString(bigFont, scoreManager.score.ToString(), adjustedPos, Color.White);

                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(bigFont, scoreManager.score.ToString(), new Vector2(400, 130), Color.White);

                    _spriteBatch.DrawString(font, "High Scores", new Vector2(400, 100), Color.White);

                    for (int i = 0; i < scoreManager.highscores.Count(); i++)
                    {
                        _spriteBatch.DrawString(font, scoreManager.highscores[i], new Vector2(400, 200+i*40), Color.White);
                    }


                    _spriteBatch.DrawString(bigFont, "High Scores", new Vector2(300 + 50, 825 + 50), Color.White);
                    break;

                case GameState.LevelEditor:

                    tileManager.UpdateDraw(_spriteBatch, gameState);

                    _spriteBatch.Draw(textureManager.bottomMenu, new Vector2(0, 672), Color.White);

                    break;
            }

            buttonManager.UpdateDraw(gameState, _spriteBatch, bigFont, paused);
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
