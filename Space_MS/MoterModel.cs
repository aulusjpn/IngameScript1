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

            public double BefAngleDegree { get; private set; }

            public IMyMotorStator MyMotor { get; set; }

            /// <summary>
            /// モーターの操作情報保持クラス。
            /// あくまで命令保持であり、データ
            /// </summary>
            public MotorOperationDataEntity DataEntity { get; set; }

            public bool finishFlg { get; set; }

            public MoterModel(IMyMotorStator moter, MotorOperationDataEntity entity)
            {
                MyMotor = moter;
                this.DataEntity = entity;

                nowTime = DateTime.UtcNow;
                befTime = DateTime.UtcNow;
            }


            public bool Update(MotorOperationDataEntity dataEntity = null)
            {


                nowTime = DateTime.UtcNow;
                if (dataEntity != null)
                {
                    this.DataEntity = dataEntity;
                }
                else
                {
                    this.DataEntity = MotorOperationFormatter.getDataEntityFromMoterCustomData(MyMotor, this.DataEntity);
                }


                return fastMove();

            }


            private bool fastMove()
            {
                TimeSpan ts = befTime - DateTime.UtcNow;

                //double ang = (motor.Angle - motor.BefAngle);

                double diffAngle = DataEntity.GetTargetAngle() - MathHelperD.ToDegrees(MyMotor.Angle);

                double rad = MathHelperD.ToRadians(diffAngle) / (-0.03);


                if (DataEntity.GetVelocity() < Math.Abs(rad * 9.549))
                {
                    MyMotor.TargetVelocityRPM = MathHelperD.ToDegrees(rad) < 0 ? DataEntity.GetVelocity() : -DataEntity.GetVelocity();
                }
                else
                {
                    MyMotor.TargetVelocityRad = -(float)(rad / 2);
                }


                befTime = DateTime.UtcNow;
                BefAngleDegree = MathHelperD.ToDegrees(MyMotor.Angle);

                if (Math.Abs(MathHelperD.ToDegrees(MyMotor.Angle) - DataEntity.GetTargetAngle()) < 1)
                {
                    MyMotor.TargetVelocityRad = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool justMove()
            {
                TimeSpan ts = befTime - nowTime;


                double ang = (MathHelperD.ToDegrees(MyMotor.Angle) - BefAngleDegree);

                double diffAngle = DataEntity.GetTargetAngle() - (MathHelperD.ToDegrees(MyMotor.Angle));

                double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;

                if (diffAngle > rad)
                {
                    MyMotor.TargetVelocityRPM = DataEntity.GetVelocity();
                }
                else
                {

                    MyMotor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
                }

                BefAngleDegree = MathHelperD.ToDegrees(MyMotor.Angle);

                if ((MathHelperD.ToDegrees(MyMotor.Angle) - DataEntity.GetTargetAngle()) < 0.5)
                {
                    MyMotor.TargetVelocityRad = 0;
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
