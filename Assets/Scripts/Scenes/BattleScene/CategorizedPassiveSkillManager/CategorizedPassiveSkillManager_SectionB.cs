using System.Collections.Generic;
using UnityEngine;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 玩家1激昂效果
    // 玩家2激昂效果
    public static void RunCharacterExcitementEffect( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        float skill_L3_MengLie_value = (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL3)) ? 0.2f : 0.0f;
        //float skill_L12_ShengShengBuXi_value = gameCharacterOne.生命積分 >= 250 && (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12)) ? 0.2f : 0.0f;
        float energyMarker_value = (gameCharacterTwo_BattleResultData.HasEnergyMarker()) ? 0.5f : 0.0f;
        //float skill_L4_JianRen_value = (gameCharacterTwo.is生命流 && gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL4)) ? 0.2f : 0.0f;
        //float skill_L12_NiFeng = (gameCharacterTwo.is以太流 && gameCharacterTwo_BattleResultData.maximumStatePoint < 80) ? 0.1f : 0.0f;
        float gameChacracterOneSkillDamage = gameCharacterOne.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;
        float gameChacracterTwoSkillDamage = gameCharacterTwo.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;

        //["玩家1"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n]*[1-0.2-n]
        //float gameCharacterOneHealthPointDamage = gameChacracterTwoSkillDamage * (1 + skill_L3_MengLie_value + skill_L12_ShengShengBuXi_value) * (1 - 0.2 - skill_L12_ShengShengBuXi_value);
        //battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, gameCharacterOneHealthPointDamage);

        //["玩家2"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n+n]*[1-n-n-n]
        //float gameCharacterTwoHealthPointDamage = gameChacracterOneSkillDamage * (1 + skill_L3_MengLie_value + skill_L12_ShengShengBuXi_value + energyMarker_value) * (1 - skill_L4_JianRen_value - skill_L12_ShengShengBuXi_value - skill_L12_NiFeng);
        //battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, gameCharacterTwoHealthPointDamage);
    }

    // 判定輕重受擊方
    public static void RunDeterminingLightOrHeavyRecipient( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter assaulter, GameCharacter recipient )
    {
        // CASE A:"受擊方"當前流向為"生命流"
        if (recipient)
        {
            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */
            if (assaulter)
            {
                /*
                 * "受擊方"得到重受擊方,
                   "直擊方"得到重直擊方
                 */
            }
            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
            }
        }
        // CASE B:"受擊方"當前流向為"以太流"
        else if (recipient)
        {
            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */

            /*
             * "受擊方"是否
                "強度負方"/"速度強度負方"?
             */

            if (assaulter || recipient) // if yes
            {
                /*
                 * "受擊方"得到重受擊方,
                   "直擊方"得到重直擊方
                 */
            }

            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
            }

        }

        // CASE C:"受擊方"當前流向為"負荷流"
        else if (recipient)
        {
            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */

            /*"受擊方"是否
            "速度負方"/"速度強度負方"?
            &&
            "受擊方"負荷等級
            是否>=2?
            */

            if (assaulter || (recipient && recipient)) // if (assaulter == yes || (recipient.speed == yes && recipient.stress < 2))
            {
                /*
                 * "受擊方"得到重受擊方,
                   "直擊方"得到重直擊方
                 */
            }
            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
            }
        }

        // CASE D:"受擊方"當前流向為無流向
        else if (recipient)
        {
            /*
             * "受擊方"得到重受擊方,
               "直擊方"得到重直擊方
             */

        }
    }

}
