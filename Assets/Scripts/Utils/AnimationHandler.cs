using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    private AnimationClip animationClip;
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController)
        {
            name = "OverrideAnimator"
        };
        animator.runtimeAnimatorController = animatorOverrideController;
    }

    public void LoadAndPlayAnimation(string filePath)
    {
        animationClip = Resources.Load<AnimationClip>(filePath);
        animatorOverrideController["default"] = animationClip;
        animator.SetTrigger("trigger");
    }
}
