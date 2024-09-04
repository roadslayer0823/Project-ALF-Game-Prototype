using UnityEngine;
using AnimationParameterData = CharacterAnimationHandler.AnimationParameterData;

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
}
