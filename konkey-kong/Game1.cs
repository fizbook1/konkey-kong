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

        public enum GameState { Title = 0, InGame = 1, GameOver = 2, Menu = 3, LevelEditor = 4 }

        GameState gameState = GameState.Title;

        SoundEffect pickupFX;

        List<Tile[,]> mapList;

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

        Button restartButton;

        Button nextLevelButton;
        Button exitButton;

        Button backButton;
        Rectangle editorNullZone;
        int currentlyEditedMap = 1;
        Button map1Button;
        Button map2Button;
        Button map3Button;


        private SpriteFont font;
        private SpriteFont bigFont;
        Vector2 mousePos;
        bool mouseLReady = false;
        bool mouseRReady = false;

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


            startButton = new Button(new Vector2(220, 205), textureManager.button, new Rectangle(220, 205, 520, 128), "Start Game");
            levelButton = new Button(new Vector2(220, 370), textureManager.button, new Rectangle(220, 370, 520, 128), "Level Select");
            editorButton = new Button(new Vector2(220, 535), textureManager.button, new Rectangle(220, 535, 520, 128), "Level Editor");

            restartButton = new Button(new Vector2(220, 370), textureManager.button, new Rectangle(220, 370, 520, 128), "Main Menu");

            nextLevelButton = new Button(new Vector2(220, 370), textureManager.button, new Rectangle(220, 370, 520, 128), "Start Next Level");
            exitButton = new Button(new Vector2(220, 635), textureManager.button, new Rectangle(220, 635, 520, 128), "Exit Game");

            backButton = new Button(new Vector2(40, 672), textureManager.button, new Rectangle(40, 672, 520, 128), "Save and Exit");
            map1Button = new Button(new Vector2(600, 672), textureManager.smallbutton, new Rectangle(600, 672, 128, 128), "1");
            map2Button = new Button(new Vector2(728, 672), textureManager.smallbutton, new Rectangle(728, 672, 128, 128), "2");
            map3Button = new Button(new Vector2(856, 672), textureManager.smallbutton, new Rectangle(856, 672, 128, 128), "3");
            editorNullZone = new Rectangle(0, 672, 960, 128);

            healthBarList = new List<HealthBar>();

            if (File.Exists("highscores.txt") == false)
            {
                File.Create("highscores.txt");
            }
            highscoreList = File.ReadAllLines("highscores.txt").ToList();

            int healthBarTempPosX = textureManager.bottomMenu.Width - 13 - textureManager.health.Width;
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(healthBarTempPosX, 685);
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
            

            mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);

            switch (gameState)
            {
                case GameState.Title:

                    if (startButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed) {
                        gameState = GameState.InGame;
                        mouseLReady = false;
                        //make this a deep copy somehow
                        tileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                        tileManager.Initialize(pickupManager, enemyManager);
                    }

                    if (levelButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        mouseLReady = false;
                        gameState = GameState.GameOver;
                    }

                    if (editorButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        mouseLReady = false;
                        gameState = GameState.LevelEditor;
                        tileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                    }
                    break;

                case GameState.InGame:

                    double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
                    double smallElapsed = gameTime.ElapsedGameTime.TotalMilliseconds;

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
                    while (player.health + healthCheck < 3 && player.health > 0)
                    {
                        healthBarList[player.health - 1].lost = false;
                        healthBarList[player.health + healthCheck].lost = true;
                        healthCheck++;
                    }
                    if (player.health < 0)
                    {
                        gameState = GameState.GameOver;
                    }

                    break;
                case GameState.Menu:

                    if (nextLevelButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        currentMap++;
                        tileManager.currentMap = (Tile[,])mapList[currentMap].Clone();
                        tileManager.Initialize(pickupManager, enemyManager);
                        gameState = GameState.InGame;
                    }

                    if (exitButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:

                    if (!scoreCalculated) {
                        if (scoreManager.score > 0) {
                            highscoreList.Add(scoreManager.score.ToString());
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
                    if (restartButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        gameState = GameState.Title;
                        scoreCalculated = false;
                        scoreManager.score = 0;
                        pickupManager.Reset();
                        enemyManager.Reset();
                        
                        mouseLReady = false;
                        player.health = 3;
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

                    if (map1Button.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        mouseLReady = false;
                        currentlyEditedMap = 1;
                        tileManager.currentMap = (Tile[,])mapList[currentlyEditedMap-1].Clone();
                    }
                    if (map2Button.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        mouseLReady = false;
                        currentlyEditedMap = 2;
                        tileManager.currentMap = (Tile[,])mapList[currentlyEditedMap - 1].Clone();
                    }
                    if (map3Button.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed && mouseLReady)
                    {
                        mouseLReady = false;
                        currentlyEditedMap = 3;
                        tileManager.currentMap = (Tile[,])mapList[currentlyEditedMap - 1].Clone();
                    }

                    if (backButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        StringBuilder stringToAdd = new StringBuilder(50);
                        gameState = GameState.Title;
                        char[,] types = new char[36, 27];
                        for (int i = 0; i < tileManager.currentMap.GetLength(1); i++)
                        {
                            for (int j = 0; j < tileManager.currentMap.GetLength(0); j++)
                            {
                                if(tileManager.currentMap[j, i].type == TileType.Wall) 
                                {
                                    types[j, i] = 'X';
                                }
                                if (tileManager.currentMap[j, i].type == TileType.Standard)
                                {
                                    types[j, i] = 'o';
                                }
                                if (tileManager.currentMap[j, i].type == TileType.PacmanSpawn)
                                {
                                    types[j, i] = 'p';
                                }
                                if (tileManager.currentMap[j, i].type == TileType.PowerUpSpawn)
                                {
                                    types[j, i] = 'P';
                                }
                                if (tileManager.currentMap[j, i].type == TileType.GhostSpawn)
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

                    startButton.Draw(_spriteBatch, bigFont);
                    levelButton.Draw(_spriteBatch, bigFont);
                    editorButton.Draw(_spriteBatch, bigFont);

                    break;

                case GameState.InGame:


                    tileManager.UpdateDraw(_spriteBatch, gameState);
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

                    nextLevelButton.Draw(_spriteBatch, bigFont);
                    exitButton.Draw(_spriteBatch, bigFont);
                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(font, scoreManager.score.ToString(), new Vector2(400, 200), Color.White);

                    _spriteBatch.DrawString(font, "High Scores", new Vector2(400, 250), Color.White);

                    for (int i = 0; i < highscoreList.Count(); i++)
                    {
                        _spriteBatch.DrawString(font, highscoreList[i], new Vector2(400, 300+i*40), Color.White);
                    }


                    _spriteBatch.DrawString(font, "High Scores", new Vector2(300 + 50, 825 + 50), Color.White);
                    restartButton.Draw(_spriteBatch, bigFont);
                    break;

                case GameState.LevelEditor:

                    tileManager.UpdateDraw(_spriteBatch, gameState);

                    _spriteBatch.Draw(textureManager.bottomMenu, new Vector2(0, 672), Color.White);

                    backButton.Draw(_spriteBatch, bigFont);
                    map1Button.Draw(_spriteBatch, bigFont);
                    map2Button.Draw(_spriteBatch, bigFont);
                    map3Button.Draw(_spriteBatch, bigFont);
                    break;
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
