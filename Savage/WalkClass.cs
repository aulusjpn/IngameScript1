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
            public override bool ForwardMovingReg_R()
            {
                if (RLeg.LegStatus == PartsMoveEnum.Move_forward)
                {
                    RLeg.MorterMoveToAngle(RLeg.myMotorThigh, -30, 1f);

                    if (MathHelperD.ToDegrees(RLeg.myMotorThigh.Angle) > -20)
                    {
                        RLeg.MorterMoveToAngle(RLeg.myMotorKnee, 80, 2f);
                    }
                    else
                    {
                        RLeg.MorterMoveToAngle(RLeg.myMotorKnee, 10, 2f);
                    }

                    if (RLeg.myMotorThigh.Angle == -30 && RLeg.myMotorKnee.Angle == 10)
                    {
                        RLeg.LegStatus = PartsMoveEnum.Ready_For;

                    }

                }
                else if (RLeg.LegStatus == PartsMoveEnum.Move_bakward)
                {
                    RLeg.MorterMoveToAngle(RLeg.myMotorThigh, 30, 0.5f);
                    if (MathHelperD.ToDegrees(RLeg.myMotorThigh.Angle) > 0)
                    {
                        RLeg.MorterMoveToAngle(RLeg.myMotorKnee, 80, 1f);
                    }
                    else
                    {
                        RLeg.MorterMoveToAngle(RLeg.myMotorKnee, 10, 1f);
                    }

                    if (RLeg.myMotorThigh.Angle == 30 && RLeg.myMotorKnee.Angle == 10)
                    {
                        RLeg.LegStatus = PartsMoveEnum.Ready_Back;

                    }
                }
                else if (RLeg.LegStatus == PartsMoveEnum.Ready_For)
                {

                }
                else if (RLeg.LegStatus == PartsMoveEnum.Ready_Back)
                {

                }
                else if (RLeg.LegStatus == PartsMoveEnum.off)
                {

                }

                return false;
            }


            public override bool ForwardMovingReg_L()
            {
                if (LLeg.LegStatus == PartsMoveEnum.Move_forward)
                {
                    LLeg.MorterMoveToAngle(LLeg.myMotorThigh, 30, 1f);

                    if (MathHelperD.ToDegrees(LLeg.myMotorThigh.Angle) < 20)
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 80, 2f);
                    }
                    else
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 10, 2f);
                    }

                }
                else if (LLeg.LegStatus == PartsMoveEnum.Move_bakward)
                {
                    LLeg.MorterMoveToAngle(LLeg.myMotorKnee, -30, 0.5f);
                    if (MathHelperD.ToDegrees(LLeg.myMotorKnee.Angle) < 0)
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 80, 1f);
                    }
                    else
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 10, 1f);
                    }
                }
                else if (LLeg.LegStatus == PartsMoveEnum.Ready_Back)
                {

                }
                else if (LLeg.LegStatus == PartsMoveEnum.Stand_Up)
                {

                }
                else if (LLeg.LegStatus == PartsMoveEnum.Ready_For)
                {

                }
                else if (LLeg.LegStatus == PartsMoveEnum.off)
                {

                }

                return false;
            }

            public override bool BackMovingLeg_R()
            {

                return false;
            }

            public override bool BackMovingLeg_L()
            {
                if (LLeg.LegStatus == PartsMoveEnum.Move_forward)
                {
                    LLeg.MorterMoveToAngle(LLeg.myMotorThigh, 30, 1f);

                    if (MathHelperD.ToDegrees(LLeg.myMotorThigh.Angle) < 20)
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 80, 2f);
                    }
                    else
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 10, 2f);
                    }

                }
                else if (LLeg.LegStatus == PartsMoveEnum.Move_bakward)
                {
                    LLeg.MorterMoveToAngle(LLeg.myMotorKnee, -30, 0.5f);
                    if (MathHelperD.ToDegrees(LLeg.myMotorKnee.Angle) < 0)
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 80, 1f);
                    }
                    else
                    {
                        LLeg.MorterMoveToAngle(LLeg.myMotorKnee, 10, 1f);
                    }
                }
                return false;
            }

            public override bool BrakingLeg_R()
            {
                return false;
            }

            public override bool BrakingLeg_L()
            {

                return false;
            }

        }
    }
}
