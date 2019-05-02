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
        bool ini = false;
        IMyTextSurfaceProvider block;
        IMyTextSurface surface;
        IMySensorBlock sensor;
        IMyCockpit cockpit;
        float rap = 0;
        public Program()
        {
            // The constructor, called only once every session and 
            // always before any other method is called. Use it to 
            // initialize your script.  
            //      
            // The constructor is optional and can be removed if not 
            // needed.
            this.block = Me;
            surface = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("Panel");
            sensor = (IMySensorBlock)GridTerminalSystem.GetBlockWithName("Sensor");
            cockpit = (IMyCockpit)GridTerminalSystem.GetBlockWithName("Azimuth Open Cockpit");
            // set surface contentType
            // this is selecting type on terminal
            surface.ContentType = ContentType.SCRIPT;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
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

        private void Ini()
        {
            if (!ini)
            {
                surface = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("Panel");
                // set surface contentType
                // this is selecting type on terminal
                surface.ContentType = ContentType.SCRIPT;

                ini = true;
            }
        }

        public void Main(string argument)
        {

            Display.dispentiy = (IMyEntity)surface;
            Display.fromentity = cockpit;
            Display.fromVector = cockpit.WorldVolume.Center;
            
            //Display.toVector = new Vector3D(-11.44,25.44,17.66);
            Display.toVector = sensor.LastDetectedEntity.Position;
            Display.toMatrix = sensor.LastDetectedEntity.BoundingBox.Matrix;
            Display.calc();


            using (MySpriteDrawFrame frame = surface.DrawFrame())
            {
                MySprite sprite;
                
                // Background
                sprite = new MySprite(
                    SpriteType.TEXTURE,
                    "CircleHollow",  // "Triangle" or "Circle"
                    size: new Vector2(512.0f, 512.0f),
                    color: new Color(0, 0, 0, 60)
                );
                sprite.Position = new Vector2(256.0f, 256.0f);
                //sprite.Color= new Color(255,0,0,255);
                frame.Add(sprite);

                IMyTextPanel t;
               

                rap += 5f;
                sprite = new MySprite(
                    SpriteType.TEXTURE,
                    "CircleHollow",  // "Triangle" or "Circle"
                    size: new Vector2(rap, rap),
                    color: new Color(128, 128, 128, 255)
                );
                sprite.Position = new Vector2(256.0f, 256.0f);
                //sprite.Color= new Color(255,0,0,255);
                frame.Add(sprite);

                if (rap > 512)
                {
                    rap = 0;
                }

        //        sprite = new MySprite(
        //SpriteType.TEXTURE,
        //"DecorativeBracketLeft",
        //size: new Vector2(60, 60),
        //color: new Color(100, 80, 155, 200)
        //    );
        //        sprite.Position = new Vector2(150f, 256.0f);
        //        frame.Add(sprite);


                sprite = new MySprite(
        SpriteType.TEXTURE,
        "DecorativeBracketRight",
        size: new Vector2(60, 60),
        color: new Color(100, 80, 155, 200)
            );

                //var vec = Vector3D.Transform(Display.dispVector, Display.fromentity.WorldMatrix);
                sprite.Position = new Vector2(256+((float)Display.dispVector.X)*128,256-((float)Display.dispVector.Y)*128);
                frame.Add(sprite);

               // sprite = MySprite.CreateText(Display.dispVector.X.ToString("###")+ " /__/ " + Display.dispVector.Y.ToString("###")+ " /__/ " + Display.dispVector.Z.ToString("###"), "UTF8",new Color(100,100,100));
                //frame.Add(sprite);

            }
        }
    }
}