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
//using static IngameScript.Program.Utilty;

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
            private double befAngle = 0;

            /// <summary>
            /// radian
            /// </summary>
            private double targetAngle = 0;
            public float TargetRPM { get; set; }
            //public bool ReverseFlg { get; set; }
            public bool finishFlg { get; set; }

            //public DateTime NowTime { get; set; }

            //public DateTime BefTime { get; set; }


            public double Angle
            {
                get
                {
                    return MathHelperD.ToDegrees(MyMotor.Angle);
                }
            }

            public bool MyMotor_reverse { get; set; }

            public IMyMotorStator MyMotor { get; set; }

            public double TargetAngle
            {
                get
                {
                    return MathHelperD.ToDegrees(targetAngle);
                }

                set
                {
                    targetAngle = MathHelperD.ToRadians(value);
                }
            }


            public double BefAngle
            {
                get
                {
                    return MathHelperD.ToDegrees(befAngle);
                }

                set
                {
                    befAngle = MathHelperD.ToRadians(value);
                }
            }

            public motorBase(IMyMotorStator moter, bool flg, float target, float rpm)
            {
                //NowTime = DateTime.UtcNow;
                //BefTime = DateTime.UtcNow;

                MyMotor = moter;
                MyMotor_reverse = flg;
                TargetAngle = target;
                TargetRPM = rpm;

                nowTime = DateTime.UtcNow;
                befTime = DateTime.UtcNow;
            }


            public void Main()
            {

                string[] str = MyMotor.CustomData.Split(',') ;

                if (str.Length < 3)
                {
                    finishFlg = false;
                    return;
                }


                float buff = float.Parse(str[1]) ;
                TargetAngle = MyMotor_reverse ? -buff: buff;
                TargetRPM = float.Parse(str[2]);

                nowTime = DateTime.UtcNow;

                finishFlg = fastMove(this);

            }

            private bool fastMove(motorBase motor)
            {
                TimeSpan ts = befTime - DateTime.UtcNow;

                //double ang = (motor.Angle - motor.BefAngle);

                double diffAngle = motor.TargetAngle - motor.Angle;

                double rad = MathHelperD.ToRadians(diffAngle) / (-0.03);

                // motor.MyMotor.TargetVelocityRad = -(float)(rad / 5);

                if (TargetRPM < Math.Abs(rad * 9.549))
                {
                    MyMotor.TargetVelocityRPM = MathHelperD.ToDegrees(rad) < 0 ? TargetRPM : -TargetRPM;
                }
                else
                {
                    motor.MyMotor.TargetVelocityRad = -(float)(rad/2);
                }


                befTime = DateTime.UtcNow;
                motor.BefAngle = motor.Angle;

                if (Math.Abs(motor.Angle - motor.TargetAngle) < 0.5)
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

                double ang = (motor.Angle - motor.BefAngle);

                double diffAngle = motor.TargetAngle - motor.Angle;

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                if (diffAngle > rad)
                {
                    motor.MyMotor.TargetVelocityRPM = motor.TargetRPM;
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


        }
    }
}
