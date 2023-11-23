using UnityEngine;
using UnityEngine.UI;

public class ATLSlotListPanelV2 : MonoBehaviour
{
    //progress bar animation testing
    [SerializeField] private ATLSlotV2[] theATLSlots = new ATLSlotV2[0];
    [SerializeField] private Image repulseAndDefendProgressBar;
    [SerializeField] private Image repulseAndDefendPointer;
    [SerializeField] private Image activeSkillProgressBar;
    [SerializeField] private Image activeSkillPointer;
    [SerializeField] private Image progressBarFiller;
    [SerializeField] private RectTransform progressBarStartPoint;
    [SerializeField] private RectTransform progressBarEndPoint;

    //stored current location
    private float progressBarStartPointX = 0.0f;
    private float progressBarLastPosition = 0.0f;
    private float progressBarLength = 0.0f;

    public void Initialize()
    {
        this.progressBarStartPointX = progressBarStartPoint.position.x;
        this.progressBarLastPosition = this.progressBarStartPoint.position.x;
        this.progressBarLength = this.progressBarEndPoint.position.x - this.progressBarLastPosition;
        repulseAndDefendProgressBar.fillAmount = 0;
        activeSkillProgressBar.fillAmount = 0;
        progressBarFiller.fillAmount = 0;
    }

    public void SetUp(BattleFlowATL[] flowATL)
    {
        for (int i = 0; i < theATLSlots.Length; i++)
        {
            if (i < flowATL.Length)
            {
                BattleFlowATL _flowATL = flowATL[i];
                theATLSlots[i].DefaultATLSetup(_flowATL);
            }
        }
    }

    public void GoToATL(int atlNumber, float ATLDuration, CharacterSkill skill)
    {
        int _atlIndex = atlNumber - 1;
        int _usedAtlIndex = _atlIndex - ((skill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse) ? 1 : 0);

        ATLSlotV2 _currentAtlSlot = null;
        for (int i = 0; i < theATLSlots.Length; i++)
        {
            ATLSlotV2 _atlSlot = theATLSlots[i];
            ATLSlotV2.ATLCurrentStatus _currentStatus = ATLSlotV2.ATLCurrentStatus.None;

            if (i < _usedAtlIndex)
            {
                _currentStatus = ATLSlotV2.ATLCurrentStatus.Used;
            }

            else if (i <= _atlIndex)
            {
                _currentStatus = ATLSlotV2.ATLCurrentStatus.Using;
                _currentAtlSlot = _atlSlot;
            }

            else
            {
                _currentStatus = ATLSlotV2.ATLCurrentStatus.Unused;
            }

            _atlSlot.Show(_currentStatus);
        }

        PlayProgressBarAnimation(repulseAndDefendPointer, repulseAndDefendProgressBar, _currentAtlSlot, ATLDuration);
        PlayProgressBarAnimation(activeSkillPointer, activeSkillProgressBar, _currentAtlSlot, ATLDuration);
    }

    public void GoToFinish(float duration)
    {
        float _atlLastSlotPoint = progressBarEndPoint.position.x;
        ATLSlotV2.ATLCurrentStatus _currentStatus = ATLSlotV2.ATLCurrentStatus.Used;

        LeanTween.value(gameObject, this.progressBarLastPosition, _atlLastSlotPoint, duration)
            .setOnUpdate((float value) =>
            {
                UpdateProgress(repulseAndDefendPointer, repulseAndDefendProgressBar, value);
                UpdateProgress(activeSkillPointer, activeSkillProgressBar, value);
            })
            .setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                repulseAndDefendPointer.gameObject.SetActive(false);
                activeSkillPointer.gameObject.SetActive(false);
                for(int i=0; i<theATLSlots.Length; i++)
                {
                    ATLSlotV2 _atlSlot = theATLSlots[i];
                    _atlSlot.Show(_currentStatus);
                }
            });
    }

    public void PlayProgressBarAnimation(Image pointer, Image progressBar, ATLSlotV2 atlSlot, float duration)
    {
        float _atlSlotStartPoint = atlSlot.GetStartingPointX();
        float _atlSlotMiddlePoiint = atlSlot.GetMiddlePointX();

        LeanTween.value(gameObject, this.progressBarLastPosition, _atlSlotStartPoint, duration * 0.1f).
            setOnUpdate((float value) =>
            {
                UpdateProgress(pointer, progressBar, value);
            }
            )
            .setEase(LeanTweenType.easeOutQuad).setOnComplete(() => {
                LeanTween.value(gameObject, this.progressBarLastPosition, _atlSlotMiddlePoiint, duration * 0.9f)
                .setOnUpdate((float value) => {
                    UpdateProgress(pointer, progressBar, value);
                });
            });

        pointer.gameObject.SetActive(true);
    }

    private void UpdateProgress(Image pointer, Image progressBar, float progress)
    {
        float _distance = progress - this.progressBarStartPointX;
        progressBar.fillAmount = _distance / this.progressBarLength;

        Vector3 _pos = pointer.transform.position;
        _pos.x = progress;
        pointer.transform.position = _pos;

        progressBarFiller.fillAmount = progressBar.fillAmount;
        this.progressBarLastPosition = progress;
    }
}
