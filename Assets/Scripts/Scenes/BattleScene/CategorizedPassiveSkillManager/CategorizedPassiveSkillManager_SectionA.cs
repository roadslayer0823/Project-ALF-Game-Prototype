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
        float skill_PSL1_HuoXin_HealthPoint = 0.0f;
        float skill_PSL1_HuoXin_MaxStatePoint = 0.0f;
        float skill_PSL12_ShengShengBuXi = 0.0f;

        float skill_PSE1_GaoYang_Value = 0.0f;
        float skill_PSE7_FuHeLiuZhuan_Value2 = 0.0f;

        float skill_PSS1_JieYa_CurrentStressValue = 0.0f;
        float skill_PSS1_JieYa_MaxStatePoint = 0.0f;

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL1, out PassiveSkill _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            skill_PSL1_HuoXin_HealthPoint = 0.05f;
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
        float lifeCycleValue = 0.0f;

        switch (gameCharacter.GetLifeCyclePoint())
        {
            case 1:
                lifeCycleValue = 0.2f;
                break;

            case 2:
                lifeCycleValue = 0.4f;
                break;

            case 3:
                lifeCycleValue = 0.6f;
                break;
        }

        switch (gameCharacter.GetSelectedPassiveSkillCategoryType())
        {
            case CategoryType.Life:
                if(gameCharacterData.virtualHealthPoint != 0)
                {
                    //["己方"虛傷]-["己方"最大生命值] *[0.05 + 0.05] *[1 + n]
                    battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * (0.05f + skill_PSL1_HuoXin_HealthPoint)*(1+lifeCycleValue), out gameCharacterData);
                    
                    resultLogList.Add("virtual health point recover:" + gameCharacterData.virtualHealthPoint);
                }
                if (gameCharacterData.virtualHealthPoint == 0)
                {
                    //"己方"生命積分是否 >= 250 ?
                    resultLogList.Add("Life Score >= 250");
                    if(gameCharacter.GetLifeScore() >= 250)
                    {
                        //["己方"生命值]+["己方"最大生命值]*[0.05+0.05]*[1+n]*0.5
                        battleResultData.AddGameCharacterResultData_ActualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint*(0.05f + skill_PSL1_HuoXin_HealthPoint)*(1+lifeCycleValue)*skill_PSL12_ShengShengBuXi, out gameCharacterData); 
                        resultLogList.Add("health point recover" + );
                    }
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
            gainLifeScore += gainLifeScore * skill_PSL8_NiJingliuZhuan_Value150; //發動8.逆境流轉, 得到生命積分 + 150%.
        }
        else if(gameCharacterData.currentHealthPoint < gameCharacterData.maximumHealthPoint * 0.5f) //"己方"是否< HP50 %
        {
            gainLifeScore += gainLifeScore * skill_PSL8_NiJingliuZhuan_Value100; //發動8.逆境流轉, 得到生命積分 + 100%。
        }
        Debug.Log("gainLifeScore" + gainLifeScore);
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

    //抵抗成功方回避當前以太值結算
    public static void SuccessfulResisterEvadeCurrentStatePointFirstCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter successfulResister, GameCharacter deuce)
    {
        //reference
        BattleResultData.BattleResultData_GameCharacter successfulResisterData = battleResultData.GetGameCharacterResultData(successfulResister);
        BattleResultData.BattleResultData_GameCharacter deuceData = battleResultData.GetGameCharacterResultData(deuce);

        float deuceEvasionStress = deuce.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().EvasionStress;
        float activatingFirstPassiveSkill = 0.0f;
        float activatingSecondPassiveSkill = 0.0f;
        float skill_PSL9_ShenMingYaZhi_One = 0.0f;
        float skill_PSL9_ShenMingYaZhi_ZeroPointFive = 0.0f;
        float skill_PSS11_FuHeYaZhi2 = 0.0f;
        float skill_PSE8_PSE9_JieLiu_YouRen = 0.0f;
        float _stressEvasionCost;

        if (successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(successfulResister, _passiveSkill, out _);
            skill_PSL9_ShenMingYaZhi_One = 1.0f;
            skill_PSL9_ShenMingYaZhi_ZeroPointFive = 0.5f;
        }
        if (successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS11, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(successfulResister, _passiveSkill, out _);
            skill_PSS11_FuHeYaZhi2 = 1.0f;
        }
        if (successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) && successfulResisterData.currentStressValue > 50
            || successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill) && successfulResisterData.maximumStatePoint >= 300)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(successfulResister, _passiveSkill, out _);
            skill_PSE8_PSE9_JieLiu_YouRen = 0.5f;
        }
        /*
        "平手方"當前流向是否生命流&
        "平手方"生命積分>=100&
        "平手方"生命值%>"抵抗成功方"20%?
        YES=1(發動9.生命壓制)
        NO=0
         */
        float activateSkill_PSL9_One = (deuce.GetLifeScore() >= 100) && (deuceData.currentHealthPoint > successfulResisterData.currentHealthPoint * 0.2f) ? skill_PSL9_ShenMingYaZhi_One : 0.0f;

        /*
        "抵抗成功方"生命積分是否>=100&
        "抵抗成功方"生命值%>"後手方"20%?YES=0.5(發動9.生命壓制)
        NO=0
         */
        float activateSkill_PSL9_ZeroPointFive = (successfulResister.GetLifeScore() >= 100) && (successfulResisterData.currentHealthPoint > successfulResisterData.currentHealthPoint * 0.2f) ? skill_PSL9_ShenMingYaZhi_ZeroPointFive : 0.0f;

        /*
        "平手方"當前流向是否負荷流&
        "對方"負荷等級>=3&
        "平手方"當前以太值>"抵抗成功方"&
        "平手方"負荷值<="抵抗成功方"50?
        YES=1(發動11.負荷壓制2)
        NO=0
        */
        float activateSkill_PSS11 = (successfulResister.GetStressLevel() >= 3) && (deuceData.currentStatePoint > successfulResisterData.currentStatePoint) &&
            (successfulResisterData.currentStressValue - deuceData.currentStressValue <= 50 ) ? skill_PSS11_FuHeYaZhi2 : 0.0f;

        /*
        "抵抗成功方"當前流向是否以太流&
        "抵抗成功方"最大以太值是否>=300?
        YES=0.5(發動9.游刃)
        NO=0
        "抵抗成功方"負荷值是否>50%?
        YES=0.5(發動8.節流)
        NO=0
        (8.節流&9.游刃的效果不會疊加)
        */
        float activateSkill_PSE8_PSE9 = skill_PSE8_PSE9_JieLiu_YouRen;

        if (deuce.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
            activatingFirstPassiveSkill = activateSkill_PSL9_One;
            activatingSecondPassiveSkill = activateSkill_PSL9_ZeroPointFive;
        }
        else if (deuce.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            activatingFirstPassiveSkill = activateSkill_PSS11;
            activatingSecondPassiveSkill = activateSkill_PSE8_PSE9;
        }

        /*["抵抗成功方"當前以太值]-<["平手方"回避壓力] *[1 + n] *[1 - n] >
        < 此為"回避壓力消耗>*/
        _stressEvasionCost = deuceEvasionStress * (1 + activatingFirstPassiveSkill) * (1 - activatingSecondPassiveSkill);
        battleResultData.AddGameCharacterResultData_StatePointDamage(successfulResister, _stressEvasionCost, false, out _);
        successfulResisterData.temp_StressEvasionCost = _stressEvasionCost;
    }

    //抵抗成功方回避最大以太值結算    
    public static void SuccessfulResisterEvadeMaximumStatePointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float skill_PSE4_KuoLiu = 0.0f;
        float skill_PSE12_NiFeng = 0.0f;
        float stressEvasionCost = gameCharacterData.temp_StressEvasionCost;
        float _stressEvasionMaxStatePointIncrease;

        /*"抵抗成功方"
        最終以太消耗是否 >= 20 ?
        YES = 0.2(發動4.擴流)
        NO = 0*/
        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out PassiveSkill _passiveSkill) && gameCharacterData.temp_FinalTotalStatePointCost >= 20)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSE4_KuoLiu = 0.2f;
        }

        /*"抵抗成功方"
        最大以太值是否 < 80 ?
        YES = 2(發動12.逆風)
        NO = 1*/
        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill) && gameCharacterData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSE12_NiFeng = 2;
        }

        /*[最大以太值]+<[回避壓力消耗*n*n]>
        < 此為"回避壓力消耗以太提升")*/
        if(stressEvasionCost != 0)
        {
            _stressEvasionMaxStatePointIncrease = stressEvasionCost * skill_PSE4_KuoLiu * skill_PSE12_NiFeng;
            battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, _stressEvasionMaxStatePointIncrease, out _);
            gameCharacterData.temp_StressEvasionMaxStatePointIncrease = _stressEvasionMaxStatePointIncrease;
        }
    }

    //抵抗成功方回避當前以太值第2次結算
    public static void SuccessfulResisterEvadeCurrentStatePointSecondCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float skill_PSE3_HuiLiu = 0.0f;
        float skill_PSE12_NiFeng = 1.0f;
        float stressEvasionMaxStatePointIncrease = gameCharacterData.temp_StressEvasionMaxStatePointIncrease;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSE3_HuiLiu = 0.5f;
        }

        /*"抵抗成功方"
        最大以太值是否 < 80 ?
        YES = 2(發動12.逆風)
        NO = 1*/
        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill) && gameCharacterData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSE12_NiFeng = 2;
        }

        //[當前以太值]+[回避壓力消耗以太提升*0.5*n]
        if(stressEvasionMaxStatePointIncrease != 0)
        {
            battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, stressEvasionMaxStatePointIncrease * skill_PSE3_HuiLiu * skill_PSE12_NiFeng, out _);
        }
    }

    //抵抗成功方防禦當前以太值結算
    public static void SuccessfulResisterDefenseCurrentStatePointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter successfulResistor, GameCharacter deuce)
    {
        CurrentStatePointCalculation(ref battleResultData, successfulResistor, deuce);
    }

    //平手方當前以太值結算
    public static void RunDeuceCurrentStatPointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        CurrentStatePointCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo);
    }

    //直擊方當前以太值結算
    public static void RunLeadCurrentStatPointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter lead, GameCharacter receipent)
    {
        CurrentStatePointCalculation(ref battleResultData, lead, receipent);
    }

    //受擊方當前以太值結算
    public static void RunRecepientCurrentStatPointCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter recepient, GameCharacter lead)
    {
        CurrentStatePointCalculation(ref battleResultData, recepient, lead);
    }

    //負荷積分等級結算
    public static void StressScoreAndLevelCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);
        float skill_PSS5_JiXiao = 0.0f;
        float _gainStressScore;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS5, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSS5_JiXiao = 0.5f;
        }

        if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            //{[給予負荷傷害]+[受到負荷傷害]}*[1+n]
            _gainStressScore = gameCharacterData.stressValueDamageDealt + gameCharacterData.stressValueDamageTaken * (1 + skill_PSS5_JiXiao);
        }
        else
        {
            //[給予負荷傷害]+[受到負荷傷害]
            _gainStressScore = gameCharacterData.stressValueDamageDealt + gameCharacterData.stressValueDamageTaken;
        }
        gameCharacter.AddStressScore(Mathf.RoundToInt(_gainStressScore));
    }

    //負荷流借力結算
    public static void StressPassiveSkillTypeJieLiCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);
        float skill_PSS8_JieLi = 0.0f;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS8, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSS8_JieLi = 0.2f;
        }

        //"己方"成為崩潰狀態的次數是否 >= 2 ?
        if (gameCharacterData.numberOfEnteringIntoBreakStatus >= 2)
        {
            //"己方"在這場戰鬥中發動 8.借力的次數是否>= 1 ?
            if (gameCharacter.GetPassiveSkillTriggeredNumber(PASSIVE_SKILL_ID_PSS8) >= 1)
            {
                //["己方"當前生命值]+["己方"最大生命值]*[0.2]
                battleResultData.AddGameCharacterResultData_ActualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * skill_PSS8_JieLi, out _);
            }
        }
    }

    //對比條件的技能強度/速度增加
    public static void IncreaseStrengthOrSpeedWithCondition(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOneData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwoData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        int skill_PSE11_YiTaiYaZhi = 0;
        int skill_PSS7_JieFeng = 0;

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE11, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSE11_YiTaiYaZhi = 1;
        }
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE11, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS7_JieFeng = 1;
        }

        switch (gameCharacterOne.GetSelectedPassiveSkillCategoryType())
        {
            case CategoryType.State:
                //"己方"當前以太值是否比"對方" >= 50 ?
                if (gameCharacterOneData.currentStatePoint - gameCharacterTwoData.currentStatePoint >= 50)
                {
                    //"己方"已按下技能強度+1&速度+1
                    battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed(gameCharacterOne, skill_PSE11_YiTaiYaZhi, out _);
                    battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength(gameCharacterOne, skill_PSE11_YiTaiYaZhi, out _);
                }
                break;

            case CategoryType.Stress:
                //"己方"負荷等級是否 >= 2 ?
                if (gameCharacterOne.GetStressLevel() >= 2)
                {
                    //"己方"負荷值是否< "對方" ?
                    if (gameCharacterOneData.currentStressValue < gameCharacterTwoData.currentStressValue)
                    {
                        //"己方"已按下技能強度 + 1 & 速度 + 1
                        battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed(gameCharacterOne, skill_PSS7_JieFeng, out _);
                        battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength(gameCharacterOne, skill_PSS7_JieFeng, out _);
                    }
                }
                break;
        }
    }

    //reuse function
    public static void CurrentStatePointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOneData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwoData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        float skill_PSE5_PoLiu = 0.0f;
        float hasEnergyMarker = gameCharacterOneData.HasEnergyMarker() ? 0.5f : 0.0f;

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE5, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSE5_PoLiu = 0.5f;
        }

        if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            //["己方"當前以太值]-["對方"以太傷害]*[1+0.5+n]
            battleResultData.AddGameCharacterResultData_StatePointDamage(gameCharacterOne, gameCharacterTwoData.statePointDamageDealt * (1 + skill_PSE5_PoLiu + hasEnergyMarker), false, out _);
        }
        else
        {
            //["己方"當前以太值]-["對方"以太傷害]*[1+n]
            battleResultData.AddGameCharacterResultData_StatePointDamage(gameCharacterOne, gameCharacterTwoData.statePointDamageDealt * (1 + hasEnergyMarker));
        }
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
        float activateSkill_PSL9 = gameCharacterTwo.GetLifeScore() >= 100 && (gameCharacterTwoData.currentHealthPoint > gameCharacterOneData.currentHealthPoint * 0.2f) ? skill_PSL9_ShengMingYaZhi : 0.0f;

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
        float activateSkill_PSS11 = (gameCharacterTwo.GetStressLevel() >= 3) && (gameCharacterTwoData.currentStatePoint > gameCharacterOneData.currentStatePoint) &&
            (gameCharacterOneData.currentStressValue <= gameCharacterTwoData.currentStressValue &&
            gameCharacterOneData.currentStressValue - gameCharacterTwoData.currentStressValue >= 50) ? skill_PSS11_FuheYaZhi2 : 0.0f;

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
                (1 - (gameCharacterOneStressResistance - activateSkill_PSS6 - activateSkill_PSS12))) * multiplyZeroPointFive, false, out _);
        }
        else if (gameCharacterOnePassiveSkillCategory == CategoryType.Stress)
        {
            //["己方"負荷值]+["對方"負荷傷害]*[1+n+n]*[1-(己方負荷抗性)-0.2-n)
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1 - (gameCharacterOneStressResistance - skill_PSS3_HuaJing - activateSkill_PSS9))) * multiplyZeroPointFive, false, out _);
        }
        else if (gameCharacterOnePassiveSkillCategory == CategoryType.Life || gameCharacterOnePassiveSkillCategory == CategoryType.None)
        {
            //["己方"負荷值]+["對方"負荷傷害]*[1+n+n]*[1-(己方負荷抗性)]
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1 - gameCharacterOneStressResistance)) * multiplyZeroPointFive, false, out _);
        }
    }
}
