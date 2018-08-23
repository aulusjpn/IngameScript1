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
        public class LegBase
        {
            private DateTime nowTime;
            private DateTime befTime;

            private PartsMoveEnum LegStatus;

            private motorBase myMotorKnee;


            private motorBase myMotorThigh;


            private motorBase myMotorAnkle;



            public LegBase(IMyMotorStator m1,bool r1,IMyMotorStator m2, bool r2, IMyMotorStator m3,bool r3)
            {
                LegStatus = PartsMoveEnum.Stand_Up;
                myMotorKnee = new motorBase(m1, r1, 0, 0);
                myMotorThigh = new motorBase(m2, r2, 0, 0);
                myMotorThigh = new motorBase(m3, r3, 0, 0);
            }



            public bool DriveParts()
            {
                nowTime = DateTime.UtcNow;
                if (LegStatus != PartsMoveEnum.Ready_For && LegStatus != PartsMoveEnum.Ready_Back)
                {
                    fastMove(myMotorKnee.Angle,,myMotorKnee,kneeTargetAngle); 
                }
                else
                {
                   
                }


                befTime = nowTime;

                return false;
            }


            public void justMove(double nowAngle, double beffAngle, IMyMotorStator motor, float targetAngle)
            {
                TimeSpan ts = befTime - nowTime;

                double ang = (nowAngle - beffAngle);

                double diffAngle = targetAngle - nowAngle;

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                motor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
            }

            public void fastMove(double nowAngle, double beffAngle, IMyMotorStator motor, float targetAngle)
            {
                TimeSpan ts = befTime - nowTime;

                double ang = (nowAngle - beffAngle);

                double diffAngle = targetAngle - nowAngle;

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                if (diffAngle > rad)
                {
                    motor.TargetVelocityRPM = 60f;
                }
                else
                {

                    motor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
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
