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
        public class ArmBase
        {
            private DateTime nowTime;
            private DateTime befTime;

            public Utilty.PartsMoveEnum LegStatus;

            public motorBase myMotor1;


            public motorBase myMotor2;


            public motorBase myMotor3;

            public motorBase myMotor4;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="m1">第１関節</param>
            /// <param name="r1">第１関節反転フラグ</param>
            /// <param name="m2">第２関節</param>
            /// <param name="r2">第２関節反転フラグ</param>
            /// <param name="m3">第３関節</param>
            /// <param name="r3">第３関節反転フラグ</param>
            public ArmBase(IMyMotorStator m1, bool r1, IMyMotorStator m2, bool r2, IMyMotorStator m3, bool r3, IMyMotorStator m4, bool r4)
            {
                LegStatus = Utilty.PartsMoveEnum.off;
                myMotor1 = new motorBase(m1, r1, 0, 0);
                myMotor2 = new motorBase(m2, r2, 0, 0);
                myMotor3 = new motorBase(m3, r3, 0, 0);
                myMotor4 = new motorBase(m4, r4, 0, 0);
            }


            /// <summary>
            /// Movinegmoter
            /// </summary>
            /// <returns>Finisih?</returns>
            public bool DriveParts()
            {


                bool returnvalue = false;

                nowTime = DateTime.UtcNow;

                myMotor1.Main();
                myMotor2.Main();
                myMotor3.Main();
                myMotor4.Main();

                befTime = nowTime;
                if (myMotor1.finishFlg && myMotor2.finishFlg && myMotor3.finishFlg && myMotor4.finishFlg)
                {
                    returnvalue = true;
                }

                return returnvalue;
            }

            //public void setAngle(string[] list)
            //{
            //    myMotor1.MyMotor.CustomData = double.Parse(list[0]);
            //    myMotor2.TargetAngle = double.Parse(list[1]);
            //}



        }
    }
}
