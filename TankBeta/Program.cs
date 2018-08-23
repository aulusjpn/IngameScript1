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
        IMyCockpit gunner;
        IMyMotorBase roter1;
        IMyMotorBase roter2;
        IMyTextPanel text;
        List<IMyTerminalBlock> rotersR;
        List<IMyTerminalBlock> rotersL;

        public Program()
        {
            rotersR = new List<IMyTerminalBlock>();
            rotersL = new List<IMyTerminalBlock>();


            gunner = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit No Oxygen") as IMyCockpit;
            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Passenger Seat") as IMyCockpit;
            roter1 = GridTerminalSystem.GetBlockWithName("Rotor Turret") as IMyMotorBase;
            roter2 = GridTerminalSystem.GetBlockWithName("Two-ended Motor") as IMyMotorBase;
            GridTerminalSystem.GetBlockGroupWithName("RotersR").GetBlocks(rotersR);
            GridTerminalSystem.GetBlockGroupWithName("RotersL").GetBlocks(rotersL);
            text = GridTerminalSystem.GetBlockWithName("Text panel") as IMyTextPanel;
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
            Vector3 move = gunner.MoveIndicator;
            Vector2 Rota = gunner.RotationIndicator;
            float Roll = gunner.RollIndicator;
            var str = "";
            var Orientation = cockpit.WorldMatrix.Forward;

            str = "MoveIndicator:" + cockpit.Name + cockpit.MoveIndicator.ToString() + "\r\n";
            str = str + "RotationIndicator:" + cockpit.RotationIndicator.ToString() + "\r\n";
            str = str + "RollIndicator:" + cockpit.RollIndicator.ToString() + "\r\n" + "\r\n" + "\r\n";

            if (Rota.Y > 0)
            {
                roter1.SetValueFloat("Velocity", 3f);
            }
            else if (Rota.Y < 0)
            {
                roter1.SetValueFloat("Velocity", -3f);
            }
            else
            {
                roter1.SetValueFloat("Velocity", -0f);
            }

            if (Rota.X > 0)
            {
                roter2.SetValueFloat("Velocity", -3f);
            }
            else if (Rota.X < 0)
            {
                roter2.SetValueFloat("Velocity", 3f);
            }
            else
            {
                roter2.SetValueFloat("Velocity", 0f);
            }

            Move(move);

            foreach (var item in rotersR)
            {
                str = str + "Roter:" + item.GetValueFloat("Velocity").ToString() + "\r\n";
            }

            text.WritePublicText(str);

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


            foreach (var item in rotersR)
            {
                item.SetValueFloat("Velocity", rotersR_indicator);
            }
            foreach (var item in rotersL)
            {
                item.SetValueFloat("Velocity", rotersL_indicator);
            }
        }

    }
}