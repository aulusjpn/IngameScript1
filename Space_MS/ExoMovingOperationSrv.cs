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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class ExoMovingOperationSrv : OperationServiceBase
        {
            public ExoMovingOperationSrv(LegModel rLeg, LegModel lLeg, ArmModel rArm, ArmModel lArm) : base(rLeg, lLeg, rArm, lArm)
            {
            }

            public override void armTarget(IMyCockpit Rota)
            {
                throw new NotImplementedException();
            }

            public override void Drive(IMyCockpit cockpit)
            {
                throw new NotImplementedException();
            }
        }
    }
}
