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
        public class ShutDown : OperationBase
        {

            DateTime lasttime = new DateTime();


            public ShutDown(LegBase Rleg, LegBase LLeg, ArmBase Rarm, ArmBase LArm) : base(Rleg, LLeg, Rarm, LArm)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
                this.RArm = Rarm;
                this.LArm = LArm;
                //TargetListMiddleward = new string[] { "5", "10", "5" };
                //TargetListBackward = new string[] { "-0", "0", "-0" };
                //TargetListMiddleward = new string[] { "0", "0", "0" };

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



                if ((DateTime.UtcNow - lasttime) > TimeSpan.FromSeconds(3))
                {
                    RLeg.LegStatus = Utilty.PartsMoveEnum.off;
                    RLeg.myMotorThigh.MyMotor.CustomData = "1,90,10";
                    RLeg.myMotorKnee.MyMotor.CustomData = "1,100,10";
                    RLeg.myMotorAnkle.MyMotor.CustomData = "1,30,10";
                    LLeg.LegStatus = Utilty.PartsMoveEnum.off;
                    LLeg.myMotorThigh.MyMotor.CustomData = "0,90,10";
                    LLeg.myMotorKnee.MyMotor.CustomData = "0,100,10";
                    LLeg.myMotorAnkle.MyMotor.CustomData = "0,30,10";
                    return;
                }

                if (RLeg.myMotorThigh.finishFlg && LLeg.myMotorThigh.finishFlg)
                {
                    RLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                    RLeg.myMotorThigh.MyMotor.CustomData = "1,5,10";
                    RLeg.myMotorKnee.MyMotor.CustomData = "1,10,10";
                    RLeg.myMotorAnkle.MyMotor.CustomData = "1,5,10";
                    LLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                    LLeg.myMotorThigh.MyMotor.CustomData = "0,5,10";
                    LLeg.myMotorKnee.MyMotor.CustomData = "0,10,10";
                    LLeg.myMotorAnkle.MyMotor.CustomData = "0,5,10";
                    lasttime = DateTime.UtcNow;
                    return;
                }



            }

        }
    }
}
