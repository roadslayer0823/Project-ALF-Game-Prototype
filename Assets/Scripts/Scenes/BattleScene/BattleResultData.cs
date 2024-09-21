using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using Subskill = DatabaseManager.Subskill;
using PassiveSkill = DatabaseManager.PassiveSkill;

public class BattleResultData
{
    private readonly List<BattleResultData_GameCharacter> gameCharacterResultDataList = new();
    private readonly List<string> resultLogList = new();

#if ALF_DEBUG

    private readonly string[] debugKeyStrings = new string[]
    {
        "gameCharacterName",
        "eventName"
    };

    private readonly string[] debugTempStrings = new string[]
    {
        "temp_FinalTotalStatePointCost",
        "temp_FinalMaximumStatePointIncrease",
        "temp_JiaoLiStatePointCost",
        "temp_JiaoLiMaxStatePointIncrease",
        "temp_StressEvasionCost",
        "temp_StressEvasionMaxStatePointIncrease"
    };

#endif

    public class BattleResultData_GameCharacter
    {
        private readonly BattleResultData battleResultData = null;
        private readonly GameCharacter gameCharacter = null;

#if ALF_DEBUG

        public string gameCharacterName = "";
        public string eventName = "";
        public string characterIdentityTypeListString = "";

#endif

        // 基本參數
        public float maximumHealthPoint = 0.0f;
        public float currentHealthPoint = 0.0f;
        [NonSerialized] public float virtualHealthPoint = 0.0f;

#if ALF_DEBUG

        public float virtualHealthDamage = 0.0f;

#endif

        public float originalStatePoint = 0.0f;
        public float maximumStatePoint = 0.0f;
        public float maximumStatePointWithoutBonus = 0.0f;
        public float minimumStatePoint = 0.0f;
        public float currentStatePoint = 0.0f;
        public float maximumStressValue = 0.0f;
        public float currentStressValue = 0.0f;

        // 技能參數
        public int currentSkillStrength = 0;
        public int currentSkillSpeed = 0;

        // 崩潰狀態
        public int stateBreakStatusRemainingATLs = 0;   // 以太崩潰維持值 (ATL)
        public int stressBreakStatusRemainingATLs = 0;  // 負荷崩潰維持值 (ATL)
        public int numberOfEnteringIntoBreakStatus = 0; // 陷入崩潰狀態的次數

        // 能量殘響
        public int energyMarkerRemainingATLs = 0;

        // 流向系統
        public int lifeScore = 0;       // 生命積分
        public int lifeScoreTarget = 0; // 得到一個循環點的生命積分目標
        public int lifeCyclePoint = 0;  // 循環點
        public int stressScore = 0;     // 負荷積分
        public int stressLevel = 0;     // 負荷等級

        // 其他狀態
        public bool isDead = false;

        // 改變參數（技能發動時）
        public float statePointCost = 0.0f; // 以太值消耗
        public float maximumStatePointIncreaseForBase = 0.0f;   // 最大以太值提升（基礎版）
        public float maximumStatePointIncreaseForBonus = 0.0f;  // 最大以太值提升（獎勵版）

        // 改變參數（命中目標時）
        public float actualHealthPointDamageDealt = 0.0f;   // 給予的HP值傷害點數(實傷)
        public float virtualHealthPointDamageDealt = 0.0f;  // 給予的HP值傷害點數(虛傷)
        public float statePointDamageDealt = 0.0f;          // 給予的以太值傷害點數
        public float stressValueDamageDealt = 0.0f;         // 給予的負荷值傷害點數
        public float actualHealthPointDamageTaken = 0.0f;   // 受到的HP值傷害點數(實傷)
        public float virtualHealthPointDamageTaken = 0.0f;  // 受到的HP值傷害點數(虛傷)
        public float statePointDamageTaken = 0.0f;          // 受到的以太值傷害點數
        public float stressValueDamageTaken = 0.0f;         // 受到的負荷值傷害點數

        // Temporary Variables    
        public float temp_FinalTotalStatePointCost = 0;             // <此以太消耗為"最終以太消耗">
        public float temp_FinalMaximumStatePointIncrease = 0;       // <此最大以太提升為"最終最大以太提升">
        public float temp_JiaoLiStatePointCost = 0;                 // <此以太消耗為"角力以太消耗">
        public float temp_JiaoLiMaxStatePointIncrease = 0;          // <此最大以太提升為"角力最大以太提升">
        public float temp_StressEvasionCost = 0;                    // <此為"回避壓力消耗>
        public float temp_StressEvasionMaxStatePointIncrease = 0;   // <此為"回避壓力消耗以太提升")

        // 流向技能
        [NonSerialized] public List<PassiveSkill> triggeredPassiveSkillList = new(); // 已經發動了的流向技能

#if ALF_DEBUG

        public string triggeredPassiveSkillListString = "";

        [NonSerialized] public string lastJsonString = "";

#endif

