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
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts

        private IMyCockpit cockpit;
        private IMyMotorStator roterR1;
        private IMyMotorStator roterR2;
        private IMyMotorStator roterR3;
        private List<motorBase> motors = new List<motorBase>();

        public Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            roterR1 = GridTerminalSystem.GetBlockWithName("Rotor") as IMyMotorStator;
           // roterR2 = GridTerminalSystem.GetBlockWithName("Rotor2") as IMyMotorStator;
           // roterR3 = GridTerminalSystem.GetBlockWithName("Rotor3") as IMyMotorStator;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            motors.Add(new motorBase(roterR1,false,0,0));
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            foreach (var item in motors)
            {
                item.Main();
           
            }

            string[] datastr = roterR1.CustomData.Split(',');

            float angle = float.Parse(datastr[1]);

            angle = angle + (cockpit.RotationIndicator.Y / 10);

            if (angle < 0)
            {
                angle = 0;
            }
            else if (angle > 360)
            {
                angle = 360;
            }

            roterR1.CustomData = string.Format("{0},{1},{2}", datastr[0],angle.ToString(), datastr[2]);

        }
    }
}
