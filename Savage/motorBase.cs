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
using static IngameScript.Program.Utilty;

namespace IngameScript
{
    partial class Program
    {
        public class motorBase
        {
            private DateTime nowTime;
            private DateTime befTime;

            private IMyMotorStator myMotor;
            private bool myMotor_reverse;
            private double angle;
            private double targetAngle;
            private float targetRPM;



            public DateTime NowTime
            {
                get
                {
                    return nowTime;
                }

                set
                {
                    nowTime = value;
                }
            }

            public DateTime BefTime
            {
                get
                {
                    return befTime;
                }

                set
                {
                    befTime = value;
                }
            }


            public double Angle
            {
                get
                {
                    return MathHelperD.ToDegrees(angle);
                }

                set
                {
                    angle = MathHelperD.ToRadians(value);
                }
            }

            public bool MyMotor_reverse
            {
                get
                {
                    return myMotor_reverse;
                }

                set
                {
                    myMotor_reverse = value;
                }
            }

            public IMyMotorStator MyMotor
            {
                get
                {
                    return myMotor;
                }

                set
                {
                    myMotor = value;
                }
            }

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

            public float TargetRPM
            {
                get
                {
                    return targetRPM;
                }

                set
                {
                    targetRPM = value;
                }
            }

            public motorBase(IMyMotorStator moter, bool flg, float target, float rpm)
            {
                nowTime = DateTime.UtcNow;
                befTime = DateTime.UtcNow;

                MyMotor = moter;
                MyMotor_reverse = flg;
                TargetAngle = target;
                TargetRPM = rpm;
            }


        }
    }
}
