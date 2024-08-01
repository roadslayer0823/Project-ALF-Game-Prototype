using UnityEngine;

public class BattleAnimationEventManager : MonoBehaviour
{
    [SerializeField] private GameObject playerContainer = null;
    [SerializeField] private GameObject opponentContainer = null;
    private BattleVisualEffectManager battleVisualEffectManager = null;

    public enum CharacterAnimationEventType
    {
        None,
        OnStart,
        OnEnd,
        PlaySoundEffect,
        ShowInfoWhenUsingSkill,
        ShowInfoOnHit,
        StartPartB,
    }

    public void SetUp(BattleVisualEffectManager battleVisualEffectManager)
    {
        this.battleVisualEffectManager = battleVisualEffectManager;
    }

    public void OnAnimationEventTriggered (CharacterAnimationEventType animationEventType, string parameter = "")
    {
        switch (animationEventType)
        {
            case CharacterAnimationEventType.OnStart:
                break;

            case CharacterAnimationEventType.OnEnd:
                break;

            case CharacterAnimationEventType.PlaySoundEffect:
                AudioManager.Instance.PlaySoundEffect(parameter);
                break;

            case CharacterAnimationEventType.ShowInfoWhenUsingSkill:
                break;

            case CharacterAnimationEventType.ShowInfoOnHit:
                break;

            case CharacterAnimationEventType.StartPartB:
                battleVisualEffectManager.ApplyBlurShaderAtRecipient();
                break;
        }
    }
}
