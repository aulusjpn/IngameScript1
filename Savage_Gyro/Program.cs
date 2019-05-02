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
    partial class Program : MyGridProgram
    {

        List<IMyTerminalBlock> gyros = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> gyrorolls = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> gyroyowss = new List<IMyTerminalBlock>();
        private IMyCockpit cockpit;
        private IMyTextPanel text;
        private DateTime beforeTime;
        private DateTime nowTime;
        private double oldAngle;
        private double nowAngle;
        static float pitch60 = (float)(2 * Math.PI);
        static float pitch30 = (float)(1 * Math.PI);
        static float pitch20 = (float)(2 / 3 * Math.PI);
        static float pitch1 = pitch60 / 60f;

        public Program()
        {
            GridTerminalSystem.GetBlockGroupWithName("Gyros").GetBlocks(gyros);
            GridTerminalSystem.GetBlockGroupWithName("GyroYows").GetBlocks(gyroyowss);
            GridTerminalSystem.GetBlockGroupWithName("GyroRolls").GetBlocks(gyrorolls);
            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            text = GridTerminalSystem.GetBlockWithName("Text panel 6") as IMyTextPanel;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            text.WritePublicText("");
            GyroControl();
            Control();
        }


        public void GyroControl()
        {
            var vector = cockpit.GetNaturalGravity();

            //var cockpitmatrix = Matrix.CreateFromDir(cockpit.WorldMatrix.Down);
            var cockpitmatrix = MatrixD.CreateWorld(cockpit.GetPosition(), cockpit.WorldMatrix.Forward, cockpit.WorldMatrix.Down);
            //cockpitmatrix.Forward = cockpitmatrix.Down;

            Vector3D direction = Vector3D.TransformNormal(vector, Matrix.Transpose(cockpitmatrix));

            var diffangle = MathHelperD.ToDegrees(Math.Atan2(direction.Z, direction.Y));

            if (diffangle < Math.Abs(oldAngle - diffangle))
            {

            }

            float pitch = 0f;


            WriteText("Gyro diffangle: " + diffangle.ToString() + " -\n");
            WriteText("Gyro pitch1: " + ((pitch1)).ToString() + " -\n");
            WriteText("Gyro pitch60: " + ((pitch60)).ToString() + " -\n");
            WriteText("Gyro pitch: " + ((float)(pitch1 * diffangle)).ToString() + " -\n");


            if (diffangle > 5)
            {
                if (diffangle > 20)
                {
                    pitch = (float)(0.8 * Math.PI);
                }
                else
                {
                    pitch = (float)(pitch1 * 10);
                }

                foreach (var item in gyros)
                {
                    var gyro = item as IMyGyro;
                    gyro.CustomData = gyro.Pitch.ToString() + " ::::" + ((float)2 * Math.PI).ToString() + " cheack " + gyro.Pitch.Equals((float)(2 * Math.PI));
                    if (!gyro.Pitch.Equals(pitch))
                    {
                        gyro.Pitch = pitch;
                    }
                }
            }
            else if (diffangle < -5)
            {
                if (diffangle < -20)
                {
                    pitch = (float)(-0.8 * Math.PI);
                }
                else
                {
                    pitch = (float)(pitch1 * -10);
                }

                foreach (var item in gyros)
                {
                    var gyro = item as IMyGyro;
                    if (!gyro.Pitch.Equals(pitch))
                    {
                        gyro.Pitch = pitch;
                    }
                }
            }
            else
            {
                foreach (var item in gyros)
                {
                    var gyro = item as IMyGyro;
                    gyro.Pitch = (float)(pitch1 * diffangle);
                }
            }


            oldAngle = diffangle;



            //diffangle = MathHelperD.ToDegrees(Math.Atan2(direction.X, direction.Y));

            //float roll = 0f;


            //if (diffangle > 5)
            //{
            //    if (diffangle > 5)
            //    {
            //        roll = (float)(1 * Math.PI);
            //    }


            //    foreach (var item in gyrorolls)
            //    {
            //        var gyro = item as IMyGyro;
            //        gyro.CustomData = gyro.Roll.ToString() + " ::::" + ((float)2 * Math.PI).ToString() + " cheack " + gyro.Roll.Equals((float)(2 * Math.PI));
            //        if (!gyro.Roll.Equals(roll))
            //        {
            //            gyro.Roll = roll;
            //        }
            //    }
            //}
            //else if (diffangle < -5)
            //{
            //    if (diffangle < -5)
            //    {
            //        roll = (float)(-1 * Math.PI);
            //    }


            //    foreach (var item in gyrorolls)
            //    {
            //        var gyro = item as IMyGyro;
            //        if (!gyro.Roll.Equals(roll))
            //        {
            //            gyro.Roll = roll;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (var item in gyrorolls)
            //    {
            //        var gyro = item as IMyGyro;
            //        if (!gyro.Roll.Equals(0))
            //        {
            //            gyro.Roll = 0;
            //        }
            //    }
            //}


        }

        public void Control()
        {

            WriteText("Gyro pitch30: " + ((float)(pitch30)).ToString() + " -\n");
            foreach (var item in gyroyowss)
            {
                var gyro = item as IMyGyro;
                WriteText("Gyro yow: " + ((float)(gyro.Yaw)).ToString() + " -\n");
                if (cockpit.MoveIndicator.X > 0.5)
                {
                    if (!gyro.Yaw.Equals(pitch30))
                    {
                        gyro.Yaw = pitch30;
                    }
                }
                else if (cockpit.MoveIndicator.X < -0.5)
                {

                    if (!gyro.Yaw.Equals(-pitch30))
                    {
                        gyro.Yaw = -pitch30;
                    }
                }
                else
                {
                    if (!gyro.Yaw.Equals(0))
                    {
                        gyro.Yaw = 0;
                    }
                }
            }

        }

        public void Log()
        {
            foreach (var item in gyros)
            {
                var gyro = item as IMyGyro;
                WriteText("Gyro : " + gyro.Pitch.ToString() + " -\n");
            }

        }

        public void WriteText(string str)
        {
            text.WritePublicText(text.GetPublicText() + str);
        }
    }
}