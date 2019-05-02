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

        // Welcome to the Formations Script. Refer to the steam workshop page for installation instructions and questions.

        private bool IsDirector = true;// Set to either true or false depending if this ship is a director or not.
        private string Channel = "DefaultChannel";// Channel for this unit. Match the channel up with other subordinates.

        // Do not have 2+ directors on the same channel.
        private string DefaultForm = "F.Line";// Default Formation for when the script runs after "Remember and Exit".

        private string DefaultCommand = "C.Follow";// Default Command for when the script runs after "Remember and Exit".

        //Director Config - Change these if this ship is a director.
        private double Dist = 200; //Scaling factor for formations, base distance between points.

        private int DroneCount = 12;//(AS DIRECTOR) Set to however many drones you want following.

        //Subordinate Config - Change these if this ship is a Subordinate.
        private bool AllowAttack = true; //Allow this subordinate to break formation to attack an enemy. Must have AI controlled turret.

        private int DroneNum = 1; // Position number for this Subordinate; Use #1-X; Do not repeat.
        private double ControlFactor = 10; //Distance in meters to target point for autopilot to shut off. Generally, use half the length of your ship.
                                           // Small ship blocks are 0.5M, large blocks are 2M.

        // Routine config
        private long GatherInterval = 120; // How many times the script runs before it gathers blocks again.
                                           // If ship is constantly getting new blocks then lower, not needed to change really.

        private bool PreventFurtherRuns = false;// Turn on for debugging purposes. Essentially skips exceptions and retrys.
                                                //bool ReportLog = true; //No log in formations script

        private string strTriggerTimerTag = "FleetCommand"; //

        //Do not touch below this line --------------------------------------------

        //Begin ship data
        private IMyShipController MainController;
        private List<IMyRemoteControl> Remotes = new List<IMyRemoteControl>();

        private List<IMyRadioAntenna> Antennae = new List<IMyRadioAntenna>();
        private List<IMyLargeTurretBase> Weapons = new List<IMyLargeTurretBase>();
        private List<MyWaypointInfo> Waypoints = new List<MyWaypointInfo>();
        private List<IMyLandingGear> Gears = new List<IMyLandingGear>();

        private bool CommandOverride = false;// Not used yet.
        private Vector3D GblPosition;
        private Vector3D LastPosition;
        private Vector3D RandVector;
        private Vector3D DirVec;
        private Vector3D Target;

        private Vector3D RefPosition = new Vector3D();

        private string Form = "";
        private string Command = "";
        private string[] PCS;

        private long GatherCounter = 0;
        private bool Init = false;


        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set RuntimeInfo.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
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

        public void Main(string argument, UpdateType updateSource)
        {
            try
            {
                if (!Init)
                {
                    Echo("Initializing:: ...");

                    if (IsDirector)
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.Update10;
                    }
                    else
                    {
                        Runtime.UpdateFrequency = UpdateFrequency.None;
                    }

                    // Load Custom Data
                    if (Me.CustomData != null)
                    {
                        Echo("Loading Custom Data...");

                        CustomData _CstmData = new CustomData(Me);
                        string _ReadValue = null;

                        // Common 共通

                        _ReadValue = null;
                        _ReadValue = _CstmData.LoadCustomData("Channel");
                        if (_ReadValue != null)
                        {
                            Channel = _ReadValue;
                        }

                        _ReadValue = null;
                        _ReadValue = _CstmData.LoadCustomData("IsDirector");
                        if (_ReadValue != null)
                        {
                            IsDirector = (_ReadValue == "true");
                        }

                        _ReadValue = null;
                        _ReadValue = _CstmData.LoadCustomData("ControlFactor");
                        if (_ReadValue != null)
                        {
                            double _dCFactor = 0;
                            if (double.TryParse(_ReadValue, out _dCFactor))
                            {
                                ControlFactor = _dCFactor;
                            }
                        }

                        if (IsDirector)
                        {
                            // Director 旗艦

                            _ReadValue = null;
                            _ReadValue = _CstmData.LoadCustomData("Dist");
                            if (_ReadValue != null)
                            {
                                double _dDist = 0;
                                if (double.TryParse(_ReadValue, out _dDist))
                                {
                                    Dist = _dDist;
                                }
                            }

                        }
                        else
                        {
                            // Drone ドローン

                            _ReadValue = null;
                            _ReadValue = _CstmData.LoadCustomData("DroneNum");
                            if (_ReadValue != null)
                            {
                                int _iDroneNum = 0;
                                if (int.TryParse(_ReadValue, out _iDroneNum))
                                {
                                    DroneNum = _iDroneNum;
                                }
                            }

                            _ReadValue = null;
                            _ReadValue = _CstmData.LoadCustomData("AllowAttack");
                            if (_ReadValue != null)
                            {
                                IsDirector = (_ReadValue == "true");
                            }
                        }
                    }

                    Echo("Initialized.");
                    Init = true;
                }

                if (GatherCounter % GatherInterval == 0)
                {
                    if (!ClearAndGather())
                    {
                        return;
                    }
                    GatherCounter = 0;
                }
                GatherCounter++;

                if (IsDirector)
                {
                    if (!argument.Contains(":"))
                    {
                        string Argument = argument;
                        Director(Argument);
                    }
                }
                else
                {
                    if (argument.Contains(":"))
                    {
                        Recieve(argument);
                    }
                }
            }
            catch (Exception EX)
            {
                Echo("Exception at site: " + EX.TargetSite.ToString());
                Echo(EX.Message);

                //Log Report
                //if (ReportLog)
                //{
                //	Echo(Log);
                //}
                if (PreventFurtherRuns)
                {
                    throw (EX);
                }
                else
                {
                    Echo(EX.Message);
                    ClearAndGather();
                }
            }
        }

        //Begin Methods
        private bool ClearAndGather()
        {
            MainController = null;
            Remotes.Clear();

            Antennae.Clear();
            Weapons.Clear();
            Waypoints.Clear();
            Gears.Clear();

            GridTerminalSystem.GetBlocksOfType(Antennae);
            if (IsDirector)
            {
                GridTerminalSystem.GetBlocksOfType(Remotes, x => x.CubeGrid == Me.CubeGrid);
                //TODO タグ付け対応
                if (Remotes.Count == 0)
                {
                    Echo("No Remote Controller Found.");
                    return false;
                }
                MainController = Remotes[0];

                GblPosition = Vector3D.Round(MainController.GetPosition());
                GblPosition = RefPosition + GblPosition;
            }
            else
            {
                GridTerminalSystem.GetBlocksOfType(Remotes, x => x.CubeGrid == Me.CubeGrid);
                if (Remotes.Count == 0)
                {
                    Echo("No Remote Controller Found.");
                    return false;
                }
                GblPosition = Vector3D.Round(Remotes[0].GetPosition());
                Remotes[0].GetWaypointInfo(Waypoints);
                GridTerminalSystem.GetBlocksOfType(Weapons, x => x.CubeGrid == Me.CubeGrid);
                GridTerminalSystem.GetBlocksOfType(Gears, x => x.CubeGrid == Me.CubeGrid);
            }

            return true;
        }

        //private void Main(string Package)
        //{
        //    try
        //    {
        //        if (!Init)
        //        {
        //            Echo("Initializing:: ...");

        //            if (IsDirector)
        //            {
        //                Runtime.UpdateFrequency = UpdateFrequency.Update10;
        //            }
        //            else
        //            {
        //                Runtime.UpdateFrequency = UpdateFrequency.None;
        //            }

        //            // Load Custom Data
        //            if (Me.CustomData != null)
        //            {
        //                Echo("Loading Custom Data...");

        //                CustomData _CstmData = new CustomData(Me);
        //                string _ReadValue = null;

        //                // Common 共通

        //                _ReadValue = null;
        //                _ReadValue = _CstmData.LoadCustomData("Channel");
        //                if (_ReadValue != null)
        //                {
        //                    Channel = _ReadValue;
        //                }

        //                _ReadValue = null;
        //                _ReadValue = _CstmData.LoadCustomData("IsDirector");
        //                if (_ReadValue != null)
        //                {
        //                    IsDirector = (_ReadValue == "true");
        //                }

        //                _ReadValue = null;
        //                _ReadValue = _CstmData.LoadCustomData("ControlFactor");
        //                if (_ReadValue != null)
        //                {
        //                    double _dCFactor = 0;
        //                    if (double.TryParse(_ReadValue, out _dCFactor))
        //                    {
        //                        ControlFactor = _dCFactor;
        //                    }
        //                }

        //                if (IsDirector)
        //                {
        //                    // Director 旗艦

        //                    _ReadValue = null;
        //                    _ReadValue = _CstmData.LoadCustomData("Dist");
        //                    if (_ReadValue != null)
        //                    {
        //                        double _dDist = 0;
        //                        if (double.TryParse(_ReadValue, out _dDist))
        //                        {
        //                            Dist = _dDist;
        //                        }
        //                    }

        //                }
        //                else
        //                {
        //                    // Drone ドローン

        //                    _ReadValue = null;
        //                    _ReadValue = _CstmData.LoadCustomData("DroneNum");
        //                    if (_ReadValue != null)
        //                    {
        //                        int _iDroneNum = 0;
        //                        if (int.TryParse(_ReadValue, out _iDroneNum))
        //                        {
        //                            DroneNum = _iDroneNum;
        //                        }
        //                    }

        //                    _ReadValue = null;
        //                    _ReadValue = _CstmData.LoadCustomData("AllowAttack");
        //                    if (_ReadValue != null)
        //                    {
        //                        IsDirector = (_ReadValue == "true");
        //                    }
        //                }
        //            }

        //            Echo("Initialized.");
        //            Init = true;
        //        }

        //        if (GatherCounter % GatherInterval == 0)
        //        {
        //            if (!ClearAndGather())
        //            {
        //                return;
        //            }
        //            GatherCounter = 0;
        //        }
        //        GatherCounter++;

        //        if (IsDirector)
        //        {
        //            if (!Package.Contains(":"))
        //            {
        //                string Argument = Package;
        //                Director(Argument);
        //            }
        //        }
        //        else
        //        {
        //            if (Package.Contains(":"))
        //            {
        //                Recieve(Package);
        //            }
        //        }
        //    }
        //    catch (Exception EX)
        //    {
        //        Echo("Exception at site: " + EX.TargetSite.ToString());
        //        Echo(EX.Message);

        //        //Log Report
        //        //if (ReportLog)
        //        //{
        //        //	Echo(Log);
        //        //}
        //        if (PreventFurtherRuns)
        //        {
        //            throw (EX);
        //        }
        //        else
        //        {
        //            Echo(EX.Message);
        //            ClearAndGather();
        //        }
        //    }
        //}

        private void Director(string Argument)
        {
            Echo("Running Director Method");
            DroneCount++;// Add 1 because 0 based index (director is 0);

            if (Argument.Contains("T."))
            {
                SendTriggerCommand(Argument);
                return;
            }

            if (Argument.Contains("M.Ref"))
            {
                SetReferencePosition(Argument);
                return;
            }

            if (Argument.Contains("M.Dist"))
            {
                Echo("Distance Changed");
                char[] digits = Command.Where(c => char.IsDigit(c)).ToArray();
                Dist = Int32.Parse(digits.ToString());
            }

            // Declare Vectors
            Vector3D Forward = MainController.WorldMatrix.Forward * Dist;
            Vector3D Back = MainController.WorldMatrix.Backward * Dist;
            Vector3D Right = MainController.WorldMatrix.Right * Dist;
            Vector3D Left = MainController.WorldMatrix.Left * Dist;
            Vector3D Up = MainController.WorldMatrix.Up * Dist;
            Vector3D Down = MainController.WorldMatrix.Down * Dist;

            Echo(Dist.ToString());
            // Declare and blank out strings
            string Package = "";
            string DirStr = "";

            // List
            List<Vector3D> AssignedDrones = new List<Vector3D>(); //Actual list of assigned vector points which represent each drone.

            // Storage & Run checks

            if (Command == "")
            {
                Command = DefaultCommand;
            }
            if (Form == "")
            {
                Form = DefaultForm;
            }
            // Argument checks
            if (Argument == "Reset")
            {
                Storage = "";
                DirStr = "";
                Package = "";
                Command = "";
                Form = "";
                AssignedDrones.Clear();
                //Subordiantes.Clear();
            }

            if (Argument.Contains("C.")) //Set command or formation
            {
                Command = Argument;
            }
            else if (Argument.Contains("F."))
            {
                Form = Argument;
            }

            //Formations
            if (Form != "")
            {
                bool DirMem = false;//Thanks Digi!

                if (Form == "F.Line")
                {
                    int j = 1;
                    for (int i = 1; i < DroneCount; i++)//Position adding
                    {
                        i = (DirMem ? (i - j) : i);//Every other
                        AssignedDrones.Add(((DirMem ? Left : Right) * i) + GblPosition);
                        DirMem = !DirMem;
                    }
                }

                if (Form == "F.Wedge")
                {
                    int j = 1;
                    for (int i = 1; i < DroneCount; i++)//Position adding
                    {
                        i = (DirMem ? (i - j) : i);//Every other
                        AssignedDrones.Add(((Back + (DirMem ? (Left) : Right)) * i) + GblPosition);
                        DirMem = !DirMem;
                    }
                }

                if (Form == "F.Inverted Wedge")
                {
                    int j = 1;
                    for (int i = 1; i < DroneCount; i++)//Position adding
                    {
                        i = (DirMem ? (i - j) : i);//Every other
                        AssignedDrones.Add(((Forward + (DirMem ? (Left) : Right)) * i) + GblPosition);
                        DirMem = !DirMem;
                    }
                }

                if (Form == "F.Right Echelon")
                {
                    for (int i = 1; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add((Back + Right) * i + GblPosition);
                    }
                }

                if (Form == "F.Left Echelon")
                {
                    for (int i = 1; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add((Back + Left) * i + GblPosition);
                    }
                }

                if (Form == "F.Dual Column")
                {
                    //AssignedDrones.Add(Right + Forward + GblPosition); AssignedDrones.Add(Left + Forward + GblPosition);
                    //AssignedDrones.Add(Right + 2 * Forward + GblPosition); AssignedDrones.Add(Left + 2 * Forward + GblPosition);
                    //AssignedDrones.Add(Right + 3 * Forward + GblPosition); AssignedDrones.Add(Left + 3 * Forward + GblPosition);

                    //AssignedDrones.Add(Right + 4 * Forward + GblPosition); AssignedDrones.Add(Left + 4 * Forward + GblPosition);
                    //AssignedDrones.Add(Right + 5 * Forward + GblPosition); AssignedDrones.Add(Left + 5 * Forward + GblPosition);
                    //AssignedDrones.Add(Right + 6 * Forward + GblPosition); AssignedDrones.Add(Left + 6 * Forward + GblPosition);

                    for (int i = 1; i < DroneCount / 2 + 1; i++)
                    {
                        AssignedDrones.Add(Right + Forward * i + GblPosition);
                        AssignedDrones.Add(Left + Forward * i + GblPosition);
                    }
                }

                if (Form == "F.Box")
                {
                    AssignedDrones.Add(Up + Right + GblPosition);
                    AssignedDrones.Add(Up + Left + GblPosition);
                    AssignedDrones.Add(2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Left + GblPosition);
                    AssignedDrones.Add(Down + Right + GblPosition);
                    AssignedDrones.Add(Down + Left + GblPosition);

                    AssignedDrones.Add(2 * Up + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Up + 2 * Left + GblPosition);
                    AssignedDrones.Add(3 * Right + GblPosition);
                    AssignedDrones.Add(3 * Left + GblPosition);
                    AssignedDrones.Add(2 * Down + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Down + 2 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.DoubleBox")
                {
                    AssignedDrones.Add(4 * Forward + Up + Right + GblPosition);
                    AssignedDrones.Add(4 * Forward + Up + Left + GblPosition);
                    AssignedDrones.Add(4 * Forward + 2 * Right + GblPosition);
                    AssignedDrones.Add(4 * Forward + 2 * Left + GblPosition);
                    AssignedDrones.Add(4 * Forward + Down + Right + GblPosition);
                    AssignedDrones.Add(4 * Forward + Down + Left + GblPosition);

                    AssignedDrones.Add(2 * Forward + 2 * Up + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Up + 2 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 3 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 3 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Down + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Down + 2 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.Vangard1")
                {
                    AssignedDrones.Add(10 * Forward + Up + Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + Up + Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + Down + Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + Down + Left + GblPosition);

                    AssignedDrones.Add(2 * Forward + 2 * Up + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Up + 2 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 3 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 3 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Down + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Down + 2 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.Vangard2")
                {
                    AssignedDrones.Add(2 * Forward + Up + Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + Up + Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 2 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + Down + Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + Down + Left + GblPosition);

                    AssignedDrones.Add(10 * Forward + 2 * Up + 2 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Up + 2 * Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + 3 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 3 * Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Down + 2 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Down + 2 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.VangardAll")
                {
                    AssignedDrones.Add(10 * Forward + Up + Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + Up + Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + Down + Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + Down + Left + GblPosition);

                    AssignedDrones.Add(10 * Forward + 2 * Up + 2 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Up + 2 * Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + 3 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 3 * Left + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Down + 2 * Right + GblPosition);
                    AssignedDrones.Add(10 * Forward + 2 * Down + 2 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.Dense")
                {
                    AssignedDrones.Add(1.0 * Forward + 0.5 * Up + 0.5 * Right + GblPosition);
                    AssignedDrones.Add(1.0 * Forward + 0.5 * Up + 0.5 * Left + GblPosition);
                    AssignedDrones.Add(1.0 * Forward + 1.0 * Right + GblPosition);
                    AssignedDrones.Add(1.0 * Forward + 1.0 * Left + GblPosition);
                    AssignedDrones.Add(1.0 * Forward + 0.5 * Down + 0.5 * Right + GblPosition);
                    AssignedDrones.Add(1.0 * Forward + 0.5 * Down + 0.5 * Left + GblPosition);

                    AssignedDrones.Add(1.0 * Up + 1.0 * Right + GblPosition);
                    AssignedDrones.Add(1.0 * Up + 1.0 * Left + GblPosition);
                    AssignedDrones.Add(1.5 * Right + GblPosition);
                    AssignedDrones.Add(1.5 * Left + GblPosition);
                    AssignedDrones.Add(1.0 * Down + 1.0 * Right + GblPosition);
                    AssignedDrones.Add(1.0 * Down + 1.0 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.Diffuse")
                {
                    AssignedDrones.Add(4 * Forward + 2 * Up + 2 * Right + GblPosition);
                    AssignedDrones.Add(4 * Forward + 2 * Up + 2 * Left + GblPosition);
                    AssignedDrones.Add(4 * Forward + 4 * Right + GblPosition);
                    AssignedDrones.Add(4 * Forward + 4 * Left + GblPosition);
                    AssignedDrones.Add(4 * Forward + 2 * Down + 2 * Right + GblPosition);
                    AssignedDrones.Add(4 * Forward + 2 * Down + 2 * Left + GblPosition);

                    AssignedDrones.Add(2 * Forward + 4 * Up + 4 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 4 * Up + 4 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 6 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 6 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 4 * Down + 4 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 4 * Down + 4 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.Sphere")
                {
                    AssignedDrones.Add(2 * Forward + 1 * Up + 1 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 1 * Up + 1 * Left + GblPosition);
                    AssignedDrones.Add(2 * Forward + 1 * Down + 1 * Right + GblPosition);
                    AssignedDrones.Add(2 * Forward + 1 * Down + 1 * Left + GblPosition);

                    AssignedDrones.Add(2 * Up + GblPosition);
                    AssignedDrones.Add(2 * Right + GblPosition);
                    AssignedDrones.Add(2 * Left + GblPosition);
                    AssignedDrones.Add(2 * Down + GblPosition);

                    AssignedDrones.Add(2 * Back + 1 * Up + 1 * Right + GblPosition);
                    AssignedDrones.Add(2 * Back + 1 * Up + 1 * Left + GblPosition);
                    AssignedDrones.Add(2 * Back + 1 * Down + 1 * Right + GblPosition);
                    AssignedDrones.Add(2 * Back + 1 * Down + 1 * Left + GblPosition);

                    for (int i = 12; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }//Extras: Single Column
                }

                if (Form == "F.Single Column")
                {
                    for (int i = 1; i < DroneCount; i++)//Position adding
                    {
                        AssignedDrones.Add(Back * i + GblPosition);
                    }
                }

                //Commands
                if (Command == "C.Land")
                {
                    double Height;
                    bool InPlanet = MainController.TryGetPlanetElevation(MyPlanetElevation.Surface, out Height);
                    if (InPlanet)
                    {
                        for (int i = 1; i < AssignedDrones.Count; i++)
                        {
                            AssignedDrones[i] = AssignedDrones[i] + ((Down / Dist) * Height);
                        }
                    }
                    else
                    {
                        Echo("Not in planet gravity: Subordinates commanded to wait.");
                        Command = ("C.Wait");
                    }
                }
            }
            DroneCount--;// Remove 1 from drone count to reset to 0 index modification
                         //Compile and transmit
            if (AssignedDrones.Count == 0)
            {
                Echo("Error: No drones assigned. Either internal error or argument has not been stated.");
            }
            else
            {
                DirStr = (GblPosition.X.ToString() + "," + GblPosition.Y.ToString() + "," + GblPosition.Z.ToString());
                Package = Channel + ":" + DirStr + ":" + Command + ":" + Form + ":";

                // TODO 指揮船の向きを追加

                // Create Package
                foreach (Vector3D drone in AssignedDrones)
                {
                    Package += (Math.Round(drone.X).ToString() + "," + Math.Round(drone.Y).ToString() + "," + Math.Round(drone.Z).ToString()) + ":";
                }

                //TODO LastPosition never assigned
                if (Distance(GblPosition, LastPosition) < ControlFactor)
                {
                    Echo("Idle Mode");
                }
                else
                {
                    // Broadcast Package
                    bool Transmitted = Antennae[0].TransmitMessage(Package, MyTransmitTarget.Default);
                    if (Transmitted) { Echo("Broadcasting Secure Package. Channel: " + Channel); } else { Echo("Error: Transmit Failed"); }
                    Echo("DroneCount: " + DroneCount);
                    Echo("Formation: " + Form);
                    Echo("Command: " + Command);
                    AssignedDrones.Clear();
                }
            }
        }

        private void Recieve(string Package)
        {
            PCS = (Package.Contains(":")) ? (PCS = Package.Split(':')) : null; Echo("Package is not able to be disassembled.");

            Command = PCS[2];// Set Command
            Form = PCS[3];// Set Formation

            if (Channel == PCS[0])
            {

                if (Package.Contains("T."))
                {
                    if (PCS[1].Contains("T.Timer"))
                    {
                        TriggerTimer(PCS[1]);
                    }

                    return;
                }
                else
                {
                    string DirStr = PCS[1];

                    DirVec.X = Convert.ToDouble(DirStr.Split(',')[0]);
                    DirVec.Y = Convert.ToDouble(DirStr.Split(',')[1]);
                    DirVec.Z = Convert.ToDouble(DirStr.Split(',')[2]);

                    // TODO 指揮船の向きを読み取り

                    // TODO ひとつずれる
                    string PosStr = PCS[DroneNum + 3];

                    Target.X = Convert.ToDouble(PosStr.Split(',')[0]);
                    Target.Y = Convert.ToDouble(PosStr.Split(',')[1]);
                    Target.Z = Convert.ToDouble(PosStr.Split(',')[2]);

                    Echo("Package Recieved and disassembled.");
                    DroneModeSelect();
                }

            }
            else
            {
                Echo("Package is not the correct channel.");
            }
        }

        private void DroneModeSelect()
        {
            Echo("Selecting Mode...");
            bool TargetFound = false;
            if (Command != "C.Recall" && Command != "C.PFollow")
            { TargetFound = AllowAttack ? WeaponCheck() : false; }
            if (TargetFound)
            {
                Freelance();
            }
            else
            {
                if (Command == "")
                {
                    Command = DefaultCommand;
                }

                if (Command == "C.Recall")
                {
                    foreach (IMyLargeTurretBase weapon in Weapons)
                    {
                        weapon.ApplyAction("OnOff_Off");
                        weapon.SetValueBool("Shoot", false);
                    }
                    Move(Target, false);
                }

                if (Command == "C.Follow" | Command == "C.Land" | Command == "C.PFollow" | Command == "C.Move")
                {
                    foreach (IMyLargeTurretBase weapon in Weapons)
                    {
                        weapon.ApplyAction("OnOff_On");
                    }
                    Move(Target, false);
                }

                if (Command == "C.Wait")
                {
                    foreach (IMyLargeTurretBase weapon in Weapons)
                    {
                        weapon.ApplyAction("OnOff_On");
                    }
                    Wait();
                }

                if (Command == "C.Freelance")
                {
                    Freelance();
                }
                else
                {
                    CommandOverride = false;
                }
            }
        }

        private void Freelance()
        {
            Echo("FreelanceMode");
            if (WeaponCheck())
            {
                Echo("Target Found");
                Move(Target, true);
            }
        }

        private bool WeaponCheck()
        {
            bool TargetFound = false;
            if (Weapons.Count > 0)
            {
                //if (Command == "C.Freelance")
                //{
                //	CommandOverride = true;
                //}

                foreach (IMyLargeTurretBase item in Weapons)
                {
                    item.ApplyAction("OnOff_On");
                }

                IMyLargeTurretBase Weapon = Weapons[0];

                for (int i = 0; i < Weapons.Count; i++)
                {
                    if (Weapons[i].IsShooting && Weapons[i].HasTarget)
                    {
                        Weapon = Weapons[i];
                        TargetFound = true;
                        break;
                    }
                }

                if (TargetFound | CommandOverride)
                {
                    Echo("Attack Mode Activated");

                    MyDetectedEntityInfo entityInfo = Weapon.GetTargetedEntity();

                    Vector3D transformedTargetVec = Vector3D.Normalize(entityInfo.Position - GblPosition);

                    Target = entityInfo.Position + transformedTargetVec * (Weapon.Range / 3);

                    Echo("Target: " + TargetFound);
                    //Echo("Position: " + entityInfo.Position);

                    return TargetFound;
                }
                else
                {
                    Echo("No enemies found");
                    return TargetFound;
                }
            }
            Echo("No turrets on grid");
            return TargetFound;
        }

        private void Move(Vector3D Target, bool TargetFound)
        {
            Echo("Moving/Following");
            double Dirdist = new double();
            double Targetdist = new double();

            //Calculate distances
            Dirdist = Distance(GblPosition, DirVec);
            Targetdist = Distance(GblPosition, Target);
            Echo(Targetdist.ToString());

            //Set FlightMode
            if (Remotes[0].GetValue<Int64>("FlightMode") != 2)
            {
                Remotes[0].SetValue<Int64>("FlightMode", 2);
            }

            //Planet checks and calculation.
            double Height;
            bool InPlanet = Remotes[0].TryGetPlanetElevation(MyPlanetElevation.Surface, out Height);

            if (Command == "C.PFollow")
            {
                Remotes[0].ApplyAction("CollisionAvoidance_Off");
                Remotes[0].ApplyAction("DockingMode_On");
            }
            else
            {
                if (InPlanet == false)
                {
                    Remotes[0].ApplyAction("CollisionAvoidance_On");
                    Remotes[0].ApplyAction("DockingMode_Off");
                }

                if (InPlanet == true)
                {
                    Height--;// Lower height by one so landing is possible.
                    Target = (Remotes[0].WorldMatrix.Down * 2 + Target);

                    if (Height < 10)
                    {
                        Remotes[0].ApplyAction("CollisionAvoidance_Off");
                        Remotes[0].ApplyAction("DockingMode_On");
                    }

                    if (Dirdist > 50 && Command != "C.Land")
                    {
                        if (Gears.Count > 0)
                        {
                            foreach (var gear in Gears)
                            {
                                gear.ApplyAction("Unlock");
                            }
                        }
                    }
                    else
                    {
                        if (Height < ControlFactor && Command == "C.Land")
                        {
                            if (Gears.Count > 0)
                            {
                                foreach (var gear in Gears)
                                {
                                    gear.ApplyAction("Lock");
                                }
                            }
                        }
                    }

                    if (Height > 20 && Height < 1000)
                    {
                        Remotes[0].ApplyAction("CollisionAvoidance_On");
                        Remotes[0].ApplyAction("DockingMode_Off");
                    }
                }
            }

            if (TargetFound)
            {
                Target = Target + RandVectorGen(1000);
            }

            Remotes[0].ClearWaypoints();
            Remotes[0].AddWaypoint(Target, "Target");
            if (Targetdist < ControlFactor)
            {
                Remotes[0].ClearWaypoints();
                Remotes[0].SetAutoPilotEnabled(false);
            }
            else
            {
                Remotes[0].SetAutoPilotEnabled(true);

                // TODO 機体の向きを親に合わせる
                if (!AllowAttack)
                {

                }
            }
        }

        private void Wait()
        {
            Echo("Waiting");

            if (Remotes[0].IsAutoPilotEnabled)
            {
                Remotes[0].SetAutoPilotEnabled(false);
            }
        }

        private Vector3D VectorAzimuthElevation(double az, double el)
        {// Whip's VectorAzimuthElevation Method
            el = el % (2 * Math.PI);
            az = az % (2 * Math.PI);
            // Whip's VectorAzimuthElevation Method
            if (az != Math.Abs(az))
            {
                az = 2 * Math.PI + az;
            }

            int x_mult = 1;
            // Whip's VectorAzimuthElevation Method
            if (az > Math.PI / 2 && az < Math.PI)
            {
                az = Math.PI - (az % Math.PI);
                x_mult = -1;
            }
            else if (az > Math.PI && az < Math.PI * 3 / 2)
            {
                az = 2 * Math.PI - (az % Math.PI);
                x_mult = -1;
            }
            // Whip's VectorAzimuthElevation Method
            double x; double y; double z;
            // Whip's VectorAzimuthElevation Method
            if (el == Math.PI / 2)
            {
                x = 0;
                y = 0;
                z = 1;
            }
            else if (az == Math.PI / 2)
            {
                x = 0;
                y = 1;
                z = y * Math.Tan(el);
            }
            else
            {
                x = 1 * x_mult;
                y = Math.Tan(az);
                double v_xy = Math.Sqrt(1 + y * y);
                z = v_xy * Math.Tan(el);
            }
            // Whip's VectorAzimuthElevation Method
            return new Vector3D(x, y, z);
        }// Whip's VectorAzimuthElevation Method

        private double Distance(Vector3D VectorOne, Vector3D VectorTwo)
        {
            double distance = Math.Round(Vector3D.Distance(VectorOne, VectorTwo));
            return distance;
        }

        private Vector3D RandVectorGen(int Max)
        {
            Random r = new Random();
            int Min = 1;
            Vector3D RandVector = new Vector3D((r.Next(Min, Max) - (r.Next(Min, Max))), (r.Next(Min, Max) - (r.Next(Min, Max))), (r.Next(Min, Max) - (r.Next(Min, Max))));
            Echo(RandVector.ToString());
            return RandVector;
        }

        void SendTriggerCommand(string TriggerCommand)
        {

            string Package = "";

            // Broadcast Trigger command
            Package = Channel + ":" + TriggerCommand;
            bool Transmitted = Antennae[0].TransmitMessage(Package, MyTransmitTarget.Default);
            if (Transmitted) { Echo("Broadcasting Secure Package. Channel: " + Channel); } else { Echo("Error: Transmit Failed"); }
        }

        void SetReferencePosition(string Argument)
        {

            string[] argSplit = Argument.Split(' ');
            if (argSplit.Length > 4)
            {
                double newX = Double.Parse(argSplit[1]);
                double newY = Double.Parse(argSplit[2]);
                double newZ = Double.Parse(argSplit[3]);
                Vector3D newRef = new Vector3D(newX, newY, newZ);

                RefPosition = newRef;
            }
        }

        void TriggerTimer(string Argument)
        {

            string targetTag = strTriggerTimerTag;

            string[] argSplit = Argument.Split(' ');
            if (argSplit.Length > 1)
            {
                targetTag = argSplit[1];
            }

            List<IMyTerminalBlock> blocks = GetBlocksWithName<IMyTimerBlock>(targetTag);
            IMyTimerBlock commandTimer = null;
            commandTimer = ReturnClosest(blocks) as IMyTimerBlock;

            if (commandTimer != null)
            {
                commandTimer.ApplyAction("TriggerNow");
            }
        }

        //------------------------------ customized Alysius's ------------------------------

        IMyTerminalBlock ReturnClosest(List<IMyTerminalBlock> blocks)
        {
            double currDist = 0;
            double closestDist = Double.MaxValue;
            IMyTerminalBlock closestBlock = null;

            for (int i = 0; i < blocks.Count; i++)
            {
                currDist = (blocks[i].GetPosition() - Me.GetPosition()).Length();
                if (currDist < closestDist)
                {
                    closestDist = currDist;
                    closestBlock = blocks[i];
                }
            }

            return closestBlock;
        }

        List<IMyTerminalBlock> GetBlocksWithName<T>(string name, int matchType = 0) where T : class, IMyTerminalBlock
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(name, blocks);

            List<IMyTerminalBlock> filteredBlocks = new List<IMyTerminalBlock>();
            for (int i = 0; i < blocks.Count; i++)
            {
                if (matchType > 0)
                {
                    bool isMatch = false;

                    switch (matchType)
                    {
                        case 1:
                            if (blocks[i].CustomName.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                            {
                                isMatch = true;
                            }
                            break;
                        case 2:
                            if (blocks[i].CustomName.EndsWith(name, StringComparison.OrdinalIgnoreCase))
                            {
                                isMatch = true;
                            }
                            break;
                        case 3:
                            if (blocks[i].CustomName.Equals(name, StringComparison.OrdinalIgnoreCase))
                            {
                                isMatch = true;
                            }
                            break;
                        default:
                            isMatch = true;
                            break;
                    }

                    if (!isMatch)
                    {
                        continue;
                    }
                }

                IMyTerminalBlock block = blocks[i] as T;
                if (block != null)
                {
                    filteredBlocks.Add(block);
                }
            }

            return filteredBlocks;
        }
    }
}