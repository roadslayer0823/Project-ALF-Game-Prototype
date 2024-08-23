using UnityEngine;
using static CharacterAnimationHandler;

public partial class BattleAnimationManager : MonoBehaviour
{
    //判定PART A演出&PART A特效
    public AnimationParameterData DetermineAnimationAndVisualEffectForPartA(ref BattleResultData battleResultData, GameCharacter lead,GameCharacter playerOne, GameCharacter playerTwo)
    {
        AnimationParameterData _animationParameterData = null;
        BattleResultData.BattleResultData_GameCharacter _playerOne_BattleResultData = battleResultData.GetGameCharacterResultData(playerOne);
        BattleResultData.BattleResultData_GameCharacter _playerTwo_BattleResultData = battleResultData.GetGameCharacterResultData(playerTwo);
        DatabaseManager.Subskill _playerOneSubskill = _playerOne_BattleResultData.gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

        int _playerOneSkillType = (int)_playerOne_BattleResultData.gameCharacter.GetCurrentSkillRangeType();
        int _playerTwoSkillType = 0;
        string _playerOneSubskillId = _playerOneSubskill.Id;
        string _playerTwoSubskillId = "";

        CharacterSkill _playerTwoCurrentSkill = _playerTwo_BattleResultData.gameCharacter.GetCurrentSkill();
        if (_playerTwoCurrentSkill != null)
        {
            _playerTwoSubskillId = _playerTwoCurrentSkill.GetCharacterSubskillData().GetSubskillData().Id;
            _playerTwoSkillType = (int)_playerTwo_BattleResultData.gameCharacter.GetCurrentSkillRangeType();
        }

        /*先手方已按下的技能速度是否3以上?*/
        /*先手方已按下的技能強度是否2以上?*/
        skillPromptPanel.PlaySpeedStrengthAnimation(lead);

        /*
         先手方是否
        "近距離遠程方"?
         */
        if (lead.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
        {
            // "己方"是否"先手方"?
            if(playerOne == lead)
            {
                /*
                 * 播放
                  "己方"已按下技能
                  camB_partA_VF
                 */
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camB_partA_VF, _playerOneSubskillId, _playerOneSkillType);
            }
            else
            {
                /*
                 * 播放
                   "敵方"已按下技能
                   camA_partA_VF
                 */
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camA_partA_VF, _playerTwoSubskillId, _playerTwoSkillType);
            }
        }
        else
        {
            if(playerOne == lead)
            {
                /*
                 * 播放
                  "己方"已按下技能
                   camB_partA_V1
                 */
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camB_partA_V1, _playerOneSubskillId, _playerOneSkillType);
            }
            else
            {
                /*
                 * 播放
                   "敵方"已按下技能
                   camA_partA_V1
                 */
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camA_partA_V1, _playerTwoSubskillId, _playerTwoSkillType);
            }
        }
        return _animationParameterData;
    }

    // 判定PART B共用特效
    public (VisualEffectParameterData visualEffectParameterDataForPlayerOne, VisualEffectParameterData visualEffectParameterDataForPlayerTwo) DetermineVisualEffectForPartB(GameCharacter playerOne, GameCharacter playerTwo)
    {
        VisualEffectParameterData _visualEffectParameterDataForPlayerOne = null;
        VisualEffectParameterData _visualEffectParameterDataForPlayerTwo = null;
    
        // "平手方"        
        if (playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce))
        {
            // PART B場景是否 camA_bg
            // yes -> 水平反轉
            _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(GetBackgroundIndex() == 1, "camB_drawef", "draw");
        }

        if (playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce))
        {
            _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(GetBackgroundIndex() == 1, "camB_drawef", "draw");
        }

        /*
         * "強度負方"&"受擊方
            (同時有這2個身份)
         */
        if (playerOne.HasCharacterIdentityTypes(new GameCharacter.CharacterIdentityType[]
        {GameCharacter.CharacterIdentityType.StrengthLoser,GameCharacter.CharacterIdentityType.Recipient}))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(true, "camB_typeB_crashef", "crash");
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, "camB_typeC_crashef", "crash");
            }
        }

        if (playerTwo.HasCharacterIdentityTypes(new GameCharacter.CharacterIdentityType[]
        { GameCharacter.CharacterIdentityType.StrengthLoser, GameCharacter.CharacterIdentityType.Recipient }))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, "camB_typeC_crashef", "crash");
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(false, "camB_typeB_crashef", "crash");
            }
        }

        /*
         * "以太崩潰方"
            或
            "負荷崩潰方"
         */
        if (playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StressBreakStatusHolder) ||
            playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StateBreakStatusHolder))
        {
            /*
             * "己方"是否
                "未能抵抗方"
                或
                "速度負方"
                或
                "速度強度負方"?
             */
            if (playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NonResister) ||
                playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser) ||
                playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, "camB_typeD_crashef", "crash");
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(true, "camB_typeB_crashef", "crash");
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, "camB_typeC_crashef", "crash");
                }
            }
        }

        if (playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StressBreakStatusHolder) ||
            playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StateBreakStatusHolder))
        {
            if (playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NonResister) ||
                playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser)  ||
                playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, "camB_typeD_crashef", "crash");
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, "camB_typeC_crashef", "crash");
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(false, "camB_typeB_crashef", "crash");
                }
            }
        }
        return (_visualEffectParameterDataForPlayerOne, _visualEffectParameterDataForPlayerTwo);
    }
}

 
