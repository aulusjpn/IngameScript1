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
        private IMyCockpit cockpit;
        List<IMyTerminalBlock> gyros = new List<IMyTerminalBlock>();

        double lastpitch;
        double lastRoll;

        private DateTime nowTime;
        private DateTime befTime;

        public Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            GridTerminalSystem.GetBlockGroupWithName("Gyros").GetBlocks(gyros);

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            nowTime = DateTime.UtcNow;
            GyroControl();
            befTime = nowTime;
        }

        public void GyroControl()
        {

            var vector = cockpit.GetNaturalGravity();

            var cockpitmatrix = cockpit.WorldMatrix;

            Vector3D direction = Vector3D.TransformNormal(vector, Matrix.Transpose(cockpitmatrix));

            float pitch = (float)direction.Z / 2;
            float roll = -((float)direction.X / 2);
            foreach (var item in gyros)
            {
                var gyro = item as IMyGyro;
                gyro.Pitch = (float)pitch / 5;
                gyro.Roll = (float)roll / 5;
                //gyro.Pitch = pitch > 0 ? (float)Math.Pow(pitch, 2) : -(float)Math.Pow(pitch, 2);
                //gyro.Roll = roll > 0 ? (float)Math.Pow(roll, 2) : -(float)Math.Pow(roll, 2);
            }


            //var rot = cockpit.WorldMatrix;
            //var rotinv = MatrixD.Transpose(rot);

            //var shipPos = cockpit.GetPosition();
            //var deltaInWorld = shipPos - (shipPos + vector);

            ////var deltaInLocal = Vector3.Transform(deltaInWorld, rotinv);
            //var deltaInLocal = Vector3D.Transform(vector, rotinv);

            //var pitch = Math.Atan2(deltaInLocal.GetDim(0), deltaInLocal.GetDim(2));
            //var roll = Math.Atan2(deltaInLocal.GetDim(0), deltaInLocal.GetDim(1));
            //var pitchBackup = pitch;
            //var rollBackup = roll;

            //pitch = ToRPM(pitch, lastpitch);
            //roll = ToRPM(roll, lastRoll);
            //lastpitch = pitchBackup;
            //lastRoll = rollBackup;


            ////foreach (var item in gyros)
            //{
            //    var gyro = item as IMyGyro;
            //    gyro.Pitch = (float)-pitch/5;
            //    gyro.Roll = (float)roll/5;
            //}

        }


        double ToRPM(double rot, double lastRot)
        {
            double deltaRot = lastRot - rot;
            deltaRot /= 0.02;
            return (rot - (deltaRot)) / (Math.PI * 2.0) * 60.0;
        }
    }
}