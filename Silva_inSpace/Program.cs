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
    /// <summary>
    /// メイン
    /// </summary>
    partial class Program : MyGridProgram
    {
        private IMyCockpit cockpit;
        private IMyMotorStator roterR1;
        private IMyMotorStator roterR2;
        private IMyMotorStator roterR3;
        private IMyMotorStator roterL1;
        private IMyMotorStator roterL2;
        private IMyMotorStator roterL3;

        private IMyMotorStator ArmR1;
        private IMyMotorStator ArmR2;
        private IMyMotorStator ArmR3;
        private IMyMotorStator ArmR4;
        private IMyMotorStator ArmL1;
        private IMyMotorStator ArmL2;
        private IMyMotorStator ArmL3;
        private IMyMotorStator ArmL4;
        public IMySensorBlock mySensor;
        double lastYaw;
        double lastRoll;

        LegBase buff1;
        LegBase buff2;
        ArmBase Arm1;
        ArmBase Arm2;
        private string st = "";
        private IMyTextPanel text;
        private Vector3D targetVector;
        private DateTime nowTime;
        private DateTime befTime;
        private List<double> angleList = new List<double>() { 0, 0, 0, 0 };
        List<IMyTerminalBlock> gyros = new List<IMyTerminalBlock>();
        private OperationBase DriveEntity;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Program()
        {

            cockpit = GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit") as IMyCockpit;
            GridTerminalSystem.GetBlockGroupWithName("Gyros").GetBlocks(gyros);
            roterR1 = GridTerminalSystem.GetBlockWithName("Hinge R1") as IMyMotorStator;
            roterR2 = GridTerminalSystem.GetBlockWithName("Hinge R2") as IMyMotorStator;
            roterR3 = GridTerminalSystem.GetBlockWithName("Hinge R3") as IMyMotorStator;
            roterL1 = GridTerminalSystem.GetBlockWithName("Hinge L1") as IMyMotorStator;
            roterL2 = GridTerminalSystem.GetBlockWithName("Hinge L2") as IMyMotorStator;
            roterL3 = GridTerminalSystem.GetBlockWithName("Hinge L3") as IMyMotorStator;

            ArmR1 = GridTerminalSystem.GetBlockWithName("Rotor R") as IMyMotorStator;
            ArmR2 = GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge R1") as IMyMotorStator;
            ArmL1 = GridTerminalSystem.GetBlockWithName("Rotor L") as IMyMotorStator;
            ArmL2 = GridTerminalSystem.GetBlockWithName("Small Conveyor Hinge L1") as IMyMotorStator;

            mySensor = GridTerminalSystem.GetBlockWithName("Sensor") as IMySensorBlock;

            text = GridTerminalSystem.GetBlockWithName("Text panel") as IMyTextPanel;
            befTime = DateTime.UtcNow;

            
            buff1 = new LegBase(roterR1, true, roterR2, false, roterR3, true);
           
            buff2 = new LegBase(roterL1, false, roterL2, false, roterL3, true);

            Arm1 = new ArmBase(ArmR1, true, ArmR2, false, ArmR2, false);

            Arm2 = new ArmBase(ArmL1, false, ArmL2, false, ArmL2, false);
            DriveEntity = new HaltClass(buff1, buff2, Arm1, Arm2);
            st = "Walk";
            Me.CustomData = "Walk";
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            
            nowTime = DateTime.UtcNow;
            befTime = DateTime.UtcNow;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (Me.CustomData == "Walk" && st != "Walk")
            {
                if (DriveEntity.ToString() != "Program+WalkClass")
                {
                    DriveEntity = new WalkClass(buff1, buff2, Arm1, Arm2);
                    DriveEntity.LLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                    DriveEntity.LLeg.myMotorThigh.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[0] + "," + "10";
                    DriveEntity.LLeg.myMotorKnee.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[1] + "," + "10";
                    DriveEntity.LLeg.myMotorAnkle.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[2] + "," + "10";
                    DriveEntity.RLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                    DriveEntity.RLeg.myMotorThigh.MyMotor.CustomData = "1," + DriveEntity.TargetListBackward[0] + "," + "10";
                    DriveEntity.RLeg.myMotorKnee.MyMotor.CustomData = "1," + DriveEntity.TargetListBackward[1] + "," + "10";
                    DriveEntity.RLeg.myMotorAnkle.MyMotor.CustomData = "1," + DriveEntity.TargetListBackward[2] + "," + "10";
                    st = "Walk";
                }

            }
            else if (Me.CustomData == "Stational" && st != "Stational")
            {
                if (DriveEntity.ToString() != "Program+HaltClass")
                {
                    DriveEntity = new HaltClass(buff1, buff2, Arm1, Arm2);

                    DriveEntity.RLeg.myMotorThigh.MyMotor.CustomData = "1," + DriveEntity.TargetListForward[0] + "," + "10";
                    DriveEntity.RLeg.myMotorKnee.MyMotor.CustomData = "1," + DriveEntity.TargetListForward[1] + "," + "10";
                    DriveEntity.RLeg.myMotorAnkle.MyMotor.CustomData = "1," + DriveEntity.TargetListForward[2] + "," + "10";
                    DriveEntity.LLeg.myMotorThigh.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[0] + "," + "10";
                    DriveEntity.LLeg.myMotorKnee.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[1] + "," + "10";
                    DriveEntity.LLeg.myMotorAnkle.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[2] + "," + "10";
                    st = "Stational";
                }

            }
            else if (Me.CustomData == "Ready" && st != "Ready")
            {
                if (DriveEntity.ToString() != "Program+ReadyClass")
                {
                    DriveEntity = new ReadyClass(buff1, buff2, Arm1, Arm2);

                    st = "Ready";
                }
            }
            else if (Me.CustomData == "ShutDown" && st != "ShutDown")
            {
                if (DriveEntity.ToString() != "Program+ShutDown")
                {
                    DriveEntity = new ShutDown(buff1, buff2, Arm1, Arm2);

                    st = "ShutDown";
                }
            }
            else if (Me.CustomData == "Run" && st != "Run")
            {
                if (DriveEntity.ToString() != "Program+RunClass")
                {
                    DriveEntity = new RunClass(buff1, buff2, Arm1, Arm2);

                    DriveEntity.LLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                    DriveEntity.LLeg.myMotorThigh.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[0] + "," + "10";
                    DriveEntity.LLeg.myMotorKnee.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[1] + "," + "10";
                    DriveEntity.LLeg.myMotorAnkle.MyMotor.CustomData = "0," + DriveEntity.TargetListForward[2] + "," + "10";
                    DriveEntity.RLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                    DriveEntity.RLeg.myMotorThigh.MyMotor.CustomData = "1," + DriveEntity.TargetListBackward[0] + "," + "10";
                    DriveEntity.RLeg.myMotorKnee.MyMotor.CustomData = "1," + DriveEntity.TargetListBackward[1] + "," + "10";
                    DriveEntity.RLeg.myMotorAnkle.MyMotor.CustomData = "1," + DriveEntity.TargetListBackward[2] + "," + "10";
                    st = "Run";
                }
            }
            else if (Me.CustomData == "KneeFire" && st != "KneeFire")
            {
                if (DriveEntity.ToString() != "Program+HaltClass" || st != "KneeFire")
                {
                    DriveEntity = new HaltClass(buff1, buff2, Arm1, Arm2);

                    DriveEntity.LLeg.LegStatus = Utilty.PartsMoveEnum.Move_forward;
                    DriveEntity.LLeg.myMotorThigh.MyMotor.CustomData = "0,0,10";
                    DriveEntity.LLeg.myMotorKnee.MyMotor.CustomData = "0,80,10";
                    DriveEntity.LLeg.myMotorAnkle.MyMotor.CustomData = "0,-30,10";
                    DriveEntity.RLeg.LegStatus = Utilty.PartsMoveEnum.Move_Backward;
                    DriveEntity.RLeg.myMotorThigh.MyMotor.CustomData = "0,110,10";
                    DriveEntity.RLeg.myMotorKnee.MyMotor.CustomData = "0,80,10";
                    DriveEntity.RLeg.myMotorAnkle.MyMotor.CustomData = "0,-10,10";
                    st = "KneeFire";
                }
            }

            Vector3 move = cockpit.MoveIndicator;
            Vector2 Rota = cockpit.RotationIndicator;
            float Roll = cockpit.RollIndicator;
            var str = "";
            var Orientation = cockpit.WorldMatrix.Forward;

            if (move.Z < 0)
            {
                //str = str + "Drive Forword";
                DriveEntity.ctrlStatus = Utilty.StatusEnum.Forword;
            }
            else if (move.Z > 0)
            {
                //str = str + "Drive Back";
                DriveEntity.ctrlStatus = Utilty.StatusEnum.Back;
            }
            else
            {
                //str = str + "Drive Halt";
                DriveEntity.ctrlStatus = Utilty.StatusEnum.Halt;
            }



            //str = str + "Drive Before";
            DriveEntity.Drive();
            if (Me.CustomData == "Stational")
            {
               
            }
            else if (Me.CustomData != "ShutDown")
            {
                // DriveEntity.armTarget(cockpit);
               // DriveEntity.AimTarget_sensor(mySensor);
            }
            //str = str + "Drive End \n";
            text.WritePublicText(str);

            Log();


            nowTime = DateTime.UtcNow;

           // GyroControl();

            befTime = nowTime;
        }

        public void Log()
        {
            WriteText("MOVE Type -> "+DriveEntity.ToString() + " - " + DriveEntity.ctrlStatus.ToString() + "\n");
            WriteText("Right Knee : " + DriveEntity.LLeg.myMotorKnee.TargetAngle + " - " + DriveEntity.LLeg.myMotorKnee.Angle + "\n");
            WriteText("Right Thigh : " + DriveEntity.LLeg.myMotorThigh.TargetAngle + " - " + DriveEntity.LLeg.myMotorThigh.Angle + "\n");
            WriteText("Right Ankle : " + DriveEntity.LLeg.myMotorAnkle.TargetAngle + " - " + DriveEntity.LLeg.myMotorAnkle.Angle + "\n");

            WriteText("Left Knee : " + DriveEntity.RLeg.myMotorKnee.TargetAngle + " - " + DriveEntity.RLeg.myMotorKnee.Angle + "\n");
            WriteText("Left Thigh : " + DriveEntity.RLeg.myMotorThigh.TargetAngle + " - " + DriveEntity.RLeg.myMotorThigh.Angle + "\n");
            WriteText("Left Ankle : " + DriveEntity.RLeg.myMotorAnkle.TargetAngle + " - " + DriveEntity.RLeg.myMotorAnkle.Angle + "\n");

            WriteText("RArm : " + DriveEntity.RArm.myMotor1.TargetAngle + " - " + DriveEntity.RArm.myMotor2.Angle + "\n");

            foreach (var item in gyros)
            {
                var gyro = item as IMyGyro;
                WriteText("Gyro : " + gyro.Pitch.ToString() + " -\n");
            }

        }


        public void GyroControl()
        {
            //var vector = cockpit.GetNaturalGravity();

            //var cockpitmatrix = cockpit.WorldMatrix;

            //Vector3D direction = Vector3D.TransformNormal(vector, Matrix.Transpose(cockpitmatrix));

            //float pitch = (float)direction.Z / 2;
            //float roll = -((float)direction.X / 2);
            //foreach (var item in gyros)
            //{
            //    var gyro = item as IMyGyro;

            //    gyro.Pitch = pitch > 0 ? (float)Math.Pow(pitch, 2) : -(float)Math.Pow(pitch, 2);
            //    gyro.Roll = roll > 0 ? (float)Math.Pow(roll, 2) : -(float)Math.Pow(roll, 2);
            //}

            var vector = cockpit.GetNaturalGravity();

            var cockpitmatrix = cockpit.WorldMatrix;

            Vector3D direction = Vector3D.TransformNormal(vector, Matrix.Transpose(cockpitmatrix));

            float pitch = (float)direction.Z / 2;
            float roll = -((float)direction.X / 2);
            foreach (var item in gyros)
            {
                var gyro = item as IMyGyro;
                gyro.Pitch = (float)pitch;
                //gyro.Roll = (float)roll;
                //gyro.Pitch = pitch > 0 ? (float)Math.Pow(pitch, 2) : -(float)Math.Pow(pitch, 2);
                //gyro.Roll = roll > 0 ? (float)Math.Pow(roll, 2) : -(float)Math.Pow(roll, 2);
            }
        }

        double ToRPM(double rot, double lastRot)
        {
            double deltaRot = lastRot - rot;
            deltaRot /= (nowTime - befTime).TotalSeconds;
            return (rot - (deltaRot)) / (Math.PI * 2.0) * 60.0;
        }

        public void WriteText(string str)
        {
            text.WritePublicText(text.GetPublicText() + str);
        }


    }
}