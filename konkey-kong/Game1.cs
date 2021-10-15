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

namespace konkey_kong
{
    public class Game1 : Game
    {
        
        public enum GameState { Title = 0, InGame = 1, GameOver = 2}

        GameState gameState = GameState.Title;

        Texture2D tileTex, ladderTex, blankTex, playerTex, player2Tex, backgroundTex, background2Tex, background3Tex, tileCapLTex, tileCapRTex, 
            tileCapBothTex, fireTex, menuButtonTex, smolButtonTex, healthTex, breadTex, hammerTex, hammerPickupTex, bossTex, switchTex;

        Tile[,] tileArray = new Tile[20, 20];
        List<DummyTile> dummyTileList;

        List<Entity> enemyList;
        List<HealthBar> healthBarList;
        List<Pickup> pickupList;
        List<string> highscoreList = new List<string>();
        List<int> sortingHighscoreList = new List<int>();
        List<PointString> pointStringList;
        List<Switch> switchList;
        int? killPointString;

        Rectangle menuButton;
        Rectangle player1Button;
        Rectangle player2Button;

        SoundEffect flameCreateFX;
        SoundEffect flameKillFX;
        SoundEffect pickupFX;

        private SpriteFont font;
        Vector2 mousePos;

        float fireTimer = 5;         //timer to spawn fires
        const float TIMER = (float)5;
        bool flameCreated = true;
        int? killFlame;

        int? killPickup;

        Hammer hammer;
        Switch switch1;
        Switch switch2;
        int createdSwitches = 0;

        Entity boss;
        Entity beloved;
        Entity player;

        
        int health = 3;
        int score = 0;
        bool scoreCalculated = false;

        Random rnd = new Random();

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            tileTex = Content.Load<Texture2D>(@"platformtile2");
            tileCapLTex = Content.Load<Texture2D>(@"platformtile2leftcap");
            tileCapRTex = Content.Load<Texture2D>(@"platformtile2rightcap");
            tileCapBothTex = Content.Load<Texture2D>(@"platformtile2doublecap");
            ladderTex = Content.Load<Texture2D>(@"laddertile2");
            blankTex = Content.Load<Texture2D>(@"blanktile");
            background3Tex = Content.Load<Texture2D>(@"bg3");
            background2Tex = Content.Load<Texture2D>(@"bg2");
            backgroundTex = Content.Load<Texture2D>(@"bg1");
            playerTex = Content.Load<Texture2D>(@"player_spritesheet");
            player2Tex = Content.Load<Texture2D>(@"player2_spritesheet");
            fireTex = Content.Load<Texture2D>(@"fire_spritesheet");
            menuButtonTex = Content.Load<Texture2D>(@"menubutton");
            smolButtonTex = Content.Load<Texture2D>(@"smolbutton");
            healthTex = Content.Load<Texture2D>(@"health");
            breadTex = Content.Load<Texture2D>(@"bread");
            hammerTex = Content.Load<Texture2D>(@"hammor");
            hammerPickupTex = Content.Load<Texture2D>(@"hammerpickup");
            bossTex = Content.Load<Texture2D>(@"konkey");
            switchTex = Content.Load<Texture2D>(@"switch");
            font = Content.Load<SpriteFont>(@"font");
            flameCreateFX = Content.Load<SoundEffect>(@"flamecreation");
            flameKillFX = Content.Load<SoundEffect>(@"bonk");
            pickupFX = Content.Load<SoundEffect>(@"pickup");

            pointStringList = new List<PointString>();
            enemyList = new List<Entity>();
            pickupList = new List<Pickup>();
            highscoreList = new List<string>();
            sortingHighscoreList = new List<int>();
            switchList = new List<Switch>();
            dummyTileList = new List<DummyTile>();

            //tileArray = new Tile[20, 20];

