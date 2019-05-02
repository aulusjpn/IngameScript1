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

namespace IngameScript
{
    partial class Program
    {
        public class Display
        {

            public static Vector3D fromVector;
            public static Vector3D toVector;
            public static MatrixD toMatrix;
            public static Vector3D dispVector;
            public static MatrixD dispMatrix;
            public static IMyEntity fromentity;
            public static IMyEntity dispentiy;


            public static void calc()
            {
                //var vector1 = fromVector - toVector;
                //var vector2 = fromentity.WorldMatrix.Right;
                //  dispVector = Vector3D.ProjectOnPlane(ref vector1, ref vector2);
                var centervector = fromVector + (fromentity.WorldMatrix.Forward * 50);
                var vec = Vector3D.Transform(toVector, MatrixD.CreateLookAt(fromVector, centervector, fromentity.WorldMatrix.Up));

                MatrixD ViewTransMatrix = MatrixD.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1, 1, 50);
                //ViewTransMatrix.Translation = fromentity.GetPosition();
                //ViewTransMatrix.Forward = fromentity.WorldMatrix.Forward;
                //ViewTransMatrix.Up = fromentity.WorldMatrix.Up;
                ////dispMatrix = MatrixD.Invert(toMatrix) *ViewTransMatrix;
                dispVector = Vector3D.Transform(vec, ViewTransMatrix);
            }


        }
    }
}
