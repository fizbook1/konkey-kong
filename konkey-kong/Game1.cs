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

        Texture2D healthTex, ghostTex, pointTex, pickupTex, blankTex, wallSheetTex, pakemanTex, deathTex
            , bottomMenuTex, orangeTex, strawberryTex, cherryTex, buttonTex, powerupWallTex, powerupGhostTex;
        SoundEffect pickupFX;

        Tile[,] loadedMap = new Tile[36, 27];

        List<Tile[,]> mapList;
        Tile[,] tileArr1 = new Tile[36, 27];
        Tile[,] tileArr2 = new Tile[36, 27];
        Tile[,] tileArr3 = new Tile[36, 27];

        Color bgcolor = new Color(13, 4, 25);

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

        Tile gate1;
        Tile gate2;
        int gatesCreated;
        float gateTimer = 0;
        const float GATETIMER = 0.7F;
        int powerupAlternate = 1;


        private SpriteFont font;
        private SpriteFont bigFont;
        Vector2 mousePos;

        int ghostscreated = 0;
        bool demoActive;
        bool finalScoreCreated = false;
        float gameOverTimer = 4;      

        bool bigPickupCreated = false;
        int bigPickupsCreated = 0;
        float pickupCreatorTimer = 20;
        const float PICKUPCREATORTIMER = 20;

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

            orangeTex = Content.Load<Texture2D>(@"orange");
            strawberryTex = Content.Load<Texture2D>(@"stawbebbi");
            cherryTex = Content.Load<Texture2D>(@"cherry");
            bottomMenuTex = Content.Load<Texture2D>(@"bottommenusketch");
            buttonTex = Content.Load<Texture2D>(@"button");
            healthTex = Content.Load<Texture2D>(@"health");
            pointTex = Content.Load<Texture2D>(@"points");
            powerupGhostTex = Content.Load<Texture2D>(@"powerup1");
            powerupWallTex = Content.Load<Texture2D>(@"powerup2");
            pickupTex = Content.Load<Texture2D>(@"pickup");
            ghostTex = Content.Load<Texture2D>(@"ghost");
            blankTex = Content.Load<Texture2D>(@"blanktile");
            wallSheetTex = Content.Load<Texture2D>(@"tilewall2");
            pakemanTex = Content.Load<Texture2D>(@"pakeman");
            deathTex = Content.Load<Texture2D>(@"pakemanDeath");
            font = Content.Load<SpriteFont>(@"font");
            bigFont = Content.Load<SpriteFont>(@"bigFont");
            pickupFX = Content.Load<SoundEffect>(@"pickupFX");

            mapList = new List<Tile[,]>();
            highscoreList = new List<string>();
            sortingHighscoreList = new List<int>();

            tileManager = new TileManager();
            pickupManager = new PickupManager();
            scoreManager = new ScoreManager();
            enemyManager = new EnemyManager();
            player = new Player(new Vector2(0, 0), new Rectangle(0, 0, 0, 0), pakemanTex, 0, 0, pakemanTex, deathTex);

            startButton = new Button(new Vector2(220, 205), buttonTex, new Rectangle(220, 205, 520, 128));
            levelButton = new Button(new Vector2(220, 370), buttonTex, new Rectangle(220, 370, 520, 128));
            editorButton = new Button(new Vector2(220, 535), buttonTex, new Rectangle(220, 535, 520, 128));

            nextLevelButton = new Button(new Vector2(220, 370), buttonTex, new Rectangle(220, 370, 520, 128));
            exitButton = new Button(new Vector2(220, 535), buttonTex, new Rectangle(220, 535, 520, 128));

            healthBarList = new List<HealthBar>();

            if (File.Exists("highscores.txt") == false)
            {
                File.Create("highscores.txt");
            }
            highscoreList = File.ReadAllLines("highscores.txt").ToList();

            int healthBarTempPosX = bottomMenuTex.Width-13-healthTex.Width;
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(healthBarTempPosX, 685);
                HealthBar healthBarCreator = new HealthBar(pos, healthTex, false);
                healthBarList.Add(healthBarCreator);
                healthBarTempPosX -= healthTex.Width + 5;
            }

            List<string> strings = new List<string>();
            StreamReader sr = new StreamReader("map1.txt");

            while (!sr.EndOfStream)
            {
                strings.Add(sr.ReadLine());
            }

            sr.Close();

            loadedMap = new Tile[strings[0].Length, strings.Count];
            
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
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Wall, wallSheetTex, i, j);
                        }
                        if (strings[j][i] == 'o')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Standard, blankTex, i, j);
                        }
                        if (strings[j][i] == 'g')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.Gate, blankTex, i, j);
                        }
                        if (strings[j][i] == 'p')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.PacmanSpawn, blankTex, i, j); 
                        }
                        if (strings[j][i] == 'P')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.PowerUpSpawn, blankTex, i, j);
                        }
                        if (strings[j][i] == 'e')
                        {
                            mapList[l][i, j] = new Tile(tempPos, tempRect, TileType.GhostSpawn, blankTex, i, j);
                        }
                    }
                }
            }



        }

        protected override void Update(GameTime gameTime)
        {
            
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { gameState = GameState.Menu; }
             //   Exit();

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);

            switch (gameState)
            {
                case GameState.Title:

                    if (startButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed) {
                        gameState = GameState.InGame;
                        demoActive = false;

                        loadedMap = (Tile[,])mapList[currentMap].Clone();
                        foreach (Tile t in loadedMap)
                        {
                            if (t.posX > 0 && t.posX < loadedMap.GetLength(0) - 1 && t.posY > 0 && t.posY < loadedMap.GetLength(1) - 1)
                            {
                                t.DetermineWallTex(loadedMap[t.posX - 1, t.posY - 1], loadedMap[t.posX, t.posY - 1], loadedMap[t.posX + 1, t.posY - 1], loadedMap[t.posX - 1, t.posY], loadedMap[t.posX + 1, t.posY], loadedMap[t.posX - 1, t.posY + 1], loadedMap[t.posX, t.posY + 1], loadedMap[t.posX + 1, t.posY + 1]);
                            }
                        }

                        for (int i = 1; i < loadedMap.GetLength(0) - 1; i++)
                        {
                            for (int j = 1; j < loadedMap.GetLength(1) - 1; j++)
                            {
                                loadedMap[i, j].DetermineWallTex(loadedMap[i - 1, j - 1], loadedMap[i, j - 1], loadedMap[i + 1, j - 1], loadedMap[i - 1, j], loadedMap[i + 1, j], loadedMap[i - 1, j + 1], loadedMap[i, j + 1], loadedMap[i + 1, j + 1]);
                            }
                        }

                        tileManager.Initialize(player, pickupManager, enemyManager, ghostTex, pointTex, powerupWallTex, powerupGhostTex);
                        
                        foreach (Enemy e in enemyManager.enemies)
                        {
                            e.NeighborTiles(loadedMap[e.tilePosX - 1, e.tilePosY], loadedMap[e.tilePosX, e.tilePosY - 1], loadedMap[e.tilePosX + 1, e.tilePosY], loadedMap[e.tilePosX, e.tilePosY + 1], loadedMap[e.tilePosX - 2, e.tilePosY], loadedMap[e.tilePosX + 2, e.tilePosY], loadedMap[e.tilePosX, e.tilePosY - 2], loadedMap[e.tilePosX, e.tilePosY + 2], loadedMap[e.tilePosX - 1, e.tilePosY - 1], loadedMap[e.tilePosX + 1, e.tilePosY - 1], loadedMap[e.tilePosX - 1, e.tilePosY + 1], loadedMap[e.tilePosX + 1, e.tilePosY + 1]);
                        }
                    }

                    if (levelButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }

                    if (editorButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.InGame;
                        demoActive = true;
                    }


                    break;

                case GameState.InGame:

                    //timer stuff
                    
                        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                        pickupCreatorTimer -= elapsed;
                        if (finalScoreCreated)
                        {
                            gameOverTimer -= elapsed;
                        }
                        if (gameOverTimer <= 0)
                        {
                            gameState = GameState.GameOver;
                        }
                        if (gateTimer > 0)
                        {
                            gateTimer -= elapsed;
                        }

                        if( pickupCreatorTimer < 0)
                        {
                            int pickupRandX = rnd.Next(4,loadedMap.GetLength(0));
                            int pickupRandY = rnd.Next(4,loadedMap.GetLength(1));

                            if(loadedMap[pickupRandX, pickupRandY].type == TileType.Standard)
                            {
                                if(bigPickupsCreated == 0)
                                {
                                    BigPickup pickupCreator = new BigPickup(new Vector2((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE), cherryTex, new Rectangle((pickupRandX   * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE));
                                    pickupCreator.value = 200;
                                    pickupCreator.BigPickupCreation();
                                    pickupManager.list.Add(pickupCreator);
                                }
                                if (bigPickupsCreated == 1)
                                {
                                    BigPickup pickupCreator = new BigPickup(new Vector2((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE), strawberryTex, new Rectangle((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE));
                                    pickupCreator.value = 350;
                                    pickupCreator.BigPickupCreation();
                                    pickupManager.list.Add(pickupCreator);
                                }
                                if (bigPickupsCreated == 2)
                                {
                                    BigPickup pickupCreator = new BigPickup(new Vector2((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE), orangeTex, new Rectangle((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE));
                                    pickupCreator.value = 500;
                                    pickupCreator.BigPickupCreation();
                                    pickupManager.list.Add(pickupCreator);
                                }
                                
                                bigPickupCreated = true;
                            }
                            if (bigPickupCreated)
                            {
                                bigPickupsCreated++;
                                pickupCreatorTimer = PICKUPCREATORTIMER;
                                bigPickupCreated = false;
                            }
                        }

                    if (player.state == EntityState.PowerupWall)
                    {
                        if(loadedMap[player.tilePosX, player.tilePosY].type == TileType.Wall && player.tilePosX > 2 & player.tilePosX < loadedMap.GetLength(0)-2)
                        {
                            loadedMap[player.tilePosX, player.tilePosY].type = TileType.Standard;
                            loadedMap[player.tilePosX, player.tilePosY].tex = blankTex;
                            foreach (Tile t in loadedMap)
                            {
                                if (t.posX > 0 && t.posX < loadedMap.GetLength(0) - 1 && t.posY > 0 && t.posY < loadedMap.GetLength(1) - 1)
                                {
                                    t.DetermineWallTex(loadedMap[t.posX - 1, t.posY - 1], loadedMap[t.posX, t.posY - 1], loadedMap[t.posX + 1, t.posY - 1], loadedMap[t.posX - 1, t.posY], loadedMap[t.posX + 1, t.posY], loadedMap[t.posX - 1, t.posY + 1], loadedMap[t.posX, t.posY + 1], loadedMap[t.posX + 1, t.posY + 1]);
                                }
                            }
                        }
                    }

                    if (gate1.posX == player.tilePosX && gate1.posY == player.tilePosY && !player.isMoving && gateTimer <= 0)
                    {
                        gateTimer = GATETIMER;
                        player.GateMove(gate2);
                    }
                    if (gate2.posX == player.tilePosX && gate2.posY == player.tilePosY && !player.isMoving && gateTimer <= 0)
                    {
                        gateTimer = GATETIMER;
                        player.GateMove(gate1);
                    }

                    scoreManager.Update(elapsed);
                    pickupManager.Update(elapsed, player, scoreManager);

                    if(pickupManager.list.Count == 0)
                    {
                        gameState = GameState.Menu;
                        pickupManager.list.Clear();
                        enemyManager.enemies.Clear();
                        bigPickupsCreated = 0;
                        gatesCreated = 0;
                        ghostscreated = 0;
                    }

                    enemyManager.Update(elapsed, player, scoreManager);

                    foreach (Enemy e in enemyManager.enemies)
                    {
                        e.NeighborTiles(loadedMap[e.tilePosX - 1, e.tilePosY], loadedMap[e.tilePosX, e.tilePosY - 1], loadedMap[e.tilePosX + 1, e.tilePosY], loadedMap[e.tilePosX, e.tilePosY + 1], loadedMap[e.tilePosX - 2, e.tilePosY], loadedMap[e.tilePosX + 2, e.tilePosY], loadedMap[e.tilePosX, e.tilePosY - 2], loadedMap[e.tilePosX, e.tilePosY + 2], loadedMap[e.tilePosX - 1, e.tilePosY - 1], loadedMap[e.tilePosX + 1, e.tilePosY - 1], loadedMap[e.tilePosX - 1, e.tilePosY + 1], loadedMap[e.tilePosX + 1, e.tilePosY + 1]);
                    }

                    player.Update(elapsed);
                    player.NeighborTiles(loadedMap[player.tilePosX - 1, player.tilePosY], loadedMap[player.tilePosX + 1, player.tilePosY], loadedMap[player.tilePosX, player.tilePosY - 1], loadedMap[player.tilePosX, player.tilePosY + 1],
                         loadedMap[player.tilePosX - 2, player.tilePosY], loadedMap[player.tilePosX + 2, player.tilePosY], loadedMap[player.tilePosX, player.tilePosY - 2], loadedMap[player.tilePosX, player.tilePosY + 2],
                         loadedMap[player.tilePosX - 1, player.tilePosY - 1], loadedMap[player.tilePosX + 1, player.tilePosY - 1], loadedMap[player.tilePosX - 1, player.tilePosY + 1], loadedMap[player.tilePosX + 1, player.tilePosY + 1]);


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
                        
                        demoActive = false;
                        currentMap++;
                        loadedMap = (Tile[,])mapList[currentMap].Clone();
                        foreach (Tile t in loadedMap)
                        {
                            if (t.posX > 0 && t.posX < loadedMap.GetLength(0) - 1 && t.posY > 0 && t.posY < loadedMap.GetLength(1) - 1)
                            {
                                t.DetermineWallTex(loadedMap[t.posX - 1, t.posY - 1], loadedMap[t.posX, t.posY - 1], loadedMap[t.posX + 1, t.posY - 1], loadedMap[t.posX - 1, t.posY], loadedMap[t.posX + 1, t.posY], loadedMap[t.posX - 1, t.posY + 1], loadedMap[t.posX, t.posY + 1], loadedMap[t.posX + 1, t.posY + 1]);
                            }
                        }

                        for (int i = 1; i < loadedMap.GetLength(0) - 1; i++)
                        {
                            for (int j = 1; j < loadedMap.GetLength(1) - 1; j++)
                            {
                                loadedMap[i, j].DetermineWallTex(loadedMap[i - 1, j - 1], loadedMap[i, j - 1], loadedMap[i + 1, j - 1], loadedMap[i - 1, j], loadedMap[i + 1, j], loadedMap[i - 1, j + 1], loadedMap[i, j + 1], loadedMap[i + 1, j + 1]);
                            }
                        }
                        foreach (Tile t in loadedMap)
                        {
                            if (t.type == TileType.GhostSpawn)
                            {
                                if (ghostscreated < 3)
                                {
                                    Enemy enemyCreator = new Enemy(t.pos, t.size, ghostTex, t.posX, t.posY, ghostscreated);
                                    enemyManager.enemies.Add(enemyCreator);
                                    ghostscreated++;
                                    t.type = TileType.Standard;
                                }
                            }
                            if (t.type == TileType.PacmanSpawn)
                            {
                                player.pos = t.pos;
                                player.size = t.size;
                                player.tilePosX = t.posX;
                                player.tilePosY = t.posY;
                                player.respawnTile = t;
                                t.type = TileType.Standard;

                            }
                            if (t.type == TileType.Standard)
                            {
                                Pickup pickupCreator = new Pickup(new Vector2(t.pos.X + 12, t.pos.Y + 12), pointTex, new Rectangle((int)(t.pos.X + 12), (int)(t.pos.Y + 12), 8, 8));
                                pickupManager.list.Add(pickupCreator);
                            }
                            if (t.type == TileType.Gate)
                            {
                                if (gatesCreated == 0) { gate1 = t; }
                                if (gatesCreated == 1) { gate2 = t; }
                                gatesCreated++;
                                t.type = TileType.Standard;
                            }
                        }
                        gameState = GameState.InGame;
                    }

                    if (levelButton.size.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }
                    break;
                case GameState.GameOver:

                    if (!demoActive) { 
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
                    }


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

                    
                    foreach (Tile t in loadedMap)
                    {
                        t.Draw(_spriteBatch, font);
                    }

                    pickupManager.UpdateDraw(_spriteBatch);
                    enemyManager.UpdateDraw(_spriteBatch);
                    scoreManager.UpdateDraw(_spriteBatch, font);
                    player.Draw(_spriteBatch);

                    _spriteBatch.DrawString(font, scoreManager.score.ToString(), new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(font, "Health", new Vector2(Window.ClientBounds.Width - 175, 10), Color.White);

                    _spriteBatch.Draw(bottomMenuTex, new Vector2(0, 672), Color.White);
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
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
