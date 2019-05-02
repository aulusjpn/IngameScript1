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
    partial class Program
    {
        public class CustomData
        {
            private IMyTerminalBlock _tgtBlock;

            public CustomData(IMyTerminalBlock tgtBlock)
            {
                _tgtBlock = tgtBlock;
            }

            public void AddCustomData(string str, int data, bool overWrite = false)
            {
                AddCustomData(str, data.ToString(), overWrite);
            }

            public void AddCustomData(string str, bool data, bool overWrite = false)
            {
                AddCustomData(str, data.ToString(), overWrite);
            }

            public void AddCustomData(string str, string data, bool overWrite = false)
            {
                if (_tgtBlock.CustomData.IndexOf(str.ToString()) == -1)
                {
                    _tgtBlock.CustomData = _tgtBlock.CustomData + str + ":" + data + "\n";
                }
                if (overWrite)
                {
                }
            }
            public string LoadCustomData(string targetStr)
            {
                string[] splstr = { "\n" };
                string[] datas = _tgtBlock.CustomData.Split(splstr, StringSplitOptions.None);

                foreach (var data in datas)
                {
                    if (data.Contains(targetStr.ToString())) return data.Substring(data.IndexOf(":") + 1);
                }
                return null;
            }
        }
    }
}
