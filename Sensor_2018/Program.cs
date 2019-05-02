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

        IMySensorBlock mySensor;

        List<MyDetectedEntityInfo> detectedEntityInfos = new List<MyDetectedEntityInfo>();
        long NowTargetEntityInfoID = new long();
        List<long> Settledlist = new List<long>();
        private string st = "";
        private IMyTextPanel text;
        static string ChageCmd = "Next";
        static string StaticCmd = "Static";
        bool getTarget = false;

        public Program()
        {
            text = GridTerminalSystem.GetBlockWithName("Text panel Sensor") as IMyTextPanel;
            mySensor = GridTerminalSystem.GetBlockWithName("Sensor") as IMySensorBlock;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            mySensor.DetectedEntities(detectedEntityInfos);

            text.WritePublicText("");
            foreach (var item in detectedEntityInfos)
            {
                WriteText(item.Name + ":" + item.Position.ToString()+ "\n");
            }

            if (detectedEntityInfos.Count == 0 || Me.CustomData == StaticCmd)
            {
                NowTargetEntityInfoID = 0;
                Settledlist = new List<long>();
                getTarget = false;
                var vector = mySensor.GetPosition() + (mySensor.WorldMatrix.Forward * 500);
                mySensor.CustomData = vector.X + "," + vector.Y + "," + vector.Z;
            }
            else
            {
                if (Me.CustomData == ChageCmd)
                {
                    Me.CustomData = "";

                    foreach (var item in detectedEntityInfos)
                    {                        
                        if (!Settledlist.Contains(item.EntityId))
                        {
                            NowTargetEntityInfoID = item.EntityId;

                            Settledlist.Add(item.EntityId);
                            mySensor.CustomData = item.Position.X + "," + item.Position.Y + "," + item.Position.Z;
                            return;
                        }
                    }

                    NowTargetEntityInfoID = new long();
                    Settledlist = new List<long>();
                    getTarget = false;
                    var vector = mySensor.GetPosition() + (mySensor.WorldMatrix.Forward * 500);
                    mySensor.CustomData = vector.X + "," + vector.Y + "," + vector.Z;
                    return;
                }

                foreach (var item in detectedEntityInfos)
                {
                    if (item.EntityId == NowTargetEntityInfoID)
                    {
                        mySensor.CustomData = item.Position.X + "," + item.Position.Y + "," + item.Position.Z;
                        return;
                    }
                }

                NowTargetEntityInfoID = new long();
                Settledlist = new List<long>();
                getTarget = false;
                var vectorDi = mySensor.GetPosition() + (mySensor.WorldMatrix.Forward * 500);
                mySensor.CustomData = vectorDi.X + "," + vectorDi.Y + "," + vectorDi.Z;
                return;

            }


        }
        public void WriteText(string str)
        {
            text.WritePublicText(text.GetPublicText() + str);
        }
    }
}