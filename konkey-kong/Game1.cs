using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;

namespace konkey_kong
{
    public class Game1 : Game
    {
        public enum Direction { Left = 0, Right = 1, Up = 2, Down = 3}
        public enum GameState { Title = 0, InGame = 1, GameOver = 2}
        public enum TileType { Standard = 1, Ladder = 2, Blank = 3}
        public enum EntityType { Player = 1, Enemy = 2, KonkeyKong = 3, Beloved = 4 }

        public enum EntityState { Default = 0, Create = 1, Death = 2 }
        public class Tile
        {
            public Vector2 pos;
            public Rectangle size;
            public TileType type;
            public Texture2D tex;
            public int number1;
            public int number2;

            public void Draw(SpriteBatch spritebatch, SpriteFont font)
            {

                spritebatch.Draw(tex, pos, Color.White);
                //spritebatch.DrawString(font, number1.ToString(), new Vector2(pos.X, pos.Y), Color.White);
                //spritebatch.DrawString(font, number2.ToString(), new Vector2(pos.X, pos.Y+12), Color.White);
            }

            public Tile(Vector2 pos, Rectangle size, TileType type, Texture2D tex, int number1, int number2)
            {
                this.pos = pos;
                this.size = size;
                this.type = type;
                this.tex = tex;

                this.number1 = number1; 
                this.number2 = number2;
            }

        }

        public class Entity
        {
            public Vector2 pos;
            public Vector2 oldPos;
            public Rectangle size;
            public Texture2D tex;
            public bool isMoving;
            public Vector2 speed;
            public EntityType type;
            public Rectangle srcRec = new Rectangle(0,0,40,40);
            public int frame = 0;
            public double frameTimer = 0;
            public double duration = 16000;
            public EntityState state = EntityState.Create;
            public double invulnerableDuration = 0;
            public double speedMult;

            public void Draw(SpriteBatch spritebatch)
            {

                if (invulnerableDuration > 0 && type == EntityType.Player)
                {
                    spritebatch.Draw(tex, pos, srcRec, Color.Red);
                }
                else { 
                spritebatch.Draw(tex, pos, srcRec, Color.White);
                }
            }

            public void PlayerMoveStart(Direction direction)
            {
                if(type == EntityType.Player || state == EntityState.Default) { 
                    switch (direction)
                    {
                        case Direction.Left:
                            speed.X = -2;
                            isMoving = true;
                            oldPos.X = pos.X;

                            break;
                        case Direction.Right:
                            speed.X = 2;
                            isMoving = true;
                            oldPos.X = pos.X;

                            break;
                        case Direction.Down:
                            speed.Y = 1;
                            isMoving = true;
                            oldPos.Y = pos.Y;

                            break;
                        case Direction.Up:
                            speed.Y = -1;
                            isMoving = true;
                            oldPos.Y = pos.Y;

                            break;
                        default:
                        
                            break;
                    }
                }
            }

            public void PlayerMove()
            {
                if (isMoving) { 

                    pos += speed;
                    if(type == EntityType.Enemy)
                    {
                        pos.X += speed.X * (float)speedMult;
                    }
                    size.X = (int)pos.X;
                    size.Y = (int)pos.Y;
                    if (oldPos.Y + 50 <= pos.Y && speed.Y > 0)
                    {
                        pos.Y = oldPos.Y + 50;
                        isMoving = false;
                        speed.Y = 0;
                    }

                    if (oldPos.Y - 50 >= pos.Y && speed.Y < 0)
                    {
                        pos.Y = oldPos.Y - 50;
                        isMoving = false;
                        speed.Y = 0;
                    }

                    if (oldPos.X + 50 <= pos.X && speed.X > 0)
                    {
                        pos.X = oldPos.X + 50;
                        isMoving = false;
                        speed.X = 0;
                    }

                    if (oldPos.X - 50 >= pos.X && speed.X < 0)
                    {
                        pos.X = oldPos.X - 50;
                        isMoving = false;
                        speed.X = 0;
                    }
                }
            }

