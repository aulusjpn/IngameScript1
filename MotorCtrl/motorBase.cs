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
        public class motorBase
        {
            private DateTime nowTime;
            private DateTime befTime;

            /// <summary>
            /// radian
            /// </summary>
            private double befRadian = 0;

            /// <summary>
            /// radian
            /// </summary>
            private double targetRadians = 0;
            private float targetRPM { get; set; }
            //public bool ReverseFlg { get; set; }
            private bool finishFlg { get; set; }

            private bool MyMotor_reverse { get; set; }

            private IMyMotorStator MyMotor { get; set; }

            private double motorRadian
            {
                get { return MyMotor.Angle; }
            }


            public motorBase(IMyMotorStator moter, bool reverseFlg, float targetAngle, float targetRpm)
            {

                MyMotor = moter;
                MyMotor_reverse = reverseFlg;
                targetRadians = MathHelperD.ToRadians(targetAngle);
                targetRPM = targetRpm;

                nowTime = DateTime.UtcNow;
                befTime = DateTime.UtcNow;
            }


            public void Main()
            {

                string[] str = MyMotor.CustomData.Split(',');

                if (str.Length < 3)
                {
                    finishFlg = false;
                    return;
                }


                float buff = float.Parse(str[1]);
                targetRadians = MyMotor_reverse ? MathHelperD.ToRadians(-buff) : MathHelperD.ToRadians(buff);
                targetRPM = float.Parse(str[2]);

                nowTime = DateTime.UtcNow;

                finishFlg = fastMove(this);

                
            }

            private bool fastMove(motorBase motor)
            {
                TimeSpan ts = befTime - DateTime.UtcNow;

                double diffAngle = MathHelperD.ToDegrees(motor.targetRadians) - MathHelperD.ToDegrees(motor.motorRadian);

                double rad = MathHelperD.ToRadians(diffAngle) / (-0.03);

                if (targetRPM < Math.Abs(rad * 9.549))
                {
                    MyMotor.TargetVelocityRPM = MathHelperD.ToDegrees(rad) < 0 ? targetRPM : -targetRPM;
                }
                else
                {
                    motor.MyMotor.TargetVelocityRad = -(float)(rad / 2);
                }

                

                befTime = DateTime.UtcNow;
                motor.befRadian = motor.motorRadian;

                if (Math.Abs(MathHelperD.ToDegrees(motor.motorRadian) - MathHelperD.ToDegrees(motor.targetRadians)) < 0.5)
                {
                    motor.MyMotor.TargetVelocityRad = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool justMove(motorBase motor)
            {
                TimeSpan ts = befTime - nowTime;

                double ang = (MathHelperD.ToDegrees(motor.motorRadian) - MathHelperD.ToDegrees(motor.befRadian));

                double diffAngle = MathHelperD.ToDegrees(motor.targetRadians) - MathHelperD.ToDegrees(motor.motorRadian);

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                if (diffAngle > rad)
                {
                    motor.MyMotor.TargetVelocityRPM = motor.targetRPM;
                }
                else
                {

                    motor.MyMotor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
                }

                motor.befRadian = motor.motorRadian;

                if ((MathHelperD.ToDegrees(motor.motorRadian) - MathHelperD.ToDegrees(motor.targetRadians)) < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
