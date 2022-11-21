using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pakeman
{
    public class Context
    {
        FSMState state;
        public Enemy enemy;
        public static bool playerPowerup = false;
        public Context(Enemy self, FSMState defaultState)
        {
            enemy = self;
            state = defaultState;
            state.Enter(this);
        }
        public void ChangeState(FSMState _state)
        {
            state.Exit();
            state = _state;
            state.Enter(this);
        }

        public void Update()
        {
            state.Update();
        }
    }
    public abstract class FSMState
    {
        protected Context context;
        protected int type;
        protected Enemy self;
        public abstract void Enter(Context context);
        public abstract void Update();
        public abstract void Exit();
    }
    public class StateRoam : FSMState
    {
        Random rnd;
        int timeRoamed;
        int untilNewDir;
        public override void Enter(Context context)
        {
            this.context = context;
            type = 0;
            rnd = new Random();
            timeRoamed = 0;
            untilNewDir = 0;
            self = context.enemy;
            self.behavior = EnemyBehavior.Roam;

        }
        public override void Update()
        {
            if (!self.isMoving)
                self.EntityMoveStart((Direction)rnd.Next(0, 4));
            if (untilNewDir > rnd.Next(0, 600))
            {
                self.queuedDirection = (Direction)rnd.Next(0, 4);
                if (!self.isMoving)
                {
                    self.EntityMoveStart(self.queuedDirection);
                }
                untilNewDir = 0;
                
            }
            untilNewDir++;
            timeRoamed++;
            if (timeRoamed > rnd.Next(0, 10000))
            {
                context.ChangeState(new StateChase());
            }
            if (Context.playerPowerup)
            {
                context.ChangeState(new StateFlee());
            }
        }
        public override void Exit()
        {
            
        }
    }
    public class StateFlee : FSMState
    {
        public override void Enter(Context context)
        {
            this.context = context;
            type = 1;
            self = context.enemy;
            self.behavior = EnemyBehavior.Flee;
        }
        public override void Update()
        {
            int differenceX;
            int differenceY;

            differenceX = Player.self.tilePosX - self.tilePosX;
            differenceY = Player.self.tilePosY - self.tilePosY;

            if (Math.Abs(differenceX) > Math.Abs(differenceY))
            {
                if (differenceX < 0)
                {
                    if (self.tRight.type == TileType.Standard)
                    {
                        if (!self.isMoving) { self.EntityMoveStart(Direction.Right); }
                        else { self.queuedDirection = Direction.Right; }

                    }
                    else
                    {
                        if (differenceY < 0 && self.tDown.type == TileType.Standard)
                        {
                            if (!self.isMoving) { self.EntityMoveStart(Direction.Down); }
                            else { self.queuedDirection = Direction.Down; }
                        }
                        if (differenceY > 0 && self.tUp.type == TileType.Standard)
                        {
                            if (!self.isMoving) { self.EntityMoveStart(Direction.Up); }
                            else { self.queuedDirection = Direction.Up; }
                        }
                        if (differenceY == 0)
                        {
                            if (self.tLeftDown.type == TileType.Standard)
                            {
                                if (!self.isMoving) { self.EntityMoveStart(Direction.Down); }
                                else { self.queuedDirection = Direction.Right; }
                            }
                            if (self.tLeftUp.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Up);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Right;
                                }
                            }
                        }
                    }
                }
                if (differenceX > 0)
                {
                    if (self.tLeft.type == TileType.Standard)
                    {
                        if (!self.isMoving)
                        {
                            self.EntityMoveStart(Direction.Left);
                        }
                        else
                        {
                            self.queuedDirection = Direction.Left;
                        }
                    }
                    else
                    {
                        if (differenceY < 0 && self.tDown.type == TileType.Standard)
                        {
                            if (!self.isMoving)
                            {
                                self.EntityMoveStart(Direction.Down);
                            }
                            else
                            {
                                self.queuedDirection = Direction.Down;
                            }
                        }
                        if (differenceY > 0 && self.tUp.type == TileType.Standard)
                        {
                            if (!self.isMoving)
                            {
                                self.EntityMoveStart(Direction.Up);
                            }
                            else
                            {
                                self.queuedDirection = Direction.Up;
                            }
                        }
                        if (differenceY == 0)
                        {
                            if (self.tRightDown.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Down);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Left;
                                }
                            }
                            if (self.tRightUp.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Up);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Left;
                                }
                            }
                        }
                    }
                }
            }//
            if (Math.Abs(differenceY) > Math.Abs(differenceX))
            {
                if (differenceY < 0)
                {
                    if (self.tDown.type == TileType.Standard)
                    {
                        if (!self.isMoving)
                        {
                            self.EntityMoveStart(Direction.Down);
                        }
                        else
                        {
                            self.queuedDirection = Direction.Down;
                        }
                    }
                    else
                    {
                        if (differenceX < 0)
                        {
                            if (!self.isMoving)
                            {
                                self.EntityMoveStart(Direction.Right);
                            }
                            else
                            {
                                self.queuedDirection = Direction.Right;
                            }
                        }
                        if (differenceX > 0)
                        {
                            if (!self.isMoving)
                            {
                                self.EntityMoveStart(Direction.Left);
                            }
                            else
                            {
                                self.queuedDirection = Direction.Left;
                            }
                        }
                        if (differenceX == 0)
                        {
                            if (self.tRightUp.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Right);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Up;
                                }
                            }
                            if (self.tLeftUp.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Left);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Up;
                                }
                            }
                        }
                    }
                    //move up
                }
                if (differenceY > 0)
                {
                    if (self.tUp.type == TileType.Standard)
                    {
                        if (!self.isMoving)
                        {
                            self.EntityMoveStart(Direction.Up);
                        }
                        else
                        {
                            self.queuedDirection = Direction.Up;
                        }
                    }
                    else
                    {
                        if (differenceX < 0)
                        {
                            if (!self.isMoving)
                            {
                                self.EntityMoveStart(Direction.Right);
                            }
                            else
                            {
                                self.queuedDirection = Direction.Right;
                            }
                        }
                        if (differenceX > 0)
                        {
                            if (!self.isMoving)
                            {
                                self.EntityMoveStart(Direction.Left);
                            }
                            else
                            {
                                self.queuedDirection = Direction.Left;
                            }
                        }
                        if (differenceX == 0)
                        {
                            if (self.tRightDown.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Right);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Down;
                                }
                            }
                            if (self.tLeftDown.type == TileType.Standard)
                            {
                                if (!self.isMoving)
                                {
                                    self.EntityMoveStart(Direction.Left);
                                }
                                else
                                {
                                    self.queuedDirection = Direction.Down;
                                }
                            }
                        }
                    }
                    //move down
                }




            }
            if (!Context.playerPowerup)
            {
                context.ChangeState(new StateRoam());
            }
        }
        public override void Exit()
        {
            
        }
    }
    public class StateChase : FSMState
    {
        Random rnd;
        int timeChased;
        public override void Enter(Context context)
        {
            this.context = context;
            type = 2;
            timeChased = 0;
            rnd = new Random();
            self = context.enemy;
            self.behavior = EnemyBehavior.Chase;

        }
        public override void Update()
        {
            if(timeChased > rnd.Next(0, 10000))
            {
                context.ChangeState(new StateRoam());
            }
            timeChased++;
            if (!self.isMoving)
            {
                Point gohere = new Point();
                //if(!isMoving)
                //{
                Graph graph = new Graph(TileManager.currentMap);
                gohere = graph.NextMove(new Point(self.tilePosX, self.tilePosY), Player.location);
                if (gohere.X < self.tilePosX)
                {
                    self.EntityMoveStart((Direction)0);
                }
                if (gohere.X > self.tilePosX)
                {
                    self.EntityMoveStart((Direction)1);
                }
                if (gohere.Y < self.tilePosY)
                {
                    self.EntityMoveStart((Direction)2);
                }
                if (gohere.Y > self.tilePosY)
                {
                    self.EntityMoveStart((Direction)3);
                }
            }
            if (self.isMoving)
            {
                Point gohere = new Point();
                //if(!isMoving)
                //{
                Graph graph = new Graph(TileManager.currentMap);
                switch (self.dir)
                {
                    case Direction.Left:
                        gohere = graph.NextMove(new Point(self.tilePosX - 1, self.tilePosY), Player.location);
                        break;
                    case Direction.Right:
                        gohere = graph.NextMove(new Point(self.tilePosX + 1, self.tilePosY), Player.location);
                        break;
                    case Direction.Up:
                        gohere = graph.NextMove(new Point(self.tilePosX, self.tilePosY - 1), Player.location);
                        break;
                    case Direction.Down:
                        gohere = graph.NextMove(new Point(self.tilePosX, self.tilePosY + 1), Player.location);
                        break;
                }

                if (gohere.X < self.tilePosX)
                {

                    self.queuedDirection = (Direction)0;
                }
                if (gohere.X > self.tilePosX)
                {
                    self.queuedDirection = (Direction)1;
                }
                if (gohere.Y < self.tilePosY)
                {
                    self.queuedDirection = (Direction)2;
                }
                if (gohere.Y > self.tilePosY)
                {
                    self.queuedDirection = (Direction)3;
                }
            }

            if(Context.playerPowerup)
            {
                context.ChangeState(new StateFlee());
            }
        }
        public override void Exit()
        {
            
        }
    }
}