            boss = new Entity(new Vector2(5, 0), new Rectangle(5, 0, 40, 40), bossTex, false, new Vector2(0, 0), EntityType.KonkeyKong, 1);
            player = new Entity(new Vector2(5, 0), new Rectangle(5, 0, 40, 40), playerTex, false, new Vector2(0, 0), EntityType.Player, 1);
            beloved = new Entity(new Vector2(5, 0), new Rectangle(5, 0, 40, 40), player2Tex, false, new Vector2(0, 0), EntityType.Beloved, 1);
            switch1 = new Switch(new Vector2(5, 0), switchTex, new Rectangle(5, 0, 40, 40));
            switch2 = new Switch(new Vector2(5, 0), switchTex, new Rectangle(5, 0, 40, 40));
            hammer = new Hammer(new Vector2(0, 0), hammerTex, new Rectangle(0, 0, 100, 40));

            player1Button = new Rectangle(300, 450, 175, 150);
            player2Button = new Rectangle(525, 450, 175, 150);
            menuButton = new Rectangle(300, 575, 400, 150);

            healthBarList = new List<HealthBar>();

            if (File.Exists("highscores.txt") == false)
            {
                File.Create("highscores.txt");
            }
            highscoreList = File.ReadAllLines("highscores.txt").ToList();

            List<string> strings = new List<string>();

            StreamReader sr = new StreamReader("map.txt");

            int healthBarTempPosX = Window.ClientBounds.Width - 200;
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = new Vector2(healthBarTempPosX, healthTex.Height + 20);
                HealthBar healthBarCreator = new HealthBar(pos, healthTex, false);
                healthBarList.Add(healthBarCreator);
                healthBarTempPosX += healthTex.Width / 4 + 10;
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
                    Vector2 tempPos = new Vector2(i * 50, j * 50);
                    Rectangle tempRect = new Rectangle(i * 50, j * 50, 50, 50);

                    if (strings[j][i] == 'W')
                    {
                        Switch switchCreator = new Switch(new Vector2(i * 50 + 5, j * 50 + 5), switchTex, new Rectangle(i * 50 + 5, j * 50 + 5, 40, 40));
                        switchList.Add(switchCreator);
                            
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                    if (strings[j][i] == 'x')
                    {
                        int hammerGenerator = rnd.Next(0, 20);
                        if (hammerGenerator == 1)
                        {
                            Pickup pickupCreator = new Pickup(new Vector2(i * 50 + 5, j * 50 + 5), hammerPickupTex, 500, new Rectangle(i * 50 + 5, j * 50 + 5, 40, 40), PickupType.Hammer);
                            pickupList.Add(pickupCreator);
                        }
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                    if (strings[j][i] == 'X')
                    {
                        Pickup pickupCreator = new Pickup(new Vector2(i * 50 + 5, j * 50 + 5), breadTex, 300, new Rectangle(i * 50 + 5, j * 50 + 5, 40, 40), PickupType.Points);
                        pickupList.Add(pickupCreator);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                    if (strings[j][i] == 'l')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Ladder, ladderTex, i, j);
                    }

                    if (strings[j][i] == 'L')
                    {
                        Pickup pickupCreator = new Pickup(new Vector2(i * 50 + 5, j * 50 + 5), breadTex, 300, new Rectangle(i * 50 + 5, j * 50 + 5, 40, 40), PickupType.Points);
                        pickupList.Add(pickupCreator);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Ladder, ladderTex, i, j);
                    }

