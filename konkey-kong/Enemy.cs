using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace pakeman
{
    public enum EnemyBehavior
    {
        Flee, Chase, Roam
    }
    public class Enemy : Entity
    {
        private int timeRoamed = 0;
        public int type;
        protected Direction chosenDir;
        protected bool stopped;
        public Texture2D defaultTex;
        public EnemyBehavior behavior = EnemyBehavior.Roam;
        protected Texture2D scaredTex;
        public Enemy(Vector2 pos, Rectangle size, Texture2D tex, int tileX, int tileY, Texture2D defaultTex, Texture2D scaredTex, int type, SoundManager sound) : base(pos, size, tex, tileX, tileY, sound)
        {
            this.type = type;
            this.defaultTex = defaultTex;
            this.scaredTex = scaredTex;
        }
        public void Anim(double time)
        {
            frameTimer -= time;
            if(state != EntityState.Death) { 
                if (frameTimer <= 0 && frame < 3)
                {
                    frameTimer = 56;
                    frame++;
                    if (frame > 2) { frame = 0; }
                }
            }

            if (state == EntityState.Death)
            {
                if (frameTimer <= 0 && frame < 30)
                {
                    frameTimer = 56;
                    frame++;
                    if (frame > 28) { Respawn(); }
                }
            }

            srcRec.X = frame * size.Width;
        }

        public void DetermineBehavior()
        {
            if(!isMoving)
            {
                DecisionTree dt = new DecisionTree(EnemyManager.world.Copy(this), this);

            }
            switch(behavior)
            {
                case EnemyBehavior.Flee:
                    Scared(Player.self);
                    break;

                case EnemyBehavior.Chase:
                    Chase();
                    break;

                case EnemyBehavior.Roam:
                    Roam();
                    break;
            }
        }
        public void Roam()
        {
            Random rnd = new Random();
            if(!isMoving)
            EntityMoveStart((Direction)rnd.Next(0, 4));
            if (timeRoamed > rnd.Next(0, 10))
            {
                queuedDirection = (Direction)rnd.Next(0, 4);
                if (!isMoving)
                {
                    EntityMoveStart(queuedDirection);
                }

            timeRoamed++;
            }
                
            
        }

        public void Chase()
        {
            //Point gohere = new Point();
            ////if(!isMoving)
            ////{
            //    Graph graph = new Graph(TileManager.currentMap);
            //    gohere = graph.NextMove(new Point(tilePosX, tilePosY), Player.location);
            //}

            if (!isMoving)
            {
                Point gohere = new Point();
                //if(!isMoving)
                //{
                Graph graph = new Graph(TileManager.currentMap);
                gohere = graph.NextMove(new Point(tilePosX, tilePosY), Player.location);
                if (gohere.X < tilePosX)
                {
                    EntityMoveStart((Direction)0);
                }
                if (gohere.X > tilePosX)
                {
                    EntityMoveStart((Direction)1);
                }
                if (gohere.Y < tilePosY)
                {
                    EntityMoveStart((Direction)2);
                }
                if (gohere.Y > tilePosY)
                {
                    EntityMoveStart((Direction)3);
                }
            }
            if (isMoving)
            {
                Point gohere = new Point();
                //if(!isMoving)
                //{
                Graph graph = new Graph(TileManager.currentMap);
                switch(dir)
                {
                    case Direction.Left:
                        gohere = graph.NextMove(new Point(tilePosX -1 , tilePosY), Player.location);
                        break;
                    case Direction.Right:
                        gohere = graph.NextMove(new Point(tilePosX + 1, tilePosY), Player.location);
                        break;
                    case Direction.Up:
                        gohere = graph.NextMove(new Point(tilePosX, tilePosY - 1), Player.location);
                        break;
                    case Direction.Down:
                        gohere = graph.NextMove(new Point(tilePosX, tilePosY + 1), Player.location);
                        break;
                }
                
                if (gohere.X < tilePosX)
                {

                    queuedDirection = (Direction)0;
                }
                if (gohere.X > tilePosX)
                {
                    queuedDirection = (Direction)1;
                }
                if (gohere.Y < tilePosY)
                {
                    queuedDirection = (Direction)2;
                }
                if (gohere.Y > tilePosY)
                {
                    queuedDirection = (Direction)3;
                }
            }

        }
        public void Scared(Player player)
        {
            int differenceX;
            int differenceY;

            differenceX = player.tilePosX - tilePosX;
            differenceY = player.tilePosY - tilePosY;

            if (Math.Abs(differenceX) > Math.Abs(differenceY))
            {
                if (differenceX < 0)
                {
                    if (tRight.type == TileType.Standard)
                    {
                        if (!isMoving) { EntityMoveStart(Direction.Right); }
                        else { queuedDirection = Direction.Right; }

                    }
                    else
                    {
                        if (differenceY < 0 && tDown.type == TileType.Standard)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Down); }
                            else { queuedDirection = Direction.Down; }
                        }
                        if (differenceY > 0 && tUp.type == TileType.Standard)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Up); }
                            else { queuedDirection = Direction.Up; }
                        }
                        if (differenceY == 0)
                        {
                            if (tLeftDown.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Down); }
                                else { queuedDirection = Direction.Right; }
                            }
                            if (tLeftUp.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Up); }
                                else { queuedDirection = Direction.Right; }
                            }
                        }
                    }
                }
                if (differenceX > 0)
                {
                    if (tLeft.type == TileType.Standard)
                    {
                        if (!isMoving) { EntityMoveStart(Direction.Left); }
                        else { queuedDirection = Direction.Left; }
                    }
                    else
                    {
                        if (differenceY < 0 && tDown.type == TileType.Standard)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Down); }
                            else { queuedDirection = Direction.Down; }
                        }
                        if (differenceY > 0 && tUp.type == TileType.Standard)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Up); }
                            else { queuedDirection = Direction.Up; }
                        }
                        if (differenceY == 0)
                        {
                            if (tRightDown.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Down); }
                                else { queuedDirection = Direction.Left; }
                            }
                            if (tRightUp.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Up); }
                                else { queuedDirection = Direction.Left; }
                            }
                        }
                    }
                }
            }//
            if (Math.Abs(differenceY) > Math.Abs(differenceX))
            {
                if (differenceY < 0)
                {
                    if (tDown.type == TileType.Standard)
                    {
                        if (!isMoving) { EntityMoveStart(Direction.Down); }
                        else { queuedDirection = Direction.Down; }
                    }
                    else
                    {
                        if (differenceX < 0)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Right); }
                            else { queuedDirection = Direction.Right; }
                        }
                        if (differenceX > 0)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Left); }
                            else { queuedDirection = Direction.Left; }
                        }
                        if (differenceX == 0)
                        {
                            if (tRightUp.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Right); }
                                else { queuedDirection = Direction.Up; }
                            }
                            if (tLeftUp.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Left); }
                                else { queuedDirection = Direction.Up; }
                            }
                        }
                    }
                    //move up
                }
                if (differenceY > 0)
                {
                    if (tUp.type == TileType.Standard)
                    {
                        if (!isMoving) { EntityMoveStart(Direction.Up); }
                        else { queuedDirection = Direction.Up; }
                    }
                    else
                    {
                        if (differenceX < 0)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Right); }
                            else { queuedDirection = Direction.Right; }
                        }
                        if (differenceX > 0)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Left); }
                            else { queuedDirection = Direction.Left; }
                        }
                        if (differenceX == 0)
                        {
                            if (tRightDown.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Right); }
                                else { queuedDirection = Direction.Down; }
                            }
                            if (tLeftDown.type == TileType.Standard)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Left); }
                                else { queuedDirection = Direction.Down; }
                            }
                        }
                    }
                    //move down
                }
            
        


            }
        }

        public void Death()
        {
            if (state != EntityState.Death)
            {
                
                state = EntityState.Death;
                speed = new Vector2(0, 0);
                frame = 0;
                sound.ghostDeathInst.Play();
            }
        }
        public void Respawn()
        {
            pos = respawnTile.pos;
            tilePosX = respawnTile.posX;
            tilePosY = respawnTile.posY;
            state = EntityState.Default;
            frame = 0;
            isMoving = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(Game1.font, behavior.ToString(), pos, Color.White);
        }
        public void ToggleState()
        {
            if(state == EntityState.Scared)
            {
                speedMult = 0.5F;
                tex = scaredTex;
            }
            else
            {
                speedMult = 1F;
                tex = defaultTex;
            }

        }
        public void ArtificialIntelligence(Player player)
        {

            DetermineBehavior();

            Random rnd = new Random();
            Direction chosenDir2;
            if (type == 4) { 
                if (!isMoving)
                {
                    chosenDir = (Direction)rnd.Next(0, 4);
                    EntityMoveStart(chosenDir);
                }
            }
            if (type == 4)
            {
                chosenDir2 = (Direction)rnd.Next(0, 4);
                if (!isMoving)
                {
                    chosenDir = (Direction)rnd.Next(0, 4);
                    EntityMoveStart(chosenDir);
                }
                queuedDirection = chosenDir2;

            }
            if (type == 4)
            {
                
                int differenceX;
                int differenceY;

                differenceX = player.tilePosX - tilePosX;
                differenceY = player.tilePosY - tilePosY;

                if (Math.Abs(differenceX) > Math.Abs(differenceY))
                {
                    if( differenceX < 0)
                    {
                        if(tLeft.type != TileType.Wall)
                        {
                            if(!isMoving) { EntityMoveStart(Direction.Left); }
                            else { queuedDirection = Direction.Left; }
                            
                        } 
                        else
                        {
                            if (differenceY < 0 && tUp.type != TileType.Wall)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Up); }
                                else { queuedDirection = Direction.Up; }
                            }
                            if (differenceY > 0 && tDown.type != TileType.Wall)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Down); }
                                else { queuedDirection = Direction.Down; }
                            }
                            if (differenceY >= -1 && differenceY <= 1  )
                            {
                                if(tLeftDown.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Down); }
                                    else { queuedDirection = Direction.Left; }
                                }
                                if (tLeftUp.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Up); }
                                    else { queuedDirection = Direction.Left; }
                                }
                            }
                        }
                    }
                    if ( differenceX > 0)
                    {
                        if (tRight.type != TileType.Wall)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Right); }
                            else { queuedDirection = Direction.Right; }
                        }
                        else
                        {
                            if (differenceY < 0 && tUp.type != TileType.Wall)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Up); }
                                else { queuedDirection = Direction.Up; }
                            }
                            if (differenceY > 0 && tDown.type != TileType.Wall)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Down); }
                                else { queuedDirection = Direction.Down; }
                            }
                            if (differenceY >= -1 && differenceY <= 1)
                            {
                                if (tRightDown.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Down); }
                                    else { queuedDirection = Direction.Right; }
                                }
                                if (tRightUp.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Up); }
                                    else { queuedDirection = Direction.Right; }
                                }
                            }
                        }
                    }
                }
                if (Math.Abs(differenceY) > Math.Abs(differenceX))
                {
                    if (differenceY < 0)
                    {
                        if (tUp.type != TileType.Wall)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Up); }
                            else { queuedDirection = Direction.Up; }
                        }
                        else
                        {
                            if (differenceX < 0)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Left); }
                                else { queuedDirection = Direction.Left; }
                            }
                            if (differenceX > 0)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Right); }
                                else { queuedDirection = Direction.Right; }
                            }
                            if (differenceX >= -1 && differenceX <= 1)
                            {
                                if (tLeftUp.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Left); }
                                    else { queuedDirection = Direction.Up; }
                                }
                                if (tRightUp.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Right); }
                                    else { queuedDirection = Direction.Up; }
                                }
                            }
                        }
                        //move up
                    }
                    if (differenceY > 0)
                    {
                        if (tDown.type != TileType.Wall)
                        {
                            if (!isMoving) { EntityMoveStart(Direction.Down); }
                            else { queuedDirection = Direction.Down; }
                        }
                        else
                        {
                            if (differenceX < 0)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Left); }
                                else { queuedDirection = Direction.Left; }
                            }
                            if (differenceX > 0)
                            {
                                if (!isMoving) { EntityMoveStart(Direction.Right); }
                                else { queuedDirection = Direction.Right; }
                            }
                            if (differenceX >= -1 && differenceX <= 1)
                            {
                                if (tLeftDown.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Left); }
                                    else { queuedDirection = Direction.Down; }
                                }
                                if (tRightDown.type != TileType.Wall)
                                {
                                    if (!isMoving) { EntityMoveStart(Direction.Right); }
                                    else { queuedDirection = Direction.Down; }
                                }
                            }
                        }   
                        //move down
                    }
                }
                

            }
        }
    }
}