        public BattleResultData_GameCharacter( BattleResultData battleResultData, GameCharacter gameCharacter )
        {
            this.battleResultData = battleResultData;
            this.gameCharacter = gameCharacter;
        }

        public BattleResultData GetBattleResultData()
        {
            return this.battleResultData;
        }

        public GameCharacter GetGameCharacter()
        {
            return this.gameCharacter;
        }

        public bool IsInBreakStatus()
        {
            return ( IsInStateBreakStatus() || IsInStressBreakStatus() );
        }

        public bool IsInStateBreakStatus()
        {
            return ( this.stateBreakStatusRemainingATLs > 0 );
        }

        public bool IsInStressBreakStatus()
        {
            return ( this.stressBreakStatusRemainingATLs > 0 );
        }

        public bool HasEnergyMarker()
        {
            return ( this.energyMarkerRemainingATLs > 0 );
        }

        public void SetCurrentHealthPoint( float value, bool needToUpdateVirtualHealthPoint, bool forVirtualDamageOnly = false )
        {
            this.currentHealthPoint = Mathf.Clamp( value, 0.0f, ( forVirtualDamageOnly ) ? this.virtualHealthPoint : this.maximumHealthPoint );

            if (needToUpdateVirtualHealthPoint)
            {
                UpdateVirtualHealthPoint();
            }

            if (this.currentHealthPoint <= 0.0f)
            {
                this.currentHealthPoint = 0.0f;
                this.isDead = true;
            }
        }

        public void SetVirtualHealthPoint( float value )
        {
            this.virtualHealthPoint = Mathf.Clamp( value, 0.0f, this.maximumHealthPoint );
            UpdateVirtualHealthPoint();
        }

        private void UpdateVirtualHealthPoint()
        {
            if (this.virtualHealthPoint < this.currentHealthPoint)
            {
                this.virtualHealthPoint = this.currentHealthPoint;
            }
        }

        public void SetMaximumStatePoint( float value )
        {
            this.maximumStatePoint = Mathf.Clamp( value,
                GameConfiguration.Instance.GetBattleConfiguration().GetLowestMaximumStatePoint(),
                GameConfiguration.Instance.GetBattleConfiguration().GetHighestMaximumStatePoint() );
        }

        public void SetMaximumStatePointWithoutBonus( float value )
        {
            this.maximumStatePointWithoutBonus = Mathf.Clamp( value,
                GameConfiguration.Instance.GetBattleConfiguration().GetLowestMaximumStatePoint(),
                GameConfiguration.Instance.GetBattleConfiguration().GetHighestMaximumStatePoint() );
        }

        public void SetCurrentStatePoint( float value )
        {
            this.currentStatePoint = Mathf.Clamp( value, this.minimumStatePoint, this.maximumStatePoint );
        }

        public void SetCurrentStressValue( float value )
        {
            this.currentStressValue = Mathf.Clamp( value, 0.0f, this.maximumStressValue );
        }

        public float GetVirtualHealthDamage()
        {
            if (this.virtualHealthPoint > this.currentHealthPoint)
            {
                return ( this.virtualHealthPoint - this.currentHealthPoint );
            }

            return 0.0f;
        }

        public float GetHealthPointDamageTaken()
        {
            if (this.actualHealthPointDamageTaken > 0)
            {
                return this.actualHealthPointDamageTaken;
            }

            if (this.virtualHealthPointDamageTaken > 0)
            {
                return this.virtualHealthPointDamageTaken;
            }

            return 0.0f;
        }

        public bool HasPassiveSkillTriggered( string skillId )
        {
            return ( triggeredPassiveSkillList.FirstOrDefault( ps => ps.Id == skillId ) != null );
        }
    }

    private void AddNewElementIntoGameCharacterResultDataList( BattleResultData_GameCharacter gameCharacterResultData, bool isNewElement )
    {
        if (isNewElement)
        {
            this.gameCharacterResultDataList.Add( gameCharacterResultData );
        }

        UpdateDebugLog( gameCharacterResultData );
    }

