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
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        IMyCockpit myCockpit;
        OperationServiceBase Operation;
        public Program()
        {
            myCockpit = (IMyCockpit)GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit No Oxygen");
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            Me.GetSurface(0).ContentType = ContentType.SCRIPT;


            var rLeg = new Exo_LegModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"));


            var lLeg = new Exo_LegModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"));


            var rArm = new Exo_ArmModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"));


            var lArm = new Exo_ArmModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"));

            Operation = new Exo_MovingOperationSrv(rLeg, lLeg, rArm, lArm);

            // arm = new ArmModel((IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor 3"), true,
            //     (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge 1"), true,
            //     (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Two-ended Motor 1"), true);
            // Operation = new S_ACS(arm, arm);


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
            var surface = Me.GetSurface(0);

            // Operation.armTarget(myCockpit);
            Operation.Drive(myCockpit);

            // using (MySpriteDrawFrame frame = surface.DrawFrame())
            // {
            //     MySprite sprite;

            //     string str = myCockpit.MoveIndicator.ToString().AddNewLine()+ myCockpit.RollIndicator.ToString().AddNewLine()+ myCockpit.RotationIndicator.ToString().AddNewLine() + myCockpit.ShowHorizonIndicator.ToString().AddNewLine();
            //     str += arm.myMotor1.TargetAngleDegree.ToString().AddNewLine();
            //     sprite = MySprite.CreateText(str, "Debug", Color.Red,0.8f,TextAlignment.LEFT);

            //     sprite.Position = new Vector2(0, 0);
            //     frame.Add(sprite);
            // }



        }
    }
}