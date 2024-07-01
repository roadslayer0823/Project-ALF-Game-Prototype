using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 基本回復效果相關數值結算
    public static void RunBasicRecoveryEffects( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter )
    {
        CategorizedPassiveSkillManager categorizedPassiveSkillManager = null;
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = null;

        float skill_PSL1_HuoXin_VirtualHealthPoint = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL1)) ? 0.05f : 0.0f;
        float skill_PSL1_HuoXin_MaxStatePoint = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL1)) ? 0.8f : 0.0f;
        float skill_PSL12_ShengShengBuXi = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12)) ? 0.5f : 0.0f;
        float skill_PSE1_GaoYang_Value = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE1)) ? 10 : 0f;
        float skill_PSE7_FuHeLiuZhuan_Value2 = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE7)) ? 10 : 0f;
        float skill_PSS1_JieYa_CurrentStressValue = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS1)) ? 10 : 0f;
        float skill_PSS1_JieYa_MaxStatePoint = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS1)) ? 0.8f : 0.0f;

        //character latest game data
        float virtualHealthPointRecover_value = 0.05f;
        switch (gameCharacter.GetSelectedPassiveSkillCategoryType())
        {
            case CategoryType.Life: //CASE A:"己方"當前流向為"生命流"
                
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, virtualHealthPointRecover_value + skill_PSL1_HuoXin_VirtualHealthPoint, out gameCharacterData); //["己方"虛傷]-["己方"最大生命值]*[0.05+0.05]>
               
                if (battleResultData.GetGameCharacterResultData(gameCharacter).virtualHealthPoint == 0)  //"己方"虛傷為0時,剩下的"虛傷回復值"改為"溢出虛傷回復值", "溢出虛傷回復值"是否>=1? 
                {
                    //"己方"生命積分是否 >= 250 ?
                    //["己方"生命值]+["己方"溢出虛傷回復值*skill_L12_ShengShengBuXi)
                }

                if(gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數 ?
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData);  //["己方"最大以太值]+["己方"當前以太值]
                }

                if(gameCharacterData.currentStatePoint > 0) //"己方"當前以太值不是負數
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData); //["己方"最大以太值]+10
                }

                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10, out gameCharacterData); //["己方"負荷值]-10
                }

                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    if (gameCharacterData.currentHealthPoint < (gameCharacterData.currentHealthPoint * 0.8f)) //["己方"當前以太值]是否 <["己方"當前以太值]的80 %?
                    {
                        //["己方"當前以太值]=["己方"最大以太值 * 0.8(skill_L1_HuoXin_MaximumStatePoint)]
                    }
                }
                break;

            case CategoryType.State:
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, 0.5f, out gameCharacterData); //["己方"虛傷]-["己方"最大生命值]*[0.5]
                if(gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData); //["己方"最大以太值]+["己方"當前以太值]
                }
                if(gameCharacterData.currentStatePoint > 0)
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10 + skill_PSE1_GaoYang_Value, out gameCharacterData); //["己方"最大以太值]+10 + 10
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, ((gameCharacterData.maximumStatePoint >= 180)) ? skill_PSE7_FuHeLiuZhuan_Value2 + 10 : 0, out gameCharacterData); //["己方"負荷值]-10-10
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_FullyRestoreCurrentStatePoint(gameCharacter, out gameCharacterData); //["己方"當前以太值] =["己方"最大以太值] 
                }
                break;

            case CategoryType.Stress:
                break;

            case CategoryType.None:
                break;
        }
    }
}
