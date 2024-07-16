using UnityEngine;
using PassiveSkill = DatabaseManager.PassiveSkill;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 基本回復效果相關數值結算
    public static void RunBasicRecoveryEffects( ref BattleResultData battleResultData, GameCharacter gameCharacter )
    {
        //reference
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        //skill variable
        float _skill_PSL1_HuoXin_HealthPoint = 0.0f;
        float _skill_PSL1_HuoXin_MaxStatePoint = 0.0f;
        float _skill_PSL5_XunHuanLi = 0.0f;
        float _skill_PSL12_ShengShengBuXi = 0.0f;

        float _skill_PSE1_GaoYang_Value = 0.0f;
        float _skill_PSE7_FuHeLiuZhuan_Value2 = 0.0f;

        float _skill_PSS1_JieYa_CurrentStressValue = 0.0f;
        float _skill_PSS1_JieYa_MaxStatePoint = 0.0f;

        //keyword
        string _formula;
        string _VirtualHealthPointDamageRecovered;
        string _currentVirtualHealthPoint = "當前虛傷";
        string _currentPassiveSkillType = "當前流向";
        string _currentCharacterName = "角色名稱";
        string _lifeCyclePoint = "循環點";
        string _basicRecover = "基本回復數值";
        string _maxHealthPoint = "最大生命值";
        string _maxStatePoint = "最大以太值";
        string _currentStatePoint = "當前以太值";
        string _currentStressValue = "當前負荷值";
        string _PSL5_XunHuanLi = "5.循環力";
        string _PSL1_HuoXin = "1.活性";
        string _PSL12_ShengShengBuXi = "12.生生不息";
        string _PSE1_GaoYang = "1.高陽";
        string _PSE7_FuHeLiuZhuan2 = "7.負荷流轉2";
        string _PSS1_JieYa = "1.解壓";

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL1, out PassiveSkill _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            _skill_PSL1_HuoXin_HealthPoint = 0.05f;
            _skill_PSL1_HuoXin_MaxStatePoint = 0.8f;
        }
        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL5, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            switch (gameCharacter.GetLifeCyclePoint())
            {
                case 1:
                    _skill_PSL5_XunHuanLi = 0.2f;
                    break;

                case 2:
                    _skill_PSL5_XunHuanLi = 0.4f;
                    break;

                case 3:
                    _skill_PSL5_XunHuanLi = 0.6f;
                    break;
            }
        }
        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL12, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            _skill_PSL12_ShengShengBuXi = 0.5f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSE1, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            _skill_PSE1_GaoYang_Value = 10.0f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSE7, out _passiveSkill) && gameCharacterData.maximumStatePoint >= 180)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            _skill_PSE7_FuHeLiuZhuan_Value2 = 10.0f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSS1, out _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, _passiveSkill, out _ );
            _skill_PSS1_JieYa_CurrentStressValue = 10.0f;
            _skill_PSS1_JieYa_MaxStatePoint = 0.8f;
        }

        //keyword with value
        /*
        string currentIdentityString = $"<color={BattleLog.KEYWORD_COLOR_CODE}> {currentCharacterName} = </color>" + gameCharacter.GetCharacterName();
        string basicRecoverString = basicRecover + 0.5f;
        string maxHealthPointString = $"<color={BattleLog.KEYWORD_COLOR_CODE}> {maxHealthPoint} = </color>" + gameCharacterData.maximumHealthPoint;
        string skill_PSL1_HuoXin_HealthPointString = $"<color={BattleLog.KEYWORD_COLOR_CODE}> {PSL1_HuoXin} = </color>" + skill_PSL1_HuoXin_HealthPoint;
        string skill_PSL5_XunHuanLiString = $"<color={BattleLog.KEYWORD_COLOR_CODE}> {PSL5_XunHuanLi} = </color>" + lifeCycleValue;
        string currentLifeCyclePointString = $"<color={BattleLog.KEYWORD_COLOR_CODE}> {lifeCyclePoint} = </color>" + gameCharacter.GetLifeCyclePoint();
        string skill_PSL12_ShengShengBuXiString = $"<color={BattleLog.KEYWORD_COLOR_CODE}> {PSL12_ShengShengBuXi} = </color>" + skill_PSL12_ShengShengBuXi;*/

        string _currentPassiveSkillTypeString = _currentPassiveSkillType + ":" + TerminologyManager.GetPassiveSkillCategorizedType(gameCharacter.GetSelectedPassiveSkillCategoryType());
        string _currentIdentityString = _currentCharacterName + ":" + gameCharacter.GetCharacterName();
        string _basicRecoverString_ZeroPointFive = _basicRecover + ":" + 0.5f;
        string _basicRecoverString_Ten = _basicRecover + ":" + 10f;
        string _maxHealthPointString = _maxHealthPoint + ":" + gameCharacterData.maximumHealthPoint;
        string _maxStatePointString = _maxStatePoint + ":" + gameCharacterData.maximumStatePoint;
        string _currentVirtualHealthPointString = _currentVirtualHealthPoint + ":" + gameCharacterData.virtualHealthPoint;
        string _currentStatePointString = _currentStatePoint + ":" + gameCharacterData.currentStatePoint;
        string _currentStressValueString = _currentStressValue + ":" + gameCharacterData.currentStressValue;
        string _currentLifeCyclePointString = _lifeCyclePoint + ":" + gameCharacter.GetLifeCyclePoint();
        string _skill_PSL1_HuoXin_HealthPointString = _PSL1_HuoXin + ":" + _skill_PSL1_HuoXin_HealthPoint;
        string _skill_PSL1_HuoXin_MaxStatePointString = _PSL1_HuoXin + ":" + _skill_PSL1_HuoXin_MaxStatePoint;
        string _skill_PSL5_XunHuanLiString = _PSL5_XunHuanLi + ":" + _skill_PSL5_XunHuanLi;
        string _skill_PSL12_ShengShengBuXiString = _PSL12_ShengShengBuXi + ":" + _skill_PSL12_ShengShengBuXi;
        string _skill_PSE1_GaoYangString = _PSE1_GaoYang + ":" + _skill_PSE1_GaoYang_Value;
        string _skill_PSE7_FuHeLiuZhuan2String = _PSE7_FuHeLiuZhuan2 + ":" + _skill_PSE7_FuHeLiuZhuan_Value2;
        string _skill_PSS1_JieYaString_CurrentStressValue = _PSS1_JieYa + ":" + _skill_PSS1_JieYa_CurrentStressValue;
        string _skill_PSS1_JieYaString_MaxStatePoint = _PSS1_JieYa + ":" + _skill_PSS1_JieYa_MaxStatePoint;

        battleResultData.AddResultLog("基本回復效果相關數值結算" + "\n" +
                            _currentPassiveSkillTypeString);
        switch (gameCharacter.GetSelectedPassiveSkillCategoryType())
        {
            case CategoryType.Life:

                if(gameCharacterData.GetVirtualHealthDamage() != 0)
                {
                    //["己方"虛傷]-["己方"最大生命值] *[0.05 + 0.05] *[1 + n]
                    battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * (0.05f + _skill_PSL1_HuoXin_HealthPoint)*(1+_skill_PSL5_XunHuanLi), out gameCharacterData);
                    _formula = "[己方虛傷] -[己方最大生命值] * [" + _basicRecover + "+" + _PSL1_HuoXin + "] *[1 +" + _PSL5_XunHuanLi + "]";
                    _VirtualHealthPointDamageRecovered = _currentVirtualHealthPointString + "\n" +
                                                         _maxHealthPointString + "\n" +
                                                         _basicRecoverString_ZeroPointFive + "\n" +
                                                         _skill_PSL1_HuoXin_HealthPointString + "\n" +
                                                         _currentLifeCyclePointString + "\n" +
                                                         _skill_PSL5_XunHuanLiString + "\n";
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                        "算式:  " + "\n" + _formula + "\n" + "\n" +
                                        _VirtualHealthPointDamageRecovered);
                }
                if (gameCharacterData.GetVirtualHealthDamage() == 0)
                {
                    //"己方"生命積分是否 >= 250 ?
                    if(gameCharacter.GetLifeScore() >= 250)
                    {
                        //["己方"生命值]+["己方"最大生命值]*[0.05+0.05]*[1+n]*0.5
                        battleResultData.AddGameCharacterResultData_ActualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint*(0.05f + _skill_PSL1_HuoXin_HealthPoint)*(1+_skill_PSL5_XunHuanLi)*_skill_PSL12_ShengShengBuXi, out gameCharacterData);
                        _formula = "[己方生命值]+[己方最大生命值]*[" + _basicRecover + "+" + _PSL1_HuoXin + "]*[1 +" + _PSL5_XunHuanLi + "]*" + _PSL12_ShengShengBuXi;
                        string _ActualHealthPointDamageRecovered = _maxHealthPointString + "\n" +
                                                                   _currentLifeCyclePointString + "\n" +
                                                                   _skill_PSL1_HuoXin_HealthPointString + "\n" + 
                                                                   _skill_PSL5_XunHuanLiString + "\n" +
                                                                   _skill_PSL12_ShengShengBuXiString + "\n";
                        battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                         "算式:  " + "\n" + _formula + "\n" + "\n" +
                                         _ActualHealthPointDamageRecovered);
                        string testing = $"<color={_basicRecover}</color>";
                    }
                }

                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是負數
                {
                    //["己方"最大以太值]+["己方"當前以太值]
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData);
                    _formula = "[己方最大以太值]+[己方當前以太值]";
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _currentStatePointString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                         "算式:  " + "\n" + _formula + "\n" + "\n" +
                                         _MaximumStatePointIncrease);
                }

                if(gameCharacterData.currentStatePoint > 0) //"己方"當前以太值不是負數
                {
                    //["己方"最大以太值]+10
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData);
                    _formula = "[己方最大以太值]+"+ _basicRecover;
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _basicRecoverString_Ten;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                         "算式:  " + "\n" + _formula + "\n" + "\n" +
                                         _MaximumStatePointIncrease);
                }

                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    //["己方"負荷值]-10
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10, out gameCharacterData);
                    _formula = "[己方負荷值]-" + _basicRecover;
                    string _StressValueDamageRecovered = _currentStressValueString + "\n" +
                                                         _basicRecoverString_Ten;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                        "算式:  " + "\n" + _formula + "\n" + "\n" +
                                        _StressValueDamageRecovered);
                }

                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    if (gameCharacterData.currentStatePoint < (gameCharacterData.currentStatePoint * 0.8f)) //["己方"當前以太值]是否 <["己方"當前以太值]的80 %?
                    {
                        //["己方"當前以太值]=["己方"最大以太值 * 0.8(skill_L1_HuoXin_MaximumStatePoint)]
                        battleResultData.AddGameCharacterResultData_RestoreCurrentStatePointByPercentage(gameCharacter, _skill_PSL1_HuoXin_MaxStatePoint, out _);
                        _formula = "[己方當前以太值] =[己方最大以太值 *" + _PSL1_HuoXin + "]";
                        string _RestoreCurrentStatePoint = _maxStatePointString + "\n" +
                                                           _skill_PSL1_HuoXin_MaxStatePointString;
                        battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                        "算式:  " + "\n" + _formula + "\n" + "\n" +
                                        _RestoreCurrentStatePoint);
                    }
                }
                else
                {
                    Debug.Log("no any effect activate");
                }
                break;

            case CategoryType.State:
                //["己方"虛傷]-["己方"最大生命值]*[0.5]
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * 0.5f, out gameCharacterData);
                _formula = "[己方虛傷] -[己方最大生命值] *[" +_basicRecover+ "]";
                _VirtualHealthPointDamageRecovered = _currentVirtualHealthPointString + "\n" +
                                                     _maxHealthPointString + "\n" +
                                                     _basicRecoverString_ZeroPointFive;
                battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                       "算式:  " + "\n" + _formula + "\n" + "\n" +
                                       _VirtualHealthPointDamageRecovered);

                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    //["己方"最大以太值]+["己方"當前以太值]
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData);
                    _formula = "[己方最大以太值] + [己方當前以太值]";
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _currentStatePointString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                      "算式:  " + "\n" + _formula + "\n" + "\n" +
                                      _MaximumStatePointIncrease);
                }
                if(gameCharacterData.currentStatePoint > 0)
                {
                    //["己方"最大以太值]+10 + skill_PSE1_GaoYang_Value
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10 + _skill_PSE1_GaoYang_Value, out gameCharacterData);
                    _formula = "[己方最大以太值] +" + _basicRecover + "+" + _PSE1_GaoYang;
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _basicRecoverString_Ten + "\n" +
                                                        _skill_PSE1_GaoYangString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                      "算式:  " + "\n" + _formula + "\n" + "\n" +
                                      _MaximumStatePointIncrease);
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    //["己方"負荷值]-10- "己方"最大以太值是否 >= 180 ? YES = 10(發動7.負荷流轉2) NO = 0
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, _skill_PSE7_FuHeLiuZhuan_Value2 + 10, out gameCharacterData);
                    _formula = "[己方負荷值] - " +_basicRecover+ "-" + _skill_PSE7_FuHeLiuZhuan2String;
                    string _StressValueDamageRecovered = _currentStressValueString + "\n" +
                                                         _basicRecoverString_Ten + "\n" +
                                                         _skill_PSE7_FuHeLiuZhuan2String;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                     "算式:  " + "\n" + _formula + "\n" + "\n" +
                                     _StressValueDamageRecovered);
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    //["己方"當前以太值] = ["己方"最大以太值]
                    battleResultData.AddGameCharacterResultData_RestoreCurrentStatePointByPercentage( gameCharacter, 1.0f, out _);
                    _formula = "[己方當前以太值] = [己方最大以太值]";
                    string _RestoreCurrentStatePoint = _maxStatePointString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                     "算式:  " + "\n" + _formula + "\n" + "\n" +
                                     _RestoreCurrentStatePoint);
                }
                break;

            case CategoryType.Stress:
                //["己方"虛傷]-["己方"最大生命值] *[0.5]
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * 0.5f, out gameCharacterData);
                _formula = "[己方虛傷] -[己方最大生命值] *[" + _basicRecover +"]";
                _VirtualHealthPointDamageRecovered = _currentVirtualHealthPointString + "\n" +
                                                     _maxHealthPointString + "\n" +
                                                     _basicRecoverString_ZeroPointFive;
                battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                     "算式:  " + "\n" + _formula + "\n" + "\n" +
                                     _VirtualHealthPointDamageRecovered);

                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    //["己方"最大以太值]+["己方"當前以太值]
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData); 
                    _formula = "[己方最大以太值]+[己方當前以太值]";
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _currentStatePointString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                     "算式:  " + "\n" + _formula + "\n" + "\n" +
                                     _MaximumStatePointIncrease);
                }
                if (gameCharacterData.currentStatePoint > 0)
                {
                    //["己方"最大以太值] + 10
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData);
                    _formula = "[己方最大以太值] +" + _basicRecover;
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _basicRecoverString_Ten;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                    "算式:  " + "\n" + _formula + "\n" + "\n" +
                                    _MaximumStatePointIncrease);
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    //["己方"負荷值]-10 - skill_PSS1_JieYa_CurrentStressValue
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10 + _skill_PSS1_JieYa_CurrentStressValue, out gameCharacterData);
                    _formula = "[己方負荷值] -" +_basicRecover+ "-" + _PSS1_JieYa;
                    string _StressValueDamageRecovered = _currentStressValueString + "\n" +
                                                         _basicRecoverString_Ten + "\n" +
                                                         _skill_PSS1_JieYaString_CurrentStressValue;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                   "算式:  " + "\n" + _formula + "\n" + "\n" +
                                   _StressValueDamageRecovered);
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    Debug.Log("did not break by state point");
                    if (gameCharacterData.currentStatePoint < gameCharacterData.currentStatePoint * 0.8f) //["己方"當前以太值]是否<["己方"當前以太值]的80 %?
                    {
                        //["己方"當前以太值]=["己方"最大以太值*skill_PSS1_JieYa_MaxStatePoint]
                        battleResultData.AddGameCharacterResultData_RestoreCurrentStatePointByPercentage(gameCharacter, _skill_PSS1_JieYa_MaxStatePoint, out _);
                        _formula = "[己方當前以太值]=[己方最大以太值*" +_PSS1_JieYa + "]";
                        string _RestoreCurrentStatePoint = _maxStatePointString + "\n" +
                                                           _skill_PSS1_JieYaString_MaxStatePoint;
                        battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                  "算式:  " + "\n" + _formula + "\n" + "\n" +
                                  _RestoreCurrentStatePoint);
                    }
                }
                break;

            case CategoryType.None:
                //["己方"虛傷]-["己方"最大生命值] *[0.5]
                battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered(gameCharacter, gameCharacterData.maximumHealthPoint * 0.5f, out gameCharacterData);
                _formula = "[己方虛傷] -[己方最大生命值] *[" +_basicRecover +"]";
                _VirtualHealthPointDamageRecovered = _currentVirtualHealthPointString + "\n" +
                                                     _maxHealthPointString + "\n" +
                                                     _basicRecoverString_ZeroPointFive;
                battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                 "算式:  " + "\n" + _formula + "\n" + "\n" +
                                 _VirtualHealthPointDamageRecovered);

                if (gameCharacterData.currentStatePoint < 0) //"己方"當前以太值是否負數?
                {
                    //["己方"最大以太值]+["己方"當前以太值]
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacterData.currentStatePoint, out gameCharacterData);
                    _formula = "[己方最大以太值]+[己方當前以太值]";
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                       _currentStatePointString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                 "算式:  " + "\n" + _formula + "\n" + "\n" +
                                 _MaximumStatePointIncrease);
                }
                if (gameCharacterData.currentStatePoint > 0)
                {
                    //["己方"最大以太值]+10
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, 10, out gameCharacterData);
                    _formula = "[己方最大以太值] +" + _basicRecover;
                    string _MaximumStatePointIncrease = _maxStatePointString + "\n" +
                                                        _basicRecoverString_Ten;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                "算式:  " + "\n" + _formula + "\n" + "\n" +
                                _MaximumStatePointIncrease);
                }
                if (!gameCharacterData.IsInStressBreakStatus()) //"己方"是否"負荷崩潰狀態" ?
                {
                    //["己方"負荷值]-10
                    battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, 10, out gameCharacterData);
                    _formula = "[己方負荷值] - " + _basicRecover;
                    string _StressValueDamageRecovered = _currentStressValueString + "\n" +
                                                          _basicRecoverString_Ten;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                "算式:  " + "\n" + _formula + "\n" + "\n" +
                                _StressValueDamageRecovered);
                }
                if (!gameCharacterData.IsInStateBreakStatus()) //"己方"是否"以太崩潰狀態" ?
                {
                    //["己方"當前以太值]=["己方"最大以太值]
                    battleResultData.AddGameCharacterResultData_RestoreCurrentStatePointByPercentage(gameCharacter, 1.0f, out _);
                    _formula = "[己方當前以太值]=[己方最大以太值]";
                    string _RestoreCurrentStatePoint = _maxStatePointString;
                    battleResultData.AddResultLog(_currentIdentityString + "\n" + "\n" +
                                "算式:  " + "\n" + _formula + "\n" + "\n" +
                                _RestoreCurrentStatePoint);
                }
                break;
        }
    }

    //生命流系統數值相關結算
    public static void CalculateLifeScoreEffect(ref BattleResultData battleResultData, GameCharacter gameCharacter, bool isDeallingHealthPointDamage)
    {
        //reference
        battleResultData.AddResultLog("生命流系統數值相關結算");
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float skill_PSL8_NiJingliuZhuan_Value150 = 0.0f;
        float skill_PSL8_NiJingliuZhuan_Value100 = 0.0f;
        float gainLifeScore = 0;

        string currentCharacterName = "角色名稱";
        string currentLifeScore = "生命積分";
        string PSL8_NiJingLiuZhuan = "8.逆境流轉";

        string currentIdentityString = currentCharacterName + ":" + gameCharacter.GetCharacterName();
        string _finalString = "";

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
            }
            if(gameCharacterData.virtualHealthPointDamageDealt > 0)
            {
                gainLifeScore = gameCharacterData.virtualHealthPointDamageDealt / attackTargetMaxHealthPoint;
            }
        }
        else //"己方"受到HP傷害後(包括實傷 & 虛傷)
        {
            float damageTakerMaxHealthPoint = gameCharacterData.maximumHealthPoint * 0.01f;

            //[受到HP總傷害]每達到[己方最大生命值]的1 %="己方"生命積分 + 1。
            if (gameCharacterData.actualHealthPointDamageTaken > 0)
            {
                gainLifeScore = gameCharacterData.actualHealthPointDamageTaken / damageTakerMaxHealthPoint;
            }
            if (gameCharacterData.virtualHealthPointDamageTaken > 0)
            {
                gainLifeScore = gameCharacterData.virtualHealthPointDamageTaken / damageTakerMaxHealthPoint;
            }
        }
        
        if(gameCharacterData.currentHealthPoint < gameCharacterData.maximumHealthPoint * 0.3f) //"己方"是否< HP30 %
        {
            //發動8.逆境流轉, 得到生命積分 + 150%.
            gainLifeScore += gainLifeScore * skill_PSL8_NiJingliuZhuan_Value150;
            string _formula = currentLifeScore + "*" + PSL8_NiJingLiuZhuan;
            _finalString =     currentIdentityString + "\n" + "\n" +
                               "算式:  " + "\n" + _formula + "\n" + "\n" +
                               PSL8_NiJingLiuZhuan + ":" + skill_PSL8_NiJingliuZhuan_Value150;
        }
        else if(gameCharacterData.currentHealthPoint < gameCharacterData.maximumHealthPoint * 0.5f) //"己方"是否< HP50 %
        {
            //發動8.逆境流轉, 得到生命積分 + 100%。
            gainLifeScore += gainLifeScore * skill_PSL8_NiJingliuZhuan_Value100;
            string _formula = currentLifeScore + "*" + PSL8_NiJingLiuZhuan;
            _finalString =     currentIdentityString + "\n" + "\n" +
                               "算式:  " + "\n" + _formula + "\n" + "\n" +
                               PSL8_NiJingLiuZhuan + ":" + skill_PSL8_NiJingliuZhuan_Value100;
        }
        battleResultData.AddGameCharacterResultData_AddLifeScore(gameCharacter, Mathf.RoundToInt(gainLifeScore), out _);
        battleResultData.AddResultLog(_finalString);
    }

    //直擊方負荷值結算
    public static void RunLeadCurrentStressPointCalculation(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter recepient)
    {
        battleResultData.AddResultLog("直擊方負荷值結算");
        CurrentStressPointCalculation(ref battleResultData ,lead, recepient, false);
    }

    //受擊方負荷值結算
    public static void RunRecepientCurrentStressPointCalculation(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter recepient)
    {
        battleResultData.AddResultLog("受擊方負荷值結算");
        CurrentStressPointCalculation(ref battleResultData, recepient, lead, true);
    }

    //平手方負荷值結算
    public static void RunDeuceCurrentStressPointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog("平手方負荷值結算");
        CurrentStressPointCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo, false);
    }

    //抵抗成功方回避當前以太值結算
    public static void SuccessfulResisterEvadeCurrentStatePointFirstCalculation(ref BattleResultData battleResultData, GameCharacter successfulResister, GameCharacter deuce)
    {
        battleResultData.AddResultLog("抵抗成功方回避當前以太值結算");

        //reference
        BattleResultData.BattleResultData_GameCharacter successfulResisterData = battleResultData.GetGameCharacterResultData(successfulResister);
        BattleResultData.BattleResultData_GameCharacter deuceData = battleResultData.GetGameCharacterResultData(deuce);
        CategoryType successfulResisterPassiveSkillType = successfulResister.GetSelectedPassiveSkillCategoryType();
        CategoryType deucePassiveSkillType = deuce.GetSelectedPassiveSkillCategoryType();

        //string
        string _formula = "";
        string _successfulResisterString;
        string _deuceString;
        string _currentFirstSkillString = "";
        string _currentActivatingSkill = "";
        string _finalString;
        string _JieLiu_YouRen = "";
        string _currentIdentity = successfulResister.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SuccessfulResister) ? "抵抗成功方" : "平手方";
        string _isPSL9 = (successfulResisterPassiveSkillType == CategoryType.Life) ? "9.生命壓制" : "";

        float _deuceEvasionStress = deuce.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().EvasionStress;
        float _activatingFirstPassiveSkill = 0.0f;
        float _skill_PSL9_ShenMingYaZhi_One = 0.0f;
        float _skill_PSL9_ShenMingYaZhi_ZeroPointFive = 0.0f;
        float _skill_PSS11_FuHeYaZhi2 = 0.0f;
        float _skill_PSE8_PSE9_JieLiu_YouRen = 0.0f;
        float _stressEvasionCost = 0.0f;

        bool isPSE8;
        /*
        "平手方"當前流向是否生命流&
        "平手方"生命積分>=100&
        "平手方"生命值%>"抵抗成功方"20%?
        YES=1(發動9.生命壓制)
        NO=0

        "抵抗成功方"生命積分是否>=100&
        "抵抗成功方"生命值%>"後手方"20%?YES=0.5(發動9.生命壓制)
        NO=0
         */
        if (successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill) && (deuce.GetLifeScore() >= 100) &&
            ((deuceData.currentHealthPoint > successfulResisterData.currentHealthPoint * 0.2f)
            || successfulResisterPassiveSkillType == CategoryType.Life && (successfulResister.GetLifeScore() >= 100) &&
            (successfulResisterData.currentHealthPoint > successfulResisterData.currentHealthPoint * 0.2f)))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(successfulResister, _passiveSkill, out _);
            _skill_PSL9_ShenMingYaZhi_One = 1.0f;
            _skill_PSL9_ShenMingYaZhi_ZeroPointFive = 0.5f;
        }

        /*
       "平手方"當前流向是否負荷流&
       "對方"負荷等級>=3&
       "平手方"當前以太值>"抵抗成功方"&
       "平手方"負荷值<="抵抗成功方"50?
       YES=1(發動11.負荷壓制2)
       NO=0
       */
        if (successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS11, out _passiveSkill) &&
            (successfulResister.GetStressLevel() >= 3) && (deuceData.currentStatePoint > successfulResisterData.currentStatePoint) &&
            (successfulResisterData.currentStressValue - deuceData.currentStressValue <= 50))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(successfulResister, _passiveSkill, out _);
            _skill_PSS11_FuHeYaZhi2 = 1.0f;
        }

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
        if (successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) && successfulResisterData.currentStressValue > 50
            || successfulResister.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill) && successfulResisterData.maximumStatePoint >= 300)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(successfulResister, _passiveSkill, out _);
            _skill_PSE8_PSE9_JieLiu_YouRen = 0.5f;
            isPSE8 = _passiveSkill.Id == PASSIVE_SKILL_ID_PSE8;
            _JieLiu_YouRen = (isPSE8) ? "8.節流" : "9.游刃";
        }

        if(deucePassiveSkillType == CategoryType.Life)
        {
            _activatingFirstPassiveSkill = _skill_PSL9_ShenMingYaZhi_One;
            _currentFirstSkillString = "9.生命壓制";
        }
        else if(deucePassiveSkillType == CategoryType.Stress)
        {
            _activatingFirstPassiveSkill = _skill_PSS11_FuHeYaZhi2;
            _currentFirstSkillString = "11.負荷壓制2";
        }

        if (successfulResisterPassiveSkillType == CategoryType.None || successfulResisterPassiveSkillType == CategoryType.Stress)
        {
            //["抵抗成功方"當前以太值]-<["平手方"回避壓力]*[1+n]
            _stressEvasionCost = _deuceEvasionStress * (1 + _activatingFirstPassiveSkill);
            _formula = "[抵抗成功方當前以太值]-<[平手方回避壓力]*[1 + （9.生命壓制 或者 11.負荷壓制2）]";
            _currentActivatingSkill = "已使用的流向技能:" + _currentFirstSkillString;
        }
        else if (successfulResisterPassiveSkillType == CategoryType.Life)
        {
            //["抵抗成功方"當前以太值]-<["平手方"回避壓力]*[1+n]*[1-n]
            _stressEvasionCost = _deuceEvasionStress * (1 + _activatingFirstPassiveSkill) * (1 - _skill_PSL9_ShenMingYaZhi_ZeroPointFive);
            _formula = "[抵抗成功方當前以太值]-<[平手方回避壓力]*[1 + （9.生命壓制 或者 11.負荷壓制2）] * [1 - 9.生命壓制]";
            _currentActivatingSkill = "已使用的流向技能:" + _currentFirstSkillString + "\n" + _isPSL9;
        }
        else if (successfulResisterPassiveSkillType == CategoryType.State)
        {
            //["抵抗成功方"當前以太值]-<["平手方"回避壓力]*[1+n]*[1-n]
            
            _stressEvasionCost = _deuceEvasionStress * (1 + _activatingFirstPassiveSkill) * (1 - _skill_PSE8_PSE9_JieLiu_YouRen);
            _formula = "[抵抗成功方當前以太值]-<[平手方回避壓力]*[1 + （9.生命壓制（平手方） 或者 11.負荷壓制2）] * [1 - " + _JieLiu_YouRen + "（抵抗成功方）]";
            _currentActivatingSkill = "已使用的流向技能:" + _currentFirstSkillString + "\n" +_JieLiu_YouRen;
        }
        battleResultData.AddGameCharacterResultData_StatePointDamage(successfulResister, _stressEvasionCost, false, out _);
        successfulResisterData.temp_StressEvasionCost = _stressEvasionCost;

        _successfulResisterString = "當前身份:" + _currentIdentity + "\n" +
                                    "當前流向:" + successfulResisterPassiveSkillType + "\n" +
                                    "當前以太值:" + successfulResisterData.currentStatePoint + "\n" +
                                    _currentActivatingSkill;

        _deuceString              = "當前身份:" + _currentIdentity + "\n" +
                                    "當前流向:" + deucePassiveSkillType + "\n" +
                                    "回避壓力:" + _deuceEvasionStress + "\n";

        _finalString              = "算式=" + _formula + "\n" + "\n" +
                                    _successfulResisterString + "\n" + "\n" +
                                    _deuceString;

        battleResultData.AddResultLog("抵抗成功方回避當前以太值結算" + "\n" + "\n" + _finalString);
    }

    //抵抗成功方回避最大以太值結算    
    public static void SuccessfulResisterEvadeMaximumStatePointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        battleResultData.AddResultLog("抵抗成功方回避最大以太值結算");

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
        YES = 0.2(發動12.逆風)
        NO = 0.1*/
        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill) && gameCharacterData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSE12_NiFeng = 0.2f;
        }

        /*[最大以太值]+<[回避壓力消耗*n*n]>
        < 此為"回避壓力消耗以太提升")*/
        if(stressEvasionCost != 0)
        {
            _stressEvasionMaxStatePointIncrease = stressEvasionCost * skill_PSE4_KuoLiu * skill_PSE12_NiFeng;
            battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, _stressEvasionMaxStatePointIncrease, out _);
            gameCharacterData.temp_StressEvasionMaxStatePointIncrease = _stressEvasionMaxStatePointIncrease;
        }
        string _formula = "[最大以太值]+<[回避壓力消耗*4.擴流*12.逆風]";
        string _finalString = "當前身份: 抵抗成功方" + "\n" +
                              "最大以太值:" + gameCharacterData.maximumStatePoint + "\n" +
                              "回避壓力消耗:" + gameCharacterData.temp_StressEvasionMaxStatePointIncrease + "\n" +
                              "4.擴流:" + skill_PSE4_KuoLiu + "\n" +
                              "4.逆風:" + skill_PSE12_NiFeng;
        battleResultData.AddResultLog("角色名稱:" + gameCharacter.GetCharacterName() + "\n" + "\n" +
                                        _formula + "\n" + "\n" + _finalString);
    }

    //抵抗成功方回避當前以太值第2次結算
    public static void SuccessfulResisterEvadeCurrentStatePointSecondCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        battleResultData.AddResultLog("抵抗成功方回避當前以太值第2次結算");
        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float skill_PSE3_HuiLiu = 0.0f;
        float skill_PSE12_NiFeng = 1f;
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
            skill_PSE12_NiFeng = 2f;
        }

        //[當前以太值]+[回避壓力消耗以太提升*0.5*n]
        if(stressEvasionMaxStatePointIncrease != 0)
        {
            battleResultData.AddGameCharacterResultData_RestoreCurrentStatePointByPercentage(gameCharacter, stressEvasionMaxStatePointIncrease * skill_PSE3_HuiLiu * skill_PSE12_NiFeng, out _);
            string _formula = "[當前以太值] +[回避壓力消耗以太提升 * 3.回流 * 12.逆風]";
            string _finalString = "當前身份: 抵抗成功方" + "\n" +
                                  "當前以太值:" + gameCharacterData.currentStatePoint + "\n" +
                                  "回避壓力消耗以太提升:" + stressEvasionMaxStatePointIncrease + "\n" +
                                  "3.回流:" + skill_PSE3_HuiLiu + "\n" +
                                  "12.逆風:" + skill_PSE12_NiFeng;
            battleResultData.AddResultLog("角色名稱:" + gameCharacter.GetCharacterName() + "\n" + "\n" +
                                          "算式:" + _formula + "\n" + "\n" +
                                          _finalString);
        }
    }

    //抵抗成功方防禦當前以太值結算
    public static void SuccessfulResisterDefenseCurrentStatePointCalculation(ref BattleResultData battleResultData, GameCharacter successfulResistor, GameCharacter deuce)
    {
        battleResultData.AddResultLog("抵抗成功方防禦當前以太值結算");
        CurrentStatePointCalculation(ref battleResultData, successfulResistor, deuce);
    }

    //平手方當前以太值結算
    public static void RunDeuceCurrentStatPointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog("平手方當前以太值結算");
        CurrentStatePointCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo);
    }

    //直擊方當前以太值結算
    public static void RunLeadCurrentStatPointCalculation(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter receipent)
    {
        battleResultData.AddResultLog("直擊方當前以太值結算");
        CurrentStatePointCalculation(ref battleResultData, lead, receipent);
    }

    //受擊方當前以太值結算
    public static void RunRecepientCurrentStatPointCalculation(ref BattleResultData battleResultData, GameCharacter recepient, GameCharacter lead)
    {
        battleResultData.AddResultLog("受擊方當前以太值結算");
        CurrentStatePointCalculation(ref battleResultData, recepient, lead);
    }

    //負荷積分等級結算
    public static void StressScoreAndLevelCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        battleResultData.AddResultLog("負荷積分等級結算");

        BattleResultData.BattleResultData_GameCharacter gameCharacterData = battleResultData.GetGameCharacterResultData(gameCharacter);
        float skill_PSS5_JiXiao = 0.0f;
        float _gainStressScore;

        string _formula;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS5, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            skill_PSS5_JiXiao = 0.5f;
        }

        if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            //{[給予負荷傷害]+[受到負荷傷害]}*[1+n]
            _gainStressScore = gameCharacterData.stressValueDamageDealt + gameCharacterData.stressValueDamageTaken * (1 + skill_PSS5_JiXiao);
            _formula = "[給予負荷傷害] +[受到負荷傷害]} *[1 + 5.積效]";
        }
        else
        {
            //[給予負荷傷害]+[受到負荷傷害]
            _gainStressScore = gameCharacterData.stressValueDamageDealt + gameCharacterData.stressValueDamageTaken;
            _formula = "[給予負荷傷害]+[受到負荷傷害]";
        }
        battleResultData.AddGameCharacterResultData_AddStressScore(gameCharacter, Mathf.RoundToInt(_gainStressScore), out _);
        battleResultData.AddResultLog(  "角色名稱:" + gameCharacter.GetCharacterName() + "\n" + "\n" +
                                        "算式:" + _formula);
        
    }

    //負荷流借力結算
    public static void StressPassiveSkillTypeJieLiCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        battleResultData.AddResultLog("負荷流借力結算");

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
                string _formula = "[己方當前生命值] +[己方最大生命值] *[8.節流]";
                string _finalString = "角色名稱:" + gameCharacter.GetCharacterName() + "\n" + "\n" +
                                      "當前生命值:" + gameCharacterData.currentHealthPoint + "\n" + "\n" +
                                      "最大生命值:" + gameCharacterData.maximumHealthPoint + "\n" + "\n" +
                                      "8.節流:" + skill_PSS8_JieLi;
                battleResultData.AddResultLog(_formula + "\n" + "\n" + _finalString);
            }
        }
    }

    //對比條件的技能強度/速度增加
    public static void IncreaseStrengthOrSpeedWithCondition(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog("對比條件的技能強度/速度增加");

        BattleResultData.BattleResultData_GameCharacter gameCharacterOneData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwoData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        int skill_PSE11_YiTaiYaZhi = 0;
        int skill_PSS7_JieFeng = 0;

        string _activatingSkill = "";
        string _finalString;

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
                    _activatingSkill = "11.以太壓制: +" + skill_PSE11_YiTaiYaZhi;  
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
                        _activatingSkill = "7.:借風 +" + skill_PSE11_YiTaiYaZhi;
                    }
                }
                break;
        }
        _finalString =  "己方:" + gameCharacterOne.GetCharacterName() + "\n" +
                        "對方:" + gameCharacterTwo.GetCharacterName() + "\n" +
                        "玩家1當前流向:" + gameCharacterOne.GetSelectedPassiveSkillCategoryType() + "\n" +
                        "已使用技能:" + _activatingSkill;
        battleResultData.AddResultLog(_finalString);
    }

    //reuse function
    public static void CurrentStatePointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOneData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwoData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        float skill_PSE5_PoLiu = 0.0f;
        float hasEnergyMarker = gameCharacterOneData.HasEnergyMarker() ? 0.5f : 0.0f;

        string _formula = "";
        string _characterOneCurrentIdentity = "";
        string _characterTwoCurrentIdentity = "";

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE5, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSE5_PoLiu = 0.5f;
        }

        _characterOneCurrentIdentity = gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce) ? "平手方" :
          gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SuccessfulResister) ? "抵抗成功方" :
          gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead) ? "直擊方" :
          gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient) ? "受擊方" : "己方";

        _characterTwoCurrentIdentity = gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce) ? "平手方" :
          gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SuccessfulResister) ? "抵抗成功方" :
          gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead) ? "直擊方" :
          gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient) ? "受擊方" : "對方";

        if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            //["己方"當前以太值]-["對方"以太傷害]*[1+0.5+n]
            battleResultData.AddGameCharacterResultData_StatePointDamage(gameCharacterOne, gameCharacterTwoData.statePointDamageDealt * (1 + skill_PSE5_PoLiu + hasEnergyMarker), false, out _);
            _formula = "[" + _characterOneCurrentIdentity +"當前以太值] -[" + _characterTwoCurrentIdentity + "以太傷害] *[1 + 5.破流 + 能量殘響]";
        }
        else
        {
            //["己方"當前以太值]-["對方"以太傷害]*[1+n]
            battleResultData.AddGameCharacterResultData_StatePointDamage(gameCharacterOne, gameCharacterTwoData.statePointDamageDealt * (1 + hasEnergyMarker), false, out _);
            _formula = "[" +_characterOneCurrentIdentity +"當前以太值]-["+ _characterTwoCurrentIdentity +"以太傷害]*[1+能量殘響]";
        }

        string _finalString = _characterOneCurrentIdentity + ":" + gameCharacterOne.GetCharacterName() + "\n" +
                              "當前以太值:" + gameCharacterOneData.currentStatePoint + "\n" +
                              "5.破流:" + skill_PSE5_PoLiu + "\n" +
                              "能量殘響:" + hasEnergyMarker + "\n" + "\n" +
                              
                              _characterTwoCurrentIdentity + ":" + gameCharacterTwo.GetCharacterName() + "\n" +
                              "以太傷害:" + gameCharacterTwoData.statePointDamageDealt;
        battleResultData.AddResultLog("算式:" + _formula + "\n"+ "\n" + _finalString);
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

        string _formula = "";
        string _gameCharacterOnefinalString = "";
        string _gameCharacterTwofinalString = "";
        string _activatingPassiveSkillString = "";
        string _characterOneCurrentIdentity;
        string _characterTwoCurrentIdentity;
        string _isMultipleWithZeroPointFive = isMultipleZeroPointFive ? "*0.5" : "";

        _characterOneCurrentIdentity = gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce) ? "平手方" :
         gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead) ? "直擊方" :
         gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient) ? "受擊方" : "己方";

        _characterTwoCurrentIdentity = gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce) ? "平手方" :
          gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead) ? "直擊方" :
          gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient) ? "受擊方" : "對方";

        /*
       "對方"當前流向是否生命流&
       "對方"生命積分>=100&
       "對方"生命值%>"己方"20%?
       YES=1(發動9.生命壓制)
       NO=0
        */
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill) &&
            gameCharacterTwo.GetLifeScore() >= 100 && (gameCharacterTwoData.currentHealthPoint > gameCharacterOneData.currentHealthPoint * 0.2f))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSL9_ShengMingYaZhi = 1.0f;
            _activatingPassiveSkillString = "9.生命壓制:" + skill_PSL9_ShengMingYaZhi;
        }

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS3, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS3_HuaJing = 0.2f;
        }

        /*"己方"最大以太值是否
        >= 120 ?
        YES = 0.1(發動6.負荷流轉)
        NO = 0
        */
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS6, out _passiveSkill) && (gameCharacterOneData.maximumStatePoint >= 120))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS6_FuHeLiuZhuan = 0.1f;
        }

        /*
         "己方"負荷等級是否
        >=2?
        YES=0.2(發動9.行雲流水)
        NO=0
         */
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS9, out _passiveSkill) && (gameCharacterOne.GetStressLevel() >= 2))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS9_XingYunLiuShui = 0.2f;
        }

        /*      
        "對方"當前流向是否負荷流&
        "對方"負荷等級>=3&
        "對方"當前以太值>"己方"&
        "對方"負荷值<="己方"50?
        YES=1(發動11.負荷壓制2)
       NO=0
        */
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL11, out _passiveSkill) && (gameCharacterTwo.GetStressLevel() >= 3) &&
            (gameCharacterTwoData.currentStatePoint > gameCharacterOneData.currentStatePoint) &&
            (gameCharacterOneData.currentStressValue <= gameCharacterTwoData.currentStressValue) &&
            (gameCharacterOneData.currentStressValue - gameCharacterTwoData.currentStressValue >= 50))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS11_FuheYaZhi2 = 1.0f;
            _activatingPassiveSkillString = "11.負荷壓制2:" + skill_PSS11_FuheYaZhi2;
        }

        /*
         "己方"
        最大以太值是否<80?
        YES=0.2(發動12.逆風)
        NO=0
         */
        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS12, out _passiveSkill) && (gameCharacterOneData.maximumStatePoint < 80))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            skill_PSS12_NiFeng = 0.2f;
        }

        /*
         "己方"是否有能量殘響?
        YES=0.5
        NO=0
         */
        float gameCharacterOneEnergyMarker = (gameCharacterOne.HasEnergyMarker() ? 0.5f : 0);

        if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            activatingPassiveSkill = skill_PSS11_FuheYaZhi2;
        }
        else if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
            activatingPassiveSkill = skill_PSL9_ShengMingYaZhi;
        }

        if (gameCharacterOnePassiveSkillCategory == CategoryType.State)
        {
            //["己方"負荷值]+["對方"負荷傷害] *[1 + n + n] *[1 - (己方負荷抗性) - n - n]
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1 - (gameCharacterOneStressResistance - skill_PSS6_FuHeLiuZhuan - skill_PSS12_NiFeng))) * multiplyZeroPointFive, false, out _);

            _formula = "[" +_characterOneCurrentIdentity + "負荷值] +[" +_characterTwoCurrentIdentity + "負荷傷害] *[1 + 9.生命壓制 或者 11.負荷壓制2 + 能量殘響] * [1 -"
                           +_characterOneCurrentIdentity + "負荷抗性) -6.負荷流轉 - 12.逆風]]" + _isMultipleWithZeroPointFive;

            _gameCharacterOnefinalString = _characterOneCurrentIdentity + ":" + gameCharacterOne.GetCharacterName() + "\n" +
                                           "當前流向:" + gameCharacterOne.GetSelectedPassiveSkillCategoryType() + "\n" +
                                           "當前負荷值:" + gameCharacterOneData.currentStressValue + "\n" +
                                           "能量殘響:" + gameCharacterOneEnergyMarker + "\n" +
                                           "負荷抗性:" + gameCharacterOneStressResistance + "\n" +
                                           _activatingPassiveSkillString + "\n" +
                                           "6.負荷流轉:" + skill_PSS6_FuHeLiuZhuan + "\n" +
                                           "2.逆風:" + skill_PSS12_NiFeng;

            _gameCharacterTwofinalString = _characterTwoCurrentIdentity + ":" + gameCharacterTwo.GetCharacterName() + "\n" +
                                           "當前流向:" + gameCharacterTwo.GetSelectedPassiveSkillCategoryType() + "\n" +
                                           "負荷傷害:" + gameCharacterTwoData.stressValueDamageDealt;
                      
        }
        else if (gameCharacterOnePassiveSkillCategory == CategoryType.Stress)
        {
            //["己方"負荷值]+["對方"負荷傷害]*[1+n+n]*[1-(己方負荷抗性)-0.2-n)
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1 - (gameCharacterOneStressResistance - skill_PSS3_HuaJing - skill_PSS9_XingYunLiuShui))) * multiplyZeroPointFive, false, out _);

            _formula = "["+_characterOneCurrentIdentity+"負荷值] +["+_characterTwoCurrentIdentity+"負荷傷害] *[1 + 9.生命壓制 或者 11.負荷壓制2 + 能量殘響] *[1 - ("
                          +_characterOneCurrentIdentity+"負荷抗性) - 3.化勁 - 9.行雲流水)" + _isMultipleWithZeroPointFive;

            _gameCharacterOnefinalString = _characterOneCurrentIdentity + ":" + gameCharacterOne.GetCharacterName() + "\n" +
                                           "當前流向:" + gameCharacterOne.GetSelectedPassiveSkillCategoryType() + "\n" +
                                           "當前負荷值:" + gameCharacterOneData.currentStressValue + "\n" +
                                           "能量殘響:" + gameCharacterOneEnergyMarker + "\n" +
                                           "負荷抗性:" + gameCharacterOneStressResistance + "\n" +
                                           _activatingPassiveSkillString + "\n" +
                                           "3.化勁:" + skill_PSS3_HuaJing + "\n" +
                                           "9.行雲流水:" + skill_PSS9_XingYunLiuShui;

            _gameCharacterTwofinalString = _characterTwoCurrentIdentity + ":" + gameCharacterTwo.GetCharacterName() + "\n" +
                                           "當前流向:" + gameCharacterTwo.GetSelectedPassiveSkillCategoryType() + "\n" +
                                           "負荷傷害:" + gameCharacterTwoData.stressValueDamageDealt;
        }
        else if (gameCharacterOnePassiveSkillCategory == CategoryType.Life || gameCharacterOnePassiveSkillCategory == CategoryType.None)
        {
            //["己方"負荷值]+["對方"負荷傷害]*[1+n+n]*[1-(己方負荷抗性)]
            battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, gameCharacterOneData.currentStressValue + (gameCharacterTwoData.stressValueDamageDealt *
                (1 + activatingPassiveSkill + gameCharacterOneEnergyMarker) *
                (1 - gameCharacterOneStressResistance)) * multiplyZeroPointFive, false, out _);
            _formula = "[" + _characterOneCurrentIdentity + "負荷值] +[" + _characterTwoCurrentIdentity + "負荷傷害] *[1 + 9.生命壓制 或者 11.負荷壓制2 + 能量殘響] " +
                "*[1 - (" + _characterOneCurrentIdentity+"負荷抗性)]" + _isMultipleWithZeroPointFive;

            _gameCharacterOnefinalString = _characterOneCurrentIdentity + ":" + gameCharacterOne.GetCharacterName() + "\n" +
                                          "當前流向:" + gameCharacterOne.GetSelectedPassiveSkillCategoryType() + "\n" +
                                          "當前負荷值:" + gameCharacterOneData.currentStressValue + "\n" +
                                          "能量殘響:" + gameCharacterOneEnergyMarker + "\n" +
                                          "負荷抗性:" + gameCharacterOneStressResistance + "\n" +
                                          _activatingPassiveSkillString + "\n";

            _gameCharacterTwofinalString = _characterTwoCurrentIdentity + ":" + gameCharacterTwo.GetCharacterName() + "\n" +
                                         "當前流向:" + gameCharacterTwo.GetSelectedPassiveSkillCategoryType() + "\n" +
                                         "負荷傷害:" + gameCharacterTwoData.stressValueDamageDealt;
        }
        battleResultData.AddResultLog(_formula + "\n" + "\n" +
                                      _gameCharacterOnefinalString + "\n" + "\n" +
                                      _gameCharacterTwofinalString);
    }
}
