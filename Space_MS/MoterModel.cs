using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
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
        public class MoterModel
        {
            private DateTime nowTime;
            private DateTime befTime;

            /// <summary>
            /// radian
            /// </summary>
            private double befAngleRadian = 0;

            /// <summary>
            /// radian
            /// </summary>
            private double targetAngleRadian = 0;



            public IMyMotorStator MyMotor { get; set; }

            public float TargetRPM { get; set; }
            public bool MyMotorReverse { get; set; }
            public bool finishFlg { get; set; }


            public double AngleDegree
            {
                get
                {
                    return MathHelperD.ToDegrees(MyMotor.Angle);
                }
            }



            public double TargetAngleDegree
            {
                get
                {
                    return MathHelperD.ToDegrees(targetAngleRadian);
                }

                set
                {
                    targetAngleRadian = MathHelperD.ToRadians(value);
                }
            }


            public double BefAngleDegree
            {
                get
                {
                    return MathHelperD.ToDegrees(befAngleRadian);
                }

                set
                {
                    befAngleRadian = MathHelperD.ToRadians(value);
                }
            }

            public MoterModel(IMyMotorStator moter, bool reverseflg, float targetangledegree, float targetrpm)
            {
                MyMotor = moter;
                MyMotorReverse = reverseflg;
                TargetAngleDegree = targetangledegree;
                TargetRPM = targetrpm;

                nowTime = DateTime.UtcNow;
                befTime = DateTime.UtcNow;
            }


            public void Update()
            {

                
                nowTime = DateTime.UtcNow;


                //finishFlg = fastMove(this);

            }

            private void getOrderTextData()
            {
                string text = MyMotor.CustomData;

                var arr  = text.Split(Environment.NewLine.ToArray(),StringSplitOptions.None);

                
            }

            private bool fastMove(MoterModel motor)
            {
                TimeSpan ts = befTime - DateTime.UtcNow;

                //double ang = (motor.Angle - motor.BefAngle);

                double diffAngle = motor.TargetAngleDegree - motor.AngleDegree;

                double rad = MathHelperD.ToRadians(diffAngle) / (-0.03);


                if (TargetRPM < Math.Abs(rad * 9.549))
                {
                    MyMotor.TargetVelocityRPM = MathHelperD.ToDegrees(rad) < 0 ? TargetRPM : -TargetRPM;
                }
                else
                {
                    motor.MyMotor.TargetVelocityRad = -(float)(rad / 2);
                }


                befTime = DateTime.UtcNow;
                motor.BefAngleDegree = motor.AngleDegree;

                if (Math.Abs(motor.AngleDegree - motor.TargetAngleDegree) < 0.5)
                {
                    motor.MyMotor.TargetVelocityRad = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool justMove(MoterModel motor)
            {
                TimeSpan ts = befTime - nowTime;

                double ang = (motor.AngleDegree - motor.BefAngleDegree);

                double diffAngle = motor.TargetAngleDegree - motor.AngleDegree;

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                if (diffAngle > rad)
                {
                    motor.MyMotor.TargetVelocityRPM = motor.TargetRPM;
                }
                else
                {

                    motor.MyMotor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
                }

                motor.BefAngleDegree = motor.AngleDegree;

                if ((motor.AngleDegree - motor.TargetAngleDegree) < 0.5)
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
