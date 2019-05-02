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
        private IMyMotorStator ArmR1;
        private IMyMotorStator ArmR2;
        private IMyMotorStator ArmR3;
        private IMyMotorStator ArmR4;
        private IMyMotorStator ArmL1;
        private IMyMotorStator ArmL2;
        public IMySensorBlock mySensor;

        ArmBase Arm1;
        ArmBase Arm2;

        static string Standstr = "Stand";
        static string Shootstr = "Shoot";
        static string FirstWeaponlack = "1st";
        static string SecondWeaponlack = "2st";

        private string st = "";
        private IMyTextPanel text;
        private OperationBase DriveEntity;

        public Program()
        {
            ArmR1 = GridTerminalSystem.GetBlockWithName("Rotor R") as IMyMotorStator;
            ArmR2 = GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge R1") as IMyMotorStator;
            ArmR3 = GridTerminalSystem.GetBlockWithName("Two-ended Motor") as IMyMotorStator;
            ArmR4 = GridTerminalSystem.GetBlockWithName("Two-ended Rotor") as IMyMotorStator;
            ArmL1 = GridTerminalSystem.GetBlockWithName("Rotor L") as IMyMotorStator;
            ArmL2 = GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge L1") as IMyMotorStator;

            mySensor = GridTerminalSystem.GetBlockWithName("Sensor") as IMySensorBlock;

            Arm1 = new ArmBase(ArmR1, true, ArmR2, false, ArmR3, false, ArmR4,false);

            Arm2 = new ArmBase(ArmL1, false, ArmL2, false, ArmL2, false, ArmL2, false);

            DriveEntity = new WalkClass(Arm1, Arm2);
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
            if (Me.CustomData == FirstWeaponlack)
            {
                DriveEntity.RArm.myMotor1.MyMotor.CustomData = "1,-5,20";
                DriveEntity.RArm.myMotor2.MyMotor.CustomData = "1,-2,20";
                DriveEntity.RArm.myMotor3.MyMotor.CustomData = "1,0,20";
            }
            else if (Me.CustomData == Standstr)
            {
                DriveEntity.RArm.myMotor1.MyMotor.CustomData = "1,0,20";
                DriveEntity.RArm.myMotor2.MyMotor.CustomData = "1,5,20";
                DriveEntity.RArm.myMotor3.MyMotor.CustomData = "1,0,20";
            }
            else if (Me.CustomData == SecondWeaponlack)
            {
                DriveEntity = new Arm1Class(Arm1, Arm2);
            }
            //else if (Me.CustomData == SecondWeaponlack)
            //{
            //    DriveEntity = new Arm2Class(Arm1, Arm2);
            //}
            else //if(Me.CustomData == Shootstr)
            {
                DriveEntity.AimTarget_sensor(mySensor);
            }


            DriveEntity.Drive();
        }


    }
}