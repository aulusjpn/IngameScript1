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
        public static class Utilty
        {
            public struct MyStruct
            {

                public IMyMotorStator myMotorKnee;
                public bool myMotorKnee_reverse;

                public IMyMotorStator myMotorThigh;
                public bool myMotorThigh_reverse;

                public IMyMotorStator myMotorAnkle;
                public bool myMotorAnkle_reverse;
            }

            public enum StatusEnum
            {
                Forword = 0,
                Back = 1,
                Halt = 2,
                off = 9,
            }

            public enum PartsMoveEnum
            {
                Ready_For = 0,
                Ready_Back = 1,
                Move_forward = 2,
                Move_bakward = 3,
                Stand_Up = 4,
                off = 9,
            }
        }
    }
}
