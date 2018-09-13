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
            private double angle;
            private double befAngle;
            private double targetAngle;

            //public DateTime NowTime { get; set; }

            //public DateTime BefTime { get; set; }


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

            public float TargetRPM { get; set; }

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
            }


        }
    }
}
