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

        #region In-game Script
        /*
        / //// / Whip's Turret Based Radar Systems / //// /

        HOW DO I USE THIS?

        1. Place this script in a programmable block.
        2. Place some turrets on your ship.
        3. Place a seat on your ship.
        4. Place some text panels with "Radar" in their name somewhere.
        5. Enjoy!




        =================================================
            DO NOT MODIFY VARIABLES IN THE SCRIPT!

         USE THE CUSTOM DATA OF THIS PROGRAMMABLE BLOCK!
        =================================================


























        HEY! DONT EVEN THINK ABOUT TOUCHING BELOW THIS LINE!

        */

        #region Fields
        const string VERSION = "30.9.4";
        const string DATE = "04/17/2019";

        enum TargetRelation : byte { Neutral = 0, Enemy = 1, Friendly = 2 }

        const string IGC_TAG = "IGC_IFF_MSG";

        const string INI_SECTION_GENERAL = "Radar - General";
        const string INI_RADAR_NAME = "Text surface name tag";
        const string INI_REF_NAME = "Optional reference block name";
        const string INI_BCAST = "Share own position";
        const string INI_NETWORK = "Share targets";
        const string INI_USE_RANGE_OVERRIDE = "Use radar range override";
        const string INI_RANGE_OVERRIDE = "Radar range override (m)";
        const string INI_PROJ_ANGLE = "Radar projection angle in degrees (0 is flat)";

        const string INI_SECTION_COLORS = "Radar - Colors";
        const string INI_TEXT = "Text";
        const string INI_BACKGROUND = "Background";
        const string INI_RADAR_LINES = "Radar lines";
        const string INI_PLANE = "Radar plane";
        const string INI_ENEMY = "Enemy icon";
        const string INI_ENEMY_ELEVATION = "Enemy elevation";
        const string INI_NEUTRAL = "Neutral icon";
        const string INI_NEUTRAL_ELEVATION = "Neutral elevation";
        const string INI_FRIENDLY = "Friendly icon";
        const string INI_FRIENDLY_ELEVATION = "Friendly elevation";

        const string INI_SECTION_TEXT_SURF_PROVIDER = "Radar - Text Surface Config";
        const string INI_TEXT_SURFACE_TEMPLATE = "Show on screen {0}";

        const int MAX_REBROADCAST_INGEST_COUNT = 6;

        IMyBroadcastListener broadcastListener;

        string referenceName = "Reference";
        float rangeOverride = 1000;
        bool useRangeOverride = false;
        bool networkTargets = true;
        bool broadcastIFF = true;

        Color backColor = new Color(0, 0, 0, 255);
        Color lineColor = new Color(255, 100, 0, 50);
        Color planeColor = new Color(100, 30, 0, 5);
        Color enemyIconColor = new Color(150, 0, 0, 255);
        Color enemyElevationColor = new Color(75, 0, 0, 255);
        Color neutralIconColor = new Color(150, 150, 0, 255);
        Color neutralElevationColor = new Color(75, 75, 0, 255);
        Color allyIconColor = new Color(0, 50, 150, 255);
        Color allyElevationColor = new Color(0, 25, 75, 255);
        Color textColor = new Color(255, 100, 0, 100);

        float MaxRange
        {
            get
            {
                return Math.Max(1, useRangeOverride ? rangeOverride : (turrets.Count == 0 ? rangeOverride : turretMaxRange));
            }
        }

        List<IMyShipController> Controllers
        {
            get
            {
                return taggedControllers.Count > 0 ? taggedControllers : allControllers;
            }
        }

        string textPanelName = "Radar";
        float projectionAngle = 60f;
        float turretMaxRange = 800f;

        Scheduler scheduler;
        RuntimeTracker runtimeTracker;
        ScheduledAction grabBlockAction;

        Dictionary<long, TargetData> targetDataDict = new Dictionary<long, TargetData>();
        Dictionary<long, TargetData> broadcastDict = new Dictionary<long, TargetData>();
        List<IMyLargeTurretBase> turrets = new List<IMyLargeTurretBase>();
        List<IMyTextSurface> textSurfaces = new List<IMyTextSurface>();
        List<IMyShipController> taggedControllers = new List<IMyShipController>();
        List<IMyShipController> allControllers = new List<IMyShipController>();
        IMyShipController reference;
        IMyShipController lastActiveShipController = null;

        const double cycleTime = 1.0 / 60.0;
        string lastSetupResult = "";
        bool isSetup = false;

        readonly List<string> _iniSections = new List<string>();
        readonly RadarSurface radarSurface;
        readonly MyIni generalIni = new MyIni();
        readonly MyIni textSurfaceIni = new MyIni();
        readonly MyCommandLine _commandLine = new MyCommandLine();
        #endregion

        #region Main Routine
        Program()
        {
            ParseCustomDataIni();
            GrabBlocks();

            radarSurface = new RadarSurface(backColor, lineColor, planeColor, textColor, projectionAngle, MaxRange);

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            runtimeTracker = new RuntimeTracker(this);

            // Scheduler creation
            scheduler = new Scheduler(this);
            grabBlockAction = new ScheduledAction(GrabBlocks, 0.1);
            scheduler.AddScheduledAction(grabBlockAction);
            scheduler.AddScheduledAction(UpdateRadarRange, 1);
            scheduler.AddScheduledAction(PrintDetailedInfo, 1);

            scheduler.AddQueuedAction(GetTurretTargets, cycleTime);                                             // cycle 1
            scheduler.AddQueuedAction(radarSurface.SortContacts, cycleTime);                                    // cycle 2

            float step = 1f / 8f;
            scheduler.AddQueuedAction(() => Draw(0 * step, 1 * step), cycleTime);                               // cycle 3
            scheduler.AddQueuedAction(() => Draw(1 * step, 2 * step), cycleTime);                               // cycle 4
            scheduler.AddQueuedAction(() => Draw(2 * step, 3 * step), cycleTime);                               // cycle 5
            scheduler.AddQueuedAction(() => Draw(3 * step, 4 * step), cycleTime);                               // cycle 6
            scheduler.AddQueuedAction(() => Draw(4 * step, 5 * step), cycleTime);                               // cycle 7
            scheduler.AddQueuedAction(() => Draw(5 * step, 6 * step), cycleTime);                               // cycle 8
            scheduler.AddQueuedAction(() => Draw(6 * step, 7 * step), cycleTime);                               // cycle 9
            scheduler.AddQueuedAction(() => Draw(7 * step, 8 * step), cycleTime);                               // cycle 10

            // IGC Register
            broadcastListener = IGC.RegisterBroadcastListener(IGC_TAG);
            broadcastListener.SetMessageCallback(IGC_TAG);
        }

        void Main(string arg, UpdateType updateSource)
        {
            runtimeTracker.AddRuntime();

            if (_commandLine.TryParse(arg))
                HandleArguments();

            scheduler.Update();

            if (arg.Equals(IGC_TAG))
            {
                ProcessNetworkMessage();
            }

            runtimeTracker.AddInstructions();
        }

        void HandleArguments()
        {
            int argCount = _commandLine.ArgumentCount;

            if (argCount == 0)
                return;

            switch (_commandLine.Argument(0).ToLowerInvariant())
            {
                case "range":
                    if (argCount != 2)
                    {
                        return;
                    }

                    float range = 0;
                    if (float.TryParse(_commandLine.Argument(1), out range))
                    {
                        useRangeOverride = true;
                        rangeOverride = range;

                        UpdateRadarRange();

                        generalIni.Clear();
                        generalIni.TryParse(Me.CustomData);
                        generalIni.Set(INI_SECTION_GENERAL, INI_RANGE_OVERRIDE, rangeOverride);
                        generalIni.Set(INI_SECTION_GENERAL, INI_USE_RANGE_OVERRIDE, useRangeOverride);
                        Me.CustomData = generalIni.ToString();
                    }
                    else if (string.Equals(_commandLine.Argument(1), "default"))
                    {
                        useRangeOverride = false;

                        UpdateRadarRange();

                        generalIni.Clear();
                        generalIni.TryParse(Me.CustomData);
                        generalIni.Set(INI_SECTION_GENERAL, INI_USE_RANGE_OVERRIDE, useRangeOverride);
                        Me.CustomData = generalIni.ToString();
                    }
                    return;

                default:
                    return;
            }
        }

        void ProcessNetworkMessage()
        {
            while (broadcastListener.HasPendingMessage)
            {
                object messageData = broadcastListener.AcceptMessage().Data;
                if (messageData is MyTuple<byte, long, Vector3D, byte>)
                {
                    var myTuple = (MyTuple<byte, long, Vector3D, byte>)messageData;
                    byte relationship = myTuple.Item1;
                    long entityId = myTuple.Item2;
                    Vector3D position = myTuple.Item3;
                    // Item4 is ignored for backwards compatibility

                    if ((byte)TargetRelation.Friendly == relationship)
                    {
                        targetDataDict[entityId] = new TargetData(position, TargetRelation.Friendly);
                    }
                    else if ((byte)TargetRelation.Neutral == relationship)
                    {
                        targetDataDict[entityId] = new TargetData(position, TargetRelation.Neutral);
                    }
                    else
                    {
                        targetDataDict[entityId] = new TargetData(position, TargetRelation.Enemy);
                    }
                }
            }
        }

        void NetworkTargets()
        {
            if (broadcastIFF)
            {
                var myTuple = new MyTuple<byte, long, Vector3D, byte>((byte)TargetRelation.Friendly, Me.CubeGrid.EntityId, Me.GetPosition(), 0);
                IGC.SendBroadcastMessage(IGC_TAG, myTuple);
            }

            if (networkTargets)
            {
                foreach (var kvp in broadcastDict)
                {
                    var targetData = kvp.Value;
                    var myTuple = new MyTuple<byte, long, Vector3D, byte>((byte)targetData.Relation, kvp.Key, targetData.Position, 0);
                    IGC.SendBroadcastMessage(IGC_TAG, myTuple);
                }
            }
        }

        void GetTurretTargets()
        {
            if (!isSetup) //setup error
                return;

            broadcastDict.Clear();
            radarSurface.ClearContacts();

            foreach (var block in turrets)
            {
                if (IsClosed(block))
                    continue;

                if (block.HasTarget && !block.IsUnderControl)
                {
                    var target = block.GetTargetedEntity();

                    TargetData targetData = default(TargetData);
                    if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies)
                    {
                        targetData = new TargetData(target.Position, TargetRelation.Enemy);
                    }
                    else
                    {
                        targetData = new TargetData(target.Position, TargetRelation.Neutral);
                    }

                    targetDataDict[target.EntityId] = targetData;
                    broadcastDict[target.EntityId] = targetData;
                }
            }

            // Define reference ship controller
            reference = GetControlledShipController(Controllers); // Primary, get active controller
            if (reference == null)
            {
                if (lastActiveShipController != null)
                {
                    // Backup, use last active controller
                    reference = lastActiveShipController;
                }
                else if (reference == null && Controllers.Count != 0)
                {
                    // Last case, resort to the first controller in the list
                    reference = Controllers[0];
                }
                else
                {
                    return;
                }
            }

            lastActiveShipController = reference;

            foreach (var kvp in targetDataDict)
            {
                var targetData = kvp.Value;

                Color targetIconColor = enemyIconColor;
                Color targetElevationColor = enemyElevationColor;
                RadarSurface.Relation relation = RadarSurface.Relation.Hostile;
                switch (targetData.Relation)
                {
                    case TargetRelation.Enemy:
                        // Already set
                        break;

                    case TargetRelation.Neutral:
                        targetIconColor = neutralIconColor;
                        targetElevationColor = neutralElevationColor;
                        relation = RadarSurface.Relation.Neutral;
                        break;

                    case TargetRelation.Friendly:
                        targetIconColor = allyIconColor;
                        targetElevationColor = allyElevationColor;
                        relation = RadarSurface.Relation.Allied;
                        break;
                }

                if (kvp.Key == Me.CubeGrid.EntityId)
                    continue;

                if (Vector3D.DistanceSquared(targetData.Position, reference.GetPosition()) < (MaxRange * MaxRange))
                    radarSurface.AddContact(targetData.Position, reference.WorldMatrix, targetIconColor, targetElevationColor, relation);
            }

            NetworkTargets();

            targetDataDict.Clear();
        }

        void Draw(float startProportion, float endProportion)
        {
            int start = (int)(startProportion * textSurfaces.Count);
            int end = (int)(endProportion * textSurfaces.Count);

            for (int i = start; i < end; ++i)
            {
                var textSurface = textSurfaces[i];
                radarSurface.DrawRadar(textSurface);
            }
        }

        void PrintDetailedInfo()
        {
            Echo($"WMI Radar System Online{RunningSymbol()}\n(Version {VERSION} - {DATE})");
            Echo($"\nNext refresh in {Math.Max(grabBlockAction.RunInterval - grabBlockAction.TimeSinceLastRun, 0):N0} seconds");
            Echo($"{lastSetupResult}");
            Echo($"Range: {MaxRange} m\n");
            Echo($"Text surfaces: {textSurfaces.Count}\n");
            Echo($"Reference seat:\n\"{(reference?.CustomName)}\"");
            Echo(runtimeTracker.Write());
        }

        void UpdateRadarRange()
        {
            turretMaxRange = GetMaxTurretRange(turrets);
            radarSurface.Range = MaxRange;
        }

        #endregion

        #region Target Data Struct
        struct TargetData
        {
            public Vector3D Position;
            public TargetRelation Relation;

            public TargetData(Vector3D position, TargetRelation relation)
            {
                Position = position;
                Relation = relation;
            }
        }
        #endregion

        #region Radar Surface
        class RadarSurface
        {
            public float Range;
            public enum Relation { None = 0, Allied = 1, Neutral = 2, Hostile = 3 } // Mutually exclusive switch
            public readonly StringBuilder Debug = new StringBuilder();

            const string FONT = "Debug";
            const float HUD_TEXT_SIZE = 1.3f;
            const float RANGE_TEXT_SIZE = 1f;
            const float TGT_ELEVATION_LINE_WIDTH = 4f;

            Color _backColor;
            Color _lineColor;
            Color _planeColor;
            Color _textColor;
            float _projectionAngleDeg;
            float _radarProjectionCos;
            float _radarProjectionSin;
            int _allyCount = 0;
            int _neutralCount = 0;
            int _hostileCount = 0;

            readonly Vector2 TGT_ICON_SIZE = new Vector2(20, 20);
            readonly Vector2 SHIP_ICON_SIZE = new Vector2(32, 16);
            readonly List<TargetInfo> _targetList = new List<TargetInfo>();
            readonly List<TargetInfo> _targetsBelowPlane = new List<TargetInfo>();
            readonly List<TargetInfo> _targetsAbovePlane = new List<TargetInfo>();
            readonly Dictionary<Relation, string> _spriteMap = new Dictionary<Relation, string>()
{
{ Relation.None, "None" },
{ Relation.Allied, "SquareSimple" },
{ Relation.Neutral, "Triangle" },
{ Relation.Hostile, "Circle" },
};

            struct TargetInfo
            {
                public Vector3 Position;
                public Color IconColor;
                public Color ElevationColor;
                public string Icon;
            }

            public RadarSurface(Color backColor, Color lineColor, Color planeColor, Color textColor, float projectionAngleDeg, float range)
            {
                UpdateFields(backColor, lineColor, planeColor, textColor, projectionAngleDeg, range);
            }

            public void UpdateFields(Color backColor, Color lineColor, Color planeColor, Color textColor, float projectionAngleDeg, float range)
            {
                _backColor = backColor;
                _lineColor = lineColor;
                _planeColor = planeColor;
                _textColor = textColor;
                _projectionAngleDeg = projectionAngleDeg;
                Range = range;

                var rads = MathHelper.ToRadians(_projectionAngleDeg);
                _radarProjectionCos = (float)Math.Cos(rads);
                _radarProjectionSin = (float)Math.Sin(rads);
            }

            public void AddContact(Vector3D position, MatrixD worldMatrix, Color iconColor, Color elevationLineColor, Relation relation)
            {
                var transformedDirection = Vector3D.TransformNormal(position - worldMatrix.Translation, Matrix.Transpose(worldMatrix));
                float xOffset = (float)(transformedDirection.X / Range);
                float yOffset = (float)(transformedDirection.Z / Range);
                float zOffset = (float)(transformedDirection.Y / Range);

                string spriteName = "";
                _spriteMap.TryGetValue(relation, out spriteName);

                var targetInfo = new TargetInfo()
                {
                    Position = new Vector3(xOffset, yOffset, zOffset),
                    ElevationColor = elevationLineColor,
                    IconColor = iconColor,
                    Icon = spriteName,
                };

                switch (relation)
                {
                    case Relation.Allied:
                        ++_allyCount;
                        break;

                    case Relation.Neutral:
                        ++_neutralCount;
                        break;

                    case Relation.Hostile:
                        ++_hostileCount;
                        break;
                }

                _targetList.Add(targetInfo);
            }

            public void SortContacts()
            {
                _targetsBelowPlane.Clear();
                _targetsAbovePlane.Clear();

                _targetList.Sort((a, b) => (a.Position.Y).CompareTo(b.Position.Y));

                foreach (var target in _targetList)
                {
                    if (target.Position.Z >= 0)
                        _targetsAbovePlane.Add(target);
                    else
                        _targetsBelowPlane.Add(target);
                }
            }

            public void ClearContacts()
            {
                _targetList.Clear();
                _targetsAbovePlane.Clear();
                _targetsBelowPlane.Clear();
                _allyCount = 0;
                _neutralCount = 0;
                _hostileCount = 0;
            }

            public void DrawRadar(IMyTextSurface surface)
            {
                surface.ContentType = ContentType.SCRIPT;
                surface.Script = "";

                Vector2 surfaceSize = surface.TextureSize;
                Vector2 screenCenter = surfaceSize * 0.5f;
                Vector2 viewportSize = surface.SurfaceSize;
                Vector2 scale = viewportSize / 512f;
                float minScale = Math.Min(scale.X, scale.Y);
                float sideLength = Math.Min(viewportSize.X, viewportSize.Y) - 12f;

                Vector2 radarPlaneSize = new Vector2(sideLength, sideLength * _radarProjectionCos);

                using (var frame = surface.DrawFrame())
                {
                    // Fill background with background color
                    MySprite sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: _backColor);
                    sprite.Position = screenCenter;
                    frame.Add(sprite);

                    // Bottom Icons
                    foreach (var targetInfo in _targetsBelowPlane)
                    {
                        DrawTargetIcon(frame, screenCenter, radarPlaneSize, targetInfo, minScale);
                    }

                    // Radar plane
                    DrawRadarPlane(frame, screenCenter, radarPlaneSize, minScale);

                    // Top Icons
                    foreach (var targetInfo in _targetsAbovePlane)
                    {
                        DrawTargetIcon(frame, screenCenter, radarPlaneSize, targetInfo, minScale);
                    }

                    DrawRadarText(frame, screenCenter, viewportSize, minScale);
                }
            }

            void DrawRadarText(MySpriteDrawFrame frame, Vector2 screenCenter, Vector2 viewportSize, float scale)
            {
                MySprite sprite;
                float textSize = scale * HUD_TEXT_SIZE;
                Vector2 halfScreenSize = viewportSize * 0.5f;
                sprite = MySprite.CreateText($"WMI Radar System", FONT, _textColor, textSize, TextAlignment.CENTER);
                sprite.Position = screenCenter + new Vector2(0, -halfScreenSize.Y);
                frame.Add(sprite);

                sprite = MySprite.CreateText($"Hostile: {_hostileCount}", FONT, _textColor, textSize, TextAlignment.CENTER);
                sprite.Position = screenCenter + new Vector2(-(halfScreenSize.X * 0.5f) + 10, halfScreenSize.Y - (90 * scale));
                frame.Add(sprite);

                sprite = MySprite.CreateText($"Neutral: {_neutralCount}", FONT, _textColor, textSize, TextAlignment.CENTER);
                sprite.Position = screenCenter + new Vector2((halfScreenSize.X * 0.5f) - 10, halfScreenSize.Y - (90 * scale));
                frame.Add(sprite);

                sprite = MySprite.CreateText($"Ally: {_allyCount}", FONT, _textColor, textSize, TextAlignment.CENTER);
                sprite.Position = screenCenter + new Vector2(0, halfScreenSize.Y - (40 * scale));
                frame.Add(sprite);
            }

            void DrawRadarPlane(MySpriteDrawFrame frame, Vector2 screenCenter, Vector2 radarPlaneSize, float scale)
            {
                MySprite sprite;

                // Transparent plane circle
                sprite = new MySprite(SpriteType.TEXTURE, "Circle", size: radarPlaneSize, color: _planeColor);
                sprite.Position = screenCenter;
                frame.Add(sprite);

                // Inner circle
                sprite = new MySprite(SpriteType.TEXTURE, "CircleHollow", size: radarPlaneSize * 0.5f, color: _lineColor);
                sprite.Position = screenCenter;
                frame.Add(sprite);

                // Outer circle
                sprite = new MySprite(SpriteType.TEXTURE, "CircleHollow", size: radarPlaneSize, color: _lineColor);
                sprite.Position = screenCenter;
                frame.Add(sprite);

                // Ship location
                sprite = new MySprite(SpriteType.TEXTURE, "Triangle", size: SHIP_ICON_SIZE * scale, color: _lineColor);
                sprite.Position = screenCenter;
                frame.Add(sprite);

                // Range markers
                string outerRange = "", innerRange = "";
                PrefixRangeWithMetricUnits(Range, "m", 1, out outerRange, out innerRange);

                float textSize = RANGE_TEXT_SIZE * scale;
                sprite = MySprite.CreateText($"    {innerRange}", "Debug", _textColor, textSize, TextAlignment.LEFT);
                sprite.Position = screenCenter + new Vector2(radarPlaneSize.X * -0.25f, 0);
                frame.Add(sprite);

                sprite = MySprite.CreateText($"    {outerRange}", "Debug", _textColor, textSize, TextAlignment.LEFT);
                sprite.Position = screenCenter + new Vector2(radarPlaneSize.X * -0.5f, 0);
                frame.Add(sprite);
            }

            void DrawTargetIcon(MySpriteDrawFrame frame, Vector2 screenCenter, Vector2 radarPlaneSize, TargetInfo targetInfo, float scale)
            {
                Vector3 targetPosPixels = targetInfo.Position * new Vector3(1, _radarProjectionCos, _radarProjectionSin) * radarPlaneSize.X * 0.5f;

                Vector2 targetPosPlane = new Vector2(targetPosPixels.X, targetPosPixels.Y);
                Vector2 iconPos = targetPosPlane - targetPosPixels.Z * Vector2.UnitY;

                float elevationLineWidth = Math.Max(1f, TGT_ELEVATION_LINE_WIDTH * scale);
                MySprite elevationSprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", color: targetInfo.ElevationColor, size: new Vector2(elevationLineWidth, targetPosPixels.Z));
                elevationSprite.Position = screenCenter + (iconPos + targetPosPlane) * 0.5f;

                Vector2 iconSize = TGT_ICON_SIZE * scale;
                MySprite iconSprite = new MySprite(SpriteType.TEXTURE, targetInfo.Icon, color: targetInfo.IconColor, size: iconSize);
                iconSprite.Position = screenCenter + iconPos;

                iconSize.Y *= _radarProjectionCos;
                MySprite projectedIconSprite = new MySprite(SpriteType.TEXTURE, "Circle", color: targetInfo.ElevationColor, size: iconSize);
                projectedIconSprite.Position = screenCenter + targetPosPlane;

                bool showProjectedElevation = Math.Abs(iconPos.Y - targetPosPlane.Y) > iconSize.Y;

                // Changing the order of drawing based on if above or below radar plane
                if (targetPosPixels.Z >= 0)
                {
                    if (showProjectedElevation)
                        frame.Add(projectedIconSprite);
                    frame.Add(elevationSprite);
                    frame.Add(iconSprite);
                }
                else
                {
                    iconSprite.RotationOrScale = MathHelper.Pi;

                    frame.Add(elevationSprite);
                    frame.Add(iconSprite);
                    if (showProjectedElevation)
                        frame.Add(projectedIconSprite);
                }
            }

            string[] _prefixes = new string[]
            {
"Y",
"Z",
"E",
"P",
"T",
"G",
"M",
"k",
            };

            double[] _factors = new double[]
            {
1e24,
1e21,
1e18,
1e15,
1e12,
1e9,
1e6,
1e3,
            };

            void PrefixRangeWithMetricUnits(double num, string unit, int digits, out string numStr, out string halfNumStr)
            {
                string prefix = "";

                for (int i = 0; i < _factors.Length; ++i)
                {
                    double factor = _factors[i];

                    if (num >= factor)
                    {
                        prefix = _prefixes[i];
                        num /= factor;
                        break;
                    }
                }

                numStr = (prefix == "" ? num.ToString("n0") : num.ToString($"n{digits}")) + $" {prefix}{unit}";
                num *= 0.5;
                halfNumStr = (prefix == "" ? num.ToString("n0") : num.ToString($"n{digits}")) + $" {prefix}{unit}";
            }
        }
        #endregion

        #region Ini stuff
        void AddTextSurfaces(IMyTerminalBlock block, List<IMyTextSurface> textSurfaces)
        {
            var textSurface = block as IMyTextSurface;
            if (textSurface != null)
            {
                textSurfaces.Add(textSurface);
                return;
            }

            var surfaceProvider = block as IMyTextSurfaceProvider;
            if (surfaceProvider == null)
                return;

            textSurfaceIni.Clear();
            textSurfaceIni.TryParse(block.CustomData);

            _iniSections.Clear();
            textSurfaceIni.GetSections(_iniSections);
            if (_iniSections.Count == 0)
                textSurfaceIni.EndContent = Me.CustomData;

            int surfaceCount = surfaceProvider.SurfaceCount;
            for (int i = 0; i < surfaceCount; ++i)
            {
                string iniKey = string.Format(INI_TEXT_SURFACE_TEMPLATE, i);
                bool display = textSurfaceIni.Get(INI_SECTION_TEXT_SURF_PROVIDER, iniKey).ToBoolean(i == 0 && !(block is IMyProgrammableBlock));
                if (display)
                    textSurfaces.Add(surfaceProvider.GetSurface(i));

                textSurfaceIni.Set(INI_SECTION_TEXT_SURF_PROVIDER, iniKey, display);
            }

            string output = textSurfaceIni.ToString();
            if (!string.Equals(output, block.CustomData))
                block.CustomData = output;
        }

        void WriteCustomDataIni()
        {
            generalIni.Set(INI_SECTION_GENERAL, INI_RADAR_NAME, textPanelName);
            generalIni.Set(INI_SECTION_GENERAL, INI_BCAST, broadcastIFF);
            generalIni.Set(INI_SECTION_GENERAL, INI_NETWORK, networkTargets);
            generalIni.Set(INI_SECTION_GENERAL, INI_USE_RANGE_OVERRIDE, useRangeOverride);
            generalIni.Set(INI_SECTION_GENERAL, INI_RANGE_OVERRIDE, rangeOverride);
            generalIni.Set(INI_SECTION_GENERAL, INI_PROJ_ANGLE, projectionAngle);
            generalIni.Set(INI_SECTION_GENERAL, INI_REF_NAME, referenceName);

            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_TEXT, textColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_BACKGROUND, backColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_RADAR_LINES, lineColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_PLANE, planeColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_ENEMY, enemyIconColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_ENEMY_ELEVATION, enemyElevationColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_NEUTRAL, neutralIconColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_NEUTRAL_ELEVATION, neutralElevationColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_FRIENDLY, allyIconColor, generalIni);
            MyIniHelper.SetColorChar(INI_SECTION_COLORS, INI_FRIENDLY_ELEVATION, allyElevationColor, generalIni);
            generalIni.SetSectionComment(INI_SECTION_COLORS, "Colors are defined with RGBAlpha color codes where\nvalues can range from 0,0,0,0 [transparent] to 255,255,255,255 [white].");

            string output = generalIni.ToString();
            if (!string.Equals(output, Me.CustomData))
                Me.CustomData = output;
        }

        void ParseCustomDataIni()
        {
            generalIni.Clear();
            generalIni.TryParse(Me.CustomData);

            // Check if there are no Ini sections
            _iniSections.Clear();
            generalIni.GetSections(_iniSections);
            if (_iniSections.Count == 0)
                generalIni.EndContent = Me.CustomData;

            textPanelName = generalIni.Get(INI_SECTION_GENERAL, INI_RADAR_NAME).ToString(textPanelName);
            referenceName = generalIni.Get(INI_SECTION_GENERAL, INI_REF_NAME).ToString(referenceName);
            broadcastIFF = generalIni.Get(INI_SECTION_GENERAL, INI_BCAST).ToBoolean(broadcastIFF);
            networkTargets = generalIni.Get(INI_SECTION_GENERAL, INI_NETWORK).ToBoolean(networkTargets);
            useRangeOverride = generalIni.Get(INI_SECTION_GENERAL, INI_USE_RANGE_OVERRIDE).ToBoolean(useRangeOverride);
            rangeOverride = generalIni.Get(INI_SECTION_GENERAL, INI_RANGE_OVERRIDE).ToSingle(rangeOverride);
            projectionAngle = generalIni.Get(INI_SECTION_GENERAL, INI_PROJ_ANGLE).ToSingle(projectionAngle);

            textColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_TEXT, generalIni, textColor);
            backColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_BACKGROUND, generalIni, backColor);
            lineColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_RADAR_LINES, generalIni, lineColor);
            planeColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_PLANE, generalIni, planeColor);
            enemyIconColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_ENEMY, generalIni, enemyIconColor);
            enemyElevationColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_ENEMY_ELEVATION, generalIni, enemyElevationColor);
            neutralIconColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_NEUTRAL, generalIni, neutralIconColor);
            neutralElevationColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_NEUTRAL_ELEVATION, generalIni, neutralElevationColor);
            allyIconColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_FRIENDLY, generalIni, allyIconColor);
            allyElevationColor = MyIniHelper.GetColorChar(INI_SECTION_COLORS, INI_FRIENDLY_ELEVATION, generalIni, allyElevationColor);

            WriteCustomDataIni();

            if (radarSurface != null)
            {
                radarSurface.UpdateFields(backColor, lineColor, planeColor, textColor, projectionAngle, MaxRange);
            }
        }

        public static class MyIniHelper
        {
            /// <summary>
            /// Adds a color character to a MyIni object
            /// </summary>
            public static void SetColorChar(string sectionName, string itemName, Color color, MyIni ini)
            {

                string colorString = string.Format("{0}, {1}, {2}, {3}", color.R, color.G, color.B, color.A);

                ini.Set(sectionName, itemName, colorString);
            }

            /// <summary>
            /// Parses a MyIni for a color character
            /// </summary>
            public static Color GetColorChar(string sectionName, string itemName, MyIni ini, Color? defaultChar = null)
            {
                string rgbString = ini.Get(sectionName, itemName).ToString("null");
                string[] rgbSplit = rgbString.Split(',');

                int r = 0, g = 0, b = 0, a = 0;
                if (rgbSplit.Length != 4)
                {
                    if (defaultChar.HasValue)
                        return defaultChar.Value;
                    else
                        return Color.Transparent;
                }

                int.TryParse(rgbSplit[0].Trim(), out r);
                int.TryParse(rgbSplit[1].Trim(), out g);
                int.TryParse(rgbSplit[2].Trim(), out b);
                bool hasAlpha = int.TryParse(rgbSplit[3].Trim(), out a);
                if (!hasAlpha)
                    a = 255;

                r = MathHelper.Clamp(r, 0, 255);
                g = MathHelper.Clamp(g, 0, 255);
                b = MathHelper.Clamp(b, 0, 255);
                a = MathHelper.Clamp(a, 0, 255);

                return new Color(r, g, b, a);
            }
        }
        #endregion

        #region General Functions
        //Whip's Running Symbol Method v8
        //•
        int runningSymbolVariant = 0;
        int runningSymbolCount = 0;
        const int increment = 1;
        string[] runningSymbols = new string[] { ".", "..", "...", "....", "...", "..", ".", "" };

        string RunningSymbol()
        {
            if (runningSymbolCount >= increment)
            {
                runningSymbolCount = 0;
                runningSymbolVariant++;
                if (runningSymbolVariant >= runningSymbols.Length)
                    runningSymbolVariant = 0;
            }
            runningSymbolCount++;
            return runningSymbols[runningSymbolVariant];
        }

        IMyShipController GetControlledShipController(List<IMyShipController> SCs)
        {
            foreach (IMyShipController thisController in SCs)
            {
                if (IsClosed(thisController))
                    continue;

                if (thisController.IsUnderControl && thisController.CanControlShip)
                    return thisController;
            }

            return null;
        }

        float GetMaxTurretRange(List<IMyLargeTurretBase> turrets)
        {
            float maxRange = 0;
            foreach (var block in turrets)
            {
                if (!block.IsWorking)
                    continue;

                float thisRange = block.Range;
                if (thisRange > maxRange)
                {
                    maxRange = thisRange;
                }
            }
            return maxRange;
        }

        public static bool IsClosed(IMyTerminalBlock block)
        {
            return block.WorldMatrix == MatrixD.Identity;
        }

        public static bool StringContains(string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
        #endregion

        #region Block Fetching
        bool PopulateLists(IMyTerminalBlock block)
        {
            if (!block.IsSameConstructAs(Me))
                return false;

            if (StringContains(block.CustomName, textPanelName))
            {
                AddTextSurfaces(block, textSurfaces);
            }

            var turret = block as IMyLargeTurretBase;
            if (turret != null)
            {
                turrets.Add(turret);
                return false;
            }

            var controller = block as IMyShipController;
            if (controller != null)
            {
                allControllers.Add(controller);
                if (StringContains(block.CustomName, referenceName))
                    taggedControllers.Add(controller);
                return false;
            }

            return false;
        }

        void GrabBlocks()
        {
            turrets.Clear();
            allControllers.Clear();
            taggedControllers.Clear();
            textSurfaces.Clear();

            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, PopulateLists);

            if (turrets.Count == 0)
                Log.Warning($"No turrets found. You will only be able to see targets that are broadcast by allies.");

            if (textSurfaces.Count == 0)
                Log.Error($"No text panels or text surface providers with name tag '{textPanelName}' were found.");

            if (allControllers.Count == 0)
                Log.Error($"No ship controllers were found.");
            else
            {
                if (taggedControllers.Count == 0)
                    Log.Info($"No ship controllers named \"{referenceName}\" were found. Using all available ship controllers. (This is NOT an error!)");
                else
                    Log.Info($"One or more ship controllers with name tag \"{referenceName}\" were found. Using these to orient the radar.");
            }

            lastSetupResult = Log.Write();

            if (textSurfaces.Count == 0)
                isSetup = false;
            else
            {
                isSetup = true;
                ParseCustomDataIni();
            }
        }
        #endregion

        #region Scheduler
        /// <summary>
        /// Class for scheduling actions to occur at specific frequencies. Actions can be updated in parallel or in sequence (queued).
        /// </summary>
        public class Scheduler
        {
            readonly List<ScheduledAction> _scheduledActions = new List<ScheduledAction>();
            readonly List<ScheduledAction> _actionsToDispose = new List<ScheduledAction>();
            Queue<ScheduledAction> _queuedActions = new Queue<ScheduledAction>();
            const double runtimeToRealtime = (1.0 / 60.0) / 0.0166666;
            private readonly Program _program;
            private ScheduledAction _currentlyQueuedAction = null;

            /// <summary>
            /// Constructs a scheduler object with timing based on the runtime of the input program.
            /// </summary>
            /// <param name="program"></param>
            public Scheduler(Program program)
            {
                _program = program;
            }

            /// <summary>
            /// Updates all ScheduledAcions in the schedule and the queue.
            /// </summary>
            public void Update()
            {
                double deltaTime = Math.Max(0, _program.Runtime.TimeSinceLastRun.TotalSeconds * runtimeToRealtime);

                _actionsToDispose.Clear();
                foreach (ScheduledAction action in _scheduledActions)
                {
                    action.Update(deltaTime);
                    if (action.JustRan && action.DisposeAfterRun)
                    {
                        _actionsToDispose.Add(action);
                    }
                }

                // Remove all actions that we should dispose
                _scheduledActions.RemoveAll((x) => _actionsToDispose.Contains(x));

                if (_currentlyQueuedAction == null)
                {
                    // If queue is not empty, populate current queued action
                    if (_queuedActions.Count != 0)
                        _currentlyQueuedAction = _queuedActions.Dequeue();
                }

                // If queued action is populated
                if (_currentlyQueuedAction != null)
                {
                    _currentlyQueuedAction.Update(deltaTime);
                    if (_currentlyQueuedAction.JustRan)
                    {
                        // If we should recycle, add it to the end of the queue
                        if (!_currentlyQueuedAction.DisposeAfterRun)
                            _queuedActions.Enqueue(_currentlyQueuedAction);

                        // Set the queued action to null for the next cycle
                        _currentlyQueuedAction = null;
                    }
                }
            }

            /// <summary>
            /// Adds an Action to the schedule. All actions are updated each update call.
            /// </summary>
            /// <param name="action"></param>
            /// <param name="updateFrequency"></param>
            /// <param name="disposeAfterRun"></param>
            public void AddScheduledAction(Action action, double updateFrequency, bool disposeAfterRun = false)
            {
                ScheduledAction scheduledAction = new ScheduledAction(action, updateFrequency, disposeAfterRun);
                _scheduledActions.Add(scheduledAction);
            }

            /// <summary>
            /// Adds a ScheduledAction to the schedule. All actions are updated each update call.
            /// </summary>
            /// <param name="scheduledAction"></param>
            public void AddScheduledAction(ScheduledAction scheduledAction)
            {
                _scheduledActions.Add(scheduledAction);
            }

            /// <summary>
            /// Adds an Action to the queue. Queue is FIFO.
            /// </summary>
            /// <param name="action"></param>
            /// <param name="updateInterval"></param>
            /// <param name="disposeAfterRun"></param>
            public void AddQueuedAction(Action action, double updateInterval, bool disposeAfterRun = false)
            {
                if (updateInterval <= 0)
                {
                    updateInterval = 0.001; // avoids divide by zero
                }
                ScheduledAction scheduledAction = new ScheduledAction(action, 1.0 / updateInterval, disposeAfterRun);
                _queuedActions.Enqueue(scheduledAction);
            }

            /// <summary>
            /// Adds a ScheduledAction to the queue. Queue is FIFO.
            /// </summary>
            /// <param name="scheduledAction"></param>
            public void AddQueuedAction(ScheduledAction scheduledAction)
            {
                _queuedActions.Enqueue(scheduledAction);
            }
        }

        public class ScheduledAction
        {
            public bool JustRan { get; private set; } = false;
            public bool DisposeAfterRun { get; private set; } = false;
            public double TimeSinceLastRun { get; private set; } = 0;
            public readonly double RunInterval;

            private readonly double _runFrequency;
            private readonly Action _action;
            protected bool _justRun = false;

            /// <summary>
            /// Class for scheduling an action to occur at a specified frequency (in Hz).
            /// </summary>
            /// <param name="action">Action to run</param>
            /// <param name="runFrequency">How often to run in Hz</param>
            public ScheduledAction(Action action, double runFrequency, bool removeAfterRun = false)
            {
                _action = action;
                _runFrequency = runFrequency;
                RunInterval = 1.0 / _runFrequency;
                DisposeAfterRun = removeAfterRun;
            }

            public virtual void Update(double deltaTime)
            {
                TimeSinceLastRun += deltaTime;

                if (TimeSinceLastRun >= RunInterval)
                {
                    _action.Invoke();
                    TimeSinceLastRun = 0;

                    JustRan = true;
                }
                else
                {
                    JustRan = false;
                }
            }
        }
        #endregion

        #region Script Logging
        public static class Log
        {
            static StringBuilder _builder = new StringBuilder();
            static List<string> _errorList = new List<string>();
            static List<string> _warningList = new List<string>();
            static List<string> _infoList = new List<string>();
            const int _logWidth = 530; //chars, conservative estimate

            public static void Clear()
            {
                _builder.Clear();
                _errorList.Clear();
                _warningList.Clear();
                _infoList.Clear();
            }

            public static void Error(string text)
            {
                _errorList.Add(text);
            }

            public static void Warning(string text)
            {
                _warningList.Add(text);
            }

            public static void Info(string text)
            {
                _infoList.Add(text);
            }

            public static string Write(bool preserveLog = false)
            {
                //WriteLine($"Error count: {_errorList.Count}");
                //WriteLine($"Warning count: {_warningList.Count}");
                //WriteLine($"Info count: {_infoList.Count}");

                if (_errorList.Count != 0 && _warningList.Count != 0 && _infoList.Count != 0)
                    WriteLine("");

                if (_errorList.Count != 0)
                {
                    for (int i = 0; i < _errorList.Count; i++)
                    {
                        WriteLine("");
                        WriteElememt(i + 1, "ERROR", _errorList[i]);
                        //if (i < _errorList.Count - 1)
                    }
                }

                if (_warningList.Count != 0)
                {
                    for (int i = 0; i < _warningList.Count; i++)
                    {
                        WriteLine("");
                        WriteElememt(i + 1, "WARNING", _warningList[i]);
                        //if (i < _warningList.Count - 1)
                    }
                }

                if (_infoList.Count != 0)
                {
                    for (int i = 0; i < _infoList.Count; i++)
                    {
                        WriteLine("");
                        WriteElememt(i + 1, "Info", _infoList[i]);
                        //if (i < _infoList.Count - 1)
                    }
                }

                string output = _builder.ToString();

                if (!preserveLog)
                    Clear();

                return output;
            }

            private static void WriteElememt(int index, string header, string content)
            {
                WriteLine($"{header} {index}:");

                string wrappedContent = TextHelper.WrapText(content, 1, _logWidth);
                string[] wrappedSplit = wrappedContent.Split('\n');

                foreach (var line in wrappedSplit)
                {
                    _builder.Append("  ").Append(line).Append('\n');
                }
            }

            private static void WriteLine(string text)
            {
                _builder.Append(text).Append('\n');
            }
        }

        // Whip's TextHelper Class v2
        public class TextHelper
        {
            static StringBuilder textSB = new StringBuilder();
            const float adjustedPixelWidth = (512f / 0.778378367f);
            const int monospaceCharWidth = 24 + 1; //accounting for spacer
            const int spaceWidth = 8;

            #region bigass dictionary
            static Dictionary<char, int> _charWidths = new Dictionary<char, int>()
{
{'.', 9},
{'!', 8},
{'?', 18},
{',', 9},
{':', 9},
{';', 9},
{'"', 10},
{'\'', 6},
{'+', 18},
{'-', 10},

{'(', 9},
{')', 9},
{'[', 9},
{']', 9},
{'{', 9},
{'}', 9},

{'\\', 12},
{'/', 14},
{'_', 15},
{'|', 6},

{'~', 18},
{'<', 18},
{'>', 18},
{'=', 18},

{'0', 19},
{'1', 9},
{'2', 19},
{'3', 17},
{'4', 19},
{'5', 19},
{'6', 19},
{'7', 16},
{'8', 19},
{'9', 19},

{'A', 21},
{'B', 21},
{'C', 19},
{'D', 21},
{'E', 18},
{'F', 17},
{'G', 20},
{'H', 20},
{'I', 8},
{'J', 16},
{'K', 17},
{'L', 15},
{'M', 26},
{'N', 21},
{'O', 21},
{'P', 20},
{'Q', 21},
{'R', 21},
{'S', 21},
{'T', 17},
{'U', 20},
{'V', 20},
{'W', 31},
{'X', 19},
{'Y', 20},
{'Z', 19},

{'a', 17},
{'b', 17},
{'c', 16},
{'d', 17},
{'e', 17},
{'f', 9},
{'g', 17},
{'h', 17},
{'i', 8},
{'j', 8},
{'k', 17},
{'l', 8},
{'m', 27},
{'n', 17},
{'o', 17},
{'p', 17},
{'q', 17},
{'r', 10},
{'s', 17},
{'t', 9},
{'u', 17},
{'v', 15},
{'w', 27},
{'x', 15},
{'y', 17},
{'z', 16}
};
            #endregion

            public static int GetWordWidth(string word)
            {
                int wordWidth = 0;
                foreach (char c in word)
                {
                    int thisWidth = 0;
                    bool contains = _charWidths.TryGetValue(c, out thisWidth);
                    if (!contains)
                        thisWidth = monospaceCharWidth; //conservative estimate

                    wordWidth += (thisWidth + 1);
                }
                return wordWidth;
            }

            public static string WrapText(string text, float fontSize, float pixelWidth = adjustedPixelWidth)
            {
                textSB.Clear();
                var words = text.Split(' ');
                var screenWidth = (pixelWidth / fontSize);
                int currentLineWidth = 0;
                foreach (var word in words)
                {
                    if (currentLineWidth == 0)
                    {
                        textSB.Append($"{word}");
                        currentLineWidth += GetWordWidth(word);
                        continue;
                    }

                    currentLineWidth += spaceWidth + GetWordWidth(word);
                    if (currentLineWidth > screenWidth) //new line
                    {
                        currentLineWidth = GetWordWidth(word);
                        textSB.Append($"\n{word}");
                    }
                    else
                    {
                        textSB.Append($" {word}");
                    }
                }

                return textSB.ToString();
            }
        }
        #endregion

        #region Runtime Tracking
        /// <summary>
        /// Class that tracks runtime history.
        /// </summary>
        public class RuntimeTracker
        {
            public int Capacity { get; set; }
            public double Sensitivity { get; set; }
            public double MaxRuntime { get; private set; }
            public double MaxInstructions { get; private set; }
            public double AverageRuntime { get; private set; }
            public double AverageInstructions { get; private set; }

            private readonly Queue<double> _runtimes = new Queue<double>();
            private readonly Queue<double> _instructions = new Queue<double>();
            private readonly StringBuilder _sb = new StringBuilder();
            private readonly int _instructionLimit;
            private readonly Program _program;

            public RuntimeTracker(Program program, int capacity = 100, double sensitivity = 0.01)
            {
                _program = program;
                Capacity = capacity;
                Sensitivity = sensitivity;
                _instructionLimit = _program.Runtime.MaxInstructionCount;
            }

            public void AddRuntime()
            {
                double runtime = _program.Runtime.LastRunTimeMs;
                AverageRuntime = Sensitivity * (runtime - AverageRuntime) + AverageRuntime;

                _runtimes.Enqueue(runtime);
                if (_runtimes.Count == Capacity)
                {
                    _runtimes.Dequeue();
                }

                MaxRuntime = _runtimes.Max();
            }

            public void AddInstructions()
            {
                double instructions = _program.Runtime.CurrentInstructionCount;
                AverageInstructions = Sensitivity * (instructions - AverageInstructions) + AverageInstructions;

                _instructions.Enqueue(instructions);
                if (_instructions.Count == Capacity)
                {
                    _instructions.Dequeue();
                }

                MaxInstructions = _instructions.Max();
            }

            public string Write()
            {
                _sb.Clear();
                _sb.AppendLine("\n_____________________________\nGeneral Runtime Info\n");
                _sb.AppendLine($"Avg instructions: {AverageInstructions:n2}");
                _sb.AppendLine($"Max instructions: {MaxInstructions:n0}");
                _sb.AppendLine($"Avg complexity: {MaxInstructions / _instructionLimit:0.000}%");
                _sb.AppendLine($"Avg runtime: {AverageRuntime:n4} ms");
                _sb.AppendLine($"Max runtime: {MaxRuntime:n4} ms");
                return _sb.ToString();
            }
        }
        #endregion
        #endregion

    }
}