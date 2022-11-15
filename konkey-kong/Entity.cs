using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using static pakeman.Game1;

namespace pakeman
{
    public enum Direction { Left = 0, Right = 1, Up = 2, Down = 3, None = 4 } 
    public enum EntityState { Default = 0, Scared = 1, Death = 2, PowerupGhost = 3, PowerupWall = 4 }
    
    public class Entity : BaseObject
{
    protected SoundManager sound;
    
    public bool isMoving = false;
    public Vector2 speed = new Vector2(0,0);
    protected float speedMult = 1;
    protected Vector2 oldPos;
    public EntityState state = EntityState.Default;
    public Direction dir;
    public Direction queuedDirection;
    public Tile respawnTile;

    protected Rectangle srcRec = new Rectangle(0, 0, TILESIZE, TILESIZE);
    protected int frame = 0;
    protected double frameTimer = 0;
    protected SpriteEffects SpriteFx;
    protected float rotation;

    protected Tile tLeft;
    protected Tile tUp;
    protected Tile tRight;
    protected Tile tDown;

    protected Tile tFarLeft;
    protected Tile tFarUp;
    protected Tile tFarRight;
    protected Tile tFarDown;

    protected Tile tLeftUp;
    protected Tile tRightUp;
    protected Tile tLeftDown;
    protected Tile tRightDown;

    public int tilePosX;
    public int tilePosY;

    public virtual void Draw(SpriteBatch spritebatch)
        {
            Vector2 adjustedPos = new Vector2(pos.X + TILESIZE/2, pos.Y + TILESIZE / 2);
            spritebatch.Draw(tex, adjustedPos, srcRec, Color.White, rotation, new Vector2(TILESIZE / 2, TILESIZE / 2), 1, SpriteFx, 1);
        }
        public void NeighborTiles(Tile[] tiles)
        {
            this.tLeft = tiles[0];
            this.tRight = tiles[1];
            this.tUp = tiles[2];
            this.tDown = tiles[3];

            this.tFarLeft = tiles[4];
            this.tFarRight = tiles[5];
            this.tFarUp = tiles[6];
            this.tFarDown = tiles[7];

            this.tLeftUp = tiles[8];
            this.tRightUp = tiles[9];
            this.tLeftDown = tiles[10];
            this.tRightDown = tiles[11];
        }

        protected void UpdateNeighborTiles(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right:
                    tUp = tRightUp;
                    tRight = tFarRight;
                    tDown = tRightDown;
                break;
                case Direction.Left:
                    tUp = tLeftUp;
                    tLeft = tFarLeft;
                    tDown = tLeftDown;
                    break;
                case Direction.Up:
                    tUp = tFarUp;
                    tRight = tRightUp;
                    tLeft = tLeftUp;
                    break;
                case Direction.Down:
                    tDown = tFarDown;
                    tRight = tRightDown;
                    tLeft = tLeftDown;
                    break;
            }
        }

    public void EntityMoveStart(Direction direction)
    {
            switch (direction)
            {
                case Direction.Left:
                    if(tLeft.type != TileType.Wall || state == EntityState.PowerupWall && tLeft.posX > 1) 
                    { 
                        { 
                            speed.X = -2 * speedMult; 
                        }
                        
                        isMoving = true;
                        oldPos.X = pos.X;
                        dir = Direction.Left;
                    }

                    break;
                case Direction.Right:
                    if (tRight.type != TileType.Wall || state == EntityState.PowerupWall && tRight.posX < 35)
                    {
                        {
                            speed.X = 2* speedMult;
                        }
                        isMoving = true;
                        oldPos.X = pos.X;
                        dir = Direction.Right;
                    }

                    break;
                case Direction.Down:
                    if (tDown.type != TileType.Wall || state == EntityState.PowerupWall && tDown.posY < 25)
                    {
                        {
                            speed.Y = 2 * speedMult;
                        }
                        isMoving = true;
                        oldPos.Y = pos.Y;
                        dir = Direction.Down;
                    } 

                    break;
                case Direction.Up:
                    if (tUp.type != TileType.Wall || state == EntityState.PowerupWall && tUp.posY > 1)
                    {
                        {
                            speed.Y = -2 * speedMult;
                        }
                        isMoving = true;
                        oldPos.Y = pos.Y;
                        dir = Direction.Up;
                    }

                    break;
                default:

                    break;
            }
            
    }

    public void GateMove(Tile targetGate, Direction tempDir)
        {

            pos = targetGate.pos;
            tilePosX = targetGate.posX;
            tilePosY = targetGate.posY;

            EntityMoveStart(tempDir);
        }
    public void EntityMove()
    {
        if (isMoving)
        {
            
            pos += speed;
            size.X = (int)pos.X;
            size.Y = (int)pos.Y;   
            
            if (oldPos.Y + TILESIZE <= pos.Y && speed.Y > 0)
            {
                pos.Y = oldPos.Y + TILESIZE;
                isMoving = false;
                speed.Y = 0;
                tilePosY++;
                if (queuedDirection == Direction.None) 
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir); 
                    }
            }

