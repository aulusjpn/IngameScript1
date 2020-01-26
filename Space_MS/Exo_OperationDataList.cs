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
    partial class Program
    {
        public static class Exo_OperationDataList
        {

            /// <summary>
            /// インデックス
            /// 1:右足
            /// 2:左足
            /// 3:右手
            /// 4:左手
            /// </summary>
            /// <returns></returns>
            public static List<List<MotorOperationDataEntity>> walk_1 = new List<List<MotorOperationDataEntity>>()
            {
               new List<MotorOperationDataEntity>() 
                    {new MotorOperationDataEntity(40,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(20,10,true),
                    new MotorOperationDataEntity(20,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(40,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(20,10,true),
                    new MotorOperationDataEntity(20,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)}
            };

            public static List<List<MotorOperationDataEntity>> walk_2 = new List<List<MotorOperationDataEntity>>()
            {
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(40,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(40,10,true),
                    new MotorOperationDataEntity(40,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(40,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(40,10,true),
                    new MotorOperationDataEntity(40,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)}
            };
            public static List<List<MotorOperationDataEntity>> walk_3 = new List<List<MotorOperationDataEntity>>()
            {
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(-40,10,true),
                    new MotorOperationDataEntity(-0,10,true),
                    new MotorOperationDataEntity(-20,10,true),
                    new MotorOperationDataEntity(-20,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(-40,10,true),
                    new MotorOperationDataEntity(-0,10,true),
                    new MotorOperationDataEntity(-20,10,true),
                    new MotorOperationDataEntity(-20,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)}
            };
            public static List<List<MotorOperationDataEntity>> walk_4 = new List<List<MotorOperationDataEntity>>()
            {
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(-40,10,true),
                    new MotorOperationDataEntity(-0,10,true),
                    new MotorOperationDataEntity(-40,10,true),
                    new MotorOperationDataEntity(-40,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(-40,10,true),
                    new MotorOperationDataEntity(-0,10,true),
                    new MotorOperationDataEntity(-40,10,true),
                    new MotorOperationDataEntity(-40,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)}
            };
            public static List<List<MotorOperationDataEntity>> walk_5 = new List<List<MotorOperationDataEntity>>()
            {
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)}
            };

            public static List<List<MotorOperationDataEntity>> stble = new List<List<MotorOperationDataEntity>>()
            {
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)},
               new List<MotorOperationDataEntity>()
                    {new MotorOperationDataEntity(0,10,true),
                    new MotorOperationDataEntity(-60,10,false),
                    new MotorOperationDataEntity(0,10,true)}
            };

        }
    }
}
