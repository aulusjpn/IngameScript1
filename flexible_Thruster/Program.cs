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

        List<IMyTerminalBlock> myBlockGroupMainThruster;

        List<IMyTerminalBlock> myBlockGroupMain;

        IMyCockpit cockpit;
        IMyTextPanel text;

        public Program()
        {
            myBlockGroupMainThruster = new List<IMyTerminalBlock>();
            myBlockGroupMain = new List<IMyTerminalBlock>();

            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit No Oxygen 3") as IMyCockpit;
            text = GridTerminalSystem.GetBlockWithName("LCD Panel") as IMyTextPanel;

            Runtime.UpdateFrequency = UpdateFrequency.Update1;

        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var str = "";
            str = cockpit.MoveIndicator + "\r\n";
            str = str + cockpit.RollIndicator + "\r\n";
            str = str + cockpit.RotationIndicator + "\r\n";
            text.WriteText(str);
        }

        public void thrusterControllr(Vector3 vec)
        {
           
        }

    }
}
