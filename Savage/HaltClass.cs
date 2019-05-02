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
        public class HaltClass : OperationBase
        {


            public HaltClass(LegBase Rleg, LegBase LLeg, ArmBase Rarm, ArmBase LArm) : base(Rleg, LLeg, Rarm, LArm)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
                this.RArm = Rarm;
                this.LArm = LArm;
                TargetListForward = new string[] { "10", "10", "5" };
                TargetListBackward = new string[] { "-0", "0", "-0" };
                TargetListMiddleward = new string[] { "0", "0", "0" };
                RArm.myMotor1.MyMotor.CustomData = "1,0,10";
                RArm.myMotor2.MyMotor.CustomData = "1,0,10";
            }

            /// <summary>
            /// メイン
            /// </summary>
            public override void Drive()
            {
                //RLeg.setAngle(TargetListForward);
                RLeg.DriveParts();
                //RArm.DriveParts();
                //LLeg.setAngle(TargetListForward);
                LLeg.DriveParts();
                //LArm.DriveParts();

                //if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward && LLeg.LegStatus == Utilty.PartsMoveEnum.Move_Backward)
                //{
                //    if (RLeg.DriveParts() && LLeg.DriveParts() && ctrlStatus == Utilty.StatusEnum.Forword)
                //    {
                //        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                //        RLeg.setAngle(TargetListBackward);
                //        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                //        LLeg.setAngle(TargetListMiddleward);
                //    }
                //}
                //else if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_Backward && LLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward)
                //{
                //    if (RLeg.DriveParts() && LLeg.DriveParts() && ctrlStatus == Utilty.StatusEnum.Forword)
                //    {
                //        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                //        RLeg.setAngle(TargetListMiddleward);
                //        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                //        LLeg.setAngle(TargetListBackward);
                //    }
                //}
                //else if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_middle)
                //{
                //    LLeg.DriveParts();
                //    if (RLeg.DriveParts())
                //    {
                //        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                //        RLeg.setAngle(TargetListForward);
                //    }
                //}
                //else if (LLeg.LegStatus == Utilty.PartsMoveEnum.Move_middle)
                //{
                //    RLeg.DriveParts();
                //    if (LLeg.DriveParts())
                //    {
                //        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                //        LLeg.setAngle(TargetListForward);
                //    }
                //}

            }
        }
    }
}
