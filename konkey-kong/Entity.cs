using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;

namespace pakeman
{
    public enum Direction { Left = 0, Right = 1, Up = 2, Down = 3, None = 4 }
    public enum EntityState { Default = 0, Scared = 1 }
    public class Entity : BaseObject
{
    protected Vector2 oldPos;
    
    public bool isMoving = false;
    public Vector2 speed = new Vector2(0,0);
    protected Rectangle srcRec = new Rectangle(0, 0, 32, 32);
    protected int frame = 0;
    protected double frameTimer = 0;
    public EntityState state = EntityState.Default;
    public Direction dir;

    public Direction queuedDirection;

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

    public void Draw(SpriteBatch spritebatch)
        {
            Vector2 adjustedPos = new Vector2(pos.X + 16, pos.Y + 16);
            spritebatch.Draw(tex, adjustedPos, srcRec, Color.White, rotation, new Vector2(16,16), 1, SpriteFx, 1);
        }
        public void NeighborTiles(Tile tLeft, Tile tRight, Tile tUp, Tile tDown, Tile tFarLeft, Tile tFarRight, Tile tFarUp, Tile tFarDown, Tile tLeftUp, Tile tRightUp, Tile tLeftDown, Tile tRightDown)
        {
            this.tLeft = tLeft;
            this.tRight = tRight;
            this.tUp = tUp;
            this.tDown = tDown;

            this.tFarLeft = tFarLeft;
            this.tFarRight = tFarRight;
            this.tFarUp = tFarUp;
            this.tFarDown = tFarDown;

            this.tLeftUp = tLeftUp;
            this.tRightUp = tRightUp;
            this.tLeftDown = tLeftDown;
            this.tRightDown = tRightDown;
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

    public void EntityMoveStart(Direction direction, bool useFar)
    {
            
            switch (direction)
            {
                case Direction.Left:
                    if(tLeft.type == TileType.Standard && !useFar) 
                    { 
                        speed.X = -2;
                        isMoving = true;
                        oldPos.X = pos.X;
                        dir = Direction.Left;
                    }

                    break;
                case Direction.Right:
                    if (tRight.type == TileType.Standard && !useFar)
                    {
                        speed.X = 2;
                        isMoving = true;
                        oldPos.X = pos.X;
                        dir = Direction.Right;
                    }

                    break;
                case Direction.Down:
                    if (tDown.type == TileType.Standard && !useFar)
                    {
                        speed.Y = 2;
                        isMoving = true;
                        oldPos.Y = pos.Y;
                        dir = Direction.Down;
                    } 

                    break;
                case Direction.Up:
                    if (tUp.type == TileType.Standard && !useFar)
                    {
                        speed.Y = -2;
                        isMoving = true;
                        oldPos.Y = pos.Y;
                        dir = Direction.Up;
                    }

                    break;
                default:

                    break;
            }
            
    }

    public void GateMove(Tile targetGate)
        {
            Direction tempDir = Direction.None;
            if (pos.X < targetGate.posX)
            {
                tempDir = Direction.Left;
            } else if (pos.X > targetGate.posX)
            {
                tempDir = Direction.Right;
            }
            pos = targetGate.pos;
            tilePosX = targetGate.posX;
            tilePosY = targetGate.posY;

            EntityMoveStart(tempDir, false);
        }
    public void EntityMove()
    {
        if (isMoving)
        {
            
            pos += speed;
            
            size.X = (int)pos.X;
            size.Y = (int)pos.Y;   
            
            if (oldPos.Y + 32 <= pos.Y && speed.Y > 0)
            {
                pos.Y = oldPos.Y + 32;
                isMoving = false;
                speed.Y = 0;
                tilePosY++;
                if (queuedDirection == Direction.None) 
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir, false); 
                    }
            }

            if (oldPos.Y - 32 >= pos.Y && speed.Y < 0)
            {
                pos.Y = oldPos.Y - 32;
                isMoving = false;
                speed.Y = 0;
                tilePosY--;
                if (queuedDirection == Direction.None)
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir, false);
                    }
                }

