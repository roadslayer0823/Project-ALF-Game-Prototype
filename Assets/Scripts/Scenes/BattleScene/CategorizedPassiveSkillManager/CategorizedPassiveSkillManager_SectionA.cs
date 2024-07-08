using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PassiveSkill = DatabaseManager.PassiveSkill;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 基本回復效果相關數值結算
    public static void RunBasicRecoveryEffects( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter )
    {
        //reference
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        //skill variable
        float skill_PSL1_HuoXin_VirtualHealthPoint = 0.0f;
        float skill_PSL1_HuoXin_MaxStatePoint = 0.0f;
        float skill_PSL12_ShengShengBuXi = 0.0f;

        float skill_PSE1_GaoYang_Value = 0.0f;
        float skill_PSE7_FuHeLiuZhuan_Value2 = 0.0f;

        float skill_PSS1_JieYa_CurrentStressValue = 0.0f;
        float skill_PSS1_JieYa_MaxStatePoint = 0.0f;

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL1, out PassiveSkill _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            skill_PSL1_HuoXin_VirtualHealthPoint = 0.05f;
            skill_PSL1_HuoXin_MaxStatePoint = 0.8f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL12, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            skill_PSL12_ShengShengBuXi = 0.5f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSE1, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            skill_PSE1_GaoYang_Value = 10.0f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSE7, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            skill_PSE7_FuHeLiuZhuan_Value2 = 10.0f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSS1, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            skill_PSS1_JieYa_CurrentStressValue = 10.0f;
            skill_PSS1_JieYa_MaxStatePoint = 0.8f;
        }

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
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, (gameCharacterData.maximumStatePoint >= 180) ? skill_PSE7_FuHeLiuZhuan_Value2 + 10 : 0, out gameCharacterData);
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

        float skill_PSL8_NiJingliuZhuan_Value150 = 0.0f;
        float skill_PSL8_NiJingliuZhuan_Value100 = 0.0f;
        float gainLifeScore = 0;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL1, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSL8_NiJingliuZhuan_Value150 = 1.5f;
            skill_PSL8_NiJingliuZhuan_Value100 = 1.0f;
        }

        if (isDeallingHealthPointDamage == true)  //"己方"給予HP傷害後(包括實傷 & 虛傷)
        {
            float attackTargetMaxHealthPoint = gameCharacter.GetCurrentAttackTarget().GetMaximumHealthPoint() * 0.01f;

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
            float damageTakerMaxHealthPoint = gameCharacterData.maximumHealthPoint * 0.01f;

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

    //直擊方負荷值結算
    public static void RunLeadCurrentStressPointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter lead, GameCharacter recepient)
    {
        CurrentStressPointCalculation(ref battleResultData ,lead, recepient, false);
    }

    //受擊方負荷值結算
    public static void RunRecepientCurrentStressPointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter lead, GameCharacter recepient)
    {
        CurrentStressPointCalculation(ref battleResultData, recepient, lead, true);
    }

    //平手方負荷值結算
    public static void RunDeuceCurrentStressPointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        CurrentStressPointCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo, false);
    }

    public static void CurrentStressPointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo, bool isMultipleZeroPointFive)
    {
        //reference
        BattleResultData.BattleResultData_GameCharacter gameCharacterOneData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwoData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);
        CategoryType gameCharacterOnePassiveSkillCategory = gameCharacterOne.GetSelectedPassiveSkillCategoryType();
        float skill_PSS11_FuheYaZhi2 = 0.0f;
        float skill_PSL9_ShengMingYaZhi = 0.0f;
        float skill_PSS6_FuHeLiuZhuan = 0.0f;
        float skill_PSS12_NiFeng = 0.0f;
        float skill_PSS3_HuaJing = 0.0f;
        float skill_PSS9_XingYunLiuShui = 0.0f;

        float activatingPassiveSkill = 0.0f;
        float multiplyZeroPointFive = isMultipleZeroPointFive ? 0.5f : 0.0f;
        float gameCharacterOneStressResistance = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().StressResistance;

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSL9_ShengMingYaZhi = 1.0f;
        }
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS3, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS3_HuaJing = 0.2f;
        }
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS6, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS6_FuHeLiuZhuan = 0.1f;
        }
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS9, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS9_XingYunLiuShui = 0.2f;
        }
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL11, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS11_FuheYaZhi2 = 1.0f;
        }
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS12, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS12_NiFeng = 0.2f;
        }

        /*
        "對方"當前流向是否生命流&
        "對方"生命積分>=100&
        "對方"生命值%>"己方"20%?
        YES=1(發動9.生命壓制)
        NO=0
         */
        float activateSkill_PSL9 = (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life) && (gameCharacterTwoData.currentHealthPoint > gameCharacterOneData.currentHealthPoint * 0.2f) ? skill_PSL9_ShengMingYaZhi : 0.0f;

        /*
         "己方"最大以太值是否
        >=120?
        YES=0.1(發動6.負荷流轉)
        NO=0
         */

        float activateSkill_PSS6 = (gameCharacterOneData.maximumStatePoint >= 120) ? skill_PSS6_FuHeLiuZhuan : 0;
        /*
         "己方"負荷等級是否
        >=2?
        YES=0.2(發動9.行雲流水)
        NO=0
         */
        float activateSkill_PSS9 = (gameCharacterOne.GetStressLevel() >= 2 ? skill_PSS9_XingYunLiuShui : 0);

        /*      
         "對方"當前流向是否負荷流&
        "對方"負荷等級>=3&
        "對方"當前以太值>"己方"&
        "對方"負荷值<="己方"50?
        YES=1(發動11.負荷壓制2)
        NO=0
         */
        float activateSkill_PSS11 = (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress) && (gameCharacterTwo.GetStressLevel() >= 3) && (gameCharacterTwoData.currentStatePoint > gameCharacterOneData.currentStatePoint) && (gameCharacterOneData.currentStressValue <= 50) ? skill_PSS11_FuheYaZhi2 : 0.0f;

        /*
         "己方"
        最大以太值是否<80?
        YES=0.2(發動12.逆風)
        NO=0
         */
        float activateSkill_PSS12 = (gameCharacterOneData.maximumStatePoint < 80) ? skill_PSS12_NiFeng : 0;

        /*
         "己方"是否有能量殘響?
        YES=0.5
        NO=0
         */
        float gameCharacterOneEnergyMarker = (gameCharacterOne.HasEnergyMarker() ? 0.5f : 0);

        if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            activatingPassiveSkill = activateSkill_PSS11;
        }
        else if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
            activatingPassiveSkill = activateSkill_PSL9;
        }

        if (gameCharacterOnePassiveSkillCategory == CategoryType.State)
        {
            //["己方"負荷值]+["對方"負荷傷害] *[1 + n + n] *[1 - (己方負荷抗性) - n - n]
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1-(gameCharacterOneStressResistance - activateSkill_PSS6 - activateSkill_PSS12))) * multiplyZeroPointFive, false, out _);
        }
        else if(gameCharacterOnePassiveSkillCategory == CategoryType.Stress)
        {
            //["己方"負荷值]+["對方"負荷傷害]*[1+n+n]*[1-(己方負荷抗性)-0.2-n)
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1-(gameCharacterOneStressResistance - skill_PSS3_HuaJing - activateSkill_PSS9))) * multiplyZeroPointFive, false, out _);
        }
        else if(gameCharacterOnePassiveSkillCategory == CategoryType.Life || gameCharacterOnePassiveSkillCategory == CategoryType.None)
        {
            //["己方"負荷值]+["對方"負荷傷害]*[1+n+n]*[1-(己方負荷抗性)]
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1 - gameCharacterOneStressResistance)) * multiplyZeroPointFive, false, out _);
        }
    }

    //抵抗成功方回避當前以太值結算    

}
