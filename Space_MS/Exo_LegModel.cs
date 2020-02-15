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
    partial class Program
    {

        /// <summary>
        /// 脚部クラス
        /// </summary>
        public class Exo_LegModel : LegModel
        {
            private DateTime nowTime;
            private DateTime befTime;

            // public MoterModel myMotorKnee;


            // public MoterModel myMotorThigh;


            // public MoterModel myMotorAnkle;


            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="m1">第１関節</param>

            /// <param name="m2">第２関節</param>

            /// <param name="m3">第３関節</param>

            /// <param name="m4">第4関節</param>
           
            public Exo_LegModel(IMyMotorStator m1,IMyMotorStator m2, IMyMotorStator m3,IMyMotorStator m4)
            {
                moters = new List<MoterModel>();
                moters.Add(new MoterModel(m1, new MotorOperationDataEntity(0, 0, true)));
                moters.Add(new MoterModel(m2, new MotorOperationDataEntity(0, 0, true)));
                moters.Add(new MoterModel(m3, new MotorOperationDataEntity(0, 0, true)));
                moters.Add(new MoterModel(m4, new MotorOperationDataEntity(0, 0, true)));
            }


            /// <summary>
            /// Movinegmoter
            /// </summary>
            /// <returns>Finisih?</returns>
            public bool Drive()
            {

                bool returnvalue = false;

                nowTime = DateTime.UtcNow;


                foreach (var item in moters)
                {
                    item.Update();
                }


                befTime = nowTime;

                return returnvalue;
            }

            public void getData()
            { 
                
            }

            public void setData(PartOperationDataEntityList dataEntityList)
            {
                //dataEntityList.
                foreach (var moter in moters)
                {
                    MotorOperationFormatter.setDataEntityToMotorCustomData(dataEntity, ref moter);
                }
              
            }
            
        }
    }
}