                    if (strings[j][i] == 'o')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Blank, blankTex, i, j);
                    }

                    if (strings[j][i] == 'd')
                    {
                        boss.pos = new Vector2(i * 50 + 5, j * 50);
                        boss.size = new Rectangle(i * 50 + 5, j * 50, 40, 40);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                    if (strings[j][i] == '<')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Blank, tileCapLTex, i, j);
                    }

                    if (strings[j][i] == '>')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Blank, tileCapRTex, i, j);
                    }

                    if (strings[j][i] == 'b')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Blank, tileCapBothTex, i, j);
                    }

                    if (strings[j][i] == 'p')
                    {
                        player.pos = new Vector2(i * 50 + 5, j * 50);
                        player.size = new Rectangle(i * 50 + 5, j * 50, 40, 40);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }
                    if (strings[j][i] == 'g')
                    {
                        beloved.pos = new Vector2(i * 50 + 5, j * 50);
                        beloved.size = new Rectangle(i * 50 + 5, j * 50, 40, 40);
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                }


            }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            mousePos = new Vector2(mouseState.Position.X, mouseState.Position.Y);
            var keyboardState = Keyboard.GetState();

            switch (gameState)
            {
                case GameState.Title:

                    if (player1Button.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed) {
                        gameState = GameState.InGame;
                        player.tex = playerTex;
                        beloved.tex = player2Tex;
                        fireTimer = 5;
                    }

                    if (player2Button.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.InGame;
                        player.tex = player2Tex;
                        beloved.tex = playerTex;
                        fireTimer = 5;
                    }

                    if (menuButton.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        gameState = GameState.GameOver;
                    }

                    break;

                case GameState.InGame:

                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    fireTimer -= elapsed;

                    if (switchList.All(s => s.toggled == false))
                    {
                        if (fireTimer < 0)
                        {
                            fireTimer = TIMER;
                            flameCreated = false;
                        }
                    }

                    if (!flameCreated) { 

                        int flameCreatorArrayPosX = rnd.Next(1, tileArray.GetLength(0));
                        int flameCreatorArrayPosY = rnd.Next(1, tileArray.GetLength(1));

                        double flameCreatorSpeedMult = rnd.Next(100, 140);

                        if (tileArray[flameCreatorArrayPosX, flameCreatorArrayPosY].type != TileType.Blank)
                        {
                            Entity flameCreation = new Entity(new Vector2(flameCreatorArrayPosX * 50 + 5, flameCreatorArrayPosY * 50), new Rectangle(flameCreatorArrayPosX * 50 + 5, flameCreatorArrayPosY * 50, 40, 40), fireTex, false, new Vector2(0, 0), EntityType.Enemy, flameCreatorSpeedMult/100);
                            enemyList.Add(flameCreation);
                            flameCreated = true;
                            flameCreateFX.Play();
                        }
                    }

                    for (int j = 0; j < tileArray.GetLength(0); j++)
                    {
                        for (int i = 0; i < tileArray.GetLength(1); i++)
                        {
                            if (i < 19 && j < 19 && i > 0)
                            {
                                if (tileArray[i + 1, j].type != TileType.Blank && tileArray[i, j].size.Contains(player.pos))
                                {
                                    if (keyboardState.IsKeyDown(Keys.Right) && player.isMoving == false)
                                    {
                                        player.PlayerMoveStart(Direction.Right);
                                    }
                                }
                                if (tileArray[i - 1, j].type != TileType.Blank && tileArray[i, j].size.Contains(player.pos))
                                {
                                    if (keyboardState.IsKeyDown(Keys.Left) && player.isMoving == false)
                                    {
                                        player.PlayerMoveStart(Direction.Left);
                                    }
                                }
                                if (tileArray[i, j + 1].type == TileType.Ladder && tileArray[i, j].size.Contains(player.pos))
                                {
                                    if (keyboardState.IsKeyDown(Keys.Down) && player.isMoving == false)
                                    {
                                        player.PlayerMoveStart(Direction.Down);
                                    }
                                }
                                if (tileArray[i, j].type == TileType.Ladder && tileArray[i, j].size.Contains(player.pos))
                                {
                                    if (keyboardState.IsKeyDown(Keys.Up) && player.isMoving == false)
                                    {
                                        player.PlayerMoveStart(Direction.Up);
                                    }
                                }
                            }
                        }
                    }

                    foreach (PointString ps in pointStringList)
                    {
                        if (ps.duration <= 0)
                        {
                            killPointString = pointStringList.IndexOf(ps);
                        }
                        PointString.PointStringAnim(ps, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    if (killPointString != null)
                    {
                        pointStringList.RemoveAt((int)killPointString);
                    }
                    killPointString = null;

                    foreach (Pickup p in pickupList)
                    {
                        if (boss.speed.Y > 0) { p.pos.Y += 4; }
                        if (p.size.Intersects(player.size))
                        {
                            Vector2 tempPos = p.pos;
                            PointString tempPointString = new PointString(p.value, tempPos);
                            pointStringList.Add(tempPointString);

                            score += p.value;
                            if( p.type == PickupType.Hammer)
                            {
                                hammer.pos.X = player.pos.X-30;
                                hammer.pos.Y = player.pos.Y;
                                hammer.duration = 8000;
                            }
                            pickupFX.Play();
                            killPickup = pickupList.IndexOf(p);
                        }
                    }

                    if(hammer.duration <= 0)
                    {
                        hammer.pos = new Vector2(0, 0);
                    }

                    if(hammer.duration > 0)
                    {
                        hammer.pos.X = player.pos.X - 30;
                        hammer.pos.Y = player.pos.Y;
                        hammer.Animation(gameTime.ElapsedGameTime.TotalMilliseconds);
                    }

                    if (killPickup != null) { pickupList.RemoveAt((int)killPickup); }
                    killPickup = null;

                    foreach(Switch s in switchList)
                    {
                        if(boss.speed.Y > 0) { s.pos.Y += 4; }
                        if (s.size.Intersects(player.size))
                        {
                            s.Activated((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                        }
                    }
                    int excludedLevel = 0;
                    if(switchList.All(s=>s.toggled == true))
                    {
                        for (int j = 0; j < tileArray.GetLength(0); j++)
                        {
                            for (int i = 0; i < tileArray.GetLength(1); i++)
                            {
                                if (tileArray[i, j].size.Contains(beloved.pos)) { excludedLevel = j; }
                                    
                                if( j != excludedLevel) 
                                {
                                    if (tileArray[i, j].size.Contains(boss.pos) && !tileArray[i, j].size.Contains(beloved.pos))
                                    {
                                        for (int y = 0; y < tileArray.GetLength(1); y++)
                                        {
                                            DummyTile dummyTileCreator = new DummyTile(tileArray[y, j].pos, new Vector2(0, 1), tileArray[y, j].type, tileArray[y, j].tex);
                                            dummyTileList.Add(dummyTileCreator);

                                            tileArray[y, j].tex = blankTex;
                                            tileArray[y, j].type = TileType.Blank;
                                        }
                                    }
                                }
                                
                            }
                        }
                        boss.isMoving = true;
                        boss.speed = new Vector2(0, 4);
                        //do the thing
                    }
                    foreach (DummyTile dt in dummyTileList)
                    {
                        dt.Fall();
                    }

                    foreach (Entity e in enemyList)
                    {
                        if (boss.speed.Y > 0) { e.pos.Y += 4; }
                        if (e.size.Intersects(player.size) && player.invulnerableDuration <= 0 && e.state == EntityState.Default)
                        {
                            player.invulnerableDuration = 1500;
                            health--;
                        }

                        if (hammer.duration > 0)
                        { 
                            if (e.size.Intersects(hammer.size) && e.state != EntityState.Death)
                            {
                                Vector2 tempPos = e.pos;
                                PointString tempPointString = new PointString(200, tempPos);
                                pointStringList.Add(tempPointString);

                                score += 200;
                                e.state = EntityState.Death;
                                e.duration = 140;
                                flameKillFX.Play();
                            }
                        }

                            if (!e.isMoving)
                        {
                            int enemyMoveRandomizer = rnd.Next(0, 2);
                            for (int j = 0; j < tileArray.GetLength(0); j++)
                            {
                                for (int i = 0; i < tileArray.GetLength(1); i++)
                                {
                                    if (i < 19 && j < 19 && i > 0)
                                    {
                                        if (tileArray[i, j].size.Contains(e.pos) && tileArray[i-1,j].type == TileType.Blank)
                                        {
                                            enemyMoveRandomizer = 1;
                                        }
                                        else if (tileArray[i, j].size.Contains(e.pos) && tileArray[i + 1, j].type == TileType.Blank)
                                        {
                                            enemyMoveRandomizer = 0;
                                        }
                                        e.PlayerMoveStart((Direction)enemyMoveRandomizer); 
                                    }
                                }
                            }                       
                        }
                        
                        e.PlayerMove();
                        e.EnemyAnim(gameTime.ElapsedGameTime.TotalMilliseconds);
                        if (e.duration < 0)
                        {
                            killFlame = enemyList.IndexOf(e);
                        }
                    }

                    if (killFlame != null) { enemyList.RemoveAt((int)killFlame); }
                    killFlame = null;

                    boss.KonkeyDrop();
                    boss.PlayerAnim(gameTime.ElapsedGameTime.TotalMilliseconds);
                    beloved.PlayerAnim(gameTime.ElapsedGameTime.TotalMilliseconds);
                    player.PlayerMove();
                    player.PlayerAnim(gameTime.ElapsedGameTime.TotalMilliseconds);
                        
                    int healthCheck = 0;
                    while (health + healthCheck < 3 && health > 0)
                    {
                        healthBarList[health - 1].lost = false;
                        healthBarList[health + healthCheck].lost = true;
                        healthCheck++;
                    }
                    if (health == 0)
                    {
                        gameState = GameState.GameOver;
                    }
                    if (player.size.Intersects(beloved.size))
                    {
                        gameState = GameState.GameOver;
                    }

                    break;
                case GameState.GameOver:

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



                    break;
            }

            // TODO: Add your update logic here
            

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            switch (gameState)
            {
                case GameState.Title:
                    _spriteBatch.Draw(backgroundTex, new Vector2(0, 0), Color.White);
                    _spriteBatch.Draw(smolButtonTex, new Vector2(300, 450), Color.White);
                    _spriteBatch.DrawString(font, "Player1", new Vector2(300+25, 450+50), Color.White);
                    _spriteBatch.Draw(smolButtonTex, new Vector2(525, 450), Color.White);
                    _spriteBatch.DrawString(font, "Player2", new Vector2(475 + 25, 450 + 50), Color.White);

                    _spriteBatch.Draw(menuButtonTex, new Vector2(300, 625), Color.White);
                    _spriteBatch.DrawString(font, "High Scores", new Vector2(300 + 50, 625 + 50), Color.White);
                    break;

                case GameState.InGame:

                    _spriteBatch.Draw(background2Tex, new Vector2(0,0), Color.White);
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
                    foreach (HealthBar h in healthBarList)
                    {
                        h.Draw(_spriteBatch, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    if (hammer.duration > 0)
                    {
                        hammer.Draw(_spriteBatch);
                    }
                    foreach (PointString ps in pointStringList)
                    {
                        ps.Draw(_spriteBatch, font, ps.pos, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }
                    foreach (Switch s in switchList)
                    {
                        s.Draw(_spriteBatch);
                    }
                    foreach(DummyTile dt in dummyTileList)
                    {
                        dt.Draw(_spriteBatch);
                    }

                    boss.Draw(_spriteBatch);
                    beloved.Draw(_spriteBatch);
                    player.Draw(_spriteBatch);

                    break;

                case GameState.GameOver:
                    _spriteBatch.Draw(background3Tex, new Vector2(0, 0), Color.White);
                    _spriteBatch.DrawString(font, score.ToString(), new Vector2(400, 200), Color.White);

                    _spriteBatch.DrawString(font, "High Scores", new Vector2(400, 250), Color.White);

                    for (int i = 0; i < highscoreList.Count(); i++)
                    {
                        _spriteBatch.DrawString(font, highscoreList[i], new Vector2(400, 300+i*40), Color.White);
                    }


                    _spriteBatch.Draw(menuButtonTex, new Vector2(300, 825), Color.White);
                    _spriteBatch.DrawString(font, "High Scores", new Vector2(300 + 50, 825 + 50), Color.White);
                    break;
            }

            

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
