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

            bool nextActionFlg = false;
            int ActionNo = 0;
            
            public Exo_MovingOperationSrv(Exo_LegModel rLeg, Exo_LegModel lLeg, Exo_ArmModel rArm, Exo_ArmModel lArm) : base(rLeg, lLeg, rArm, lArm)
            {
                var list = Exo_OperationDataList.stble;
                operationList_rLeg.Add(createPartODEList(rLeg, list[0]));
                operationList_lLeg.Add(createPartODEList(lLeg, list[1]));
                operationList_rArm.Add(createPartODEList(rArm, list[2]));
                operationList_lArm.Add(createPartODEList(lArm, list[3]));


                list = Exo_OperationDataList.walk_1;
                operationList_rLeg.Add(createPartODEList(rLeg, list[0]));
                operationList_lLeg.Add(createPartODEList(lLeg, list[1]));
                operationList_rArm.Add(createPartODEList(rArm, list[2]));
                operationList_lArm.Add(createPartODEList(lArm, list[3]));

                list = Exo_OperationDataList.walk_2;
                operationList_rLeg.Add(createPartODEList(rLeg, list[0]));
                operationList_lLeg.Add(createPartODEList(lLeg, list[1]));
                operationList_rArm.Add(createPartODEList(rArm, list[2]));
                operationList_lArm.Add(createPartODEList(lArm, list[3]));

                list = Exo_OperationDataList.walk_3;
                operationList_rLeg.Add(createPartODEList(rLeg, list[0]));
                operationList_lLeg.Add(createPartODEList(lLeg, list[1]));
                operationList_rArm.Add(createPartODEList(rArm, list[2]));
                operationList_lArm.Add(createPartODEList(lArm, list[3]));

                list = Exo_OperationDataList.walk_4;
                operationList_rLeg.Add(createPartODEList(rLeg, list[0]));
                operationList_lLeg.Add(createPartODEList(lLeg, list[1]));
                operationList_rArm.Add(createPartODEList(rArm, list[2]));
                operationList_lArm.Add(createPartODEList(lArm, list[3]));

            }

            PartOperationDataEntityList createPartODEList(Part part,List<MotorOperationDataEntity> dataEntities)
            {
                var entityList = new PartOperationDataEntityList(part);
                var dictionary = new Dictionary<MoterModel, MotorOperationDataEntity>();

                for (int i = 0; i < part.moters.Count; i++)
                {
                    dictionary.Add(part.moters[i], dataEntities[i]);
                }

                entityList.entiityDictionaly = dictionary;


                return entityList;
            }

            public override void armTarget(IMyCockpit Rota)
            {
                //throw new NotImplementedException();
            }



    

            public override void Drive(IMyCockpit cockpit)
            {
                var move = cockpit.MoveIndicator.Z;
                bool finish = false;



                foreach (var moter in operationList_rLeg[ActionNo].entiityDictionaly)
                {
                    var flg = moter.Key.Update();
                    if (flg && !finish) finish = true;
                }
                foreach (var moter in operationList_lLeg[ActionNo].entiityDictionaly)
                {
                    var flg = moter.Key.Update();
                    if (flg && !finish) finish = true;
                }
                foreach (var moter in operationList_rArm[ActionNo].entiityDictionaly)
                {
                    var flg = moter.Key.Update();
                    if (flg && !finish) finish = true;
                }
                foreach (var moter in operationList_lArm[ActionNo].entiityDictionaly)
                {
                    var flg = moter.Key.Update();
                    if (flg && !finish) finish = true;
                }

                if (finish || ActionNo == 0)
                {
                    if (move < 0)
                    {
                        if (ActionNo < 4)
                        {
                            ActionNo += 1;
                        }
                        else
                        {
                            ActionNo = 1;
                        }
                    }
                    else if (move > 0)
                    {
                        if (ActionNo > 0)
                        {
                            ActionNo -= 1;
                        }
                        else
                        {
                            ActionNo = 4;
                        }
                    }
                    else
                    {
                        ActionNo = 0;
                    }

                    foreach (var moter in operationList_rLeg[ActionNo].entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                    }
                    foreach (var moter in operationList_lLeg[ActionNo].entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                    }
                    foreach (var moter in operationList_rArm[ActionNo].entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                    }
                    foreach (var moter in operationList_lArm[ActionNo].entiityDictionaly)
                    {
                        moter.Key.dataEntity = moter.Value;
                    }
                }


                //if (!finish) finish = true;
            }

        }
    }
}