            if (oldPos.X + 32 <= pos.X && speed.X > 0)
            {
                pos.X = oldPos.X + 32;
                isMoving = false;
                speed.X = 0;
                tilePosX++;
                if (queuedDirection == Direction.None)
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir, false);
                    }
                }

            if (oldPos.X - 32 >= pos.X && speed.X < 0)
            {
                pos.X = oldPos.X - 32;
                isMoving = false;
                speed.X = 0;
                tilePosX--;
                if (queuedDirection == Direction.None)
                    {
                        UpdateNeighborTiles(dir);
                        EntityMoveStart(dir, false);
                    }
                }

            if(!isMoving && queuedDirection != Direction.None)
                {
                    switch (queuedDirection)
                    {
                        case Direction.Left:
                            UpdateNeighborTiles(dir);
                            EntityMoveStart(queuedDirection, false);

                            break;

                        case Direction.Up:
                            UpdateNeighborTiles(dir);
                            EntityMoveStart(queuedDirection, false);

                            break;

                        case Direction.Right:
                            UpdateNeighborTiles(dir);
                            EntityMoveStart(queuedDirection, false);

                            break;

                        case Direction.Down:
                            UpdateNeighborTiles(dir);
                            EntityMoveStart(queuedDirection, false);

                            break;
                    }
                    queuedDirection = Direction.None;

                }
        }
    }

    public Entity(Vector2 pos, Rectangle size, Texture2D tex, int tilePosX, int tilePosY) : base(pos, tex, size)
    {
        this.pos = pos;
        this.size = size;
        this.tex = tex;
        this.tilePosX = tilePosX;
        this.tilePosY = tilePosY;
    }
    }


    public class Player : Entity
    {

        private bool reverseAnim = false;
        public int usedGate;
        public Tile respawnTile;
        private Texture2D aliveTex;
        private Texture2D deathTex;

        public Player(Vector2 pos, Rectangle size, Texture2D tex, int tileX, int tileY, Texture2D aliveTex, Texture2D deathTex) : base(pos, size, tex, tileX, tileY) {
            this.aliveTex = aliveTex;
            this.deathTex = deathTex;
            }

        public void Input()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if(isMoving) { queuedDirection = Direction.Up; } 
                else { EntityMoveStart(Direction.Up, false); }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (isMoving) { queuedDirection = Direction.Right; }
                else { EntityMoveStart(Direction.Right, false); }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (isMoving) { queuedDirection = Direction.Left; }
                else { EntityMoveStart(Direction.Left, false); }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (isMoving) { queuedDirection = Direction.Down; }
                else { EntityMoveStart(Direction.Down, false); }
            }
        }

        public void Anim(double time)
        {
            frameTimer -= time;
            if (state == EntityState.Default) { 
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
            if (state == EntityState.Scared)
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
            if(state != EntityState.Scared) { 
            tex = deathTex;
            state = EntityState.Scared;
            speed = new Vector2(0, 0);
            frame = 0;
            }

        }
        public void Respawn()
        {
            tex = aliveTex;
            pos = respawnTile.pos;
            tilePosX = respawnTile.posX;
            tilePosY = respawnTile.posY;
            state = EntityState.Default;
            frame = 0;
            isMoving = false;
        }
    }
    public class Enemy : Entity
    {
        public int type;
        protected Direction chosenDir;
        protected bool stopped;
        public Enemy(Vector2 pos, Rectangle size, Texture2D tex, int tileX, int tileY, int type) : base(pos, size, tex, tileX, tileY)
        {
            this.type = type;
        }
        public void Anim(double time)
        {
            frameTimer -= time;

            if (frameTimer <= 0 && frame < 3)
            {
                frameTimer = 56;
                frame++;
                if (frame > 2) { frame = 0; }
            }
            srcRec.X = frame * size.Width;
        }
        public void ArtificialIntelligence()
        {
            Random rnd = new Random();
            if (!isMoving) {
                chosenDir = (Direction)rnd.Next(0, 4);
                EntityMoveStart(chosenDir, false);
            }

        }


    }
}
