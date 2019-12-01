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
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
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
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        IMyCockpit cockpit;
        IMyMotorStator roter;
        IMyMotorStator roterR2;
        IMyMotorStator roterL1;
        IMyMotorStator roterL2;
        IMyTextPanel text;
        Vector2 targetAngle;

        //照準角度
        private float elevationAngle;
        private float azimuthangle;

        public Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            roter = GridTerminalSystem.GetBlockWithName("Rotor") as IMyMotorStator;
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
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {

            Vector3 move = cockpit.MoveIndicator;
            Vector2 Rota = cockpit.RotationIndicator;
            float Roll = cockpit.RollIndicator;

            string str = "";

            str = "MoveIndicator:" + cockpit.Name + cockpit.MoveIndicator.ToString() + "\r\n";
            str = str + "RotationIndicator:" + cockpit.RotationIndicator.ToString() + "\r\n";
            str = str + "RollIndicator:" + cockpit.RollIndicator.ToString() + "\r\n" + "\r\n" + "\r\n";

            string[] datastr = roter.CustomData.Split(',');

            float angle = float.Parse(datastr[1]);

            angle = angle + (cockpit.RotationIndicator.Y / 10);

            roter.CustomData = string.Format("0,100,10", angle.ToString(), datastr[2]);

            Me.GetSurface(0).WriteText(str);
        }
    }
}
