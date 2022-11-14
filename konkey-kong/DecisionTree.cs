using System;
using System.Collections.Generic;
using System.Text;

namespace pakeman
{
    class DecisionTree
    {
        
    }
    public class WorldState
    {
        
    }
    public abstract class Decision
    {
        protected Decision[] children;
        public abstract void Init();
        public abstract void Evaluate(WorldState world);
    }
    public class IsPoweredUp : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }
    public class AmDead : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }
    public class ChasePlayer : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }
    public class RunAway : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }
    public class AlreadyChased : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }
    public class FoodRemaining : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }
    public class Roam : Decision
    {
        public override void Init()
        {
            children = new Decision[2];
        }
        public override void Evaluate(WorldState world)
        {

        }
    }

}
