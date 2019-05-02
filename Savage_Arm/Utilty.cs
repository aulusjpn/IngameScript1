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
        public class Utilty
        {

            /// <summary>
            /// 動作命令
            /// </summary>
            public enum StatusEnum
            {
                Forword = 0,
                Back = 1,
                Halt = 2,
                off = 9,
            }


            /// <summary>
            /// 脚部動作ステータス
            /// </summary>
            public enum PartsMoveEnum
            {
                Ready_Forward = 0,
                Ready_Back = 1,
                Move_forward = 2,
                Move_middle = 3,
                Move_Backward = 4,
                off = 9,
            }
        }
    }
}
