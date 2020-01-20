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
        public class Exo_ArmModel : ArmModel
        {
            private DateTime nowTime;
            private DateTime befTime;




            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="m1">第１関節</param>
            /// <param name="r1">第１関節反転フラグ</param>
            /// <param name="m2">第２関節</param>
            /// <param name="r2">第２関節反転フラグ</param>
            /// <param name="m3">第３関節</param>
            /// <param name="r3">第３関節反転フラグ</param>
            public Exo_ArmModel(IMyMotorStator m1, IMyMotorStator m2, IMyMotorStator m3)
            {
                moters = new List<MoterModel>();
                moters.Add(new MoterModel(m1, new MotorOperationDataEntity(0, 0, true)));
                moters.Add(new MoterModel(m2, new MotorOperationDataEntity(0, 0, true)));
            }

            public bool Drive(IMyCockpit cockpit)
            {


                bool returnvalue = false;

                nowTime = DateTime.UtcNow;

                // if (cockpit.RotationIndicator.X > 0)
                // {
                //     myMotor1.MyMotor.TargetVelocityRad = cockpit.RotationIndicator.X/2;
                // }
                // else if (cockpit.RotationIndicator.X < 0)
                // {
                //     myMotor1.MyMotor.TargetVelocityRad = cockpit.RotationIndicator.X/2;
                // }
                // else
                // {
                //     myMotor1.MyMotor.TargetVelocityRad = 0;
                // }


                // if (cockpit.RotationIndicator.Y > 0)
                // {
                //     myMotor2.MyMotor.TargetVelocityRad = -cockpit.RotationIndicator.Y/2;
                // }
                // else if (cockpit.RotationIndicator.Y < 0)
                // {
                //     myMotor2.MyMotor.TargetVelocityRad = -cockpit.RotationIndicator.Y/2;
                // }
                // else
                // {
                //     myMotor2.MyMotor.TargetVelocityRad = 0;
                // }

                foreach (var item in moters)
                {
                    item.Update();
                }

                befTime = nowTime;

                return returnvalue;
            }


            // public void MoveUp()
            // {
            //     if (myMotor1.TargetAngleDegree < 90)
            //     {
            //         myMotor1.TargetAngleDegree = myMotor1.TargetAngleDegree + 1;
            //     }
            //     else
            //     {
            //         myMotor1.TargetAngleDegree = 90;
            //     }

            // }

            // public void MoveDown()
            // {
            //     if (myMotor1.TargetAngleDegree > -90)
            //     {
            //         myMotor1.TargetAngleDegree = myMotor1.TargetAngleDegree - 1;
            //     }
            //     else
            //     {
            //         myMotor1.TargetAngleDegree = -90;
            //     }
            // }

        }
    }
}
