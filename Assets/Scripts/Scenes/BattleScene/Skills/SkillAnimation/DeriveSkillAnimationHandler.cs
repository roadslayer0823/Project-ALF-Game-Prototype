using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class DeriveSkillAnimationHandler : MonoBehaviour
{
    [SerializeField] private Image part1_Animation = null;
    [SerializeField] private GameObject deriveSkillContainer = null;
    [SerializeField] private GameObject Part1_Container = null;
    [SerializeField] private GameObject Part2_Container = null;
    [SerializeField] private PlayableDirector part2_Animation = null;
    [SerializeField] private Image[] part2_AnimationList = null;
    [SerializeField] private Transform pivot = null;

    private const string AUDIO_ID_DERIVE_SKILL_PART_A = "derive_skill_partA";

    public void PlayDeriveAnimationPart1(float duration)
    {
        ResetAnimation();
        LeanTween.alpha(part1_Animation.GetComponent<RectTransform>(), 0.9f, duration).setOnComplete(() =>
        {
            Part2_Container.SetActive(true);
        });
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_DERIVE_SKILL_PART_A);
    }

    public void PlayDeriveAnimationPart2()
    {
        ResetAnimation();
        part2_Animation.Play();
        Part1_Container.SetActive(false);
    }

    public void ResetAnimation()
    {
        Color part1AnimationAlpha = this.part1_Animation.color;
        part1AnimationAlpha.a = 0;
        this.part1_Animation.color = part1AnimationAlpha;
        Part1_Container.SetActive(true);
        ResetAnimationPart2();
    }

    public void ResetAnimationPart2()
    {
       Part2_Container.SetActive(false);
       for(int i=2; i < part2_AnimationList.Length; i++)
       {
            Color color = part2_AnimationList[i].color;

            color.a = 0.0f;

            part2_AnimationList[i].color = color;
       }

       Color color02 = part2_AnimationList[0].color;
       Color color03 = part2_AnimationList[1].color;

       color02.a = 0.9f;
       color03.a = 0.3f;

       part2_AnimationList[0].color = color02;
       part2_AnimationList[1].color = color03;
    }

    public Transform GetPivot()
    {
        return this.pivot;
    }
}
