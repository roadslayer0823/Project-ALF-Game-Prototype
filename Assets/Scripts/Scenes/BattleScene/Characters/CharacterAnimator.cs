using UnityEngine;
using CharacterAnimationEventType = BattleAnimationEventManager.CharacterAnimationEventType;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private GameCharacter gameCharacter = null;

    public enum CharacterAnimationEventTrigger
    {
        None,
        OnStart,
        OnEnd,
        ShowLeftView,
        ShowRightView,
        PlaySoundEffect,
        ShowInfoWhenUsingSkill,
        ShowInfoOnHit,
        ReadyForRepulse,
        ReadyForDefense,
        ReadyForEvasion
    }

    public void OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger eventTrigger, string parameter = "")
    {
        switch (eventTrigger)
        {
            case CharacterAnimationEventTrigger.OnStart:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.OnStart);
                break;

            case CharacterAnimationEventTrigger.OnEnd:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.OnEnd);
                break;

            case CharacterAnimationEventTrigger.ShowLeftView:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowLeftView);
                break;

            case CharacterAnimationEventTrigger.ShowRightView:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowRightView);
                break;

            case CharacterAnimationEventTrigger.PlaySoundEffect:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.PlaySoundEffect);
                break;

            case CharacterAnimationEventTrigger.ShowInfoWhenUsingSkill:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowInfoWhenUsingSkill);
                break;

            case CharacterAnimationEventTrigger.ShowInfoOnHit:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowInfoOnHit);
                break;

            case CharacterAnimationEventTrigger.ReadyForRepulse:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ReadyForRepulse);
                break;

            case CharacterAnimationEventTrigger.ReadyForDefense:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ReadyForDefense);
                break;

            case CharacterAnimationEventTrigger.ReadyForEvasion:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ReadyForEvasion);
                break;
        }
    }

    public void OnStart()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.OnStart);
    }

    public void OnEnd()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.OnEnd);
    }

    public void ShowLeftView()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ShowLeftView);
    }

    public void ShowRightView()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ShowRightView);
    }

    public void PlaySoundEffect(string audioId)
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.PlaySoundEffect, audioId);
    }

    public void ShowInfoWhenUsingSkill()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ShowInfoWhenUsingSkill);
    }

    public void ShowInfoOnHit()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ShowInfoOnHit);
    }

    public void ReadyForRepulse()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ReadyForRepulse);
    }

    public void ReadyForDefense()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ReadyForDefense);
    }

    public void ReadyForEvasion()
    {
        OnGameCharacterAnimationEventTriggered(CharacterAnimationEventTrigger.ReadyForEvasion);
    }
}
