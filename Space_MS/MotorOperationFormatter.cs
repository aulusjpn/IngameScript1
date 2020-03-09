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
        public static class MotorOperationFormatter
        {

            public static MotorOperationDataEntity setToDataEntityFromValue(MotorOperationDataEntity dataEntity, float? targetAngle = null, float? velocity = null, bool? reverce = null)
            {
                if (targetAngle != null) dataEntity.SetTargetAngle((float)targetAngle);
                if (velocity != null) dataEntity.SetVelocity((float)velocity);
                if (reverce != null) dataEntity.SetReverce((bool)reverce);

                return dataEntity;
            }


            public static void setDataEntityToMotorCustomData(MotorOperationDataEntity dataEntity, ref IMyMotorStator motorStator)
            {
                string text = "";

                text += "Angle:" + dataEntity.GetTargetAngle().ToString();
                text.AddNewLine();

                text += "Velocity:" + dataEntity.GetVelocity().ToString();
                text.AddNewLine();
                text += "Reverce:" + dataEntity.GetReverce().ToString();
                motorStator.CustomData = text;

            }


            public static MotorOperationDataEntity getDataEntityFromMoterCustomData(IMyMotorStator motor, MotorOperationDataEntity dataEntity)
            {
                string text = motor.CustomData;

                var arr = text.Split(Environment.NewLine.ToArray(), StringSplitOptions.None);

                foreach (var item in arr)
                {
                    var array = item.Split(':');

                    switch (array[0])
                    {
                        case "Angle":
                            dataEntity.SetTargetAngle(float.Parse(array[1]));
                            break;
                        case "Velocity":
                            dataEntity.SetVelocity(float.Parse(array[1]));
                            break;
                        case "Reverce":
                            dataEntity.SetReverce(bool.Parse(array[1]));
                            break;
                        default:
                            break;
                    }
                }

                return dataEntity;
            }
        }
    }
}
