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
        const int TILESIZE = 32;

        public enum GameState { Title = 0, InGame = 1, GameOver = 2, Menu = 3, LevelEditor = 4}

        GameState gameState = GameState.Title;

        SoundEffect pickupFX;

        List<Tile[,]> mapList;
        Tile[,] tileArr1 = new Tile[36, 27];
        Tile[,] tileArr2 = new Tile[36, 27];
        Tile[,] tileArr3 = new Tile[36, 27];

        Color bgcolor = new Color(13, 4, 25);

        TextureManager textureManager;
        TileManager tileManager;
        EnemyManager enemyManager;
        ScoreManager scoreManager;
        PickupManager pickupManager;

        List<HealthBar> healthBarList;
        List<string> highscoreList = new List<string>();
        List<int> sortingHighscoreList = new List<int>();

        Button startButton;
        Button levelButton;
        Button editorButton;

        Button nextLevelButton; 
        Button exitButton;

        private SpriteFont font;
        private SpriteFont bigFont;
        Vector2 mousePos;

        bool finalScoreCreated = false;

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

            mapList = new List<Tile[,]>();
            highscoreList = new List<string>();
            sortingHighscoreList = new List<int>();

            player = new Player(new Vector2(0, 0), new Rectangle(0, 0, 0, 0), textureManager.pakeman, 0, 0, textureManager);

            tileManager = new TileManager(textureManager, player);
            pickupManager = new PickupManager(textureManager);
            scoreManager = new ScoreManager();
            enemyManager = new EnemyManager(textureManager);
            

            startButton = new Button(new Vector2(220, 205), textureManager.button, new Rectangle(220, 205, 520, 128));
            levelButton = new Button(new Vector2(220, 370), textureManager.button, new Rectangle(220, 370, 520, 128));
            editorButton = new Button(new Vector2(220, 535), textureManager.button, new Rectangle(220, 535, 520, 128));

            nextLevelButton = new Button(new Vector2(220, 370), textureManager.button, new Rectangle(220, 370, 520, 128));
            exitButton = new Button(new Vector2(220, 535), textureManager.button, new Rectangle(220, 535, 520, 128));

            healthBarList = new List<HealthBar>();

            if (File.Exists("highscores.txt") == false)
            {
                File.Create("highscores.txt");
            }
            highscoreList = File.ReadAllLines("highscores.txt").ToList();

            int healthBarTempPosX = textureManager.bottomMenu.Width-13- textureManager.health.Width;
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(healthBarTempPosX, 685);
                HealthBar healthBarCreator = new HealthBar(pos, textureManager.health, false);
                healthBarList.Add(healthBarCreator);
                healthBarTempPosX -= textureManager.health.Width + 5;
            }

            List<string> strings = new List<string>();
            StreamReader sr = new StreamReader("map1.txt");

            while (!sr.EndOfStream)
            {
                strings.Add(sr.ReadLine());
            }

            sr.Close();

            tileArr1 = new Tile[strings[0].Length, strings.Count];
            tileArr2 = new Tile[strings[0].Length, strings.Count];
            tileArr3 = new Tile[strings[0].Length, strings.Count];
            mapList.Add(tileArr1);
            mapList.Add(tileArr2);
            mapList.Add(tileArr3);

            for (int l = 0; l < 3; l++) { 
                for (int i = 0; i < mapList[l].GetLength(0); i++)
                {
                    for (int j = 0; j < mapList[l].GetLength(1); j++)
                    {
                        Vector2 tempPos = new Vector2((i * TILESIZE) - 3*TILESIZE, (j * TILESIZE) - 3*TILESIZE);
                        Rectangle tempRect = new Rectangle((i * TILESIZE) - 3*TILESIZE, (j * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE);
                        if (strings[j][i] == 'X')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Wall, textureManager.wallSheet, i, j);
                        }
                        if (strings[j][i] == 'o')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Standard, textureManager.blank, i, j);
                        }
                        if (strings[j][i] == 'g')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Gate, textureManager.blank, i, j);
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

            mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);

            switch (gameState)
            {
                case GameState.Title:

                    if (startButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed) {
                        gameState = GameState.InGame;

                        tileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                        tileManager.Initialize( pickupManager, enemyManager);
                    }

                    if (levelButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }

                    if (editorButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.LevelEditor;
                        tileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                    }
                    break;

                case GameState.InGame:
    
                    double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
                    double smallElapsed = gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (pickupManager.list.Count == 0)
                    {
                        gameState = GameState.Menu;
                        pickupManager.Reset();
                        enemyManager.Reset();
                    }

                    tileManager.Update(smallElapsed, gameState);
                    scoreManager.Update(smallElapsed);
                    pickupManager.Update(smallElapsed, player, scoreManager, tileManager);
                    enemyManager.Update(smallElapsed, player, scoreManager, tileManager);
                    player.Update(smallElapsed, tileManager);

                    int healthCheck = 0;
                    while (player.health + healthCheck < 3 && player.health > 0)
                    {
                        healthBarList[player.health-1].lost = false;
                        healthBarList[player.health + healthCheck].lost = true;
                        healthCheck++;
                    }
                    if (player.health < 0)
                    {
                        gameState = GameState.GameOver;
                    }

                    break;
                case GameState.Menu:

                    if (startButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        currentMap++;
                        tileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                        tileManager.Initialize(pickupManager, enemyManager);
                        gameState = GameState.InGame;
                    }

                    if (levelButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:

                if ( !scoreCalculated ) { 
                    if (scoreManager.score > 0) { 
                        highscoreList.Add(scoreManager.score.ToString()) ;
                    }
                    foreach (string s in highscoreList)
                    {
                        sortingHighscoreList.Add(Int32.Parse(s));
                    }
                    sortingHighscoreList.Sort();
                    sortingHighscoreList.Reverse();

                    highscoreList.Clear();
                    if (sortingHighscoreList.Count > 10)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            highscoreList.Add(sortingHighscoreList[j].ToString());
                        }
                    }
                    else
                    {
                        foreach (int i in sortingHighscoreList)
                        {
                            highscoreList.Add(i.ToString());
                        }
                    }
                    File.WriteAllLines("highscores.txt", highscoreList, Encoding.UTF8);
                    scoreCalculated = true;
                }
                    
                break;

                case GameState.LevelEditor:

                    tileManager.LevelEdit();
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

                    startButton.Draw(_spriteBatch, bigFont, "Start Game");
                    levelButton.Draw(_spriteBatch, bigFont, "Level Select");
                    editorButton.Draw(_spriteBatch, bigFont, "Level Editor");

                    break;

                case GameState.InGame:


                    tileManager.UpdateDraw(_spriteBatch);
                    pickupManager.UpdateDraw(_spriteBatch);
                    enemyManager.UpdateDraw(_spriteBatch);
                    scoreManager.UpdateDraw(_spriteBatch, font);
                    player.Draw(_spriteBatch);

                    _spriteBatch.DrawString(font, scoreManager.score.ToString(), new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(font, "Health", new Vector2(Window.ClientBounds.Width - 175, 10), Color.White);

                    _spriteBatch.Draw(textureManager.bottomMenu, new Vector2(0, 672), Color.White);
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

                    nextLevelButton.Draw(_spriteBatch, bigFont, "Start Next Level");
                    exitButton.Draw(_spriteBatch, bigFont, "Exit Game");
                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(font, scoreManager.score.ToString(), new Vector2(400, 200), Color.White);

                    _spriteBatch.DrawString(font, "High Scores", new Vector2(400, 250), Color.White);

                    for (int i = 0; i < highscoreList.Count(); i++)
                    {
                        _spriteBatch.DrawString(font, highscoreList[i], new Vector2(400, 300+i*40), Color.White);
                    }


                    _spriteBatch.DrawString(font, "High Scores", new Vector2(300 + 50, 825 + 50), Color.White);
                    break;

                case GameState.LevelEditor:

                    tileManager.UpdateDraw(_spriteBatch);

                    break;
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
