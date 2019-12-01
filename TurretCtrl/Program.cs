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

        IMyCockpit cockpit;
        IMyMotorStator roterR1;
        IMyMotorStator roterR2;
        private List<motorBase> motors = new List<motorBase>();
        Vector2 targetAngle;

        public Program()
        {
            cockpit = GridTerminalSystem.GetBlockWithName("Control Stations") as IMyCockpit;
            roterR1 = GridTerminalSystem.GetBlockWithName("LargeAdvancedRingStator 2") as IMyMotorStator;
            roterR2 = GridTerminalSystem.GetBlockWithName("Two-ended Motor") as IMyMotorStator;
            // roterR3 = GridTerminalSystem.GetBlockWithName("Rotor3") as IMyMotorStator;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            motors.Add(new motorBase(roterR1, false, 0, 0));
            motors.Add(new motorBase(roterR2, false, 0, 0));
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
            Vector3D Orientation = cockpit.WorldMatrix.Forward;

            str = "MoveIndicator:" + cockpit.Name + cockpit.MoveIndicator.ToString() + "\r\n";
            str = str + "RotationIndicator:" + cockpit.RotationIndicator.ToString() + "\r\n";
            str = str + "RollIndicator:" + cockpit.RollIndicator.ToString() + "\r\n" + "\r\n" + "\r\n";

            motorMove(roterR1, (cockpit.RotationIndicator.Y / 10));
            motorMove(roterR2, (cockpit.RotationIndicator.X / 10));
            foreach (var item in motors)
            {            
                item.Main();
            }

            Me.GetSurface(0).WriteText(str);
        }


        private void motorMove(IMyMotorStator motorStator,float indicate)
        {

            string[] datastr = motorStator.CustomData.Split(',');

            float angle = float.Parse(datastr[1]);

            angle = angle + indicate;

            if (angle < 0)
            {
                angle = 0;
            }
            else if (angle > 360)
            {
                angle = 360;
            }

            motorStator.CustomData = string.Format("{0},{1},{2}", datastr[0], angle.ToString(), datastr[2]);
        }

    }
}
