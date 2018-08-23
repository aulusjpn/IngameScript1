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
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        IMyCockpit cockpit;
        List<IMyTerminalBlock> gyros;
        IMyTextPanel text;

        IMyMotorStator myMotorR1, myMotorR2, myMotorR3, myMotorL1, myMotorL2, myMotorL3;
        DateTime lastMoveTime;

        /// <summary>
        /// 脚部稼動状態　0:前へ移動中 1:後ろへ移動中 2:前で停止 3：後ろで停止
        /// </summary>
        int R1Flg = 0;

        /// <summary>
        /// 脚部稼動状態　0:前へ移動中 1:後ろへ移動中 2:前で停止 3：後ろで停止
        /// </summary>
        int L1Flg = 1;

        /// <summary>
        /// 稼動状態
        /// 今後、格闘実装も香料する 0：固定　1：固定から移動　2：通常稼動
        /// </summary>
        int MovingFlg = 0;


        string str;

        public Program()
        {

            gyros = new List<IMyTerminalBlock>();
            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            GridTerminalSystem.GetBlockGroupWithName("Gyros").GetBlocks(gyros);

            text = GridTerminalSystem.GetBlockWithName("Text panel") as IMyTextPanel;

            myMotorR1 = GridTerminalSystem.GetBlockWithName("Hinge R1") as IMyMotorStator;
            myMotorR2 = GridTerminalSystem.GetBlockWithName("Hinge R2") as IMyMotorStator;
            myMotorR3 = GridTerminalSystem.GetBlockWithName("Hinge R3") as IMyMotorStator;

            myMotorL1 = GridTerminalSystem.GetBlockWithName("Hinge L1") as IMyMotorStator;
            myMotorL2 = GridTerminalSystem.GetBlockWithName("Hinge L2") as IMyMotorStator;
            myMotorL3 = GridTerminalSystem.GetBlockWithName("Hinge L3") as IMyMotorStator;

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            R1Flg = 0;
            L1Flg = 1;
            MovingFlg = 0;
        }

        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.
            str = "";

            GyroControl();
            string param = cockpit.CustomData;
            if (param == "0")
            {
                float vel = 1f;
                MorterMoveToAngle(myMotorR1, -10, vel);
                MorterMoveToAngle(myMotorR2, 20, vel);
                MorterMoveToAngle(myMotorR3, -10, vel);
                MorterMoveToAngle(myMotorL1, 10, vel);
                MorterMoveToAngle(myMotorL2, 20, vel);
                MorterMoveToAngle(myMotorL3, -10, vel); 
            }
            else if (param == "R")
            {
                R1Flg = 0;
                L1Flg = 1;
                HingeThighMoving();
                HingeKnee();
                HingeAnkle();
            }
            else if (param == "L")
            {
                R1Flg = 1;
                L1Flg = 0;
                HingeThighMoving();
                HingeKnee();
                HingeAnkle();
            }
            else if (param == "Run")
            {

            }

            Echo(param);

            text.WritePublicText(str);
        }

        public void Running()
        {
            float velocity = 2f;
            if (R1Flg == 0)
            {
                MorterMoveToAngle(myMotorR1, -80, velocity);
            }
            else if (R1Flg == 1)
            {
                MorterMoveToAngle(myMotorR1, 20, velocity);
            }



            if (L1Flg == 0)
            {
                MorterMoveToAngle(myMotorL1, 80, velocity);
            }
            else if (L1Flg == 1)
            {
                MorterMoveToAngle(myMotorL1, -20, velocity);
            }

            if (R1Flg == 0)
            {
                if (MathHelperD.ToDegrees(myMotorR1.Angle) > -20)
                {
                    MorterMoveToAngle(myMotorR2, 80, velocity * 2);
                }
                else
                {
                    MorterMoveToAngle(myMotorR2, 10, velocity * 2);
                }
            }


            if (L1Flg == 0)
            {
                if (MathHelperD.ToDegrees(myMotorL1.Angle) < 20)
                {
                    MorterMoveToAngle(myMotorL2, 80, velocity * 2);
                }
                else
                {
                    MorterMoveToAngle(myMotorL2, 10, velocity * 2);
                }
            }




        }

        public void GyroControl()
        {
            var vector = cockpit.GetNaturalGravity();

            var cockpitmatrix = cockpit.WorldMatrix;

            Vector3D direction = Vector3D.TransformNormal(vector, Matrix.Transpose(cockpitmatrix));

            float pitch = (float)direction.Z / 5;
            float roll = -((float)direction.X / 5);

            foreach (var item in gyros)
            {
                var gyro = item as IMyGyro;
                gyro.Pitch = pitch;
                gyro.Roll = roll;
            }


        }

        public void HingeThighMoving()
        {
            float velocity = 1f;

            if (R1Flg == 0)
            {
                MorterMoveToAngle(myMotorR1, -30, velocity);
            }
            else if (R1Flg == 1)
            {
                MorterMoveToAngle(myMotorR1, 20, velocity);
            }



            if (L1Flg == 0)
            {
                MorterMoveToAngle(myMotorL1, 30, velocity);
            }
            else if (L1Flg == 1)
            {
                MorterMoveToAngle(myMotorL1, -20, velocity);
            }


            //if (R1Flg == 0 && MathHelperD.ToDegrees(myMotorR1.Angle) < -30)
            //{
            //    R1Flg = 2;
            //}

            //if (R1Flg == 1 && MathHelperD.ToDegrees(myMotorR1.Angle) > 20)
            //{
            //    R1Flg = 3;
            //}

            //if (L1Flg == 0 && MathHelperD.ToDegrees(myMotorL1.Angle) > 30)
            //{
            //    L1Flg = 2;
            //}

            //if (L1Flg == 1 && MathHelperD.ToDegrees(myMotorL1.Angle) < 20)
            //{
            //    L1Flg = 3;
            //}


        }

        public void HingeKnee()
        {
            float velocity = 2f;
            if (R1Flg == 0)
            {
                if (MathHelperD.ToDegrees(myMotorR1.Angle) > -20)
                {
                    MorterMoveToAngle(myMotorR2, 80, velocity*2);
                }
                else
                {
                    MorterMoveToAngle(myMotorR2, 10, velocity*2);
                }
            }
            else
            {
            }

            if (L1Flg == 0)
            {
                if (MathHelperD.ToDegrees(myMotorL1.Angle) < 20)
                {
                    MorterMoveToAngle(myMotorL2, 80, velocity * 2);
                }
                else
                {
                    MorterMoveToAngle(myMotorL2, 10, velocity * 2);
                }
            }
            else
            {
            }
        }

        public void HingeAnkle()
        {
            float velocity = 1f;
            if (R1Flg == 0)
            {
                MorterMoveToAngle(myMotorR3, 20, velocity);
            }
            else if (R1Flg == 1)
            {
                MorterMoveToAngle(myMotorR3, -30, velocity );
            }

            if (L1Flg == 0)
            {
                MorterMoveToAngle(myMotorL3, 20, velocity);
            }
            else if (L1Flg == 1)
            {
                MorterMoveToAngle(myMotorL3, -30, velocity);
            }
        }

        public void MorterMoveToAngle(IMyMotorStator motorStator,double TargetAngle, float velocity)
        {
            var angle = MathHelperD.ToDegrees(motorStator.Angle) - TargetAngle;
            if (Math.Abs(angle) < velocity)
            {
                motorStator.TargetVelocityRad = 0;
            }
            else if (MathHelperD.ToDegrees(motorStator.Angle) > TargetAngle)
            {
                motorStator.TargetVelocityRad = -velocity;
            }
            else if (MathHelperD.ToDegrees(motorStator.Angle) < TargetAngle)
            {
                motorStator.TargetVelocityRad = velocity;
            }
        }


        public void HingeControl()
        {

            
            str += "myMotorR1:" + MathHelperD.ToDegrees(myMotorR1.Angle).ToString() + "   \r\n";
            str += "myMotorR2:" + MathHelperD.ToDegrees(myMotorR2.Angle).ToString() + "   \r\n";
            str += "myMotorR3:" + MathHelperD.ToDegrees(myMotorR3.Angle).ToString() + "   \r\n";
            str += "myMotorL1:" + MathHelperD.ToDegrees(myMotorL1.Angle).ToString() + "   \r\n";
            str += "myMotorL2:" + MathHelperD.ToDegrees(myMotorL2.Angle).ToString() + "   \r\n";
            str += "myMotorL3:" + MathHelperD.ToDegrees(myMotorL3.Angle).ToString() + "   \r\n";




        }


    }
}