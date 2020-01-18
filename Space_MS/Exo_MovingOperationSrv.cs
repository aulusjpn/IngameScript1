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
        public class Exo_MovingOperationSrv : OperationServiceBase
        {
            List<PartOperationDataEntityList> operationList_rLeg = new List<PartOperationDataEntityList>();
            List<PartOperationDataEntityList> operationList_lLeg = new List<PartOperationDataEntityList>();
            List<PartOperationDataEntityList> operationList_rArm = new List<PartOperationDataEntityList>();
            List<PartOperationDataEntityList> operationList_lArm = new List<PartOperationDataEntityList>();
            
            public Exo_MovingOperationSrv(Exo_LegModel rLeg, Exo_LegModel lLeg, Exo_ArmModel rArm, Exo_ArmModel lArm) : base(rLeg, lLeg, rArm, lArm)
            {
                var list = Exo_OperationDataList.stble;

                PartOperationDataEntityList buff = new PartOperationDataEntityList(rLeg);
                Dictionary<MoterModel, MotorOperationDataEntity> buffdic = new Dictionary<MoterModel, MotorOperationDataEntity>();

                for (int i = 0; i < rLeg.moters.Count; i++)
                {
                    buffdic.Add(rLeg.moters[i],list[0][i]);
                }

                buff.entiityDictionaly = buffdic;
                operationList_rLeg.Add(buff);

                buff = new PartOperationDataEntityList(lLeg);
                buffdic = new Dictionary<MoterModel, MotorOperationDataEntity>();
                for (int i = 0; i < lLeg.moters.Count; i++)
                {
                    buffdic.Add(lLeg.moters[i], list[0][i]);
                }

                buff.entiityDictionaly = buffdic;
                operationList_lLeg.Add(buff);

                buff = new PartOperationDataEntityList(rArm);
                buffdic = new Dictionary<MoterModel, MotorOperationDataEntity>();
                for (int i = 0; i < rArm.moters.Count; i++)
                {
                    buffdic.Add(rArm.moters[i], list[0][i]);
                }

                buff.entiityDictionaly = buffdic;
                operationList_rArm.Add(buff);

                buff = new PartOperationDataEntityList(lArm);
                buffdic = new Dictionary<MoterModel, MotorOperationDataEntity>();
                for (int i = 0; i < lArm.moters.Count; i++)
                {
                    buffdic.Add(lArm.moters[i], list[0][i]);
                }

                buff.entiityDictionaly = buffdic;
                operationList_lArm.Add(buff);            


            }

            public override void armTarget(IMyCockpit Rota)
            {
                throw new NotImplementedException();
            }

            public override void Drive(IMyCockpit cockpit)
            {
                foreach (var item in operationList_lArm)
                {
                    foreach (var moter in item.entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                        moter.Key.Update();
                    }
                }

                foreach (var item in operationList_lLeg)
                {
                    foreach (var moter in item.entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                        moter.Key.Update();
                    }
                }
                foreach (var item in operationList_rArm)
                {
                    foreach (var moter in item.entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                        moter.Key.Update();
                    }
                }
                foreach (var item in operationList_rLeg)
                {
                    foreach (var moter in item.entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                        moter.Key.Update();
                    }
                }

            }

        }
    }
}
