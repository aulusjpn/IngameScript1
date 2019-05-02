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
        /// 動作管理クラス
        /// これを基底とし、歩行・速歩などを継承して作成する。
        /// </summary>
        public abstract class OperationBase
        {

            //コントロール取得用
            //public IMyCockpit cockpit;

            public string[] TargetListForward = new string[] { };
            public string[] TargetListBackward = new string[] { };
            public string[] TargetListMiddleward = new string[] { };

            public Utilty.StatusEnum ctrlStatus;

            /// <summary>
            /// 右足
            /// </summary>
            internal LegBase RLeg { get; set; }


            /// <summary>
            /// 左脚
            /// </summary>
            internal LegBase LLeg { get; set; }

            /// <summary>
            /// 右腕
            /// </summary>
            internal ArmBase RArm { get; set; }


            /// <summary>
            /// 左腕
            /// </summary>
            internal ArmBase LArm { get; set; }


            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="RLeg">右足</param>
            /// <param name="LLeg">左脚</param>
            public OperationBase(LegBase RLeg, LegBase LLeg,ArmBase Rarm,ArmBase LArm)
            {
                this.RLeg = RLeg;
                this.LLeg = LLeg;
                this.RArm = Rarm;
                this.LArm = LArm;
                ctrlStatus = Utilty.StatusEnum.off;
            }

            public void AimTarget_sensor(IMySensorBlock sensor)
            {
                string[] vs = RArm.myMotor1.MyMotor.CustomData.Split(',');
                var str = sensor.CustomData.Split(',');
                var vector = new Vector3D(double.Parse(str[0]), double.Parse(str[1]), double.Parse(str[2]));
                
                Vector3D worldDirection = vector - sensor.GetPosition();

                Vector3D direction = Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(sensor.WorldMatrix));

                vs[1] = MathHelperD.ToDegrees(-Math.Atan2(direction.Z, direction.Y)).ToString();
                RArm.myMotor1.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];

                vs = RArm.myMotor2.MyMotor.CustomData.Split(',');
                vs[1] = "0"; //MathHelperD.ToDegrees(Math.Atan2(direction.X, direction.Y)).ToString();
                RArm.myMotor2.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];


                IMyTextPanel t;
                t.sc
            }

            public void armTarget(IMyCockpit Rota)
            {

                //var str1 = RArm.myMotor2.MyMotor.CustomData;
                //string[] vs = str1.Split(',');
                //var buf = Rota.GetNaturalGravity();
                //Vector3D.ro
                //var dot = Vector3D.Dot(buf,Rota.WorldMatrix.Forward);

                //double sita = Math.Acos(dot);
                //sita = sita * 180.0 / Math.PI;

                //RArm.myMotor1.MyMotor.CustomData = vs[0] + "," + -sita + ',' + vs[2];
                //---------------------------------------------------------------------------------↓使いみちあり
                //var str1 = RArm.myMotor2.MyMotor.CustomData;
                //string[] vs = str1.Split(',');
                //double i = double.Parse(vs[1]);
                //if (Rota.RotationIndicator.Y > 0.5)
                //{
                //    vs[1] = (i - 1).ToString();
                //}
                //else if (Rota.RotationIndicator.Y < -0.5)
                //{
                //    vs[1] = (i + 1).ToString();
                //}

                //RArm.myMotor2.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];


                //撤去
                //var str1 = RArm.myMotor2.MyMotor.CustomData;
                //string[] vs = str1.Split(',');
                //double i = double.Parse(vs[1]);
                //if (Rota.RotationIndicator.Y > 1)
                //{
                //    vs[1] = (i - (Rota.RotationIndicator.Y / 10)).ToString();

                //    if (i > 90)
                //    {
                //        vs[1] = "90";
                //    }

                //}
                //else if (Rota.RotationIndicator.Y < -1)
                //{
                //    vs[1] = (i - (Rota.RotationIndicator.Y / 10)).ToString();

                //    if (i < -90)
                //    {
                //        vs[1] = "-90";
                //    }

                //}

                //RArm.myMotor2.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];


                //vs = null;
                //var str2 = RArm.myMotor1.MyMotor.CustomData;
                //vs = str2.Split(',');
                //i = double.Parse(vs[1]);
                //if (Rota.RotationIndicator.X > 1)
                //{
                //    vs[1] = (i - (Rota.RotationIndicator.X / 10)).ToString();

                //    if (i < 0)
                //    {
                //        vs[1] = "0";
                //    }

                //}
                //else if (Rota.RotationIndicator.X < -1)
                //{
                //    vs[1] = (i - (Rota.RotationIndicator.X / 10)).ToString();
                //    if (i > 180)
                //    {
                //        vs[1] = "180";
                //    }

                //}

                //RArm.myMotor1.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];



            }

            public abstract void Drive();

        }
    }
}