    public void UpdateDebugLog( BattleResultData_GameCharacter gameCharacterResultData, string eventName = "" )
    {
#if ALF_DEBUG

        if (eventName != "")
        {
            gameCharacterResultData.eventName = eventName;
        }

        gameCharacterResultData.virtualHealthDamage = gameCharacterResultData.GetVirtualHealthDamage();
        gameCharacterResultData.characterIdentityTypeListString = GetCharacterIdentityTypeListString( gameCharacterResultData.GetGameCharacter().GetAllCharacterIdentityTypes() );

        string _triggeredPassiveSkillListString = "";

        List<PassiveSkill> _triggeredPassiveSkillList = gameCharacterResultData.triggeredPassiveSkillList;
        for (int i = 0; i < _triggeredPassiveSkillList.Count; i++)
        {
            _triggeredPassiveSkillListString += ( ( i > 0 ) ? " | " : "" ) + _triggeredPassiveSkillList[ i ].DisplayName;
        }

        gameCharacterResultData.triggeredPassiveSkillListString = _triggeredPassiveSkillListString;

        string _lastJsonString = gameCharacterResultData.lastJsonString;

        gameCharacterResultData.lastJsonString = JsonConvert.SerializeObject( gameCharacterResultData );

        char[] _charactersToTrim = new char[] { '{', '}' };
        string[] _oldJsonStringArray = _lastJsonString.Trim( _charactersToTrim ).Split( ',' );
        string[] _newJsonStringArray = gameCharacterResultData.lastJsonString.Trim( _charactersToTrim ).Split( ',' );

        List<string> _oldTextArray = new();
        List<string> _newTextArray = new();
        for (int i = 0; i < _oldJsonStringArray.Length; i++)
        {
            string _oldJsonString = _oldJsonStringArray[ i ];
            string _newJsonString = _newJsonStringArray[ i ];
            if (_oldJsonString != _newJsonString
                || _newJsonString.Contains( "gameCharacterName" )
                || _newJsonString.Contains( "eventName" ))
            {
                _oldTextArray.Add( _oldJsonString );
                _newTextArray.Add( _newJsonString );
            }
        }

        string _battleLog = "";
        string _jsonString = JsonConvert.SerializeObject( gameCharacterResultData, Formatting.Indented );
        for (int i = 0; i < _newTextArray.Count; i++)
        {
            string _oldText = _oldTextArray[ i ];
            string _newText = _newTextArray[ i ];

            string _oldTextValue = _oldText.Split( ':' )[ 1 ];
            string _newTextValue = _newText.Split( ':' )[ 1 ];

            string _extraString = "";
            if (int.TryParse( _oldTextValue, out int _oldTextValueInt )
                && int.TryParse( _newTextValue, out int _newTextValueInt ))
            {
                int _valueDifferenceInt = _newTextValueInt - _oldTextValueInt;
                if (_valueDifferenceInt != 0)
                {
                    _extraString += $" ({ ( ( _valueDifferenceInt > 0 ) ? "+" : "" ) }{ _valueDifferenceInt })";
                }
            }
            else if (float.TryParse( _oldTextValue, out float _oldTextValueFloat )
                && float.TryParse( _newTextValue, out float _newTextValueFloat ))
            {
                float _valueDifferenceFloat = _newTextValueFloat - _oldTextValueFloat;
                if (_valueDifferenceFloat != 0)
                {
                    _extraString += $" ({ ( ( _valueDifferenceFloat > 0 ) ? "+" : "" ) }{ _valueDifferenceFloat })";
                }
            }

            string _stringToReplace = _newText.Replace( ":", ": " );
            string[] _stringArray = _newText.Split( ':' );

            string _log = "";
            if (debugKeyStrings.Any( s => _stringToReplace.Contains( s )))
            {
                _log = $"<color=#FFAAFF>{ _stringToReplace }</color>";
            }
            else if (debugTempStrings.Any( s => _stringToReplace.Contains( s ) ))
            {
                _log = $"<color=#DCDCDC>{ _stringToReplace }</color>";
            }
            else
            {
                _log = $"<color=#FFFF00>{ _oldText.Replace( ":", ": " ) } -> { _stringArray[ 1 ] }{ _extraString }</color>";
            }

            _jsonString = _jsonString.Replace( _stringToReplace, _log );

            _battleLog += ( ( _battleLog != "" ) ? "\n" : "" ) + _log;
        }

        Debug.Log( $"######[ DEBUG ]######\n{ _jsonString }\n#####################\n" );

        AddResultLog( _battleLog.Replace( "\"", "" )
                        .ReplaceFirst( "gameCharacterName", "角色" )
                        .ReplaceFirst( "eventName", "事件" )
                        .ReplaceFirst( "characterIdentityTypeListString", "身份" )
                        .ReplaceFirst( "maximumHealthPoint", "最大生命值" )
                        .ReplaceFirst( "currentHealthPoint", "當前生命值" )
                        .ReplaceFirst( "virtualHealthDamage", "虛傷" )
                        .ReplaceFirst( "maximumStatePointIncreaseForBase", "最大以太值提升(基礎版)" )
                        .ReplaceFirst( "maximumStatePointIncreaseForBonus", "最大以太值提升(獎勵版)" )
                        .ReplaceFirst( "maximumStatePointWithoutBonus", "最大以太值(不包括獎勵版)" )
                        .ReplaceFirst( "maximumStatePoint", "最大以太值" )
                        .ReplaceFirst( "currentStatePoint", "當前以太值" )
                        .ReplaceFirst( "maximumStressValue", "最大負荷值" )
                        .ReplaceFirst( "currentStressValue", "當前負荷值" )
                        .ReplaceFirst( "currentSkillStrength", "技能強度" )
                        .ReplaceFirst( "currentSkillSpeed", "技能速度" )
                        .ReplaceFirst( "stateBreakStatusRemainingATLs", "以太崩潰(ATL)" )
                        .ReplaceFirst( "stressBreakStatusRemainingATLs", "負荷崩潰(ATL)" )
                        .ReplaceFirst( "numberOfEnteringIntoBreakStatus", "陷入崩潰狀態的次數" )
                        .ReplaceFirst( "energyMarkerRemainingATLs", "能量殘響(ATL)" )
                        .ReplaceFirst( "lifeScoreTarget", "生命積分目標" )
                        .ReplaceFirst( "lifeScore", "生命積分" )
                        .ReplaceFirst( "lifeCyclePoint", "循環點" )
                        .ReplaceFirst( "stressScore", "負荷積分" )
                        .ReplaceFirst( "stressLevel", "負荷等級" )
                        .ReplaceFirst( "isDead", "是否死亡" )
                        .ReplaceFirst( "statePointCost", "以太值消耗" )
                        .ReplaceFirst( "actualHealthPointDamageDealt", "給予實傷" )
                        .ReplaceFirst( "virtualHealthPointDamageDealt", "給予虛傷" )
                        .ReplaceFirst( "statePointDamageDealt", "給予以太傷害" )
                        .ReplaceFirst( "stressValueDamageDealt", "給予負荷傷害" )
                        .ReplaceFirst( "actualHealthPointDamageTaken", "受到實傷" )
                        .ReplaceFirst( "virtualHealthPointDamageTaken", "受到虛傷" )
                        .ReplaceFirst( "statePointDamageTaken", "受到以太傷害" )
                        .ReplaceFirst( "stressValueDamageTaken", "受到負荷傷害" )
                        .ReplaceFirst( "triggeredPassiveSkillListString", "發動了的流向技能" )
                        .ReplaceFirst( "temp_FinalTotalStatePointCost", "最終以太消耗" )
                        .ReplaceFirst( "temp_FinalMaximumStatePointIncrease", "最終最大以太提升" )
                        .ReplaceFirst( "temp_JiaoLiStatePointCost", "角力以太消耗" )
                        .ReplaceFirst( "temp_JiaoLiMaxStatePointIncrease", "角力最大以太提升" )
                        .ReplaceFirst( "temp_StressEvasionCost", "回避壓力消耗" )
                        .ReplaceFirst( "temp_StressEvasionMaxStatePointIncrease", "回避壓力消耗以太提升" )
                        .Replace( "true", "是" )
                        .Replace( "false", "否" )
                        );

#endif
    }

