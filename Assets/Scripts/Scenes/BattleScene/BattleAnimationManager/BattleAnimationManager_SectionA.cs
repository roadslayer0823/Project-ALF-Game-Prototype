using UnityEngine;
using System.Collections;
using Subskill = DatabaseManager.Subskill;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;

public partial class BattleAnimationManager: MonoBehaviour
{
    public void DeterminePartBAnimation(GameCharacter gameCharacter, GameCharacter opponent)
    {
        string[] lastCodeV1andV2 = { "V1", "V2" };
        Subskill characterSubskillData = null;
        Subskill opponentSubskillData = null;

        CharacterSkill _characterSkill = gameCharacter.GetCurrentSkill();
        if (_characterSkill != null)
        {
            characterSubskillData = _characterSkill.GetCharacterSubskillData().GetSubskillData();
        }

        CharacterSkill _opponentCurrentSkill = opponent.GetCurrentSkill();
        if (_opponentCurrentSkill != null)
        {
            opponentSubskillData = _opponentCurrentSkill.GetCharacterSubskillData().GetSubskillData();
        }

        CharacterAnimationHandler characterAnimationHandler = gameCharacter.GetCharacterAnimationHandler();
        BattleDistanceManager battleDistanceManager = battleGameManager.GetBattleDistanceManager();

        string subSkillID = ( characterSubskillData != null ) ? characterSubskillData.Id : "";
        int skillType = 0;

        if(gameCharacter.GetCurrentSkillRangeType() == Subskill.RangeType.melee)
        {
            skillType = 1;
        }
        else if(gameCharacter.GetCurrentSkillRangeType() == Subskill.RangeType.ranged)
        {
            skillType = 2;
        }

        //抵抗成功方
        if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.SuccessfulResister))
        {
            //角色是已按下技能是否回避技能 ?
            if (characterSubskillData.IsEvadingSkill)
            {
                //己方當前流向是否"以太流" / "無流向" ?
                if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.State || gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.None)
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1);
                }
                //己方當前流向是否"生命流" ? 或者 當前距離是否近距離 ?
                else if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life || battleDistanceManager.GetCurrentDistanceType() == BattleDistanceManager.DistanceType.Near)
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV2, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, false, DatabaseManager.AnimationData.CodeType.camB_type_CDV3, subSkillID, skillType);
                }
            }
            else
            {
                CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1);
            }
        }

        //平手方
        if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Deuce))
        {
            //己方為先手方
            if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Lead))
            {
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDVC, subSkillID, skillType);
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDV2, subSkillID, skillType);
                }
                else
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_BDV1);
                }
            }
            //己方為後手方
            else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Improviser))
            {
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV2, subSkillID, skillType);
                }
                //己方是否"近距離遠程方" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CDVF);
                }
                else
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1);
                }
            }
        }

        //"直擊方"/"輕直擊方"/重直擊方"
        if(gameCharacter.HasCharacterIdentityTypes(new CharacterIdentityType[] {CharacterIdentityType.Assaulter, CharacterIdentityType.LightAssaulter, CharacterIdentityType.HeavyAssaulter}))
        {
            //己方為先手方
            if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Lead))
            {
                //敵方是否"未能抵抗方" ?
                //雙方已按下技能是否都是"遠程" ?
                //己方是否"速度勝方"?
                if (opponent.HasCharacterIdentityType(CharacterIdentityType.NonResister) ||
                    (characterSubskillData.Range == Subskill.RangeType.ranged && opponentSubskillData.Range == Subskill.RangeType.ranged) ||
                    (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.SpeedWinner)))
                {
                    //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AVC, subSkillID, skillType);
                    }

                    //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                    else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AV2, subSkillID, skillType);
                    }
                    else
                    {
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_AV1);
                    }
                }
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDVC, subSkillID, skillType);
                }
                //已按下技能是否與上1ATL 相同&有"v2演出" ?
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType("V2") && gameCharacter.GetLastAtlSkill() == _characterSkill)
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDV2, subSkillID, skillType);
                }
                else
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_BDV1);
                }
            }
            //己方為後手方
            else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Improviser))
            {
                //己方是否"無視遠程方" ?
                if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.IgnoreRangedSkill))
                {
                    StartCoroutine(PlayCFWAnimation(gameCharacter,subSkillID, skillType));
                }

                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV2, subSkillID, skillType);
                }
                //是否有強度負方&雙方已按下技能是"遠程" ?
                else if((gameCharacter.HasCharacterIdentityType(CharacterIdentityType.StrengthLoser) || opponent.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StrengthLoser)) &&
                    (characterSubskillData.Range == Subskill.RangeType.ranged) && (opponentSubskillData.Range == Subskill.RangeType.ranged))
                {
                    //己方是否"近距離遠程方" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                    {
                        StartCoroutine(PlayCFWVFAnimation(gameCharacter,subSkillID, skillType));
                    }
                    else
                    {
                        StartCoroutine(PlayCFWAnimation(gameCharacter,subSkillID, skillType));
                    }
                }
                //己方是否"近距離遠程方" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CDVF);
                }
                else
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1);
                }
            }
        }

        //"受擊方"/"輕受擊方"/"重受擊方"
        if (gameCharacter.HasCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.LightRecipient, CharacterIdentityType.HeavyRecipient}))
        {
            if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Lead))
            {
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                {
                    //己方是否"輕受擊方" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPVC_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPVC_H, subSkillID, skillType);
                    }
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                {
                    //己方是否"輕受擊方" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPV2_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPV2_H, subSkillID, skillType);
                    }
                }
                //己方是否"輕受擊方" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                {
                    //己方是否"速度負方" ? or 雙方已按下技能是否都是"遠程" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) || (characterSubskillData.Range == Subskill.RangeType.ranged && opponentSubskillData.Range == Subskill.RangeType.ranged))
                    {
                        StartCoroutine(PlayBFLAnimation(gameCharacter,subSkillID, skillType));
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_D_L);
                    }
                    else
                    {
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_BLPV1_L);
                    }
                }
                //己方是否"速度負方" ?  or 雙方已按下技能是否都是"遠程" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) || (characterSubskillData.Range == Subskill.RangeType.ranged && opponentSubskillData.Range == Subskill.RangeType.ranged))
                {
                    StartCoroutine(PlayBFLAnimation(gameCharacter,subSkillID, skillType));
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_D_H);
                }
                else
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_BLPV1_H);
                }
            }
            //己方為後手方
            else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.Improviser))
            {
                //己方是否"未能抵抗方" ?
                if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NonResister))
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_D_H);
                }
                //己方是否"速度負方" /"速度強度負方" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.SpeedLoser) || gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser))
                {
                    if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                    {
                        //己方是否"輕受擊方" ?
                        if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                        {
                            characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_L, subSkillID, skillType);
                        }
                        else
                        {
                            characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_H, subSkillID, skillType);
                        }
                    }
                    //己方是否"輕受擊方" ?
                    else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                    {
                        //己方是否"近距離遠程方" ?
                        if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                        {
                            CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSVF_L);
                        }
                        else
                        {
                            CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV1_L);
                        }
                    }
                    //己方是否"近距離遠程方" ?
                    else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                    {
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSVF_H);
                    }
                    else
                    {
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV1_H);
                    }
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2) && gameCharacter.GetLastAtlSkill() == _characterSkill)
                {
                    //己方是否"輕受擊方" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_H, subSkillID, skillType);
                    }
                }
                //己方是否"輕受擊方" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.LightRecipient))
                {
                    //己方是否"近距離遠程方" ?
                    if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                    {
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPVF_L);
                    }
                    else
                    {
                        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPV1_L);
                    }
                }
                //己方是否"近距離遠程方" ?
                else if (gameCharacter.HasCharacterIdentityType(CharacterIdentityType.NearDistanceRangedDealer))
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPVF_L);
                }
                else
                {
                    CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPV1_L);
                }
            }
        }
    }

    private IEnumerator PlayCFWAnimation(GameCharacter gameCharacter, string subSkillID, int skillType)
    {
        CharacterAnimationHandler characterAnimationHandler = gameCharacter.GetCharacterAnimationHandler();
        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CFW);
        yield return new WaitForSeconds(characterAnimationHandler.GetAnimationClipTotalLength());
        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_AV1);
    }

    private IEnumerator PlayCFWVFAnimation(GameCharacter gameCharacter, string subSkillID, int skillType)
    {
        CharacterAnimationHandler characterAnimationHandler = gameCharacter.GetCharacterAnimationHandler();
        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, true, DatabaseManager.AnimationData.CodeType.camB_type_CFWVF);
        yield return new WaitForSeconds(characterAnimationHandler.GetAnimationClipTotalLength());
        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_AV1);
    }

    private IEnumerator PlayBFLAnimation(GameCharacter gameCharacter, string subSkillID, int skillType)
    {
        CharacterAnimationHandler characterAnimationHandler = gameCharacter.GetCharacterAnimationHandler();
        CheckSkillMeleeOrRange(gameCharacter, skillType, subSkillID, false, DatabaseManager.AnimationData.CodeType.camA_type_BFL);
        yield return new WaitForSeconds(characterAnimationHandler.GetAnimationClipTotalLength());
    }

    private void CheckSkillMeleeOrRange(GameCharacter gameCharacter, int skillType, string subSkillID, bool isFront ,DatabaseManager.AnimationData.CodeType animationData)
    {
        int _skillType = skillType;
        CharacterAnimationHandler characterAnimationHandler = gameCharacter.GetCharacterAnimationHandler();
        if (_skillType == 0)
        {
            characterAnimationHandler.LoadAndPlayAnimation(isFront, true, animationData, subSkillID, 0);
        }
        else if (_skillType == 1)
        {
            characterAnimationHandler.LoadAndPlayAnimation(isFront, true, animationData, subSkillID, 1);
        }
        else if (_skillType == 2)
        {
            characterAnimationHandler.LoadAndPlayAnimation(isFront, true, animationData, subSkillID, 2);
        }
        else
        {
            return;
        }
    }
}
