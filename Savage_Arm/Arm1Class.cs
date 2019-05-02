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
        public class Arm1Class : OperationBase
        {
            int status = 0;

            public Arm1Class(ArmBase Rarm, ArmBase LArm) : base(Rarm,LArm)
            {
                this.RArm = Rarm;

                this.LArm = LArm;

                RArm.myMotor1.MyMotor.CustomData = "1,220,10";
                RArm.myMotor2.MyMotor.CustomData = "1,0,10";
                RArm.myMotor3.MyMotor.CustomData = "1,140,10";
                RArm.myMotor4.MyMotor.CustomData = "1,90,10";
                status = 0;
            }

            /// <summary>
            /// メイン
            /// </summary>
            public override void Drive()
            {
                bool finish = RArm.DriveParts();
                LArm.DriveParts();

                if (status == 0)
                {
                    if (finish)
                    {
                        status = 1;
                        RArm.myMotor1.MyMotor.CustomData = "1,90,10";
                        RArm.myMotor2.MyMotor.CustomData = "1,0,10";
                        RArm.myMotor3.MyMotor.CustomData = "1,0,10";
                        RArm.myMotor4.MyMotor.CustomData = "1,0,10";
                    }
                }
                else if (status == 1)
                {

                }

            }




        }
    }
}
