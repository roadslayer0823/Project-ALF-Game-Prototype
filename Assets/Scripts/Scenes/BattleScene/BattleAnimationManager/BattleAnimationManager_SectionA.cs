using UnityEngine;
using System.Collections;
using Subskill = DatabaseManager.Subskill;

public partial class BattleAnimationManager: MonoBehaviour
{
    public static CharacterAnimationHandler characterAnimationHandler;
    public static BattleDistanceManager battleDistanceManager;

    public void DeterminePartBAnimation(GameCharacter gameCharacter, GameCharacter opponent, string subSkillID, int skillType)
    {
        string[] lastCodeV1andV2 = { "V1", "V2" };
        Subskill characterSubskillData = gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        Subskill oppoentSubskillData = opponent.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

        //抵抗成功方
        if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SuccessfulResister))
        {
            //角色是已按下技能是否回避技能 ?
            if (characterSubskillData.IsEvadingSkill)
            {
                //己方當前流向是否"以太流" / "無流向" ?
                if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.State || gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.None)
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1, subSkillID, skillType);
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
                characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1, subSkillID, skillType);
            }
        }

        //平手方
        if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Deuce))
        {
            //己方為先手方
            if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead))
            {
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDVC, subSkillID, skillType);
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                //TODO: add a condition for checking the last skill used by character
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDV2, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDV1, subSkillID, skillType);
                }
            }
            //己方為後手方
            else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Improviser))
            {
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                //TODO: add a condition for checking the last skill used by character
                if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV2, subSkillID, skillType);
                }
                //己方是否"近距離遠程方" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDVF, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1, subSkillID, skillType);
                }
            }
        }

        //"直擊方"/"輕直擊方"/重直擊方"
        if(gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Assaulter) ||
            gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter) ||
            gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter))
        {
            //己方為先手方
            if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead))
            {
                //敵方是否"未能抵抗方" ?
                //雙方已按下技能是否都是"遠程" ?
                //己方是否"速度勝方"?
                if (opponent.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NonResister) ||
                    (characterSubskillData.Range == Subskill.RangeType.ranged && oppoentSubskillData.Range == Subskill.RangeType.ranged) ||
                    (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedWinner)))
                {
                    //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AVC, subSkillID, skillType);
                    }

                    //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                    //TODO: add a condition for checking the last skill used by character
                    else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AV2, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AV1, subSkillID, skillType);
                    }
                }
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                //TODO: add a condition for checking the last skill used by character
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDVC, subSkillID, skillType);
                }
                //已按下技能是否與上1ATL 相同&有"v2演出" ?
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType("V2"))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDV2, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BDV1, subSkillID, skillType);
                }
            }
            //己方為後手方
            else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Improviser))
            {
                //己方是否"無視遠程方" ?
                if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreRangedSkill))
                {
                    StartCoroutine(PlayCFWAnimation(subSkillID, skillType));
                }

                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                //TODO: add a condition for checking the last skill used by character
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV2, subSkillID, skillType);
                }
                //是否有強度負方&雙方已按下技能是"遠程" ?
                else if((gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StrengthLoser) || opponent.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StrengthLoser)) &&
                    (characterSubskillData.Range == Subskill.RangeType.ranged) && (oppoentSubskillData.Range == Subskill.RangeType.ranged))
                {
                    //己方是否"近距離遠程方" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                    {
                        StartCoroutine(PlayCFWVFAnimation(subSkillID, skillType));
                    }
                    else
                    {
                        StartCoroutine(PlayCFWAnimation(subSkillID, skillType));
                    }
                }
                //己方是否"近距離遠程方" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDVF, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CDV1, subSkillID, skillType);
                }
            }
        }

        //"受擊方"/"輕受擊方"/"重受擊方"
        if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Recipient) ||
          gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient) ||
          gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient))
        {
            if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead))
            {
                //己方是否"中距離近戰方"&已按下技能有"vc演出" ?
                if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer) && characterAnimationHandler.CheckIfSameAsLastATLCodeType("VC"))
                {
                    //己方是否"輕受擊方" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPVC_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPVC_H, subSkillID, skillType);
                    }
                }
                //已按下技能是否與上1ATL 相同 & 上1ATL播放了"v1演出" & 有"v2演出" ?
                //TODO: add a condition for checking the last skill used by character
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                {
                    //己方是否"輕受擊方" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPV2_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPV2_H, subSkillID, skillType);
                    }
                }
                //己方是否"輕受擊方" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                {
                    //己方是否"速度負方" ? or 雙方已按下技能是否都是"遠程" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser) || (characterSubskillData.Range == Subskill.RangeType.ranged && oppoentSubskillData.Range == Subskill.RangeType.ranged))
                    {
                        StartCoroutine(PlayBFLAnimation(subSkillID, skillType));
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_D_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPV1_L, subSkillID, skillType);
                    }
                }
                //己方是否"速度負方" ?  or 雙方已按下技能是否都是"遠程" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser) || (characterSubskillData.Range == Subskill.RangeType.ranged && oppoentSubskillData.Range == Subskill.RangeType.ranged))
                {
                    StartCoroutine(PlayBFLAnimation(subSkillID, skillType));
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_D_H, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BLPV1_H, subSkillID, skillType);
                }
            }
            //己方為後手方
            else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Improviser))
            {
                //己方是否"未能抵抗方" ?
                if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NonResister))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_D_H, subSkillID, skillType);
                }
                //己方是否"速度負方" /"速度強度負方" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedLoser) || gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser))
                {
                    //TODO: add a condition for checking the last skill used by character
                    if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                    {
                        //己方是否"輕受擊方" ?
                        if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                        {
                            characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_L, subSkillID, skillType);
                        }
                        else
                        {
                            characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_H, subSkillID, skillType);
                        }
                    }
                    //己方是否"輕受擊方" ?
                    else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                    {
                        //己方是否"近距離遠程方" ?
                        if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                        {
                            characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSVF_L, subSkillID, skillType);
                        }
                        else
                        {
                            characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV1_L, subSkillID, skillType);
                        }
                    }
                    //己方是否"近距離遠程方" ?
                    else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSVF_H, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV1_H, subSkillID, skillType);
                    }
                }
                //TODO: add a condition for checking the last skill used by character
                else if (characterAnimationHandler.CheckIfSameAsLastATLCodeType(lastCodeV1andV2))
                {
                    //己方是否"輕受擊方" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLSV2_H, subSkillID, skillType);
                    }
                }
                //己方是否"輕受擊方" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient))
                {
                    //己方是否"近距離遠程方" ?
                    if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPVF_L, subSkillID, skillType);
                    }
                    else
                    {
                        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPV1_L, subSkillID, skillType);
                    }
                }
                //己方是否"近距離遠程方" ?
                else if (gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer))
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPVF_H, subSkillID, skillType);
                }
                else
                {
                    characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CLPV1_H, subSkillID, skillType);
                }
            }
        }
    }

    private IEnumerator PlayCFWAnimation(string subSkillID, int skillType)
    {
        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CFW, subSkillID, skillType);
        yield return new WaitForSeconds(characterAnimationHandler.GetAnimationClipTotalLength());
        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AV1, subSkillID, skillType);
    }

    private IEnumerator PlayCFWVFAnimation(string subSkillID, int skillType)
    {
        characterAnimationHandler.LoadAndPlayAnimation(true, true, DatabaseManager.AnimationData.CodeType.camB_type_CFWVF, subSkillID, skillType);
        yield return new WaitForSeconds(characterAnimationHandler.GetAnimationClipTotalLength());
        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_AV1, subSkillID, skillType);
    }

    private IEnumerator PlayBFLAnimation(string subSkillID, int skillType)
    {
        characterAnimationHandler.LoadAndPlayAnimation(false, true, DatabaseManager.AnimationData.CodeType.camA_type_BFL, subSkillID, skillType);
        yield return new WaitForSeconds(characterAnimationHandler.GetAnimationClipTotalLength());
    }
}