            public void PlayerAnim(double time)
            {
                if (isMoving)
                {
                    if(speed.X == 0)
                    {
                        srcRec.Y = 80;
                    }
                    if (speed.Y == 0)
                    {
                        if (speed.X > 0) { srcRec.Y = 160; }
                        
                        if (speed.X < 0) { srcRec.Y = 40; }
                           
                    }

                }
                else { srcRec.Y = 0; }

                frameTimer -= time;
                invulnerableDuration -= time;

                if (frameTimer <= 0 && frame < 6)
                {
                    frameTimer = 56;
                    frame++;
                    if (frame > 5) { frame = 0; }
                }
                srcRec.X = frame*40;
            }

            public void EnemyAnim(double time)
            {
                if(state == EntityState.Create)
                {
                    srcRec.Y = 80;
                }

                if (isMoving)
                {
                    if (speed.Y == 0)
                    {
                        if (speed.X > 0) { srcRec.Y = 40; }

                        if (speed.X < 0) { srcRec.Y = 0; }

                    }

                }

                if (duration < 500)
                {
                    if (state != EntityState.Death) { 
                        frame = 0;
                    }
                    state = EntityState.Death;
                    
                    
                    srcRec.Y = 120;
                }

                duration -= time;
                frameTimer -= time;

                if (frameTimer <= 0 && frame < 5)
                {
                    frameTimer = 56;
                    frame++;
                    if (frame > 4)
                    { 
                        if (state == EntityState.Death)
                        {
                            duration = 0;
                        }
                        else { 
                            frame = 0;
                            if (state == EntityState.Create)
                            {
                                state = EntityState.Default;
                            }
                        }
                    }
                }
                srcRec.X = frame * 40;
            }

            public Entity(Vector2 pos, Rectangle size, Texture2D tex, bool isMoving, Vector2 speed, EntityType type, double speedMult)
            {
                this.pos = pos;
                this.size = size;
                this.type = type;
                this.tex = tex;
                this.speed = speed;
                this.isMoving = isMoving;
                this.speedMult = speedMult;
            }


        }

        class HealthBar
        {
            public Vector2 pos;
            public Texture2D tex;
            public bool lost;
            private double frameTimer;
            private int frame;
            Rectangle srcRec = new Rectangle(0, 0, 50, 50);

            public void Draw(SpriteBatch spriteBatch, double time)
            {
                
                if (lost)
                {
                    frameTimer -= time;
                    if (frameTimer <= 0 && frame < 3)
                    {
                        frameTimer = 56;
                        frame++;
                    }
                }
                srcRec.X = frame * 50;


                spriteBatch.Draw(tex, pos, srcRec, Color.White);
            }

            public HealthBar()
            {
            }
            public HealthBar(Vector2 pos, Texture2D tex, bool lost) { this.pos = pos; this.tex = tex; this.lost = lost; }
        }

        GameState gameState = GameState.Title;

        Texture2D tileTex, ladderTex, blankTex, playerTex, player2Tex, backgroundTex, background2Tex, tileCapLTex, tileCapRTex, tileCapBothTex, fireTex, menuButtonTex, smolButtonTex, healthTex;

        Tile[,] tileArray = new Tile[20, 20];
        List<Entity> enemyList;
        List<HealthBar> healthBarList;

        Rectangle menuButton;
        Rectangle player1Button;
        Rectangle player2Button;

        private SpriteFont font;
        Vector2 mousePos;

        float fireTimer = 5;         //timer to spawn fires
        const float TIMER = (float)5;
        bool flameCreated = true;
        int? killFlame;

        Entity beloved;
        Entity player;
        int health = 3;

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
            background2Tex = Content.Load<Texture2D>(@"bg2");
            backgroundTex = Content.Load<Texture2D>(@"bg1");
            playerTex = Content.Load<Texture2D>(@"player_spritesheet");
            player2Tex = Content.Load<Texture2D>(@"player2_spritesheet");
            fireTex = Content.Load<Texture2D>(@"fire_spritesheet");
            menuButtonTex = Content.Load<Texture2D>(@"menubutton");
            smolButtonTex = Content.Load<Texture2D>(@"smolbutton");
            healthTex = Content.Load<Texture2D>(@"health");
            font = Content.Load<SpriteFont>(@"font");

