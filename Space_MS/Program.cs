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

        bool ini = false;

        public Program()
        {
            myCockpit = (IMyCockpit)GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit");
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            Me.GetSurface(0).ContentType = ContentType.SCRIPT;


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

        public void init()
        {
 
            var rLeg = new Exo_LegModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RLeg1"),
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RLeg2"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RLeg3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RLeg4"));


            var lLeg = new Exo_LegModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LLeg1"),
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LLeg2"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LLeg3"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LLeg4"));


            var rArm = new Exo_ArmModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RArm1"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RArm2"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor RArm3"));


            var lArm = new Exo_ArmModel(
                (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LArm1"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LArm2"),
            (IMyMotorStator)GridTerminalSystem.GetBlockWithName("Rotor LArm3"));

            Operation = new Exo_MovingOperationSrv(rLeg, lLeg, rArm, lArm);
        }

        public void Main(string argument, UpdateType updateSource)
        {

            try
            {
                if (!ini)
                {
                    init();
                    ini = true;
                }

                var surface = Me.GetSurface(0);

                // Operation.armTarget(myCockpit);
                Operation.DriveLeg(myCockpit);

            }
            catch (Exception e)
            {

                myCockpit.CustomData = e.StackTrace.ToString();
            }


        }
    }
}