            if (oldPos.Y - TILESIZE >= pos.Y && speed.Y < 0)
            {
                pos.Y = oldPos.Y - TILESIZE;
                isMoving = false;
                speed.Y = 0;
                tilePosY--;
                if (queuedDirection == Direction.None)
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir);
                    }
                }

            if (oldPos.X + TILESIZE <= pos.X && speed.X > 0)
            {
                pos.X = oldPos.X + TILESIZE;
                isMoving = false;
                speed.X = 0;
                tilePosX++;
                if (queuedDirection == Direction.None)
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir);
                    }
                }

            if (oldPos.X - TILESIZE >= pos.X && speed.X < 0)
            {
                pos.X = oldPos.X - TILESIZE;
                isMoving = false;
                speed.X = 0;
                tilePosX--;
                if (queuedDirection == Direction.None)
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir);
                    }
                }

            if(!isMoving && queuedDirection != Direction.None)
                {
                    switch (queuedDirection)
                    {
                        case Direction.Left:
                            UpdateNeighborTiles(dir);
                            if(tLeft.type != TileType.Wall || state == EntityState.PowerupWall)
                            {
                                EntityMoveStart(queuedDirection);
                            }
                            else { EntityMoveStart(dir); }

                            break;

                        case Direction.Up:
                            UpdateNeighborTiles(dir);

                            if (tUp.type != TileType.Wall || state == EntityState.PowerupWall)
                            {
                                EntityMoveStart(queuedDirection);
                            }
                            else { EntityMoveStart(dir); }

                            break;

                        case Direction.Right:
                            UpdateNeighborTiles(dir);
                            if (tRight.type != TileType.Wall || state == EntityState.PowerupWall)
                            {
                                EntityMoveStart(queuedDirection);
                            }
                            else { EntityMoveStart(dir); }

                            break;

                        case Direction.Down:
                            UpdateNeighborTiles(dir);
                            if (tDown.type != TileType.Wall || state == EntityState.PowerupWall)
                            {
                                EntityMoveStart(queuedDirection);
                            }
                            else { EntityMoveStart(dir); }


                            break;
                    }
                    queuedDirection = Direction.None;

                }
        }
    }

    public Entity(Vector2 pos, Rectangle size, Texture2D tex, int tilePosX, int tilePosY, SoundManager sound) : base(pos, tex, size)
    {
        this.pos = pos;
        this.size = size;
        this.tex = tex;
        this.tilePosX = tilePosX;
        this.tilePosY = tilePosY;
        this.sound = sound;
    }
    }


    public class Player : Entity
    {
        TextureManager textures;
        public int health = 3;
        private bool reverseAnim = false;
        public int usedGate;
        public double PowerupDuration;
        public static Point location;
        public static Player self;

        public Player(Vector2 pos, Rectangle size, Texture2D tex, int tileX, int tileY, TextureManager textures, SoundManager sound) : base(pos, size, tex, tileX, tileY, sound) {
            this.textures = textures;
            self = this;
            }
        public void Update(double time, TileManager tiles)
        {
            location = new Point(tilePosX, tilePosY);
            NeighborTiles(tiles.NeighborTiles(tilePosX, tilePosY));
            if(state == EntityState.PowerupWall || state == EntityState.PowerupGhost)
            {
                PowerupTimer(time);
                tex = textures.pakemanAngy;
            } 
            else if (state != EntityState.Death) { tex = textures.pakeman; }
            
            EntityMove();
            Input();
            Anim(time);

        }
        public void PowerupTimer(double time)
        {
            PowerupDuration -= time;
            if (PowerupDuration < 0)
            {
                state = EntityState.Default;
                tex = textures.pakeman;
            }
        }

        public void Input()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if(isMoving) { queuedDirection = Direction.Up; } 
                else { EntityMoveStart(Direction.Up); }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (isMoving) { queuedDirection = Direction.Right; }
                else { EntityMoveStart(Direction.Right); }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (isMoving) { queuedDirection = Direction.Left; }
                else { EntityMoveStart(Direction.Left); }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (isMoving) { queuedDirection = Direction.Down; }
                else { EntityMoveStart(Direction.Down); }
            }
        }

        public void Anim(double time)
        {
            frameTimer -= time;
            if (state != EntityState.Death) { 
                if (frameTimer <= 0 && frame < 3 && reverseAnim == false)
                {
                    frameTimer = 56;
                    frame++;
                    if (frame > 2) { reverseAnim = true; }
                }

                if (frameTimer <= 0 && frame > -1 && reverseAnim == true)
                {
                    frameTimer = 56;
                    frame--;
                    if (frame < 1) { reverseAnim = false; }
                }

                if (dir == Direction.Left)
                {
                    rotation = MathHelper.ToRadians(0);
                    SpriteFx = SpriteEffects.FlipHorizontally;
                }
                if (dir == Direction.Right)
                {
                    rotation = MathHelper.ToRadians(0);
                    SpriteFx = SpriteEffects.None;
                }
                if (dir == Direction.Up)
                {
                    SpriteFx = SpriteEffects.FlipHorizontally;
                    rotation = MathHelper.ToRadians(90);
                }
                if (dir == Direction.Down)
                {
                    SpriteFx = SpriteEffects.None;
                    rotation = MathHelper.ToRadians(90);
                }
            }
            if (state == EntityState.Death)
            {
                if (frameTimer <= 0)
                {
                    frameTimer = 56;
                    frame++;
                    if(frame > 15) { Respawn(); }
                }
            }
            srcRec.X = frame * size.Width;
        }

        public void Death()
        {
            state = EntityState.Death;
            tex = textures.pakemandeath;
            speed = new Vector2(0, 0);
            frame = 0;
            sound.pakemanDeathInst.Play();
        }
        public void Respawn()
        {
            tex = textures.pakeman;
            pos = respawnTile.pos;
            size.X = (int)pos.X;
            size.Y = (int)pos.Y;
            tilePosX = respawnTile.posX;
            tilePosY = respawnTile.posY;
            state = EntityState.Default;
            frame = 0;
            isMoving = false;
            health--;
        }


    }
}
