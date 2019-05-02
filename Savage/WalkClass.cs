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
        public class WalkClass : OperationBase
        {
            public string[] TargetListHalsBackward = new string[] { };

            public WalkClass(LegBase Rleg,LegBase LLeg, ArmBase Rarm, ArmBase LArm) : base(Rleg,LLeg,Rarm,LArm)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
                this.RArm = Rarm;

                this.LArm = LArm;
                TargetListForward = new string[] { "40", "30", "-10" };
                TargetListBackward = new string[] { "-30", "10", "20" };
                TargetListHalsBackward = new string[] { "-30", "10", "20" };
                TargetListMiddleward = new string[] { "30", "60", "40" };

                RArm.myMotor1.MyMotor.CustomData = "1,90,20";
                RArm.myMotor2.MyMotor.CustomData = "1,0,20";

            }

            /// <summary>
            /// メイン
            /// </summary>
            public override void Drive()
            {
                RLeg.DriveParts();
                //RArm.DriveParts();

                LLeg.DriveParts();
                //LArm.DriveParts();


                if (ctrlStatus == Utilty.StatusEnum.Forword)
                {

                    if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward)
                    {


                        if (RLeg.myMotorKnee.finishFlg && RLeg.myMotorAnkle.finishFlg && RLeg.myMotorThigh.finishFlg)
                        {
                            RLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                            RLeg.myMotorThigh.MyMotor.CustomData = "1," + TargetListBackward[0] + "," + "10";
                            RLeg.myMotorKnee.MyMotor.CustomData = "1," + TargetListBackward[1] + "," + "10";
                            RLeg.myMotorAnkle.MyMotor.CustomData = "1," + TargetListBackward[2] + "," + "5";
                            LLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                            LLeg.myMotorThigh.MyMotor.CustomData = "0," + TargetListMiddleward[0] + "," + "10";
                            LLeg.myMotorKnee.MyMotor.CustomData = "0," + TargetListMiddleward[1] + "," + "30";
                            LLeg.myMotorAnkle.MyMotor.CustomData = "0," + TargetListMiddleward[2] + "," + "10";
                            return;
                        }
                    }
                    else if (LLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward)
                    {
                        

                        if (LLeg.myMotorKnee.finishFlg && LLeg.myMotorAnkle.finishFlg && LLeg.myMotorThigh.finishFlg)
                        {

                            LLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                            LLeg.myMotorThigh.MyMotor.CustomData = "0," + TargetListBackward[0] + "," + "10";
                            LLeg.myMotorKnee.MyMotor.CustomData = "0," + TargetListBackward[1] + "," + "10";
                            LLeg.myMotorAnkle.MyMotor.CustomData = "0," + TargetListBackward[2] + "," + "5";
                            RLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                            RLeg.myMotorThigh.MyMotor.CustomData = "1," + TargetListMiddleward[0] + "," + "10";
                            RLeg.myMotorKnee.MyMotor.CustomData = "1," + TargetListMiddleward[1] + "," + "30";
                            RLeg.myMotorAnkle.MyMotor.CustomData = "1," + TargetListMiddleward[2] + "," + "10";
                            return;
                        }
                    }
                }


                if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward)
                {
                    if (RLeg.myMotorKnee.finishFlg && RLeg.myMotorAnkle.finishFlg && RLeg.myMotorThigh.finishFlg && ctrlStatus == Utilty.StatusEnum.Halt)
                    {
                        LLeg.myMotorThigh.MyMotor.CustomData = "1," + TargetListHalsBackward[0] + "," + "10";
                        LLeg.myMotorKnee.MyMotor.CustomData = "1," + TargetListHalsBackward[1] + "," + "10";
                        LLeg.myMotorAnkle.MyMotor.CustomData = "1," + TargetListHalsBackward[2] + "," + "5";
                    }
                }
                else if (LLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward)
                {
                    if (LLeg.myMotorKnee.finishFlg && LLeg.myMotorAnkle.finishFlg && LLeg.myMotorThigh.finishFlg && ctrlStatus == Utilty.StatusEnum.Halt)
                    {

                        RLeg.myMotorThigh.MyMotor.CustomData = "0," + TargetListHalsBackward[0] + "," + "10";
                        RLeg.myMotorKnee.MyMotor.CustomData = "0," + TargetListHalsBackward[1] + "," + "10";
                        RLeg.myMotorAnkle.MyMotor.CustomData = "0," + TargetListHalsBackward[2] + "," + "5";

                    }

                }

                if (LLeg.LegStatus == Utilty.PartsMoveEnum.Move_middle)
                {
                    if (LLeg.myMotorThigh.finishFlg)
                    {
                        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                        LLeg.myMotorThigh.MyMotor.CustomData = "0," + TargetListForward[0] + "," + "10";
                        LLeg.myMotorKnee.MyMotor.CustomData = "0," + TargetListForward[1] + "," + "20";
                        LLeg.myMotorAnkle.MyMotor.CustomData = "0," + TargetListForward[2] + "," + "20";
                    }
                }

                if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_middle)
                {
                    if (RLeg.myMotorThigh.finishFlg)
                    {
                        RLeg.myMotorThigh.MyMotor.CustomData = "1," + TargetListForward[0] + "," + "10";
                        RLeg.myMotorKnee.MyMotor.CustomData = "1," + TargetListForward[1] + "," + "20";
                        RLeg.myMotorAnkle.MyMotor.CustomData = "1," + TargetListForward[2] + "," + "20";
                        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                    }
                }




            }




        }
    }
}
