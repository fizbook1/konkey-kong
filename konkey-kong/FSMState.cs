using System;
using System.Collections.Generic;
using System.Text;

namespace pakeman
{
    public abstract class FSMState
    {
        int type;
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
    public class StateRoam : FSMState
    {
        public override void Enter()
        {
            throw new NotImplementedException();
        }
        public override void Update()
        {
            throw new NotImplementedException();
        }
        public override void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
