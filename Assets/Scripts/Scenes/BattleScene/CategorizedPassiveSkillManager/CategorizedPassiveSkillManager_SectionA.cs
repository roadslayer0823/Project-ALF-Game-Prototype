using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 基本回復效果相關數值結算
    public static void RunBasicRecoveryEffects( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter )
    {
        //reference
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        //skill variable
        float skill_PSL1_HuoXin_VirtualHealthPoint = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL1)) ? 0.05f : 0.0f;
        float skill_PSL1_HuoXin_MaxStatePoint = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL1)) ? 0.8f : 0.0f;
        float skill_PSL12_ShengShengBuXi = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12)) ? 0.5f : 0.0f;

        float skill_PSE1_GaoYang_Value = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE1)) ? 10 : 0f;
        float skill_PSE7_FuHeLiuZhuan_Value2 = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE7)) ? 10 : 0f;

        float skill_PSS1_JieYa_CurrentStressValue = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS1)) ? 10 : 0f;
        float skill_PSS1_JieYa_MaxStatePoint = (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS1)) ? 0.8f : 0.0f;

        //character latest game data
        float virtualHealthPointRecover_value = 0.05f;
        Debug.Log("基本回復效果相關數值結算");
        switch (gameCharacter.GetSelectedPassiveSkillCategoryType())
        {
            case CategoryType.Life:
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, virtualHealthPointRecover_value * (gameCharacterData.maximumHealthPoint + skill_PSL1_HuoXin_VirtualHealthPoint), out gameCharacterData); //["己方"虛傷]-["己方"最大生命值]*[0.05+0.05]
                Debug.Log("virtual health point recover");
                if (gameCharacterData.GetVirtualDamage() >= 1)  //"己方"虛傷為0時,剩下的"虛傷回復值"改為"溢出虛傷回復值", "溢出虛傷回復值"是否>=1? 
                {
                    //"己方"生命積分是否 >= 250 ?
                    battleResultData.AddGameCharacterResultData_ActualHealthPointDamageRecovered(gameCharacter, gameCharacterData.GetVirtualDamage() * skill_PSL12_ShengShengBuXi, out gameCharacterData); //["己方"生命值]+["己方"溢出虛傷回復值*skill_L12_ShengShengBuXi)
                    Debug.Log("health point recover");
                }

                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是負數
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData);  //["己方"最大以太值]+["己方"當前以太值]
                    Debug.Log("max state point increase because stress point less than 0 with current state point");
                }

                if(gameCharacterData.currentStatePoint > 0) //"己方"當前以太值不是負數
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData); //["己方"最大以太值]+10
                    Debug.Log("max state point increase because more than 0");
                }

                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10, out gameCharacterData); //["己方"負荷值]-10
                    Debug.Log("break because of stress value");
                }

                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    Debug.Log("break because of state point");
                    if (gameCharacterData.currentStatePoint < (gameCharacterData.currentStatePoint * 0.8f)) //["己方"當前以太值]是否 <["己方"當前以太值]的80 %?
                    {
                        battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, gameCharacterData.maximumStatePoint * skill_PSL1_HuoXin_MaxStatePoint, out _); //["己方"當前以太值]=["己方"最大以太值 * 0.8(skill_L1_HuoXin_MaximumStatePoint)]
                        Debug.Log("restore current state point as 80%"); 
                    }
                }
                else
                {
                    Debug.Log("no any effect activate");
                }
                break;

            case CategoryType.State:
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * 0.5f, out gameCharacterData); //["己方"虛傷]-["己方"最大生命值]*[0.5]
                Debug.Log("virtual health point recover");
                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData); //["己方"最大以太值]+["己方"當前以太值]
                    Debug.Log("max state point increase because stress point less than 0");
                }
                if(gameCharacterData.currentStatePoint > 0)
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10 + skill_PSE1_GaoYang_Value, out gameCharacterData); //["己方"最大以太值]+10 + skill_PSE1_GaoYang_Value
                    Debug.Log("max state point increase because stress point less than 0 with PSE1 skill");
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    //["己方"負荷值]-10- "己方"最大以太值是否 >= 180 ? YES = 10(發動7.負荷流轉2) NO = 0
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, ((gameCharacterData.maximumStatePoint >= 180)) ? skill_PSE7_FuHeLiuZhuan_Value2 + 10 : 0, out gameCharacterData);
                    Debug.Log("stress value recover with PSE7");
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint( gameCharacter, 1.0f, out _); //["己方"當前以太值] = ["己方"最大以太值]
                    Debug.Log("restore curent state point");
                }
                break;

            case CategoryType.Stress:
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * 0.5f, out gameCharacterData); //["己方"虛傷]-["己方"最大生命值] *[0.5]
                Debug.Log("virtual health point recover");
                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData); //["己方"最大以太值]+["己方"當前以太值]
                    Debug.Log("max state point increase because stress point less than 0");
                }
                if (gameCharacterData.currentStatePoint > 0)
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData); //["己方"最大以太值]+10
                    Debug.Log("max state point increase because stress point more than 0");
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10 + skill_PSS1_JieYa_CurrentStressValue, out gameCharacterData);//["己方"負荷值]-10 - skill_PSS1_JieYa_CurrentStressValue
                    Debug.Log("did not break by stress point");
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    Debug.Log("did not break by state point");
                    if (gameCharacterData.currentStatePoint < gameCharacterData.currentStatePoint * 0.8f) //["己方"當前以太值]是否<["己方"當前以太值]的80 %?
                    {
                        battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, gameCharacterData.maximumStatePoint*skill_PSS1_JieYa_MaxStatePoint, out _); //["己方"當前以太值]=["己方"最大以太值*skill_PSS1_JieYa_MaxStatePoint]
                        Debug.Log("restore curent state point from 80% of maximum state point");
                    }
                }
                break;

            case CategoryType.None:
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * 0.5f, out gameCharacterData); //["己方"虛傷]-["己方"最大生命值] *[0.5]
                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData); //["己方"最大以太值]+["己方"當前以太值]
                    Debug.Log("max state point increase because stress point less than 0");
                }
                if (gameCharacterData.currentStatePoint > 0)
                {
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData); //["己方"最大以太值]+10
                    Debug.Log("max state point increase because stress point more than 0");
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10, out gameCharacterData);//["己方"負荷值]-10
                    Debug.Log("no break by stress value");
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, 1.0f, out _); //["己方"當前以太值]=["己方"最大以太值]
                    Debug.Log("no break by state point");
                }
                break;
        }
    }

    //生命流系統數值相關結算
    public static void CalculateLifeScoreEffect(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter, bool isDeallingHealthPointDamage)
    {
        //reference
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float skill_PSL8_NiJingliuZhuan_Value150 = gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL8) ? 1.5f : 0.0f;
        float skill_PSL8_NiJingliuZhuan_Value100 = gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL8) ? 1.0f : 0.0f;
        float attackTargetMaxHealthPoint = gameCharacter.GetCurrentAttackTarget().GetMaximumHealthPoint() * 0.01f;
        float damageTakerMaxHealthPoint = gameCharacterData.maximumHealthPoint * 0.01f;
        float gainLifeScore = 0;

        if (isDeallingHealthPointDamage == true)  //"己方"給予HP傷害後(包括實傷 & 虛傷)
        {
            //[給予HP總傷害]每達到[對方最大生命值]的1 %="己方"生命積分 + 1。
            if (gameCharacterData.actualHealthPointDamageDealt > 0)
            {
                gainLifeScore = gameCharacterData.actualHealthPointDamageDealt / attackTargetMaxHealthPoint;
                Debug.Log("dealing actual damage");
            }
            if(gameCharacterData.virtualHealthPointDamageDealt > 0)
            {
                gainLifeScore = gameCharacterData.virtualHealthPointDamageDealt / attackTargetMaxHealthPoint;
                Debug.Log("dealing virtual damage");
            }
        }
        else //"己方"受到HP傷害後(包括實傷 & 虛傷)
        {
            //[受到HP總傷害]每達到[己方最大生命值]的1 %="己方"生命積分 + 1。
            if (gameCharacterData.actualHealthPointDamageTaken > 0)
            {
                gainLifeScore = gameCharacterData.actualHealthPointDamageTaken / damageTakerMaxHealthPoint;
                Debug.Log("taking actual damage");
            }
            if (gameCharacterData.virtualHealthPointDamageTaken > 0)
            {
                gainLifeScore = gameCharacterData.virtualHealthPointDamageTaken / damageTakerMaxHealthPoint;
                Debug.Log("taking virtual damage");
            }
        }
        
        if(gameCharacterData.currentHealthPoint < gameCharacterData.maximumHealthPoint * 0.3f) //"己方"是否< HP30 %
        {
            gainLifeScore += gainLifeScore * skill_PSL8_NiJingliuZhuan_Value150; //發動8.逆境流轉, 得到生命積分 + 150%。
        }
        else if(gameCharacterData.currentHealthPoint < gameCharacterData.maximumHealthPoint * 0.5f) //"己方"是否< HP50 %
        {
            gainLifeScore += gainLifeScore * skill_PSL8_NiJingliuZhuan_Value100; //發動8.逆境流轉, 得到生命積分 + 100%。
        }

        gameCharacter.AddLifeScore(Mathf.RoundToInt(gainLifeScore));
    }
}