            enemyList = new List<Entity>();
            //tileArray = new Tile[20, 20];

            player = new Entity(new Vector2(5, 0), new Rectangle(5, 0, 40, 40), playerTex, false, new Vector2(0, 0), EntityType.Player, 1);
            beloved = new Entity(new Vector2(5, 0), new Rectangle(5, 0, 40, 40), player2Tex, false, new Vector2(0, 0), EntityType.Beloved, 1);

            player1Button = new Rectangle(300, 450, 175, 150);
            player2Button = new Rectangle(525, 450, 175, 150);
            healthBarList = new List<HealthBar>();

            List<string> strings = new List<string>();
            StreamReader sr = new StreamReader("map.txt");

            int healthBarTempPosX = Window.ClientBounds.Width - 300;
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
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                    if (strings[j][i] == 'x')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }

                    if (strings[j][i] == 'L')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Ladder, ladderTex, i, j);
                    }

                    if (strings[j][i] == 'o')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Blank, blankTex, i, j);
                    }

                    if (strings[j][i] == 'd')
                    {
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Blank, blankTex, i, j);
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
                        tileArray[i, j] = new Tile(tempPos, tempRect, TileType.Standard, tileTex, i, j);
                    }
                    if (strings[j][i] == 'g')
                    {
                        beloved.pos = new Vector2(i * 50 + 5, j * 50);
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

                    //TITLE SCREEN GOES HERE


                    break;

                case GameState.InGame:

                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    fireTimer -= elapsed;

                    if (fireTimer < 0)
                    {
                        fireTimer = TIMER;
                        flameCreated = false;
                    }

                    if (!flameCreated) { 

                        int flameCreatorArrayPosX = rnd.Next(1, tileArray.GetLength(0));
                        int flameCreatorArrayPosY = rnd.Next(1, tileArray.GetLength(1));

                        double flameCreatorSpeedMult = rnd.Next(70, 110)/100;

                        if (tileArray[flameCreatorArrayPosX, flameCreatorArrayPosY].type != TileType.Blank)
                        {
                            Entity flameCreation = new Entity(new Vector2(flameCreatorArrayPosX * 50 + 5, flameCreatorArrayPosY * 50), new Rectangle(flameCreatorArrayPosX * 50 + 5, flameCreatorArrayPosY * 50, 40, 40), fireTex, false, new Vector2(0, 0), EntityType.Enemy, flameCreatorSpeedMult);
                            enemyList.Add(flameCreation);
                            flameCreated = true;
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

                    foreach (Entity e in enemyList)
                    {

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

                    break;
                case GameState.GameOver:






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
                    _spriteBatch.DrawString(font, "Player1", new Vector2(300+50, 450+50), Color.White);
                    _spriteBatch.Draw(smolButtonTex, new Vector2(525, 450), Color.White);
                    _spriteBatch.DrawString(font, "Player2", new Vector2(475 + 50, 450 + 50), Color.White);

                    _spriteBatch.Draw(menuButtonTex, new Vector2(300, 625), Color.White);
                    _spriteBatch.DrawString(font, "High Scores", new Vector2(300 + 50, 625 + 50), Color.White);
                    break;

                case GameState.InGame:

                    _spriteBatch.Draw(background2Tex, new Vector2(0,0), Color.White);
                    foreach (Tile t in tileArray)
                    {
                        t.Draw(_spriteBatch, font);

                    }

                    foreach (Entity e in enemyList)
                    {
                        e.Draw(_spriteBatch);
                        if (e.size.Intersects(player.size) && player.invulnerableDuration <= 0)
                        {
                            player.invulnerableDuration = 1500;
                            health--;
                        }

                    }
                    foreach (HealthBar h in healthBarList)
                    {
                        h.Draw(_spriteBatch, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }

                    beloved.Draw(_spriteBatch);
                    player.Draw(_spriteBatch);

                    break;

            }

            

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
