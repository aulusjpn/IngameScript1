using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using static IngameScript.Program.Utilty;

namespace IngameScript
{
    partial class Program
    {
        public abstract class OperationBase
        {


            protected IMyCockpit cockpit;

            protected LegBase RLeg;
            protected LegBase LLeg;
            public StatusEnum ctrlStatus;
            

            public void Drive()
            {
                               
            }




        }
    }
}
