using UnityEngine;
using static GameCharacter;
using AnimationParameterData = CharacterAnimationHandler.AnimationParameterData;
using VisualEffectParameterData = CharacterAnimationHandler.VisualEffectParameterData;

public partial class BattleAnimationManager : MonoBehaviour
{
    //判定PART A演出&PART A特效
    public AnimationParameterData DetermineAnimationAndVisualEffectForPartA(GameCharacter lead, GameCharacter playerOne, GameCharacter playerTwo)
    {
        AnimationParameterData _animationParameterData = null;

        string _playerOne_SubskillId = "";
        int _playerOne_AnimationType = 0;

        string _playerTwo_SubskillId = "";
        int _playerTwo_AnimationType = 0;

        CharacterSkill _playerOneCurrentSkill = playerOne.GetCurrentSkill();
        if (_playerOneCurrentSkill != null)
        {
            _playerOne_SubskillId = _playerOneCurrentSkill.GetCharacterSubskillData().GetSubskillData().Id;
            _playerOne_AnimationType = AnimationParameterData.ConvertToAnimationType( playerOne );
        }

        CharacterSkill _playerTwoCurrentSkill = playerTwo.GetCurrentSkill();
        if (_playerTwoCurrentSkill != null)
        {
            _playerTwo_SubskillId = _playerTwoCurrentSkill.GetCharacterSubskillData().GetSubskillData().Id;
            _playerTwo_AnimationType = AnimationParameterData.ConvertToAnimationType( playerTwo );
        }

        /*先手方已按下的技能速度是否3以上?*/
        /*先手方已按下的技能強度是否2以上?*/
        //skillPromptPanel.PlaySpeedStrengthAnimation(lead);

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
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camB_partA_VF, _playerOne_SubskillId, _playerOne_AnimationType);
            }
            else
            {
                /*
                 * 播放
                   "敵方"已按下技能
                   camA_partA_VF
                 */
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camA_partA_VF, _playerTwo_SubskillId, _playerTwo_AnimationType);
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
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camB_partA_V1, _playerOne_SubskillId, _playerOne_AnimationType);
            }
            else
            {
                /*
                 * 播放
                   "敵方"已按下技能
                   camA_partA_V1
                 */
                _animationParameterData = new AnimationParameterData(true, false, DatabaseManager.AnimationData.CodeType.camA_partA_V1, _playerTwo_SubskillId, _playerTwo_AnimationType);
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
            Debug.Log("camB_drawef");
            _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(GetBackgroundIndex() == 1, true, "camB_drawef", "draw");
        }

        if (playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce))
        {
            _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(GetBackgroundIndex() == 1, true, "camB_drawef", "draw");
        }

        /*
         * "強度負方"&"受擊方
            (同時有這2個身份)
         */
        if (playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StrengthLoser) && playerOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(true, true, "camB_typeB_crashef", "crash");
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeC_crashef", "crash");
            }
        }

        if (playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StrengthLoser) && playerTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeC_crashef", "crash");
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(false, true, "camB_typeB_crashef", "crash");
            }
        }

        /*
         * "以太崩潰方"
            或
            "負荷崩潰方"
         */
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.StressBreakStatusHolder,
            GameCharacter.CharacterIdentityType.StateBreakStatusHolder}))
        {
            /*
             * "己方"是否
                "未能抵抗方"
                或
                "速度負方"
                或
                "速度強度負方"?
             */
            if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.NonResister,
                GameCharacter.CharacterIdentityType.SpeedLoser, GameCharacter.CharacterIdentityType.SpeedStrengthLoser }))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeD_crashef", "crash");
                Debug.Log("camB_typeD_crashef");
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(true, true, "camB_typeB_crashef", "crash");
                    Debug.Log("camB_typeB_crashef");
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeC_crashef", "crash");
                    Debug.Log("camB_typeC_crashef");
                }
            }
        }

        if (playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.StressBreakStatusHolder,
        GameCharacter.CharacterIdentityType.StateBreakStatusHolder }))
        {
            if (playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.NonResister,
            GameCharacter.CharacterIdentityType.SpeedLoser, GameCharacter.CharacterIdentityType.SpeedStrengthLoser }))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeD_crashef", "crash");
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeC_crashef", "crash");
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(false, true, "camB_typeB_crashef", "crash");
                }
            }
        }
        return (_visualEffectParameterDataForPlayerOne, _visualEffectParameterDataForPlayerTwo);
    }

    public (VisualEffectParameterData visualEffectParameterDataForPlayerOne, VisualEffectParameterData visualEffectParameterDataForPlayerTwo) DetermineVisualEffectForPartB_V2(GameCharacter playerOne, GameCharacter playerTwo)
    {
        VisualEffectParameterData _visualEffectParameterDataForPlayerOne = null;
        VisualEffectParameterData _visualEffectParameterDataForPlayerTwo = null;

        //"己方"&"平手方"&"先手方"
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Deuce, CharacterIdentityType.Lead }))
        {
            //"後手方"是否"抵抗成功方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerTwo.HasCharacterIdentityType(CharacterIdentityType.SuccessfulResister))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_drawef", "draw");
            }
        }

        //"己方"&"平手方"&"後手方"
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Deuce, CharacterIdentityType.Improviser }))
        {
            _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(false, true, "camB_drawef", "draw");
        }

        //"己方"&"強度負方"&"受擊方"&"先手方"
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StrengthLoser, CharacterIdentityType.Recipient, CharacterIdentityType.Lead }))
        {
            //"己方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerOne.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                //"己方"已按下技能是否"近戰" ?
                if (playerOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeB_crashef", "crash");
                }
            }
        }

        //"己方"&"強度負方"&"受擊方"&"後手方"
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StrengthLoser, CharacterIdentityType.Recipient, CharacterIdentityType.Improviser }))
        {
            //"己方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerOne.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                //"己方"已按下技能是否"近戰" ?
                if (playerOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeC_crashef", "crash");
                }
            }
        }

        //"敵方"&"強度負方"&"受擊方"&"先手方"
        if (playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StrengthLoser, CharacterIdentityType.Recipient, CharacterIdentityType.Lead }))
        {
            //"敵方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerTwo.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                //"敵方"已按下技能是否"近戰" ?
                if (playerTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeB_crashef", "crash");
                }
            }
        }

        //"敵方"&"強度負方"&"受擊方"&"後手方"
        if (playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StrengthLoser, CharacterIdentityType.Recipient, CharacterIdentityType.Improviser }))
        {
            //"敵方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerTwo.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                //"敵方"已按下技能是否"近戰" ?
                if (playerTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeC_crashef", "crash");
                }
            }
        }

        //"己方"&"以太崩潰方"&"先手方"
        //"己方"&"負荷崩潰方"&"先手方"
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StateBreakStatusHolder, CharacterIdentityType.Lead}) ||
            playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StressBreakStatusHolder, CharacterIdentityType.Lead }))
        {
            //"己方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerOne.HasCharacterIdentityType(CharacterIdentityType.NonResister) ||
                playerOne.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) ||
                playerOne.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeD_crashef", "crash");
            }
            else
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeB_crashef", "crash");
            }
        }

        //"己方"&"以太崩潰方"&"後手方"
        //"己方"&"負荷崩潰方"&"後手方"
        if (playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StateBreakStatusHolder, CharacterIdentityType.Improviser }) ||
            playerOne.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StressBreakStatusHolder, CharacterIdentityType.Improviser }))
        {
            //"己方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerOne.HasCharacterIdentityType(CharacterIdentityType.NonResister) ||
                playerOne.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) ||
                playerOne.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeD_crashef", "crash");
            }
            else
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeC_crashef", "crash");
            }
        }

        //"敵方"&"以太崩潰方"&"先手方"
        //"敵方"&"負荷崩潰方"&"先手方"
        if (playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StateBreakStatusHolder, CharacterIdentityType.Lead }) ||
            playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StressBreakStatusHolder, CharacterIdentityType.Lead }))
        {
            //"敵方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerTwo.HasCharacterIdentityType(CharacterIdentityType.NonResister) ||
                playerTwo.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) ||
                playerTwo.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeD_crashef", "crash");
            }
            else
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData(false, true, "camB_typeB_crashef", "crash");
            }
        }

        //"敵方"&"以太崩潰方"&"後手方"
        //"敵方"&"負荷崩潰方"&"後手方"
        if (playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StateBreakStatusHolder, CharacterIdentityType.Improviser }) ||
            playerTwo.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.StressBreakStatusHolder, CharacterIdentityType.Improviser }))
        {
            //"敵方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerTwo.HasCharacterIdentityType(CharacterIdentityType.NonResister) ||
                playerTwo.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) ||
                playerTwo.HasCharacterIdentityType(CharacterIdentityType.SpeedStrengthLoser))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeD_crashef", "crash");
            }
            else
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData(true, true, "camB_typeC_crashef", "crash");
            }
        }
        return (_visualEffectParameterDataForPlayerOne, _visualEffectParameterDataForPlayerTwo);
    }
}

 
