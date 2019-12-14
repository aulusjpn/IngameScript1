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

        /// <summary>
        /// 脚部クラス
        /// </summary>
        public class LegModel
        {
            private DateTime nowTime;
            private DateTime befTime;

            public MoterModel myMotorKnee;


            public MoterModel myMotorThigh;


            public MoterModel myMotorAnkle;


            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="m1">第１関節</param>
            /// <param name="r1">第１関節反転フラグ</param>
            /// <param name="m2">第２関節</param>
            /// <param name="r2">第２関節反転フラグ</param>
            /// <param name="m3">第３関節</param>
            /// <param name="r3">第３関節反転フラグ</param>
            public LegModel(IMyMotorStator m1,bool r1,IMyMotorStator m2, bool r2, IMyMotorStator m3,bool r3)
            {               
                myMotorThigh = new MoterModel(m1, r1, 0, 0);
                myMotorKnee = new MoterModel(m2, r2, 0, 0);
                myMotorAnkle = new MoterModel(m3, r3, 0, 0);
            }


            /// <summary>
            /// Movinegmoter
            /// </summary>
            /// <returns>Finisih?</returns>
            public bool Drive(IMyCockpit cockpit)
            {

                bool returnvalue = false;

                nowTime = DateTime.UtcNow;

                //myMotorKnee.Update();
                //myMotorThigh.Update();
                //myMotorAnkle.Update();

                befTime = nowTime;

                return returnvalue;
            }

            public void setAngle(double[] list)
            {
                myMotorKnee.TargetAngleDegree = list[0];
                myMotorThigh.TargetAngleDegree = list[1];
                myMotorAnkle.TargetAngleDegree = list[2];
            }
            
        }
    }
}
