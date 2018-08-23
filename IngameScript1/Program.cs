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
   
        //List<IMyTerminalBlock> myBlockGroupRightArm;
        //List<IMyTerminalBlock> myBlockGroupLeftArm;
        //List<IMyTerminalBlock> myBlockGroupRightLegh;
        //List<IMyTerminalBlock> myBlockGroupLeftLegh;

        List<IMyThrust> thrusters;

        List<IMyTerminalBlock> myBlockGroupMain;

        List<IMyGyro> Gyros;

        IMyCockpit cockpit;
        IMyCameraBlock myCamera;
        IMyTextPanel text;
        IMyTerminalBlock me;
        
        string str;

        public Program()
        {
            //myBlockGroupRightArm = new List<IMyTerminalBlock>();
            //myBlockGroupLeftArm = new List<IMyTerminalBlock>();
            //myBlockGroupRightLegh = new List<IMyTerminalBlock>();
            //myBlockGroupLeftLegh = new List<IMyTerminalBlock>();
            Gyros = new List<IMyGyro>();
            thrusters = new List<IMyThrust>();
            myBlockGroupMain = new List<IMyTerminalBlock>();

            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit No Oxygen 3") as IMyCockpit;
            myCamera = GridTerminalSystem.GetBlockWithName("Camera") as IMyCameraBlock;
            GridTerminalSystem.GetBlocksOfType(thrusters);
            GridTerminalSystem.GetBlockGroupWithName("Main Thrusters").GetBlocks(myBlockGroupMain);
            GridTerminalSystem.GetBlocksOfType(Gyros);
            text = GridTerminalSystem.GetBlockWithName("LCD Panel") as IMyTextPanel;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {

            str = "";
            if (Me.CustomData =="0")
            {
                foreach (var th in thrusters)
                {
                    IMyThrust thr = th as IMyThrust;
                    th.ApplyAction("OnOff_On");
                    thr.ThrustOverridePercentage = 0;
                }
                foreach (var item in Gyros)
                {
                    item.GyroOverride = false;
                }
            }
            else
            {
                foreach (var th in thrusters)
                {
                    
                    IMyThrust thr = th as IMyThrust;
                    thrusterControllr(thr);
                }
                gyroControll();
            }
            text.WritePublicText(str);
            Echo(Me.CustomData);

        }


        public void thrusterControllr(IMyThrust thruster)
        {
            var Orientation = cockpit.WorldMatrix.Forward;
            float ang = 10;

            var move = cockpit.MoveIndicator;
            var Rota = cockpit.RotationIndicator;
            var Roll = cockpit.RollIndicator;

            //str = "MoveIndicator:" + cockpit.Name + cockpit.MoveIndicator.ToString() + "\r\n";
            //str = str + "RotationIndicator:" + cockpit.RotationIndicator.ToString() + "\r\n";
            //str = str + "RollIndicator:"  + cockpit.RollIndicator.ToString() + "\r\n" + "\r\n" + "\r\n";

            if (move.Length() == 0 && Rota.Length() == 0 && Roll == 0)
            {
                thruster.ApplyAction("OnOff_On");
                return;
            }


            if (move.Z > 0)
            {
                var angle = Math.Acos(Vector3D.Normalize(thruster.WorldMatrix.Forward).Dot(Vector3D.Normalize(cockpit.WorldMatrix.Up)));
                if (MathHelper.ToDegrees(angle) < ang && MathHelper.ToDegrees(angle) > 0)
                {
                    thruster.ApplyAction("OnOff_On");
                    str = str + MathHelper.ToDegrees(angle).ToString() + "\r\n";
                    thruster.ThrustOverridePercentage = 100;
                    return;
                }
                else if (MathHelper.ToDegrees(angle) > (180 - ang))
                {
                    thruster.ApplyAction("OnOff_Off");
                }

            }
            else if (move.Z < 0)
            {
                //var angle = Math.Acos(thruster.WorldMatrix.Forward.Dot(cockpit.WorldMatrix.Down));
                var angle = Math.Acos(Vector3D.Normalize(thruster.WorldMatrix.Forward).Dot(Vector3D.Normalize(cockpit.WorldMatrix.Down)));
                if (MathHelper.ToDegrees(angle) < ang && MathHelper.ToDegrees(angle) > 0)
                {
                    thruster.ApplyAction("OnOff_On");
                    str = str + MathHelper.ToDegrees(angle).ToString() + "\r\n";
                    thruster.ThrustOverridePercentage = 100;
                    return;
                }
                else if (MathHelper.ToDegrees(angle) > (180 - ang))
                {
                    thruster.ApplyAction("OnOff_Off");
                }

            }

            if (move.X > 0)
            {
                var angle = Math.Acos(Vector3D.Normalize(thruster.WorldMatrix.Forward).Dot(Vector3D.Normalize(cockpit.WorldMatrix.Right)));
                //var angle = Math.Acos(thruster.WorldMatrix.Forward.Dot(cockpit.WorldMatrix.Right));
                if (MathHelper.ToDegrees(angle) < ang && MathHelper.ToDegrees(angle) > 0)
                {
                    thruster.ApplyAction("OnOff_On");
                    str = str + MathHelper.ToDegrees(angle).ToString() + "\r\n";
                    thruster.ThrustOverridePercentage = 100;
                    return;
                }
                else if (MathHelper.ToDegrees(angle) > (180 - ang))
                {
                    thruster.ApplyAction("OnOff_Off");
                }

            }
            else if (move.X < 0)
            {
                var angle = Math.Acos(Vector3D.Normalize(thruster.WorldMatrix.Forward).Dot(Vector3D.Normalize(cockpit.WorldMatrix.Left)));
                //var angle = Math.Acos(thruster.WorldMatrix.Forward.Dot(cockpit.WorldMatrix.Left));
                if (MathHelper.ToDegrees(angle) < ang && MathHelper.ToDegrees(angle) > 0)
                {
                    thruster.ApplyAction("OnOff_On");
                    str = str + MathHelper.ToDegrees(angle).ToString() + "\r\n";
                    thruster.ThrustOverridePercentage = 100;
                    return;
                }
                else if (MathHelper.ToDegrees(angle) > (180 - ang))
                {
                    thruster.ApplyAction("OnOff_Off");
                }

            }



            if (move.Y > 0)
            {
                var angle = Math.Acos(Vector3D.Normalize(thruster.WorldMatrix.Forward).Dot(Vector3D.Normalize(cockpit.WorldMatrix.Backward)));
                //var angle = Math.Acos(thruster.WorldMatrix.Forward.Dot(cockpit.WorldMatrix.Backward));
                if (MathHelper.ToDegrees(angle) < ang && MathHelper.ToDegrees(angle) > 0)
                {
                    thruster.ApplyAction("OnOff_On");
                    str = str + MathHelper.ToDegrees(angle).ToString() + "\r\n";
                    thruster.ThrustOverridePercentage = 100;
                    return;
                }
                else if (MathHelper.ToDegrees(angle) > (180 - ang))
                {
                    thruster.ApplyAction("OnOff_Off");
                }

            }
            else if (move.Y < 0)
            {
                var angle = Math.Acos(Vector3D.Normalize(thruster.WorldMatrix.Forward).Dot(Vector3D.Normalize(cockpit.WorldMatrix.Forward)));
                //var angle = Math.Acos(thruster.WorldMatrix.Forward.Dot(cockpit.WorldMatrix.Forward));
                if (MathHelper.ToDegrees(angle) < ang && MathHelper.ToDegrees(angle) > 0)
                {
                    thruster.ApplyAction("OnOff_On");
                    str = str + MathHelper.ToDegrees(angle).ToString() + "\r\n";
                    thruster.ThrustOverridePercentage = 100;
                    return;
                }
                else if (MathHelper.ToDegrees(angle) > (180 - ang))
                {
                    thruster.ApplyAction("OnOff_On");
                    thruster.ApplyAction("OnOff_Off");
                }

            }

            //thruster.ApplyAction("OnOff_Off");
            thruster.ThrustOverridePercentage = 0;
            //text.WritePublicText(str);
        }

        public void gyroControll()
        {
            var Orientation = cockpit.WorldMatrix.Forward;

            var Rota = cockpit.RotationIndicator;
            var Roll = cockpit.RollIndicator;

            foreach (var gyro in Gyros)
            {
                gyro.GyroOverride = true;
                //str = str + gyro.DisplayName + gyro.Orientation + ":";
                //str = str + "Pitch:" + gyro.Pitch + " / ";
                //str = str + "Yaw:" + gyro.Yaw + " / ";
                //str = str + "Roll:" + gyro.Roll + "\r\n";
                //str = str + Base6Directions.Direction.Forward + "\r\n";
                if (gyro.Orientation.Forward == myCamera.Orientation.Forward && gyro.Orientation.Up == myCamera.Orientation.Up)
                {
                    gyro.Pitch = Rota.X * 60;
                    gyro.Yaw = Rota.Y * 60;
                    gyro.Roll = Roll * 60;
                }
            }

        }

    }
}