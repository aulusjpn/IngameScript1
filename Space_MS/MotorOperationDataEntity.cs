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
        public class MotorOperationDataEntity
        {
            private double targetAngleRadian;

            public double GetTargetAngle()
            {
                return MathHelperD.ToDegrees(targetAngleRadian);
            }

            public void SetTargetAngle(double value)
            {
                targetAngleRadian =  MathHelperD.ToRadians(value);
            }

            public double GetTargetAngleWithRadian()
            {
                return (targetAngleRadian);
            }

            public void SetTargetAngleWithRadian(double value)
            {
                targetAngleRadian = value;            }

            //private double currentAngleRadian;

            //public double GetCurrentTargetAngle()
            //{
            //    return MathHelperD.ToDegrees(currentAngleRadian);
            //}

            //public void SetCurrentTargetAngle(double value)
            //{
            //    currentAngleRadian = MathHelperD.ToRadians(value);
            //}


            private float velocity;

            public float GetVelocity()
            {
                return velocity;
            }

            public void SetVelocity(float value)
            {
                velocity = value;
            }

            private bool reverce;

            public bool GetReverce()
            {
                return reverce;
            }

            public void SetReverce(bool value)
            {
                reverce = value;
            }

            public MotorOperationDataEntity(double targetAngle,float targetVelocity,bool reverce)
            {
                this.SetTargetAngle(targetAngle);
                this.SetVelocity(targetVelocity);
                this.SetReverce(reverce);
            }

        }
    }
}
