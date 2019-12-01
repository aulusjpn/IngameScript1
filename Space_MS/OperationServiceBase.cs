﻿using Sandbox.Game.EntityComponents;
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

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// 動作管理クラス
        /// これを基底とし、歩行・速歩などを継承して作成する。
        /// </summary>
        public abstract class OperationServiceBase
        {

            //コントロール取得用
            public string[] TargetListForward = new string[] { };
            public string[] TargetListBackward = new string[] { };
            public string[] TargetListMiddleward = new string[] { };

            /// <summary>
            /// 右足
            /// </summary>
            protected IPart RPart { get; set; }

            /// <summary>
            /// 左脚
            /// </summary>
            protected IPart LPart { get; set; }


            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="RLeg">右足</param>
            /// <param name="LLeg">左脚</param>
            public OperationServiceBase(IPart RPart, IPart LPart)
            {
                this.RPart = RPart;
                this.LPart = LPart;

            }

            //public abstract void MovingPart();

            public void AimTarget_sensor(IMySensorBlock sensor)
            {
                //string[] vs = RArm.myMotor1.MyMotor.CustomData.Split(',');
                //var str = sensor.CustomData.Split(',');
                //var vector = new Vector3D(double.Parse(str[0]), double.Parse(str[1]), double.Parse(str[2]));
                
                //Vector3D worldDirection = vector - sensor.GetPosition();

                //Vector3D direction = Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(sensor.WorldMatrix));

                //vs[1] = MathHelperD.ToDegrees(-Math.Atan2(direction.Z, direction.Y)).ToString();
                //RArm.myMotor1.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];

                //vs = RArm.myMotor2.MyMotor.CustomData.Split(',');
                //vs[1] = "0"; //MathHelperD.ToDegrees(Math.Atan2(direction.X, direction.Y)).ToString();
                //RArm.myMotor2.MyMotor.CustomData = vs[0] + "," + vs[1] + ',' + vs[2];

            }

            public abstract void armTarget(IMyCockpit Rota);

            public abstract void Drive(IMyCockpit cockpit);

        }
    }
}