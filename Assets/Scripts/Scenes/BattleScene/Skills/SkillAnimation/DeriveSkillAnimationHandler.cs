using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class DeriveSkillAnimationHandler : MonoBehaviour
{
    [SerializeField] private Image part1_Animation = null;
    [SerializeField] private GameObject deriveSkillContainer = null;
    [SerializeField] private GameObject Part1Container = null;
    [SerializeField] private GameObject Part2Container = null;
    [SerializeField] private PlayableDirector part2_Animation = null;
    [SerializeField] private Image[] part2_AnimationList = null; 

    private const string AUDIO_ID_DERIVE_SKILL_PART_A = "derive_skill_partA";

    public void DeriveAnimationPart1(float duration)
    {
        LeanTween.alpha(part1_Animation.GetComponent<RectTransform>(), 0.9f, duration).setOnComplete(() =>
        {
            Part2Container.SetActive(true);
        });
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_DERIVE_SKILL_PART_A);
    }

    public void DeriveAnimationPart2()
    {
        part2_Animation.Play();
        Part1Container.SetActive(false);
    }

    public void ResetAnimation()
    {
        LeanTween.alpha(part1_Animation.GetComponent<RectTransform>(), 0f, 0.1f).setOnComplete(() =>
        {
            Part1Container.SetActive(true);
            ResetAnimationPart2();
        });
    }

    public void ResetAnimationPart2()
    {
       Part2Container.SetActive(false);
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
}
