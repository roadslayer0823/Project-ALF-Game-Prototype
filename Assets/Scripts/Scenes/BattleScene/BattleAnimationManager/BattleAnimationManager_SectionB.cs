using UnityEngine;

public partial class BattleAnimationManager : MonoBehaviour
{
    //判定PART A演出&PART A特效
    public void RunPartAPerformance(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser)
    {
        Debug.Log("RunPartAPerformance");
        BattleResultData.BattleResultData_GameCharacter _lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter _improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);
        DatabaseManager.Subskill _leadSubskill = _lead_BattleResultData.gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        
        CharacterAnimationHandler _leadAnimationHandler = lead.GetCharacterAnimationHandler();
        CharacterAnimationHandler _improviserAnimationHandler = improviser.GetCharacterAnimationHandler();

        int _leadSkillType = (int)_lead_BattleResultData.gameCharacter.GetCurrentSkillRangeType();
        int _improviserSkillType = 0;
        string _leadSubskillId = _leadSubskill.Id;    
        string _improviserSubskillId = "";
        
        CharacterSkill _improviserCurrentSkill = _improviser_BattleResultData.gameCharacter.GetCurrentSkill();
        if (_improviserCurrentSkill != null)
        {
            _improviserSubskillId = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().Id;
            _improviserSkillType = (int)_improviser_BattleResultData.gameCharacter.GetCurrentSkillRangeType();
        }
        /*先手方已按下的技能速度是否3以上?*/
        /*先手方已按下的技能強度是否2以上?*/
        skillPromptPanel.PlaySpeedStrengthAnimation(lead);

        if (_improviserCurrentSkill != null)
        {
            skillPromptPanel.PlaySpeedStrengthAnimation( improviser );
        }

        /*
         先手方是否
        "近距離遠程方"?
         */
        if (_lead_BattleResultData.gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
        {       
            _leadAnimationHandler.LoadAndPlayAnimation(true, false, DatabaseManager.AnimationData.CodeType.camB_partA_VF, _leadSubskillId, _leadSkillType);
        }
        else
        {            
            _leadAnimationHandler.LoadAndPlayAnimation(true, false, DatabaseManager.AnimationData.CodeType.camB_partA_V1, _leadSubskillId, _leadSkillType);
        }

        // same code type but flip
        if(_improviserCurrentSkill != null)
        {
            _improviserAnimationHandler.FlipContainer();
            if (_improviser_BattleResultData.gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
            {
                _improviserAnimationHandler.LoadAndPlayAnimation(false, false, DatabaseManager.AnimationData.CodeType.camB_partA_VF, _improviserSubskillId, _improviserSkillType);
            }
            else
            {
                _improviserAnimationHandler.LoadAndPlayAnimation(false, false, DatabaseManager.AnimationData.CodeType.camB_partA_V1, _improviserSubskillId, _improviserSkillType);
            }
        }  
    }

    // 判定PART B共用特效
    public void RunPartBShareEffects(GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        Debug.Log(gameCharacterOne.GetCharacterName() + " RunPartBShareEffects");
        CharacterAnimationHandler _characterOneAnimationHandler = gameCharacterOne.GetCharacterAnimationHandler();
        CharacterAnimationHandler _characterTwoAnimationHandler = gameCharacterTwo.GetCharacterAnimationHandler();
    
        // "平手方"        
        if (gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _characterOneAnimationHandler.FlipVisualEffectContainer();
            }
            _characterOneAnimationHandler.LoadAndPlayVisualEffect("camB_drawef","draw");
        }

        if (gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _characterTwoAnimationHandler.FlipVisualEffectContainer();
            }
            _characterTwoAnimationHandler.LoadAndPlayVisualEffect("camB_drawef","draw");
        }

        /*
         * "強度負方"&"受擊方
            (同時有這2個身份)
         */
        if (gameCharacterOne.HasCharacterIdentityTypes(new GameCharacter.CharacterIdentityType[]
        {GameCharacter.CharacterIdentityType.StrengthLoser,GameCharacter.CharacterIdentityType.Recipient}))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _characterOneAnimationHandler.FlipVisualEffectContainer();
                _characterOneAnimationHandler.LoadAndPlayVisualEffect("camB_typeB_crashef","crash");
            }
            else if(GetBackgroundIndex() == 2)
            {
                _characterOneAnimationHandler.LoadAndPlayVisualEffect("camB_typeC_crashef", "crash");
            }      
        }

        if (gameCharacterTwo.HasCharacterIdentityTypes(new GameCharacter.CharacterIdentityType[]
        { GameCharacter.CharacterIdentityType.StrengthLoser, GameCharacter.CharacterIdentityType.Recipient }))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _characterTwoAnimationHandler.FlipVisualEffectContainer();
                _characterTwoAnimationHandler.LoadAndPlayVisualEffect("camB_typeC_crashef", "crash");
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _characterTwoAnimationHandler.LoadAndPlayVisualEffect("camB_typeB_crashef", "crash");
            }
        }

        /*
         * "以太崩潰方"
            或
            "負荷崩潰方"
         */
        if (gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StressBreakStatusHolder) ||
            gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StateBreakStatusHolder))
        {
            /*
             * "敵方"是否
                "未能抵抗方"
                或
                "速度負方"
                或
                "速度強度負方"?
             */
            if (gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NonResister) ||
                gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser) ||
                gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser))
            {
                _characterOneAnimationHandler.LoadAndPlayVisualEffect("camB_typeD_crashef", "crash");
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _characterOneAnimationHandler.FlipVisualEffectContainer();
                    _characterOneAnimationHandler.LoadAndPlayVisualEffect("camB_typeB_crashef", "crash");
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _characterOneAnimationHandler.LoadAndPlayVisualEffect("camB_typeC_crashef", "crash");
                }
            }
        }

        if (gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StressBreakStatusHolder) ||
            gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StateBreakStatusHolder))
        {
            if (gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NonResister) ||
                gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser)  ||
                gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser))
            {
                _characterTwoAnimationHandler.FlipVisualEffectContainer();
                _characterTwoAnimationHandler.LoadAndPlayVisualEffect("camB_typeD_crashef", "crash");
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _characterTwoAnimationHandler.FlipVisualEffectContainer();
                    _characterTwoAnimationHandler.LoadAndPlayVisualEffect("camB_typeC_crashef", "crash");
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _characterTwoAnimationHandler.LoadAndPlayVisualEffect("camB_typeB_crashef", "crash");
                }
            }
        }
    }
}

 
