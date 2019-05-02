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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class ReadyClass : OperationBase
        {


            public ReadyClass(LegBase Rleg, LegBase LLeg, ArmBase Rarm, ArmBase LArm) : base(Rleg, LLeg, Rarm, LArm)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
                this.RArm = Rarm;
                this.LArm = LArm;

                TargetListMiddleward = new string[] { "5", "10", "5" };
            }


            /// <summary>
            /// メイン
            /// </summary>
            public override void Drive()
            {
                RLeg.DriveParts();
                LLeg.DriveParts();
                //RArm.DriveParts();
                //LArm.DriveParts();

                if (RLeg.myMotorKnee.finishFlg && RLeg.myMotorAnkle.finishFlg && RLeg.myMotorThigh.finishFlg)
                {
                    RLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                    RLeg.myMotorThigh.MyMotor.CustomData = "1," + TargetListMiddleward[0] + "," + "10";
                    RLeg.myMotorKnee.MyMotor.CustomData = "1," + TargetListMiddleward[1] + "," + "10";
                    RLeg.myMotorAnkle.MyMotor.CustomData = "1," + TargetListMiddleward[2] + "," + "10";
                    LLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                    LLeg.myMotorThigh.MyMotor.CustomData = "0," + TargetListMiddleward[0] + "," + "10";
                    LLeg.myMotorKnee.MyMotor.CustomData = "0," + TargetListMiddleward[1] + "," + "30";
                    LLeg.myMotorAnkle.MyMotor.CustomData = "0," + TargetListMiddleward[2] + "," + "10";


                    RArm.myMotor1.MyMotor.CustomData = "0,0,10";
                    RArm.myMotor2.MyMotor.CustomData = "0,0,10";
                    LArm.myMotor1.MyMotor.CustomData = "0,0,10";
                    LArm.myMotor2.MyMotor.CustomData = "0,0,10";

                }




            }
        }
    }
}
