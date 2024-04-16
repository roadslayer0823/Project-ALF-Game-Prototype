using UnityEngine;
using UnityEngine.UI;

public class ATLSlotListPanelV3 : MonoBehaviour
{
    //progress bar animation testing
    [SerializeField] private ATLSlotV2[] theATLSlots = new ATLSlotV2[0];
    [SerializeField] private Image repulseAndDefendProgressBar;
    [SerializeField] private Image repulseAndDefendPointer;
    [SerializeField] private Image activeSkillProgressBar;
    [SerializeField] private Image activeSkillPointer;
    [SerializeField] private RectTransform yellowProgressBarStartPoint;
    [SerializeField] private RectTransform yellowProgressBarEndPoint;
    [SerializeField] private RectTransform blueProgressBarStartPoint;
    [SerializeField] private RectTransform blueProgressBarEndPoint;

    //stored current location
    private float yellowProgressBarStartPointX = 0.0f;
    private float yellowProgressBarLastPosition = 0.0f;
    private float yellowProgressBarLength = 0.0f;

    private float blueProgressBarStartPointX = 0.0f;
    private float blueProgressBarLastPosition = 0.0f;
    private float blueProgressBarLength = 0.0f;

    private int lastAtlNumber = 0;
    private ATLSlotV2 currentAtlSlot = null;

    public void Initialize()
    {
        this.yellowProgressBarStartPointX = this.yellowProgressBarStartPoint.position.x;
        this.yellowProgressBarLength = this.yellowProgressBarEndPoint.position.x - this.yellowProgressBarStartPointX;

        this.blueProgressBarStartPointX = this.blueProgressBarStartPoint.position.x;
        this.blueProgressBarLength = this.blueProgressBarEndPoint.position.x - this.blueProgressBarStartPointX;
        Reset();
    }

    public void SetUp(BattleFlowATL[] flowATLs)
    {
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            if (i < flowATLs.Length)
            {
                this.theATLSlots[i].DefaultATLSetup(flowATLs[i]);
            }
        }

