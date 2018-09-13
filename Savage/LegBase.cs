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
        /// 脚部クラス
        /// </summary>
        public class LegBase
        {
            private DateTime nowTime;
            private DateTime befTime;

            public PartsMoveEnum LegStatus;

            private motorBase myMotorKnee;


            private motorBase myMotorThigh;


            private motorBase myMotorAnkle;


            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="m1">第１関節</param>
            /// <param name="r1">第１関節反転フラグ</param>
            /// <param name="m2">第２関節</param>
            /// <param name="r2">第２関節反転フラグ</param>
            /// <param name="m3">第３関節</param>
            /// <param name="r3">第３関節反転フラグ</param>
            public LegBase(IMyMotorStator m1,bool r1,IMyMotorStator m2, bool r2, IMyMotorStator m3,bool r3)
            {
                LegStatus = PartsMoveEnum.off;
                myMotorKnee = new motorBase(m1, r1, 0, 0);
                myMotorThigh = new motorBase(m2, r2, 0, 0);
                myMotorAnkle = new motorBase(m3, r3, 0, 0);
            }


            /// <summary>
            /// Movinegmoter
            /// </summary>
            /// <returns>Finisih?</returns>
            public bool DriveParts()
            {
                bool returnvalue = false;
                nowTime = DateTime.UtcNow;

                if (fastMove(myMotorKnee) && fastMove(myMotorThigh) && fastMove(myMotorAnkle))
                {
                    returnvalue = true;
                }

                befTime = nowTime;

                return returnvalue;
            }


            public void justMove(double nowAngle, double beffAngle, IMyMotorStator motor, float targetAngle)
            {
                TimeSpan ts = befTime - nowTime;

                double ang = (nowAngle - beffAngle);

                double diffAngle = targetAngle - nowAngle;

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                motor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
            }

            public bool fastMove(motorBase motor)
            {
                TimeSpan ts = befTime - nowTime;

                double ang = (motor.Angle - motor.BefAngle);

                double diffAngle = motor.TargetAngle - motor.Angle;

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                if (diffAngle > rad)
                {
                    motor.MyMotor.TargetVelocityRPM = 60f;
                }
                else
                {

                    motor.MyMotor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
                }

                motor.BefAngle = motor.Angle;

                if ((motor.Angle - motor.TargetAngle) < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            public void MorterMoveToAngle(IMyMotorStator motorStator, double TargetAngle, float velocity)
            {
                var angle = MathHelperD.ToDegrees(motorStator.Angle) - TargetAngle;
                if (Math.Abs(angle) < velocity)
                {
                    motorStator.TargetVelocityRad = 0;
                }
                else if (MathHelperD.ToDegrees(motorStator.Angle) > TargetAngle)
                {
                    motorStator.TargetVelocityRad = -velocity;
                }
                else if (MathHelperD.ToDegrees(motorStator.Angle) < TargetAngle)
                {
                    motorStator.TargetVelocityRad = velocity;
                }
            }
        }
    }
}
