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

        public enum GameState { Title = 0, InGame = 1, GameOver = 2}

        GameState gameState = GameState.Title;

        Texture2D healthTex, ghostTex, pointTex, pickupTex, blankTex, wallSheetTex, pakemanTex, deathTex
            , bottomMenuTex, orangeTex, strawberryTex, cherryTex, buttonTex;
        SoundEffect pickupFX;

        Tile[,] tileArray = new Tile[32, 25];
        Color bgcolor = new Color(13, 4, 25);
        List<Enemy> enemyList;
        List<HealthBar> healthBarList;
        List<Pickup> pickupList;
        List<string> highscoreList = new List<string>();
        List<int> sortingHighscoreList = new List<int>();
        List<PointString> pointStringList;
        int? killPointString;

        Rectangle startButton;
        Rectangle levelButton;
        Rectangle editorButton;

        Tile gate1;
        Tile gate2;
        int gatesCreated;
        float gateTimer = 0;
        const float GATETIMER = 0.7F;


        private SpriteFont font;
        Vector2 mousePos;

        int ghostscreated = 0;
        bool attemptInput = false;
        bool demoActive;
        bool finalScoreCreated = false;
        float gameOverTimer = 4;      

        const float DEMOTIMER = 2;
        float demoTimer = 2;

        int? killPickup;

        bool bigPickupCreated = false;
        int bigPickupsCreated = 0;
        float pickupCreatorTimer = 20;
        const float PICKUPCREATORTIMER = 20;

        Player player;

        int health = 3;
        int score = 0;
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
            pickupTex = Content.Load<Texture2D>(@"pickup");
            ghostTex = Content.Load<Texture2D>(@"ghost");
            blankTex = Content.Load<Texture2D>(@"blanktile");
            wallSheetTex = Content.Load<Texture2D>(@"tilewall2");
            pakemanTex = Content.Load<Texture2D>(@"pakeman");
            deathTex = Content.Load<Texture2D>(@"pakemanDeath");
            font = Content.Load<SpriteFont>(@"font");
            pickupFX = Content.Load<SoundEffect>(@"pickupFX");

            pointStringList = new List<PointString>();
            enemyList = new List<Enemy>();
            pickupList = new List<Pickup>();
            highscoreList = new List<string>();
            sortingHighscoreList = new List<int>();

            player = new Player(new Vector2(0, 0), new Rectangle(0, 0, 0, 0), pakemanTex, 0, 0, pakemanTex, deathTex);

            startButton = new Rectangle(220, 205, 520, 128);
            levelButton = new Rectangle(220, 370, 520, 128);
            editorButton = new Rectangle(220, 535, 520, 128);

            healthBarList = new List<HealthBar>();

            if (File.Exists("highscores.txt") == false)
            {
                File.Create("highscores.txt");
            }
            highscoreList = File.ReadAllLines("highscores.txt").ToList();

            List<string> strings = new List<string>();
            StreamReader sr = new StreamReader("map1.txt");

            int healthBarTempPosX = bottomMenuTex.Width-13-healthTex.Width;
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(healthBarTempPosX, 685);
                HealthBar healthBarCreator = new HealthBar(pos, healthTex, false);
                healthBarList.Add(healthBarCreator);
                healthBarTempPosX -= healthTex.Width + 5;
            }

            while (!sr.EndOfStream)
            {
                strings.Add(sr.ReadLine());
            }

            sr.Close();

            tileArray = new Tile[strings[0].Length, strings.Count];

            for (int i = 0; i < tileArray.GetLength(0); i++)
            {
                for (int j = 0; j < tileArray.GetLength(1); j++)
                {
                    Vector2 tempPos = new Vector2((i * TILESIZE) - 3*TILESIZE, (j * TILESIZE) - 3*TILESIZE);
                    Rectangle tempRect = new Rectangle((i * TILESIZE) - 3*TILESIZE, (j * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE);
                    if (strings[j][i] == 'X')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Wall, wallSheetTex, i, j);
                    }

                    if (strings[j][i] == 'o')
                    {
                        Pickup pickupCreator = new Pickup(new Vector2(tempPos.X + 8, tempPos.Y + 8), pointTex, new Rectangle((int)(tempPos.X + 8), (int)(tempPos.Y + 8), 8, 8));
                        pickupList.Add(pickupCreator);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, blankTex, i, j);
                    }

                    if (strings[j][i] == 'g')
                    {
                        Pickup pickupCreator = new Pickup(new Vector2(tempPos.X + 8, tempPos.Y + 8), pointTex, new Rectangle((int)(tempPos.X + 8), (int)(tempPos.Y + 8), 8, 8));
                        pickupList.Add(pickupCreator);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, blankTex, i, j);
                        if (gatesCreated == 0) { gate1 = tileArray[i, j]; }
                        if (gatesCreated == 1) { gate2 = tileArray[i, j]; }
                        gatesCreated++;
                    }

                    if (strings[j][i] == 'p')
                    {
                        player.pos = tempPos;
                        player.size = tempRect;
                        player.tilePosX = i;
                        player.tilePosY = j;
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, blankTex, i, j);
                        player.respawnTile = tileArray[i, j];
                    }

                    if (strings[j][i] == 'e')
                    {
                        if (ghostscreated < 3) { 
                        Enemy enemyCreator = new Enemy(tempPos, tempRect, ghostTex, i, j, ghostscreated);
                        enemyList.Add(enemyCreator);
                        ghostscreated++;
                        }
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, blankTex, i, j);
                    }
                }
            }

            foreach (Tile t in tileArray)
            {
                if (t.posX > 0 && t.posX < tileArray.GetLength(0)-1 && t.posY > 0 && t.posY < tileArray.GetLength(1)-1) { 
                t.DetermineWallTex(tileArray[t.posX - 1, t.posY - 1], tileArray[t.posX, t.posY - 1], tileArray[t.posX + 1, t.posY - 1], tileArray[t.posX - 1, t.posY], tileArray[t.posX + 1, t.posY], tileArray[t.posX - 1, t.posY + 1], tileArray[t.posX, t.posY + 1], tileArray[t.posX + 1, t.posY + 1]);
                }
            }

            for (int i = 1 ; i< tileArray.GetLength(0)-1 ; i++ ) {
                for (int j = 1; j < tileArray.GetLength(1)-1; j++)
                {
                    tileArray[i, j].DetermineWallTex(tileArray[i - 1, j - 1], tileArray[i, j - 1], tileArray[i + 1, j - 1], tileArray[i - 1, j], tileArray[i + 1, j], tileArray[i - 1, j + 1], tileArray[i, j + 1], tileArray[i + 1, j + 1]);
                }
            }

            foreach (Enemy e in enemyList)
            {
                e.NeighborTiles(tileArray[e.tilePosX - 1, e.tilePosY], tileArray[e.tilePosX, e.tilePosY - 1], tileArray[e.tilePosX + 1, e.tilePosY], tileArray[e.tilePosX, e.tilePosY + 1], tileArray[e.tilePosX - 2, e.tilePosY], tileArray[e.tilePosX + 2, e.tilePosY], tileArray[e.tilePosX, e.tilePosY - 2], tileArray[e.tilePosX, e.tilePosY + 2], tileArray[e.tilePosX - 1, e.tilePosY - 1], tileArray[e.tilePosX + 1, e.tilePosY - 1], tileArray[e.tilePosX - 1, e.tilePosY + 1], tileArray[e.tilePosX + 1, e.tilePosY + 1]);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if( keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Right)) { attemptInput = true; }
            else { attemptInput = false; }

            mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);

            switch (gameState)
            {
                case GameState.Title:

                    if (startButton.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed) {
                        gameState = GameState.InGame;
                        demoActive = false;
                    }

                    if (levelButton.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }

                    if (editorButton.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.InGame;
                        demoActive = true;
                    }

                    break;

                case GameState.InGame:

                    //timer stuff
                    { 
                        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                        demoTimer -= elapsed;
                        pickupCreatorTimer -= elapsed;
                        if (finalScoreCreated)
                        {
                            gameOverTimer -= elapsed;
                        }
                        if (gameOverTimer <= 0)
                        {
                            gameState = GameState.GameOver;
                        }
                        if (demoTimer <= 0)
                        {
                            demoTimer = DEMOTIMER;
                        }
                        if (gateTimer > 0)
                        {
                            gateTimer -= elapsed;
                        }

                        if( pickupCreatorTimer < 0)
                        {
                            int pickupRandX = rnd.Next(4,tileArray.GetLength(0));
                            int pickupRandY = rnd.Next(4,tileArray.GetLength(1));

                            if(tileArray[pickupRandX, pickupRandY].type == TileType.Standard)
                            {
                                if(bigPickupsCreated == 0)
                                {
                                    BigPickup pickupCreator = new BigPickup(new Vector2((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE), cherryTex, new Rectangle((pickupRandX   * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE));
                                    pickupCreator.value = 200;
                                    pickupCreator.BigPickupCreation();
                                    pickupList.Add(pickupCreator);
                                }
                                if (bigPickupsCreated == 1)
                                {
                                    BigPickup pickupCreator = new BigPickup(new Vector2((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE), strawberryTex, new Rectangle((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE));
                                    pickupCreator.value = 350;
                                    pickupCreator.BigPickupCreation();
                                    pickupList.Add(pickupCreator);
                                }
                                if (bigPickupsCreated == 2)
                                {
                                    BigPickup pickupCreator = new BigPickup(new Vector2((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE), orangeTex, new Rectangle((pickupRandX * TILESIZE) - 3 * TILESIZE, (pickupRandY * TILESIZE) - 3 * TILESIZE, TILESIZE, TILESIZE));
                                    pickupCreator.value = 500;
                                    pickupCreator.BigPickupCreation();
                                    pickupList.Add(pickupCreator);
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
                    

                    foreach (PointString ps in pointStringList)
                    {
                        if (ps.duration <= 0)
                        {
                            killPointString = pointStringList.IndexOf(ps);
                        }
                        ps.PointStringAnim(gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    if (killPointString != null)
                    {
                        pointStringList.RemoveAt((int)killPointString);
                    }
                    killPointString = null;

                    foreach (Pickup p in pickupList)
                    {
                        if (p.size.Intersects(player.size))
                        {
                            Vector2 tempPos = p.pos;
                            PointString tempPointString = new PointString(p.value, tempPos);
                            pointStringList.Add(tempPointString);

                            score += p.value;

                            pickupFX.Play();
                            killPickup = pickupList.IndexOf(p);
                        }
                    }

                    if (killPickup != null) { pickupList.RemoveAt((int)killPickup); }
                    killPickup = null;

                    if(pickupList.Count == 0)
                    {
                        //WIN!
                        gameState = GameState.GameOver;
                    }

                    foreach (Enemy e in enemyList)
                    {
                        if (e.size.Intersects(player.size) && e.state == EntityState.Default && player.state == EntityState.Default)
                        {
                            player.Death();
                            --health;
                        }

                        

                        e.EntityMove();
                        e.ArtificialIntelligence();
                        e.NeighborTiles(tileArray[e.tilePosX - 1, e.tilePosY], tileArray[e.tilePosX, e.tilePosY - 1], tileArray[e.tilePosX + 1, e.tilePosY], tileArray[e.tilePosX, e.tilePosY + 1], tileArray[e.tilePosX - 2, e.tilePosY], tileArray[e.tilePosX + 2, e.tilePosY], tileArray[e.tilePosX, e.tilePosY - 2], tileArray[e.tilePosX, e.tilePosY + 2], tileArray[e.tilePosX - 1, e.tilePosY - 1], tileArray[e.tilePosX + 1, e.tilePosY - 1], tileArray[e.tilePosX - 1, e.tilePosY + 1], tileArray[e.tilePosX + 1, e.tilePosY + 1]);
                        e.Anim(gameTime.ElapsedGameTime.TotalMilliseconds);
                    }

                    
                    player.EntityMove();
                    player.Input();
                    player.NeighborTiles(tileArray[player.tilePosX - 1, player.tilePosY], tileArray[player.tilePosX + 1, player.tilePosY], tileArray[player.tilePosX, player.tilePosY - 1], tileArray[player.tilePosX, player.tilePosY + 1],
                         tileArray[player.tilePosX - 2, player.tilePosY], tileArray[player.tilePosX + 2, player.tilePosY], tileArray[player.tilePosX, player.tilePosY - 2], tileArray[player.tilePosX, player.tilePosY + 2],
                         tileArray[player.tilePosX - 1, player.tilePosY - 1], tileArray[player.tilePosX + 1, player.tilePosY - 1], tileArray[player.tilePosX - 1, player.tilePosY + 1], tileArray[player.tilePosX + 1, player.tilePosY + 1]);
                    player.Anim(gameTime.ElapsedGameTime.TotalMilliseconds);


                    int healthCheck = 0;
                    while (health + healthCheck < 3 && health > 0)
                    {
                        healthBarList[health-1].lost = false;
                        healthBarList[health + healthCheck].lost = true;
                        healthCheck++;
                    }
                    if (health < 0)
                    {
                        gameState = GameState.GameOver;
                    }

                    break;
                case GameState.GameOver:

                    if (!demoActive) { 
                        if ( !scoreCalculated ) { 
                            if (score > 0) { 
                                highscoreList.Add(score.ToString()) ;
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

                    _spriteBatch.Draw(buttonTex, new Vector2(220, 205), Color.White);
                    _spriteBatch.DrawString(font, "Start Game", new Vector2(220 + 50, 205 + 40), Color.White);
                    _spriteBatch.Draw(buttonTex, new Vector2(220, 370), Color.White);
                    _spriteBatch.DrawString(font, "Level Select", new Vector2(220 + 50, 370 + 40), Color.White);
                    _spriteBatch.Draw(buttonTex, new Vector2(220, 535), Color.White);
                    _spriteBatch.DrawString(font, "Level Editor", new Vector2(220 + 50, 535 + 40), Color.White);

                    break;

                case GameState.InGame:

                    
                    foreach (Tile t in tileArray)
                    {
                        t.Draw(_spriteBatch, font);
                    }
                    foreach (Pickup p in pickupList)
                    {
                        p.Draw(_spriteBatch, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    foreach (Entity e in enemyList)
                    {
                        e.Draw(_spriteBatch);
                    }
                    _spriteBatch.DrawString(font, score.ToString(), new Vector2(50, 50), Color.White);
                    _spriteBatch.DrawString(font, "Health", new Vector2(Window.ClientBounds.Width-175, 10), Color.White);
                    
                    foreach (PointString ps in pointStringList)
                    {
                        ps.Draw(_spriteBatch, font, ps.pos, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }

                    player.Draw(_spriteBatch);

                    _spriteBatch.Draw(bottomMenuTex, new Vector2(0, 672), Color.White);
                    foreach (HealthBar h in healthBarList)
                    {
                        h.Draw(_spriteBatch, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }


                    if (demoTimer > 1 && demoActive)
                    {
                        _spriteBatch.DrawString(font, "DEMO", new Vector2(Window.ClientBounds.Width / 2 - 56, 50), Color.White);
                    }

                    break;

                case GameState.GameOver:
                    _spriteBatch.DrawString(font, score.ToString(), new Vector2(400, 200), Color.White);

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
