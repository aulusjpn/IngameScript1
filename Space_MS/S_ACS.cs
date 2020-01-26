using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class S_ACS : OperationServiceBase
        {


            public S_ACS(LegModel rLeg, LegModel lLen, ArmModel rArm, ArmModel lArm) : base(rLeg, lLen, rArm, lArm)
            {
                this.RPart = rLeg;
                this.LPart = lLen;
                this.RArm = rArm;
                this.LArm = lArm;
            }


            public override void armTarget(IMyCockpit Rota)
            {
                TargettingMouse(Rota);
            }

            public override void DriveLeg(IMyCockpit cockpit)
            {
                // RPart.Drive(cockpit);
            }

            public void TargettingMouse(IMyCockpit cockpit)
            {

                //if (cockpit.RotationIndicator.X > 0)
                //{
                //    ((ArmModel)RPart).MoveUp();
                //}
                //else if (cockpit.RotationIndicator.X < 0)
                //{
                //    ((ArmModel)RPart).MoveDown();
                //}
                //else
                //{

                //}

                //if (cockpit.RotationIndicator.Y > 0)
                //{

                //}
                //else if (cockpit.RotationIndicator.Y < 0)
                //{

                //}
                //else
                //{

                //}

            }
        }
    }
}