        Reset();
    }

    public void SetUp(BattleFlowATL_V2[] flowATLs)
    {
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            if (i < flowATLs.Length)
            {
                this.theATLSlots[i].DefaultATLSetup(flowATLs[i], 1);
            }
        }

        Reset();
    }

    public void GoToATL(int atlNumber, float animationDuration, CharacterSkill skill = null)
    {
        if (atlNumber == lastAtlNumber)
        {
            MarkPreviousATLSlotsAsUsed();
            return;
        }

        if (atlNumber > this.theATLSlots.Length)
        {
            MarkPreviousATLSlotsAsUsed();
            return;
        }

        int _atlIndex = atlNumber - 1;
        int _usedAtlIndex = _atlIndex;

        if (skill != null)
        {
            if (skill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.repulse)
            {
                _usedAtlIndex -= 1;
            }
        }

        currentAtlSlot = null;
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            ATLSlotV2 _atlSlot = this.theATLSlots[i];
            ATLSlotV2.ATLCurrentStatus _currentStatus = ATLSlotV2.ATLCurrentStatus.None;

            if (i < _usedAtlIndex)
            {
                _currentStatus = ATLSlotV2.ATLCurrentStatus.Used;
            }

            else if (i <= _atlIndex)
            {
                _currentStatus = ATLSlotV2.ATLCurrentStatus.Using;
                currentAtlSlot = _atlSlot;
            }

            else
            {
                _currentStatus = ATLSlotV2.ATLCurrentStatus.Unused;
            }

            _atlSlot.Show(_currentStatus);
        }

        PlayBlueProgressBarAnimation(currentAtlSlot, animationDuration);
        PlayYellowProgressBarAnimation(currentAtlSlot, animationDuration);

        this.lastAtlNumber = atlNumber;
    }

    public void GoToMiddleAtCurrentAtlSlot(float animationDuration)
    {
        LeanTween.cancel(this.gameObject);

        float _atlSlotMiddlePoint = currentAtlSlot.GetMiddlePointX();
        PlayBlueProgressBarAnimation(_atlSlotMiddlePoint, animationDuration);
        PlayYellowProgressBarAnimation(_atlSlotMiddlePoint, animationDuration);
    }

    public void GoToEndAtCurrentAtlSlot(float animationDuration)
    {
        LeanTween.cancel(this.gameObject);

        float _atlSlotEndingPoint = currentAtlSlot.GetEndingPointX();
        PlayBlueProgressBarAnimation(_atlSlotEndingPoint, animationDuration);
        PlayYellowProgressBarAnimation( _atlSlotEndingPoint, animationDuration);
    }

    public void GoToFinish(float duration)
    {
        MarkPreviousATLSlotsAsUsed();

        //blue progress bar
        LeanTween.value(this.gameObject, this.blueProgressBarLastPosition, this.blueProgressBarEndPoint.position.x, duration)
            .setOnUpdate((float value) =>
            {
                UpdateBlueProgress(value);
            })
            .setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                this.repulseAndDefendPointer.gameObject.SetActive(false);
                for (int i = 0; i < this.theATLSlots.Length; i++)
                {
                    this.theATLSlots[i].Show(ATLSlotV2.ATLCurrentStatus.Used);
                }
            });

        //yellow progress bar
        LeanTween.value(this.gameObject, this.yellowProgressBarLastPosition, this.yellowProgressBarEndPoint.position.x, duration)
            .setOnUpdate((float value) =>
            {
                UpdateYellowProgress(value);
            })
            .setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                this.activeSkillPointer.gameObject.SetActive(false);
                for (int i = 0; i < this.theATLSlots.Length; i++)
                {
                    this.theATLSlots[i].Show(ATLSlotV2.ATLCurrentStatus.Used);
                }
            });
    }

    public void PlayBlueProgressBarAnimation(ATLSlotV2 atlSlot, float duration)
    {
        float _atlSlotStartPoint = atlSlot.GetStartingPointX();
        float _atlSlotMiddlePoint = atlSlot.GetMiddlePointX();

        PlayBlueProgressBarAnimation(_atlSlotStartPoint, duration * 0.1f)
            .setEase(LeanTweenType.easeOutQuad).setOnComplete(() => PlayBlueProgressBarAnimation(_atlSlotMiddlePoint, duration * 0.9f));

        repulseAndDefendPointer.gameObject.SetActive(true);
    }

    public void PlayYellowProgressBarAnimation(ATLSlotV2 atlSlot, float duration)
    {
        float _atlSlotStartPoint = atlSlot.GetStartingPointX();
        float _atlSlotMiddlePoint = atlSlot.GetMiddlePointX();

        PlayYellowProgressBarAnimation( _atlSlotStartPoint, duration * 0.1f)
           .setEase(LeanTweenType.easeOutQuad).setOnComplete(() => PlayYellowProgressBarAnimation( _atlSlotMiddlePoint, duration * 0.9f));

        activeSkillPointer.gameObject.SetActive(true);
    }


    public LTDescr PlayBlueProgressBarAnimation(float targetPoint, float duration)
    {
        return LeanTween.value(this.gameObject, this.blueProgressBarLastPosition, targetPoint, duration)
                    .setOnUpdate((float value) =>
                    {
                        UpdateBlueProgress(value);
                    });
    }

    public LTDescr PlayYellowProgressBarAnimation(float targetPoint, float duration)
    {
        return LeanTween.value(this.gameObject, this.yellowProgressBarLastPosition, targetPoint, duration)
                    .setOnUpdate((float value) =>
                    {
                        UpdateYellowProgress(value);
                    });
    }

    private void UpdateBlueProgress(float progress)
    {
        float _distance = progress - this.blueProgressBarStartPointX;
        repulseAndDefendProgressBar.fillAmount = _distance / this.blueProgressBarLength;

        Vector3 _pos = repulseAndDefendPointer.transform.position;
        _pos.x = progress;
        repulseAndDefendPointer.transform.position = _pos;

        this.blueProgressBarLastPosition = progress;
    }

    private void UpdateYellowProgress(float progress)
    {
        float _distance = progress - this.yellowProgressBarStartPointX;
        activeSkillProgressBar.fillAmount = _distance / this.yellowProgressBarLength;

        Vector3 _pos = activeSkillPointer.transform.position;
        _pos.x = progress;
        activeSkillPointer.transform.position = _pos;

        this.yellowProgressBarLastPosition = progress;
    }

    private void MarkPreviousATLSlotsAsUsed()
    {
        int _atlIndex = this.lastAtlNumber - 1;
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            if (i < _atlIndex)
            {
                this.theATLSlots[i].Show(ATLSlotV2.ATLCurrentStatus.Used);
            }
        }
    }

    private void Reset()
    {
        this.blueProgressBarLastPosition = this.blueProgressBarStartPointX;
        this.yellowProgressBarLastPosition = this.yellowProgressBarStartPointX;
        this.repulseAndDefendProgressBar.fillAmount = 0;
        this.activeSkillProgressBar.fillAmount = 0;
        this.lastAtlNumber = 0;
        this.currentAtlSlot = null;
    }
}
