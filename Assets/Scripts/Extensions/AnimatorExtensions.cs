using UnityEngine;

public static class AnimatorExtensions
{
    public static AnimationClip GetAnimationClip( this Animator animator, string clipName )
    {
        AnimationClip[] _clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < _clips.Length; i++)
        {
            AnimationClip _clip = _clips[ i ];
            if (_clip.name == clipName)
            {
                return _clip;
            }
        }

        return null;
    }
}
