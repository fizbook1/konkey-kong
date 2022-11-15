using System;
using System.Collections.Generic;
using System.Text;

namespace pakeman
{
    class DecisionTree
    {
        Decision root;
        public DecisionTree(WorldState world, Enemy self)
        {
            world.self = self;
            root = new IsPoweredUp();
            root.Evaluate(world);
        } 
    }
    public class WorldState
    {
        public bool isPoweredUp;
        public bool halfFoodLeft;
        public bool isGhostChasing;
        public bool amDead;
        public Enemy self;
        public WorldState Copy(Enemy enemy)
        {
            WorldState copy = new WorldState();
            if(EnemyManager.chasingEnemy == enemy)
            {
                copy.isGhostChasing = false;
            }
            else
            {
                copy.isGhostChasing = true;
            }
            copy.isPoweredUp = isPoweredUp;
            copy.halfFoodLeft = halfFoodLeft;
            if(enemy.state == EntityState.Death)
            {
                copy.amDead = true;
            }
            copy.self = enemy;
            return copy;

        }
    }
    public abstract class Decision
    {
        protected Decision[] children;
        public abstract void Evaluate(WorldState world);
    }
    public class IsPoweredUp : Decision
    {
        public IsPoweredUp()
        {
            children = new Decision[2];
            children[0] = new AlreadyChased();
            children[1] = new AmDead();
        }
        public override void Evaluate(WorldState world)
        {
            if(world.isPoweredUp)
            {
                children[1].Evaluate(world);
            }
            else
            {
                children[0].Evaluate(world);
            }
        }
    }
    public class AmDead : Decision
    {
        public AmDead()
        {
            children = new Decision[2];
            children[0] = new ChasePlayer();
            children[1] = new RunAway();
        }
        public override void Evaluate(WorldState world)
        {
            if(world.amDead)
            {
                children[0].Evaluate(world);
            }
            else
            {
                children[1].Evaluate(world);
            }
        }
    }
    public class ChasePlayer : Decision
    {
        public ChasePlayer()
        {
            
        }
        public override void Evaluate(WorldState world)
        {
            world.self.behavior = EnemyBehavior.Chase;
        }
    }
    public class RunAway : Decision
    {
        public RunAway()
        {
            
        }
        public override void Evaluate(WorldState world)
        {
            world.self.behavior = EnemyBehavior.Flee;
        }
    }
    public class AlreadyChased : Decision
    {
        public AlreadyChased()
        {
            children = new Decision[2];
            children[0] = new ChasePlayer();
            children[1] = new FoodRemaining();
        }
        public override void Evaluate(WorldState world)
        {
            if(world.isGhostChasing)
            {
                children[1].Evaluate(world);
            }
            else
            {
                children[0].Evaluate(world);
            }
        }
    }
    public class FoodRemaining : Decision
    {
        public FoodRemaining()
        {
            children = new Decision[2];
            children[0] = new ChasePlayer();
            children[1] = new Roam();
        }
        public override void Evaluate(WorldState world)
        {
            if(world.halfFoodLeft)
            {
                children[0].Evaluate(world);
            }
            else
            {
                children[1].Evaluate(world);
            }
        }
    }
    public class Roam : Decision
    {
        public Roam()
        {
            
        }
        public override void Evaluate(WorldState world)
        {
            world.self.behavior = EnemyBehavior.Roam;
        }
    }

}