    private string GetCharacterIdentityTypeListString( List<CharacterIdentityType> characterIdentityTypeList )
    {
        string _characterIdentityTypeString = "";

        for (int i = 0; i < characterIdentityTypeList.Count; i++)
        {
            _characterIdentityTypeString += ( ( i > 0 ) ? " | " : "" ) +
                                            characterIdentityTypeList[ i ] switch
                                            {
                                                CharacterIdentityType.PlayerOne => "玩家1",
                                                CharacterIdentityType.PlayerTwo => "玩家2",
                                                CharacterIdentityType.Lead => "先手方",
                                                CharacterIdentityType.Improviser => "後手方",
                                                CharacterIdentityType.Deuce => "平手方",
                                                CharacterIdentityType.SuccessfulResister => "抵抗成功方",
                                                CharacterIdentityType.Assaulter => "直擊方",
                                                CharacterIdentityType.LightAssaulter => "輕直擊方",
                                                CharacterIdentityType.HeavyAssaulter => "重直擊方",
                                                CharacterIdentityType.Recipient => "受擊方",
                                                CharacterIdentityType.LightRecipient => "輕受擊方",
                                                CharacterIdentityType.HeavyRecipient => "重受擊方",
                                                CharacterIdentityType.NonResister => "未能抵抗方",
                                                CharacterIdentityType.SpeedWinner => "速度勝方",
                                                CharacterIdentityType.StrengthWinner => "強度勝方",
                                                CharacterIdentityType.SpeedLoser => "速度負方",
                                                CharacterIdentityType.StrengthLoser => "強度負方",
                                                CharacterIdentityType.SpeedStrengthLoser => "速度強度負方",
                                                CharacterIdentityType.StateBreakStatusHolder => "以太崩潰方",
                                                CharacterIdentityType.StressBreakStatusHolder => "負荷崩潰方",
                                                CharacterIdentityType.WinningBenefitHolder => "勝利優惠機制方",
                                                CharacterIdentityType.NearDistanceRangedDealer => "近距離遠程方",
                                                CharacterIdentityType.NormalDistanceMeleeDealer => "中距離近戰方",
                                                CharacterIdentityType.UpdatedSelectedSkill => "已更新按下技能方",
                                                CharacterIdentityType.IgnoreZhuiFengJiaoLi => "無視追風角力方",
                                                CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng => "無視追風角力激昂方",
                                                CharacterIdentityType.IgnoreRangedSkill => "無視遠程方",
                                                _ => ""
                                            };
        }

        return _characterIdentityTypeString;
    }

