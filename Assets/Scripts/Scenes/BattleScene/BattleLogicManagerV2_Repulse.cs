using System.Collections.Generic;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using RangeType = DatabaseManager.Subskill.RangeType;

public partial class BattleLogicManagerV2
{
    private static void SettleResultForRepulseDraw( ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        RangeType _gameCharacterOne_SkillRangeType = gameCharacterOne.GetCurrentSkillRangeType();
        RangeType _gameCharacterTwo_SkillRangeType = gameCharacterTwo.GetCurrentSkillRangeType();

        // 玩家1的已按下技能是否"遠程"? NO
        if (_gameCharacterOne_SkillRangeType != RangeType.ranged)
        {
            // 平手方當前以太值結算
            // 平手方負荷值結算
        }

        // 玩家2的已按下技能是否"遠程"? NO
        if (_gameCharacterTwo_SkillRangeType != RangeType.ranged)
        {
            // 平手方當前以太值結算
            // 平手方負荷值結算
        }

        // 是否雙方都是"平手方"? YES
        if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.Deuce )
            && gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
        {
            // 雙方已按下技能的接觸判定是否相同? YES
            if (_gameCharacterOne_SkillRangeType == _gameCharacterTwo_SkillRangeType)
            {
                // "玩家1"當前流向是否"生命流"? YES
                if (gameCharacterOne.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life)
                {
                    // 進入“玩家1激昂效果”頁面。
                    CategorizedPassiveSkillManager.RunCharacterExcitementEffect( ref battleResultData, gameCharacterOne, gameCharacterTwo );
                }

                // "玩家2"當前流向是否"生命流"? YES
                if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life)
                {
                    // 進入“玩家2激昂效果”頁面。
                    CategorizedPassiveSkillManager.RunCharacterExcitementEffect( ref battleResultData, gameCharacterTwo, gameCharacterOne );
                }
            }

            // TODO:
            // "雙方"的技能持續效果更新(例如:能量殘響)
            // 負荷積分等級結算
            // 發動流向效果B
            // 進入“Part B”頁面
        }
        else
        {
            // "玩家1"&"玩家2"是否也是"重受擊方"? YES
            if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient )
                && gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient ))
            {
                // TODO:
                // 進入“重受擊相殺”頁面
            }
            // "玩家1"&"玩家2"是否也是"重受擊方"? NO
            else
            {
                // 平手方的"玩家"取消"平手方",得到"重直擊方"

                if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
                {
                    gameCharacterOne.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
                    gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
                }

                if (gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
                {
                    gameCharacterTwo.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
                    gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
                }

                // TODO:
                // 進入“重受擊方生命值結算”頁面
            }

            // TODO:
            // "雙方"的技能持續效果更新(例如:能量殘響)
            // 負荷積分等級結算
            // 發動流向效果B
            // 進入“Part B”頁面
        }
    }
}
