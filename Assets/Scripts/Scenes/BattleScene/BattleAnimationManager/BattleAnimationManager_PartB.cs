using UnityEngine;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using CodeType = DatabaseManager.AnimationData.CodeType;
using AnimationParameterData = CharacterAnimationHandler.AnimationParameterData;
using VisualEffectParameterData = CharacterAnimationHandler.VisualEffectParameterData;

public partial class BattleAnimationManager: MonoBehaviour
{
    private readonly string[] lastCodeV1andV2 = { "V1", "V2" };

    public ( AnimationParameterData animationParameterData, AnimationParameterData extraAnimationParameterData ) DetermineAnimationOfPlayerOneForPartB( GameCharacter playerOne, GameCharacter playerTwo )
    {
        AnimationParameterData _animationParameterData = null;
        AnimationParameterData _extraAnimationParameterData = null;

        BattleDistanceManager _battleDistanceManager = this.battleGameManager.GetBattleDistanceManager();
        Subskill _playerOne_SubskillData = null;
        Subskill _playerTwo_SubskillData = null;

        CharacterSkill _playerOne_CurrentSkill = playerOne.GetCurrentSkill();
        if (_playerOne_CurrentSkill != null)
        {
            _playerOne_SubskillData = _playerOne_CurrentSkill.GetCharacterSubskillData().GetSubskillData();
        }

        CharacterSkill _playerTwo_CurrentSkill = playerTwo.GetCurrentSkill();
        if (_playerTwo_CurrentSkill != null)
        {
            _playerTwo_SubskillData = _playerTwo_CurrentSkill.GetCharacterSubskillData().GetSubskillData();
        }

        CharacterAnimationHandler _playerOne_CharacterAnimationHandler = playerOne.GetCharacterAnimationHandler();
        string _playerOne_SubskillId = ( _playerOne_SubskillData != null ) ? _playerOne_SubskillData.Id : "";
        int _playerOne_AnimationType = AnimationParameterData.ConvertToAnimationType( playerOne );

        //抵抗成功方
        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
        {
            //角色是已按下技能是否回避技能 ?
            if (_playerOne_SubskillData.IsEvadingSkill)
            {
                //己方當前流向是否"以太流" / "無流向" ?
                if (playerOne.GetSelectedPassiveSkillCategoryType() is CategorizedPassiveSkillManager.CategoryType.State or CategorizedPassiveSkillManager.CategoryType.None)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV1, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                //己方當前流向是否"生命流" ? 或者 當前距離是否近距離 ?
                else if (playerOne.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life || _battleDistanceManager.GetCurrentDistanceType() == BattleDistanceManager.DistanceType.Near)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV2, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( true, false, CodeType.camB_type_CDV3, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
            else
            {
                _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV1, _playerOne_SubskillId, _playerOne_AnimationType );
            }
        }

        //平手方
        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
        {
            //己方為先手方
            if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ))
            {
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BDVC, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BDV2, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BDV1, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
            //己方為後手方
            else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
            {
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV2, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                //己方是否"近距離遠程方" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDVF, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV1, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
        }

        //"直擊方"/"輕直擊方"/重直擊方"
        if (playerOne.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Assaulter, CharacterIdentityType.LightAssaulter, CharacterIdentityType.HeavyAssaulter } ))
        {
            //己方為先手方
            if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ))
            {
                //敵方是否"未能抵抗方" ?
                //雙方已按下技能是否都是"遠程" ?
                //己方是否"速度勝方"?
                if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NonResister ) ||
                    ( _playerOne_SubskillData.Range == RangeType.ranged && _playerTwo_SubskillData.Range == RangeType.ranged ) ||
                    ( playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedWinner ) ))
                {
                    //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_AVC, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                    else if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_AV2, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_AV1, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BDVC, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                //已按下技能是否與上1ATL 相同&有"v2演出" ?
                else if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "V2" ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BDV2, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_BDV1, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
            //己方為後手方
            else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
            {
                //己方是否"無視遠程方" ?
                if (playerOne.HasCharacterIdentityType( CharacterIdentityType.IgnoreRangedSkill ))
                {
                    _extraAnimationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CFW, _playerOne_SubskillId, _playerOne_AnimationType );
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_AVC, _playerOne_SubskillId, _playerOne_AnimationType );
                }

                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV2, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                //是否有強度負方&雙方已按下技能是"遠程" ?
                else if (( playerOne.HasCharacterIdentityType( CharacterIdentityType.StrengthLoser ) || playerTwo.HasCharacterIdentityType( CharacterIdentityType.StrengthLoser ) ) &&
                    ( _playerOne_SubskillData.Range == RangeType.ranged ) && ( _playerTwo_SubskillData.Range == RangeType.ranged ))
                {
                    //己方是否"近距離遠程方" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                    {
                        _extraAnimationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CFWVF, _playerOne_SubskillId, _playerOne_AnimationType );
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_AV1, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _extraAnimationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CFW, _playerOne_SubskillId, _playerOne_AnimationType );
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_AV1, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //己方是否"近距離遠程方" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDVF, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CDV1, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
        }

        //"受擊方"/"輕受擊方"/"重受擊方"
        if (playerOne.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.LightRecipient, CharacterIdentityType.HeavyRecipient } ))
        {
            if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ))
            {
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                {
                    //己方是否"輕受擊方" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BLPVC_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BLPVC_H, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                {
                    //己方是否"輕受擊方" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BLPV2_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BLPV2_H, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //己方是否"輕受擊方" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                {
                    //己方是否"速度負方" ? or 雙方已按下技能是否都是"遠程" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) || ( _playerOne_SubskillData.Range == RangeType.ranged && _playerTwo_SubskillData.Range == RangeType.ranged ))
                    {
                        _extraAnimationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BFL, _playerOne_SubskillId, _playerOne_AnimationType );
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_D_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BLPV1_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //己方是否"速度負方" ?  or 雙方已按下技能是否都是"遠程" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) || ( _playerOne_SubskillData.Range == RangeType.ranged && _playerTwo_SubskillData.Range == RangeType.ranged ))
                {
                    _extraAnimationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BFL, _playerOne_SubskillId, _playerOne_AnimationType );
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_D_H, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camA_type_BLPV1_H, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
            //己方為後手方
            else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
            {
                //己方是否"未能抵抗方" ?
                if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NonResister ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_D_H, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                //己方是否"速度負方" /"速度強度負方" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) || playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
                {
                    if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                    {
                        //己方是否"輕受擊方" ?
                        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSV2_L, _playerOne_SubskillId, _playerOne_AnimationType );
                        }
                        else
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSV2_H, _playerOne_SubskillId, _playerOne_AnimationType );
                        }
                    }
                    //己方是否"輕受擊方" ?
                    else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        //己方是否"近距離遠程方" ?
                        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSVF_L, _playerOne_SubskillId, _playerOne_AnimationType );
                        }
                        else
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSV1_L, _playerOne_SubskillId, _playerOne_AnimationType );
                        }
                    }
                    //己方是否"近距離遠程方" ?
                    else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSVF_H, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSV1_H, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (_playerOne_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerOne.GetLastAtlSkill() == _playerOne_CurrentSkill)
                {
                    //己方是否"輕受擊方" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSV2_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLSV2_H, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //己方是否"輕受擊方" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                {
                    //己方是否"近距離遠程方" ?
                    if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLPVF_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                    else
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLPV1_L, _playerOne_SubskillId, _playerOne_AnimationType );
                    }
                }
                //己方是否"近距離遠程方" ?
                else if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLPVF_H, _playerOne_SubskillId, _playerOne_AnimationType );
                }
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_CLPV1_H, _playerOne_SubskillId, _playerOne_AnimationType );
                }
            }
        }

        return ( animationParameterData: _animationParameterData, extraAnimationParameterData: _extraAnimationParameterData );
    }

    public ( AnimationParameterData animationParameterData, AnimationParameterData extraAnimationParameterData ) DetermineAnimationOfPlayerTwoForPartB( GameCharacter playerOne, GameCharacter playerTwo )
    {
        AnimationParameterData _animationParameterData = null;
        AnimationParameterData _extraAnimationParameterData = null;

        BattleDistanceManager _battleDistanceManager = this.battleGameManager.GetBattleDistanceManager();
        Subskill _playerOne_SubskillData = null;
        Subskill _playerTwo_SubskillData = null;

        CharacterSkill _playerOne_CurrentSkill = playerOne.GetCurrentSkill();
        if (_playerOne_CurrentSkill != null)
        {
            _playerOne_SubskillData = _playerOne_CurrentSkill.GetCharacterSubskillData().GetSubskillData();
        }

        CharacterSkill _playerTwo_CurrentSkill = playerTwo.GetCurrentSkill();
        if (_playerTwo_CurrentSkill != null)
        {
            _playerTwo_SubskillData = _playerTwo_CurrentSkill.GetCharacterSubskillData().GetSubskillData();
        }

        CharacterAnimationHandler _playerTwo_CharacterAnimationHandler = playerTwo.GetCharacterAnimationHandler();
        string _playerTwo_SubskillId = ( _playerTwo_SubskillData != null ) ? _playerTwo_SubskillData.Id : "";
        int _playerTwo_AnimationType = AnimationParameterData.ConvertToAnimationType( playerTwo );

        // 抵抗成功方
        if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
        {
            // 敵方是已按下技能是否回避技能?
            // YES
            if (_playerTwo_SubskillData.IsEvadingSkill)
            {
                // 敵方當前流向是否"生命流"/"無流向"?
                // YES
                if (playerTwo.GetSelectedPassiveSkillCategoryType() is CategorizedPassiveSkillManager.CategoryType.State or CategorizedPassiveSkillManager.CategoryType.None)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                // 敵方當前流向是否"生命流"? or 當前距離是否近距離?
                // YES
                else if (playerTwo.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life || _battleDistanceManager.GetCurrentDistanceType() == BattleDistanceManager.DistanceType.Near)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV2, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( true, false, CodeType.camA_type_CDV3, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
            // NO
            else
            {
                _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
            }
        }

        // 平手方
        if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
        {
            // 敵方為先手方
            if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ))
            {
                // 敵方是否"中距離近戰方"&已按下技能有"vc演出"?
                // YES
                if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BDVC, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // 已按下技能是否與上1ATL相同&上1ATL播放了"v1演出"&有"v2演出"?
                // YES
                else if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BDV2, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BDV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
            // 敵方為後手方
            else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
            {
                // 已按下技能是否與上1ATL相同&上1ATL播放了"v1演出"&有"v2演出"?
                // YES
                if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV2, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // 敵方是否"近距離遠程方"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDVF, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
        }

        // "直擊方"/"輕直擊方"/重直擊方"
        if (playerTwo.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Assaulter, CharacterIdentityType.LightAssaulter, CharacterIdentityType.HeavyAssaulter } ))
        {
            // 敵方為先手方
            if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ))
            {
                // 己方是否"未能抵抗方"?
                // 雙方已按下技能是否都是"遠程"?
                // 敵方是否"速度勝方"?
                // YES
                if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NonResister ) ||
                    ( _playerOne_SubskillData.Range == RangeType.ranged && _playerTwo_SubskillData.Range == RangeType.ranged ) ||
                    ( playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedWinner ) ))
                {
                    // 敵方是否"中距離近戰方"&已按下技能有"vc演出"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_AVC, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    // 已按下技能是否與上1ATL相同&有"v2演出"?
                    // YES
                    else if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_AV2, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_AV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 敵方是否"中距離近戰方"&已按下技能有"vc演出"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BDVC, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                // 已按下技能是否與上1ATL相同&上1ATL播放了"v1演出"&有"v2演出"?
                // YES
                else if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "V2" ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BDV2, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camB_type_BDV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
            // 敵方為後手方
            else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
            {
                // 敵方是否"無視遠程方"?
                // YES
                if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.IgnoreRangedSkill ))
                {
                    _extraAnimationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CFW, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_AVC, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                // 已按下技能是否與上1ATL 相同&上1ATL播放了"v1演出"&有"v2演出"?
                // YES
                else if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV2, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                // 是否有強度負方&雙方已按下技能是"遠程"?
                // YES
                else if (( playerOne.HasCharacterIdentityType( CharacterIdentityType.StrengthLoser ) || playerTwo.HasCharacterIdentityType( CharacterIdentityType.StrengthLoser ) ) &&
                    ( _playerOne_SubskillData.Range == RangeType.ranged ) && ( _playerTwo_SubskillData.Range == RangeType.ranged ))
                {
                    // 敵方是否"近距離遠程方"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                    {
                        _extraAnimationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CFWVF, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_AV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _extraAnimationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CFW, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_AV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 敵方是否"近距離遠程方"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDVF, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CDV1, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
        }

        // "受擊方"/"輕受擊方"/"重受擊方"
        if (playerTwo.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.LightRecipient, CharacterIdentityType.HeavyRecipient } ))
        {
            // 敵方為先手方
            if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ))
            {
                // 敵方是否"中距離近戰方"&已按下技能有"vc演出"?
                // YES
                if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NormalDistanceMeleeDealer ) && _playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( "VC" ))
                {
                    // 敵方是否"輕受擊方"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BLPVC_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BLPVC_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 已按下技能是否與上1ATL相同&上1ATL播放了"v1演出"&有"v2演出"?
                // YES
                else if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                {
                    // 敵方是否"輕受擊方"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BLPV2_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BLPV2_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 敵方是否"輕受擊方"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                {
                    // 敵方是否"速度負方"? or 雙方已按下技能是否都是"遠程"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) || ( _playerOne_SubskillData.Range == RangeType.ranged && _playerTwo_SubskillData.Range == RangeType.ranged ))
                    {
                        _extraAnimationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BFL, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_D_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BLPV1_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // 敵方是否"速度負方"? or 雙方已按下技能是否都是"遠程"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) || ( _playerOne_SubskillData.Range == RangeType.ranged && _playerTwo_SubskillData.Range == RangeType.ranged ))
                {
                    _extraAnimationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BFL, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_D_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( false, true, CodeType.camB_type_BLPV1_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
            // 敵方為後手方
            else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
            {
                // 敵方是否"未能抵抗方"?
                // YES
                if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NonResister ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_D_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                // 敵方是否"速度負方"/"速度強度負方"?
                // YES
                else if (playerTwo.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.SpeedLoser, CharacterIdentityType.SpeedStrengthLoser } ))
                {
                    // 已按下技能是否與上1ATL相同&上1ATL播放了"v1演出"&有"v2演出"?
                    // YES
                    if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                    {
                        // 敵方是否"輕受擊方"?
                        // YES
                        if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSV2_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        }
                        // NO
                        else
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSV2_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        }
                    }
                    // NO
                    // 敵方是否"輕受擊方"?
                    // YES
                    else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        // 敵方是否"近距離遠程方"?
                        // YES
                        if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSVF_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        }
                        // NO
                        else
                        {
                            _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSV1_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                        }
                    }
                    // 敵方是否"近距離遠程方"?
                    // YES
                    else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSVF_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSV1_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 已按下技能是否與上1ATL相同&上1ATL播放了"v1演出"&有"v2演出"?
                // YES
                else if (_playerTwo_CharacterAnimationHandler.CheckIfSameAsLastATLCodeType( lastCodeV1andV2 ) && playerTwo.GetLastAtlSkill() == _playerTwo_CurrentSkill)
                {
                    // 敵方是否"輕受擊方"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSV2_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLSV2_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 敵方是否"輕受擊方"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.LightRecipient ))
                {
                    // 敵方是否"近距離遠程方"?
                    // YES
                    if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLPVF_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                    // NO
                    else
                    {
                        _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLPV1_L, _playerTwo_SubskillId, _playerTwo_AnimationType );
                    }
                }
                // NO
                // 敵方是否"近距離遠程方"?
                // YES
                else if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer ))
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLPVF_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
                // NO
                else
                {
                    _animationParameterData = new AnimationParameterData( true, true, CodeType.camA_type_CLPV1_H, _playerTwo_SubskillId, _playerTwo_AnimationType );
                }
            }
        }

        return ( animationParameterData: _animationParameterData, extraAnimationParameterData: _extraAnimationParameterData );
    }

    // 判定PART B共用特效 ( Version 1 )
    public ( VisualEffectParameterData visualEffectParameterDataForPlayerOne, VisualEffectParameterData visualEffectParameterDataForPlayerTwo ) DetermineVisualEffectForPartB( GameCharacter playerOne, GameCharacter playerTwo )
    {
        VisualEffectParameterData _visualEffectParameterDataForPlayerOne = null;
        VisualEffectParameterData _visualEffectParameterDataForPlayerTwo = null;

        // "平手方"        
        if (playerOne.HasCharacterIdentityType( GameCharacter.CharacterIdentityType.Deuce ))
        {
            // PART B場景是否 camA_bg
            // yes -> 水平反轉
            Debug.Log( "camB_drawef" );
            _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( GetBackgroundIndex() == 1, true, "camB_drawef", "draw" );
        }

        if (playerTwo.HasCharacterIdentityType( GameCharacter.CharacterIdentityType.Deuce ))
        {
            _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( GetBackgroundIndex() == 1, true, "camB_drawef", "draw" );
        }

        /*
         * "強度負方"&"受擊方
            (同時有這2個身份)
         */
        if (playerOne.HasCharacterIdentityType( GameCharacter.CharacterIdentityType.StrengthLoser ) && playerOne.HasCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient ))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( true, true, "camB_typeB_crashef", "crash" );
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeC_crashef", "crash" );
            }
        }

        if (playerTwo.HasCharacterIdentityType( GameCharacter.CharacterIdentityType.StrengthLoser ) && playerTwo.HasCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient ))
        {
            // PART B場景是否 camA_bg
            if (GetBackgroundIndex() == 1)
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeC_crashef", "crash" );
            }
            // PART B場景為camB_bg
            else if (GetBackgroundIndex() == 2)
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( false, true, "camB_typeB_crashef", "crash" );
            }
        }

        /*
         * "以太崩潰方"
            或
            "負荷崩潰方"
         */
        if (playerOne.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.StressBreakStatusHolder,
            GameCharacter.CharacterIdentityType.StateBreakStatusHolder} ))
        {
            /*
             * "己方"是否
                "未能抵抗方"
                或
                "速度負方"
                或
                "速度強度負方"?
             */
            if (playerOne.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.NonResister,
                GameCharacter.CharacterIdentityType.SpeedLoser, GameCharacter.CharacterIdentityType.SpeedStrengthLoser } ))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeD_crashef", "crash" );
                Debug.Log( "camB_typeD_crashef" );
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( true, true, "camB_typeB_crashef", "crash" );
                    Debug.Log( "camB_typeB_crashef" );
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeC_crashef", "crash" );
                    Debug.Log( "camB_typeC_crashef" );
                }
            }
        }

        if (playerTwo.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.StressBreakStatusHolder,
        GameCharacter.CharacterIdentityType.StateBreakStatusHolder } ))
        {
            if (playerTwo.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[] { GameCharacter.CharacterIdentityType.NonResister,
            GameCharacter.CharacterIdentityType.SpeedLoser, GameCharacter.CharacterIdentityType.SpeedStrengthLoser } ))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeD_crashef", "crash" );
            }
            else
            {
                // PART B場景是否 camA_bg
                if (GetBackgroundIndex() == 1)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeC_crashef", "crash" );
                }
                // PART B場景為camB_bg
                else if (GetBackgroundIndex() == 2)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( false, true, "camB_typeB_crashef", "crash" );
                }
            }
        }

        return ( _visualEffectParameterDataForPlayerOne, _visualEffectParameterDataForPlayerTwo );
    }

    // 判定PART B共用特效 ( Version 2 )
    public ( VisualEffectParameterData visualEffectParameterDataForPlayerOne, VisualEffectParameterData visualEffectParameterDataForPlayerTwo ) DetermineVisualEffectForPartB_V2( GameCharacter playerOne, GameCharacter playerTwo )
    {
        VisualEffectParameterData _visualEffectParameterDataForPlayerOne = null;
        VisualEffectParameterData _visualEffectParameterDataForPlayerTwo = null;

        //"己方"&"平手方"&"先手方"
        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Deuce ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ))
        {
            //"後手方"是否"抵抗成功方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerTwo.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_drawef", "draw" );
            }
        }

        //"己方"&"平手方"&"後手方"
        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.Deuce ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
        {
            _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( false, true, "camB_drawef", "draw" );
        }

        //"己方"&"強度負方"&"受擊方"&"先手方"
        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Recipient ) &&
            playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ))
        {
            //"己方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                //"己方"已按下技能是否"近戰" ?
                if (playerOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeB_crashef", "crash" );
                }
            }
        }

        //"己方"&"強度負方"&"受擊方"&"後手方"
        if (playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Recipient ) &&
            playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
        {
            //"己方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                //"己方"已按下技能是否"近戰" ?
                if (playerOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeC_crashef", "crash" );
                }
            }
        }

        //"敵方"&"強度負方"&"受擊方"&"先手方"
        if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) && playerTwo.HasCharacterIdentityType( CharacterIdentityType.Recipient ) &&
            playerTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ))
        {
            //"敵方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                //"敵方"已按下技能是否"近戰" ?
                if (playerTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeB_crashef", "crash" );
                }
            }
        }

        //"敵方"&"強度負方"&"受擊方"&"後手方"
        if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) && playerTwo.HasCharacterIdentityType( CharacterIdentityType.Recipient ) &&
            playerTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
        {
            //"敵方"是否"速度強度負方" ?
            //YES -> Part B, NO -> 播放共同特效
            if (!playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                //"敵方"已按下技能是否"近戰" ?
                if (playerTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Range == DatabaseManager.Subskill.RangeType.melee)
                {
                    _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeC_crashef", "crash" );
                }
            }
        }

        //"己方"&"以太崩潰方"&"先手方"
        //"己方"&"負荷崩潰方"&"先手方"
        if (( playerOne.HasCharacterIdentityType( CharacterIdentityType.StateBreakStatusHolder ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ) ) ||
            ( playerOne.HasCharacterIdentityType( CharacterIdentityType.StressBreakStatusHolder ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Lead ) ))
        {
            //"己方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NonResister ) ||
                playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) ||
                playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeD_crashef", "crash" );
            }
            else
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeB_crashef", "crash" );
            }
        }

        //"己方"&"以太崩潰方"&"後手方"
        //"己方"&"負荷崩潰方"&"後手方"
        if (( playerOne.HasCharacterIdentityType( CharacterIdentityType.StateBreakStatusHolder ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ) ) ||
            ( playerOne.HasCharacterIdentityType( CharacterIdentityType.StressBreakStatusHolder ) && playerOne.HasCharacterIdentityType( CharacterIdentityType.Improviser ) ))
        {
            //"己方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerOne.HasCharacterIdentityType( CharacterIdentityType.NonResister ) ||
                playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) ||
                playerOne.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeD_crashef", "crash" );
            }
            else
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeC_crashef", "crash" );
            }
        }

        //"敵方"&"以太崩潰方"&"先手方"
        //"敵方"&"負荷崩潰方"&"先手方"
        if (( playerTwo.HasCharacterIdentityType( CharacterIdentityType.StateBreakStatusHolder ) && playerTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ) ) ||
            ( playerTwo.HasCharacterIdentityType( CharacterIdentityType.StressBreakStatusHolder ) && playerTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ) ))
        {
            //"敵方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NonResister ) ||
                playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) ||
                playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeD_crashef", "crash" );
            }
            else
            {
                _visualEffectParameterDataForPlayerOne = new VisualEffectParameterData( false, true, "camB_typeB_crashef", "crash" );
            }
        }

        //"敵方"&"以太崩潰方"&"後手方"
        //"敵方"&"負荷崩潰方"&"後手方"
        if (( playerTwo.HasCharacterIdentityType( CharacterIdentityType.StateBreakStatusHolder ) && playerTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ) ) ||
            ( playerTwo.HasCharacterIdentityType( CharacterIdentityType.StressBreakStatusHolder ) && playerTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ) ))
        {
            //"敵方"是否"未能抵抗方"或"速度負方"或"速度強度負方" ?
            if (playerTwo.HasCharacterIdentityType( CharacterIdentityType.NonResister ) ||
                playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedLoser ) ||
                playerTwo.HasCharacterIdentityType( CharacterIdentityType.SpeedStrengthLoser ))
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeD_crashef", "crash" );
            }
            else
            {
                _visualEffectParameterDataForPlayerTwo = new VisualEffectParameterData( true, true, "camB_typeC_crashef", "crash" );
            }
        }

        return ( _visualEffectParameterDataForPlayerOne, _visualEffectParameterDataForPlayerTwo );
    }
}
