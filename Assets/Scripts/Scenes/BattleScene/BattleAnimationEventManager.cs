using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimationEventManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite leftBackgroundSprite = null;
    [SerializeField] private Sprite rightBackgroundSprite = null;

    public enum CharacterAnimationEventType
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

    public void OnAnimationEventTriggered (CharacterAnimationEventType animationEventType, string parameter = "")
    {
        switch (animationEventType)
        {
            case CharacterAnimationEventType.OnStart:
                break;

            case CharacterAnimationEventType.OnEnd:
                break;

            case CharacterAnimationEventType.ShowLeftView:
                this.background.sprite = this.leftBackgroundSprite;
                break;

            case CharacterAnimationEventType.ShowRightView:
                this.background.sprite = this.rightBackgroundSprite;
                break;

            case CharacterAnimationEventType.PlaySoundEffect:
                AudioManager.Instance.PlaySoundEffect(parameter);
                break;

            case CharacterAnimationEventType.ShowInfoWhenUsingSkill:
                break;

            case CharacterAnimationEventType.ShowInfoOnHit:
                break;

            case CharacterAnimationEventType.ReadyForRepulse:
                break;

            case CharacterAnimationEventType.ReadyForDefense:
                break;

            case CharacterAnimationEventType.ReadyForEvasion:
                break;
        }
    }
}
