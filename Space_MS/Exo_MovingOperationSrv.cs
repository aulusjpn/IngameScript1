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
            bool nextActionFlg = false;
            int ActionNo = 0;

            public Exo_MovingOperationSrv(Exo_LegModel rLeg, Exo_LegModel lLeg, Exo_ArmModel rArm, Exo_ArmModel lArm) : base(rLeg, lLeg, rArm, lArm)
            {

            }

            public override void updateArmPartOparationData(PartOperationDataEntityList dataEntityList, ArmModel armModel)
            {
                

            }

            public override void updateLegPartOparationData(PartOperationDataEntityList dataEntityList, LegModel legModel)
            {}

            private void updatePartsData()
            {

            }

            public override void DriveLeg(IMyCockpit cockpit)
            {
                var move = cockpit.MoveIndicator.Z;

                bool finish = false;

            }

            public override void armTarget(IMyCockpit Rota)
            {

            }

            PartOperationDataEntityList createPartODEList(Part part, List<MotorOperationDataEntity> dataEntities)
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

        }


    }
}
