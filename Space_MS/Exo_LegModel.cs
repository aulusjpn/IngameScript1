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

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="m1">第１関節</param>

            /// <param name="m2">第２関節</param>

            /// <param name="m3">第３関節</param>

            /// <param name="m4">第4関節</param>
           
            public Exo_LegModel(IMyMotorStator m1,IMyMotorStator m2, IMyMotorStator m3,IMyMotorStator m4)
            {
                this.Moters = new Dictionary<string, MoterModel>();
                this.Moters.Add("1",new MoterModel(m1, new MotorOperationDataEntity(0, 0, true)));
                this.Moters.Add("2",new MoterModel(m2, new MotorOperationDataEntity(0, 0, true)));
                this.Moters.Add("3",new MoterModel(m3, new MotorOperationDataEntity(0, 0, true)));
                this.Moters.Add("4",new MoterModel(m4, new MotorOperationDataEntity(0, 0, true)));
                entiityDictionaly = new Dictionary<MoterModel, MotorOperationDataEntity>();

                foreach (var moter in this.Moters)
                {
                    entiityDictionaly.Add(moter.Value, new MotorOperationDataEntity());
                }
        
            }

            public void setData(MoterModel moter ,MotorOperationDataEntity dataEntity)
            {
                this.entiityDictionaly[moter] = dataEntity;
            }

            /// <summary>
            /// Movinegmoter
            /// </summary>
            /// <returns>Finisih?</returns>
            public bool Drive()
            {


                bool returnvalue = false;

                nowTime = DateTime.UtcNow;


                foreach (var item in Moters)
                {
                    item.Value.Update(entiityDictionaly[item.Value]);
                }


                befTime = nowTime;

                return returnvalue;
            }
            
        }
    }
}
