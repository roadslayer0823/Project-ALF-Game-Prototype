using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleAnimationEventManager;

public class CharacterAnimator : MonoBehaviour
{

    [SerializeField] private GameCharacter gameCharacter = null;

    public void OnStart()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.OnStart);
    }

    public void OnEnd()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.OnEnd);
    }

    public void ShowLeftView()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowLeftView);
    }

    public void ShowRightView()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowRightView);
    }

    public void PlaySoundEffect(string audioId)
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.PlaySoundEffect, audioId);
    }

    public void ShowInfoWhenUsingSkill()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowInfoWhenUsingSkill);
    }

    public void ShowInfoOnHit()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowInfoOnHit);
    }

    public void ReadyForRepulse()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ReadyForRepulse);
    }

    public void ReadyForDefense()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ReadyForDefense);
    }

    public void ReadyForEvasion()
    {
        this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ReadyForEvasion);
    }
}
