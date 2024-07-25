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
        PlaySoundEffect,
        ShowInfoWhenUsingSkill,
        ShowInfoOnHit,
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

            case CharacterAnimationEventTrigger.PlaySoundEffect:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.PlaySoundEffect, parameter);
                break;

            case CharacterAnimationEventTrigger.ShowInfoWhenUsingSkill:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowInfoWhenUsingSkill);
                break;

            case CharacterAnimationEventTrigger.ShowInfoOnHit:
                this.gameCharacter.OnAnimationEventTriggered(CharacterAnimationEventType.ShowInfoOnHit);
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
}