    // 回復受到的“虛傷”。
    public BattleResultData AddGameCharacterResultData_VirtualHealthPointDamageRecovered( GameCharacter gameCharacter, float virtualHealthPointDamageRecovered, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint + Mathf.Round(virtualHealthPointDamageRecovered), false, true );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "回復受到的“虛傷”";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 回復受到的“實傷”。
    public BattleResultData AddGameCharacterResultData_ActualHealthPointDamageRecovered( GameCharacter gameCharacter, float actualHealthPointDamageRecovered, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint + Mathf.Round(actualHealthPointDamageRecovered), true );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "回復受到的“實傷”";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 降低最大以太值。
    public BattleResultData AddGameCharacterResultData_MaximumStatePointDecrease( GameCharacter gameCharacter, float maximumStatePointDecrease, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetMaximumStatePoint( gameCharacterResultData.maximumStatePoint - Mathf.Round(maximumStatePointDecrease) );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "降低最大以太值";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 提升最大以太值（基礎版）。
    public BattleResultData AddGameCharacterResultData_MaximumStatePointIncreaseForBase( GameCharacter gameCharacter, float maximumStatePointIncrease, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        maximumStatePointIncrease = Mathf.Round(maximumStatePointIncrease);
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.maximumStatePointIncreaseForBase += maximumStatePointIncrease;
        gameCharacterResultData.SetMaximumStatePoint( gameCharacterResultData.maximumStatePoint + maximumStatePointIncrease );
        gameCharacterResultData.SetMaximumStatePointWithoutBonus( gameCharacterResultData.maximumStatePointWithoutBonus + maximumStatePointIncrease );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "提升最大以太值（基礎版）";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 提升最大以太值（獎勵版）。
    public BattleResultData AddGameCharacterResultData_MaximumStatePointIncreaseForBonus( GameCharacter gameCharacter, float maximumStatePointIncrease, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        maximumStatePointIncrease = Mathf.Round( maximumStatePointIncrease );
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.maximumStatePointIncreaseForBonus += maximumStatePointIncrease;
        gameCharacterResultData.SetMaximumStatePoint( gameCharacterResultData.maximumStatePoint + maximumStatePointIncrease );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "提升最大以太值（獎勵版）";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 降低當前負荷值。
    public BattleResultData AddGameCharacterResultData_StressValueDamageRecovered( GameCharacter gameCharacter, float stressValueDamageRecovered, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStressBreakStatus())
        {
            gameCharacterResultData.SetCurrentStressValue( gameCharacterResultData.currentStressValue - Mathf.Round(stressValueDamageRecovered) );
        }

#if ALF_DEBUG

        gameCharacterResultData.eventName = "降低當前負荷值";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 以太值消耗
    public BattleResultData AddGameCharacterResultData_StatePointCost( GameCharacter gameCharacter, float statePointCost, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        statePointCost = Mathf.Round(statePointCost);
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStateBreakStatus())
        {
            gameCharacterResultData.statePointCost += statePointCost;
            gameCharacterResultData.SetCurrentStatePoint( gameCharacterResultData.currentStatePoint - statePointCost );
        }

#if ALF_DEBUG

        gameCharacterResultData.eventName = "以太值消耗";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 以太值傷害
    public BattleResultData AddGameCharacterResultData_StatePointDamage( GameCharacter gameCharacter, float statePointDamage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        statePointDamage = Mathf.Round(statePointDamage);
        GameCharacter _currentAttacker = gameCharacter.GetCurrentAttacker();
        if (_currentAttacker != null)
        {
            BattleResultData_GameCharacter _attackerResultData = GetGameCharacterResultData( _currentAttacker );
            _attackerResultData.statePointDamageDealt += statePointDamage;
        }

        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStateBreakStatus())
        {
            gameCharacterResultData.statePointDamageTaken += statePointDamage;
            gameCharacterResultData.SetCurrentStatePoint( gameCharacterResultData.currentStatePoint - statePointDamage );

            if (gameCharacterResultData.currentStatePoint < 1 && gameCharacter.HasCharacterIdentityType(CharacterIdentityType.WinningBenefitHolder))
            {
                // 勝利優惠機制解說
                // 當前以太值最低為1
                Debug.Log("gameCharacter.HasCharacterIdentityType 勝利優惠機制 當前以太值最低為1");
                gameCharacterResultData.SetCurrentStatePoint(1);
            }
            //else if (gameCharacterResultData.currentStatePoint < 0)
            //{
            //    // 陷入以太崩潰狀態。
            //    gameCharacterResultData.stateBreakStatusRemainingATLs = 1;
            //    gameCharacterResultData.numberOfEnteringIntoBreakStatus++;
            //    BattleLogicManagerV2.OnGameCharacterBeingInBreakStatus( gameCharacter );
            //}
        }

#if ALF_DEBUG

        gameCharacterResultData.eventName = "以太值傷害";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 以太崩潰狀態
    public BattleResultData AddGameCharacterResultData_StateBreakStatus( GameCharacter gameCharacter, int numberOfATLs, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.stateBreakStatusRemainingATLs = numberOfATLs;
        gameCharacterResultData.numberOfEnteringIntoBreakStatus++;

        // 記錄崩潰前一刻的以太值數值。
        gameCharacter.SetStatePointBeforeBreakStatus( gameCharacter.GetCurrentStatePoint() );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "以太崩潰狀態";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 負荷值傷害
    public BattleResultData AddGameCharacterResultData_StressValueDamage( GameCharacter gameCharacter, float stressValueDamage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        stressValueDamage = Mathf.Round(stressValueDamage);
        GameCharacter _currentAttacker = gameCharacter.GetCurrentAttacker();
        if (_currentAttacker != null)
        {
            BattleResultData_GameCharacter _attackerResultData = GetGameCharacterResultData( _currentAttacker );
            _attackerResultData.stressValueDamageDealt += stressValueDamage;
        }

        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStressBreakStatus())
        {
            gameCharacterResultData.stressValueDamageTaken += stressValueDamage;
            gameCharacterResultData.SetCurrentStressValue( gameCharacterResultData.currentStressValue + stressValueDamage );

            if (gameCharacterResultData.currentStressValue >= gameCharacterResultData.maximumStressValue)
            {
                if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.WinningBenefitHolder))
                {
                    Debug.Log("gameCharacter.HasCharacterIdentityType 勝利優惠機制 只能達到最高 99%");
                    // 勝利優惠機制解說
                    // 由於不會陷入崩潰狀態，該角色的負荷值只能達到最高 99% 而已。
                    float _stressValueReduction = gameCharacterResultData.currentStressValue - (gameCharacterResultData.maximumStressValue - 1);
                    gameCharacterResultData.stressValueDamageTaken -= _stressValueReduction;
                    gameCharacterResultData.currentStressValue -= _stressValueReduction;
                }
                //else
                //{                  
                //    // 陷入負荷崩潰狀態。
                //    gameCharacterResultData.stressBreakStatusRemainingATLs = 1;
                //    gameCharacterResultData.numberOfEnteringIntoBreakStatus++;
                //    BattleLogicManagerV2.OnGameCharacterBeingInBreakStatus(gameCharacter);
                //}
            }
        }

#if ALF_DEBUG

        gameCharacterResultData.eventName = "負荷值傷害";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 負荷崩潰狀態
    public BattleResultData AddGameCharacterResultData_StressBreakStatus( GameCharacter gameCharacter, int numberOfATLs, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.stressBreakStatusRemainingATLs = numberOfATLs;
        gameCharacterResultData.numberOfEnteringIntoBreakStatus++;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "負荷崩潰狀態";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // HP 傷害（實傷）
    public BattleResultData AddGameCharacterResultData_ActualHealthPointDamage( GameCharacter gameCharacter, float actualHealthPointDamage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        actualHealthPointDamage = ( actualHealthPointDamage < 1.0f ) ? 1.0f : Mathf.Round( actualHealthPointDamage );

        GameCharacter _currentAttacker = gameCharacter.GetCurrentAttacker();
        if (_currentAttacker != null)
        {
            BattleResultData_GameCharacter _attackerResultData = GetGameCharacterResultData( _currentAttacker );
            _attackerResultData.actualHealthPointDamageDealt += actualHealthPointDamage;
        }

        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.actualHealthPointDamageTaken += actualHealthPointDamage;
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint - actualHealthPointDamage, false );
        gameCharacterResultData.SetVirtualHealthPoint( gameCharacterResultData.virtualHealthPoint - actualHealthPointDamage );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "生命值傷害（實傷）";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // HP 傷害（虛傷）
    public BattleResultData AddGameCharacterResultData_VirtualHealthPointDamage( GameCharacter gameCharacter, float virtualHealthPointDamage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        virtualHealthPointDamage = ( virtualHealthPointDamage < 1.0f ) ? 1.0f : Mathf.Round( virtualHealthPointDamage );

        GameCharacter _currentAttacker = gameCharacter.GetCurrentAttacker();
        if (_currentAttacker != null)
        {
            BattleResultData_GameCharacter _attackerResultData = GetGameCharacterResultData( _currentAttacker );
            _attackerResultData.virtualHealthPointDamageDealt += virtualHealthPointDamage;
        }

        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.virtualHealthPointDamageTaken += virtualHealthPointDamage;
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint - virtualHealthPointDamage, true );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "生命值傷害（虛傷）";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 更新能量殘響的 ATL 。
    public BattleResultData AddGameCharacterResultData_RenewedEnergyMarkerATLs( GameCharacter gameCharacter, int renewedEnergyMarkerATLs, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.energyMarkerRemainingATLs = renewedEnergyMarkerATLs;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "更新能量殘響的ATL";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 改變能量殘響的 ATL。
    public BattleResultData AddGameCharacterResultData_ModifyEnergyMarkerATL( GameCharacter gameCharacter, int valueChange, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.energyMarkerRemainingATLs += valueChange;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "改變能量殘響的ATL";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 消去能量殘響。
    public BattleResultData AddGameCharacterResultData_RemoveEnergyMarker( GameCharacter gameCharacter, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.energyMarkerRemainingATLs = 0;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "消去能量殘響";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 提升當前以太值。
    public BattleResultData AddGameCharacterResultData_IncreaseCurrentStatePoint( GameCharacter gameCharacter, float statePoint, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentStatePoint( gameCharacterResultData.currentStatePoint + Mathf.Round( statePoint ) );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "提升當前以太值";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 當前以太值回復至最大以太值的指定巴仙率。
    public BattleResultData AddGameCharacterResultData_RestoreCurrentStatePointByPercentage( GameCharacter gameCharacter, float restorationPercentage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentStatePoint( Mathf.Round( gameCharacterResultData.maximumStatePoint * Mathf.Clamp01( restorationPercentage ) ) );

#if ALF_DEBUG

        gameCharacterResultData.eventName = $"當前以太值回復至最大以太值的{ restorationPercentage.ConvertToIntegerInPercentage() }%";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 尚未回復的虛傷部分全數轉化為實傷。
    public BattleResultData AddGameCharacterResultData_ConvertAllVirtualDamageToActualDamage( GameCharacter gameCharacter, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetVirtualHealthPoint( gameCharacterResultData.currentHealthPoint );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "尚未回復的虛傷部分全數轉化為實傷";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 改變當前技能的強度。
    public BattleResultData AddGameCharacterResultData_ChangeCurrentSkillStrength( GameCharacter gameCharacter, int valueToChange, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.currentSkillStrength += valueToChange;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "改變當前技能的強度";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 改變當前技能的速度。
    public BattleResultData AddGameCharacterResultData_ChangeCurrentSkillSpeed( GameCharacter gameCharacter, int valueToChange, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.currentSkillSpeed += valueToChange;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "改變當前技能的速度";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 更新狀態。
    public BattleResultData_GameCharacter AddGameCharacterResultData( GameCharacter gameCharacter,
                                                                      int stateBreakStatusRemainingATLs = 0, float maximumStatePoint = 0.0f, float currentStatePoint = 0.0f,
                                                                      int stressBreakStatusRemainingATLs = 0, float currentStressValue = 0.0f )
    {
        BattleResultData_GameCharacter _gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        // 以太
        _gameCharacterResultData.stateBreakStatusRemainingATLs = stateBreakStatusRemainingATLs;
        _gameCharacterResultData.SetMaximumStatePoint( maximumStatePoint );
        _gameCharacterResultData.SetCurrentStatePoint( currentStatePoint );

        // 負荷
        _gameCharacterResultData.stressBreakStatusRemainingATLs = stressBreakStatusRemainingATLs;
        _gameCharacterResultData.SetCurrentStressValue( currentStressValue );

        AddNewElementIntoGameCharacterResultDataList( _gameCharacterResultData, _isNewElement );
        return _gameCharacterResultData;
    }

    // 發動流向技能。
    public BattleResultData AddGameCharacterResultData_TriggerPassiveSkill( GameCharacter gameCharacter, PassiveSkill passiveSkill, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        string _passiveSkillId = passiveSkill.Id;

        if (!gameCharacterResultData.HasPassiveSkillTriggered( _passiveSkillId ))
        {
            gameCharacterResultData.triggeredPassiveSkillList.Add( passiveSkill );
        }

        gameCharacter.TriggerPassiveSkill( _passiveSkillId );

#if ALF_DEBUG

        gameCharacterResultData.eventName = "發動流向技能";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 增加生命積分。
    public BattleResultData AddGameCharacterResultData_AddLifeScore( GameCharacter gameCharacter, int score, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.lifeScore += score;

        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();
        int _lifeScoreTargetToGainLifeCyclePoint = _battleConfiguration.GetLifeScoreTargetToGainLifeCyclePoint();

        if (_lifeScoreTargetToGainLifeCyclePoint > 0)
        {
            if (gameCharacterResultData.lifeScoreTarget <= 0)
            {
                gameCharacterResultData.lifeScoreTarget = _lifeScoreTargetToGainLifeCyclePoint;
            }

            while (gameCharacterResultData.lifeScore >= gameCharacterResultData.lifeScoreTarget)
            {
                if (gameCharacterResultData.lifeCyclePoint < _battleConfiguration.GetMaximumLifeCyclePoint())
                {
                    gameCharacterResultData.lifeCyclePoint += 1;
                }

                gameCharacterResultData.lifeScoreTarget += _lifeScoreTargetToGainLifeCyclePoint;
            }
        }

#if ALF_DEBUG

        gameCharacterResultData.eventName = "增加生命積分";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 消耗全部循環點。
    public BattleResultData AddGameCharacterResultData_ResetLifeCyclePoint( GameCharacter gameCharacter, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.lifeCyclePoint = 0;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "消耗全部循環點";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 增加負荷積分。
    public BattleResultData AddGameCharacterResultData_AddStressScore( GameCharacter gameCharacter, int score, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.stressScore += score;

        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();
        int _stressScore = gameCharacterResultData.stressScore;

        gameCharacterResultData.stressLevel = ( _stressScore >= _battleConfiguration.GetStressScoreTargetForStressLevelThree() ) ? 3 :
                                              ( _stressScore >= _battleConfiguration.GetStressScoreTargetForStressLevelTwo() )   ? 2 :
                                              ( _stressScore >= _battleConfiguration.GetStressScoreTargetForStressLevelOne() )   ? 1 :
                                              0;

#if ALF_DEBUG

        gameCharacterResultData.eventName = "增加負荷積分";

#endif

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    public BattleResultData_GameCharacter GetGameCharacterResultData( GameCharacter gameCharacter )
    {
        return GetGameCharacterResultData( gameCharacter, out _ );
    }

    public BattleResultData_GameCharacter GetGameCharacterResultData( GameCharacter gameCharacter, out bool isNewElement )
    {
        isNewElement = false;

        BattleResultData_GameCharacter _gameCharacterResultData = gameCharacterResultDataList.FirstOrDefault( p => p.GetGameCharacter() == gameCharacter );

        if (_gameCharacterResultData == null)
        {
            isNewElement = true;

            CharacterSkill _currentSkill = gameCharacter.GetCurrentSkill();
            Subskill _subskillData = _currentSkill?.GetCharacterSubskillData().GetSubskillData();

            _gameCharacterResultData = new BattleResultData_GameCharacter( this, gameCharacter )
            {
#if ALF_DEBUG

                gameCharacterName = $"{ gameCharacter.GetCharacterName() } { ( gameCharacter.GetIsPlayer() ? "<color=#ADD8E6>[ PLAYER ]</color>" : "<color=#FF0000>[ ENEMY ]</color>" ) }",
                characterIdentityTypeListString = GetCharacterIdentityTypeListString( gameCharacter.GetPermanentCharacterIdentityTypeList() ),

#endif
                maximumHealthPoint = gameCharacter.GetMaximumHealthPoint(),
                currentHealthPoint = gameCharacter.GetCurrentHealthPoint(),
                virtualHealthPoint = gameCharacter.GetVirtualHealthPoint(),
                originalStatePoint = gameCharacter.GetOriginalStatePoint(),
                maximumStatePoint = gameCharacter.GetMaximumStatePoint(),
                maximumStatePointWithoutBonus = gameCharacter.GetMaximumStatePoint(),
                minimumStatePoint = gameCharacter.GetMinimumStatePoint(),
                currentStatePoint = gameCharacter.GetCurrentStatePoint(),
                maximumStressValue = gameCharacter.GetMaximumStressValue(),
                currentStressValue = gameCharacter.GetCurrentStressValue(),

                currentSkillStrength = ( _currentSkill != null ) ? _subskillData.Strength : 0,
                currentSkillSpeed = ( _currentSkill != null ) ? _subskillData.Speed : 0,

                stateBreakStatusRemainingATLs = gameCharacter.GetStateBreakStatusRemainingATLs(),
                stressBreakStatusRemainingATLs = gameCharacter.GetStressBreakStatusRemainingATLs(),
                numberOfEnteringIntoBreakStatus = gameCharacter.GetNumberOfEnteringIntoBreakStatus(),

                energyMarkerRemainingATLs = gameCharacter.GetEnergyMarkerRemainingATLs(),

                lifeScore = gameCharacter.GetLifeScore(),
                lifeScoreTarget = gameCharacter.GetLifeScoreTarget(),
                lifeCyclePoint = gameCharacter.GetLifeCyclePoint(),
                stressScore = gameCharacter.GetStressScore(),
                stressLevel = gameCharacter.GetStressLevel()
            };

#if ALF_DEBUG

            _gameCharacterResultData.lastJsonString = JsonConvert.SerializeObject( _gameCharacterResultData );

#endif

            this.gameCharacterResultDataList.Add( _gameCharacterResultData );
        }

        gameCharacter.SetTemporaryBattleResultData( _gameCharacterResultData );

        return _gameCharacterResultData;
    }

    public void AddResultLog( string resultLog )
    {
        this.resultLogList.Add( resultLog );
    }

    public List<string> GetResultLogList()
    {
        return this.resultLogList;
    }
}
