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
using static IngameScript.Program.Utilty;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// 動作管理クラス
        /// これを基底とし、歩行・速歩などを継承して作成する。
        /// </summary>
        public abstract class OperationBase
        {

            //コントロール取得用
            protected IMyCockpit cockpit;


            public StatusEnum ctrlStatus;


            /// <summary>
            /// 右足
            /// </summary>
            internal LegBase RLeg { get; set; }


            /// <summary>
            /// 左脚
            /// </summary>
            internal LegBase LLeg { get; set; }


            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="RLeg">右足</param>
            /// <param name="LLeg">左脚</param>
            public OperationBase(LegBase RLeg, LegBase LLeg)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
                ctrlStatus = StatusEnum.off;
            }

            public abstract void Drive();



        }
    }
}
