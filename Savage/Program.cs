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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private IMyCockpit cockpit;
        private IMyMotorStator roterR1;
        private IMyMotorStator roterR2;
        private IMyMotorStator roterL1;
        private IMyMotorStator roterL2;
        private IMyTextPanel text;
        private Vector3D targetVector;
        private DateTime nowTime;
        private DateTime befTime;
        private List<double> angleList = new List<double>() { 0, 0, 0, 0 };

        private OperationBase DriveEntity;



        public Program()
        {

            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            roterR1 = GridTerminalSystem.GetBlockWithName("Rotor R") as IMyMotorStator;
            roterR2 = GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge R1") as IMyMotorStator;
            roterL1 = GridTerminalSystem.GetBlockWithName("Rotor L") as IMyMotorStator;
            roterL2 = GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge L1") as IMyMotorStator;
            text = GridTerminalSystem.GetBlockWithName("Text panel") as IMyTextPanel;
            befTime = DateTime.UtcNow;
            DriveEntity = new WalkClass();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            //for (int i = 0; i < 4; i++)
            //{
            //    angleList[i] = 0;
            //}
            nowTime = DateTime.UtcNow;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            Vector3 move = cockpit.MoveIndicator;
            Vector2 Rota = cockpit.RotationIndicator;
            float Roll = cockpit.RollIndicator;
            var str = "";
            var Orientation = cockpit.WorldMatrix.Forward;

            if (move.X > 0)
            {
                DriveEntity.ctrlStatus = OperationBase.StatusEnum.Forword;
            }
            else if (move.X < 0)
            {
                DriveEntity.ctrlStatus = OperationBase.StatusEnum.Back;
            }
            else
            {
                DriveEntity.ctrlStatus = OperationBase.StatusEnum.Halt;
            }


            nowTime = DateTime.UtcNow;
            JustMoving(MathHelperD.ToDegrees(roterR1.Angle), angleList[0],roterR1, -90);
            angleList[0] = MathHelperD.ToDegrees(roterR1.Angle);
            JustMoving(MathHelperD.ToDegrees(roterR2.Angle), angleList[1], roterR2, 0);
            angleList[1] = MathHelperD.ToDegrees(roterR2.Angle);
            JustMoving(MathHelperD.ToDegrees(roterL1.Angle), angleList[2], roterL1, 90);
            angleList[2] = MathHelperD.ToDegrees(roterL1.Angle);
            JustMoving(MathHelperD.ToDegrees(roterL2.Angle), angleList[3], roterL2, 0);
            angleList[3] = MathHelperD.ToDegrees(roterL2.Angle);


            befTime = nowTime;
        }


        public void JustMoving(double nowAngle, double beffAngle,IMyMotorStator motor,float targetAngle)
        {
           

            double ang = (nowAngle - beffAngle);

            double diffAngle = targetAngle - nowAngle;

            motor.TargetVelocityRPM = (float)(diffAngle / (Math.PI * 2));
            //if (diffAngle > ang)
            //{
            //    motor.TargetVelocityRPM = ang > 0 ? 60 : -60;
            //}
            //else
            //{
            //    motor.TargetVelocityRPM = (float)ang;
            //}

        }

        public void FastMoving(double nowAngle, double beffAngle, IMyMotorStator motor, float targetAngle)
        {
            TimeSpan ts = befTime - nowTime;

            double ang = (nowAngle - beffAngle);

            double diffAngle = targetAngle - nowAngle;

            double rad = MathHelperD.ToRadians(diffAngle) / ts.TotalSeconds;


        }


    }
}