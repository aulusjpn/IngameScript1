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
        public abstract class ArmModel : Part
        {

        }

        public abstract class LegModel : Part
        { }

        public abstract class Part
        {
             public Dictionary<string,MoterModel> Moters { set; get; }

            protected Dictionary<MoterModel, MotorOperationDataEntity> entiityDictionaly { set; get; }
            // public Dictionary<MoterModel,MotorOperationDataEntity> Moters{ get; set; }

        }
    }
}