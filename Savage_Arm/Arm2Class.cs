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

namespace IngameScript
{
    partial class Program
    {
        public class Arm2Class : OperationBase
        {
            int status = 0;

            public Arm2Class(ArmBase Rarm, ArmBase LArm) : base(Rarm,LArm)
            {
                this.RArm = Rarm;

                this.LArm = LArm;

                RArm.myMotor1.MyMotor.CustomData = "1,190,20";
                RArm.myMotor2.MyMotor.CustomData = "1,0,20";
                RArm.myMotor3.MyMotor.CustomData = "1,80,20";

            }

            /// <summary>
            /// メイン
            /// </summary>
            public override void Drive()
            {
                RArm.DriveParts();
                LArm.DriveParts();
            }




        }
    }
}
