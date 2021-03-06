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
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        IMyCockpit cockpit;
        IMyMotorStator roterR1;
        IMyMotorStator roterR2;
        IMyMotorStator roterL1;
        IMyMotorStator roterL2;
        IMyTextPanel text;
        Vector2 targetAngle;
        StatusEnum Status = StatusEnum.off;

        //照準角度
        private float elevationAngle;
        private float azimuthangle;

        enum StatusEnum
        {
            Stand = 0,
            Ready = 1,
            Move = 2,
            Run = 3,
            Stand_Up =4, 
            off = 9,
        }

        public Program()
        {


            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            //roterR1 = GridTerminalSystem.GetBlockWithName("Arm R1") as IMyMotorStator;
            //roterR2 = GridTerminalSystem.GetBlockWithName("Arm R2") as IMyMotorStator;
            //roterL1 = GridTerminalSystem.GetBlockWithName("Arm L1") as IMyMotorStator;
            //roterL2 = GridTerminalSystem.GetBlockWithName("Arm L2") as IMyMotorStator;
            //text = GridTerminalSystem.GetBlockWithName("Text panel") as IMyTextPanel;
            targetAngle = Vector2.Zero;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            elevationAngle = 0f;
            azimuthangle = 0f;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {

            Vector3 move = cockpit.MoveIndicator;
            Vector2 Rota = cockpit.RotationIndicator;
            float Roll = cockpit.RollIndicator;

            string str = "";
            Vector3D Orientation = cockpit.WorldMatrix.Forward;

            str = "MoveIndicator:" + cockpit.Name + cockpit.MoveIndicator.ToString() + "\r\n";
            str = str + "RotationIndicator:" + cockpit.RotationIndicator.ToString() + "\r\n";
            str = str + "RollIndicator:" + cockpit.RollIndicator.ToString() + "\r\n" + "\r\n" + "\r\n";

            if (Me.CustomData == "0")
            {
                str += "00" + "\r\n";
                if (cockpit.RotationIndicator.X > 0)
                {
                    targetAngle.X += 1f;
                }
                else if (cockpit.RotationIndicator.X < 0)
                {
                    targetAngle.X -= 1f;
                }
                
                if (cockpit.RotationIndicator.Y > 0)
                {
                    targetAngle.Y -= 1f;
                }
                else if (cockpit.RotationIndicator.Y < 0)
                {
                    targetAngle.Y += 1f;
                }
                str += targetAngle.X.ToString() + "\r\n";
                str += targetAngle.Y.ToString() + "\r\n";

            }
            else
            {

            }
            Me.GetSurface(0).WriteText(str);
        }

        private void Move(Vector3 move)
        {
            float rotersR_indicator = 0f;
            float rotersL_indicator = 0f;

            if (move.Z > 0)
            {
                rotersR_indicator -= 60;
                rotersL_indicator += 60;
            }
            else if (move.Z < 0)
            {
                rotersR_indicator += 60;
                rotersL_indicator -= 60;
            }
            else
            {
                rotersR_indicator = 0f;
                rotersL_indicator = 0f;
            }

            if (move.X > 0)
            {
                rotersR_indicator -= 40f;
                rotersL_indicator -= 40f;
            }
            else if (move.X < 0)
            {
                rotersR_indicator += 40f;
                rotersL_indicator += 40f;
            }
        }

        public void MorterMoveToAngle(IMyMotorStator motorStator, double TargetAngle, float velocity)
        {
            
        }
    }
}