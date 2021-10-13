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
    public enum Direction { Left = 0, Right = 1, Up = 2, Down = 3 }
    public enum EntityType { Player = 1, Enemy = 2, KonkeyKong = 3, Beloved = 4 }
    public enum EntityState { Default = 0, Create = 1, Death = 2 }
    public class Entity
{
    public Vector2 pos;
    public Vector2 oldPos;
    public Rectangle size;
    public Texture2D tex;
    public bool isMoving;
    public Vector2 speed;
    public EntityType type;
    public Rectangle srcRec = new Rectangle(0, 0, 40, 40);
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
        else
        {
            spritebatch.Draw(tex, pos, srcRec, Color.White);
        }
    }

    public void KonkeyDrop()
        {
            if (speed.Y < 4) { speed.Y *= (float)1.1; }
            pos += speed;
        }
    public void PlayerMoveStart(Direction direction)
    {
        if (type == EntityType.Player || state == EntityState.Default)
        {
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
        if (isMoving)
        {

            
            if (type == EntityType.Enemy)
            {
                pos.X += speed.X * (float)speedMult;
            } 
            else
            {
                pos += speed;
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
            if (speed.X == 0)
            {
                srcRec.Y = 80;
            }
            if (speed.Y == 0)
            {
                if (speed.X > 0) { srcRec.Y = 160; }

                if (speed.X < 0) { srcRec.Y = 40; }

            }

        }
        else if (type != EntityType.KonkeyKong) { srcRec.Y = 0; }
        if (type == EntityType.KonkeyKong && speed.Y > 0)
            {
                srcRec.Y = 40;
            }

        frameTimer -= time;
        invulnerableDuration -= time;

        

        if (frameTimer <= 0 && frame < 6)
        {
            frameTimer = 56;
            frame++;
            if (frame > 5) { frame = 0; }
        }
        srcRec.X = frame * size.Width;
    }

    public void EnemyAnim(double time)
    {
        if (state == EntityState.Create)
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

        if (duration < 300)
        {
            if (state != EntityState.Death)
            {
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
                else
                {
                    frame = 0;
                    if (state == EntityState.Create)
                    {
                        state = EntityState.Default;
                    }
                }
            }
        }
        srcRec.X = frame * size.Width;
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

        public Entity()
        {
        }
    }
}
