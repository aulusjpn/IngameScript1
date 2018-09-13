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

            public WalkClass(LegBase Rleg,LegBase LLeg) : base(Rleg,LLeg)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
            }

            /// <summary>
            /// メイン
            /// </summary>
            public override void Drive()
            {
                if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward && LLeg.LegStatus == Utilty.PartsMoveEnum.Move_Backward)
                {
                    if (RLeg.DriveParts() && LLeg.DriveParts() && ctrlStatus == Utilty.StatusEnum.Forword)
                    {
                        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                    }                   
                }
                else if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_Backward && LLeg.LegStatus == Utilty.PartsMoveEnum.Move_forward)
                {
                    if (RLeg.DriveParts() && LLeg.DriveParts() && ctrlStatus == Utilty.StatusEnum.Forword)
                    {
                        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_middle;
                        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                    }
                }
                else if (RLeg.LegStatus == Utilty.PartsMoveEnum.Move_middle)
                {
                    LLeg.DriveParts();
                    if (RLeg.DriveParts())
                    {
                        RLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                    }
                }
                else if (LLeg.LegStatus == Utilty.PartsMoveEnum.Move_middle)
                {
                    RLeg.DriveParts();
                    if (LLeg.DriveParts())
                    {
                        LLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                    }
                }

            }

        }
    }
}
