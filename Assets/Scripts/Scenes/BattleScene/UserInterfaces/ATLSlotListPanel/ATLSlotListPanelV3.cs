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
    private float blueProgressBarStartPointX = 0.0f;
    private float blueProgressBarLastPosition = 0.0f;
    private float blueProgressBarLength = 0.0f;

    private float yellowProgressBarStartPointX = 0.0f;
    private float yellowProgressBarLastPosition = 0.0f;
    private float yellowProgressBarLength = 0.0f;

    private int lastAtlNumber = 0;
    private ATLSlotV2 currentAtlSlot = null;

    public void Initialize()
    {
        this.blueProgressBarStartPointX = this.blueProgressBarStartPoint.position.x;
        this.blueProgressBarLength = this.blueProgressBarEndPoint.position.x - this.blueProgressBarStartPointX;

        this.yellowProgressBarStartPointX = this.yellowProgressBarStartPoint.position.x;
        this.yellowProgressBarLength = this.yellowProgressBarEndPoint.position.x - this.yellowProgressBarStartPointX;

        Reset();
    }

    public void SetUp( BattleFlowATL[] flowATLs )
    {
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            if (i < flowATLs.Length)
            {
                this.theATLSlots[ i ].DefaultATLSetup( flowATLs[ i ] );
            }
        }

        Reset();
    }

    public void SetUp( BattleFlowATL_V2[] flowATLs )
    {
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            if (i < flowATLs.Length)
            {
                this.theATLSlots[ i ].DefaultATLSetup( flowATLs[ i ], 0 );
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

        PlayProgressBarAnimation( this.repulseAndDefendPointer, this.repulseAndDefendProgressBar, currentAtlSlot, animationDuration );
        PlayProgressBarAnimation( this.activeSkillPointer, this.activeSkillProgressBar, currentAtlSlot, animationDuration );

        this.lastAtlNumber = atlNumber;
    }

    public void GoToMiddleAtCurrentAtlSlot( float animationDuration )
    {
        LeanTween.cancel( this.gameObject );

        float _atlSlotMiddlePoint = currentAtlSlot.GetMiddlePointX();
        PlayBlueProgressBarAnimation( this.repulseAndDefendPointer, this.repulseAndDefendProgressBar, _atlSlotMiddlePoint, animationDuration );
        PlayYellowProgressBarAnimation( this.activeSkillPointer, this.activeSkillProgressBar, _atlSlotMiddlePoint, animationDuration );
    }

    public void GoToEndAtCurrentAtlSlot( float animationDuration )
    {
        LeanTween.cancel( this.gameObject );

        float _atlSlotEndingPoint = currentAtlSlot.GetEndingPointX();
        PlayBlueProgressBarAnimation( this.repulseAndDefendPointer, this.repulseAndDefendProgressBar, _atlSlotEndingPoint, animationDuration );
        PlayYellowProgressBarAnimation( this.activeSkillPointer, this.activeSkillProgressBar, _atlSlotEndingPoint, animationDuration );
    }

    public void GoToFinish(float duration)
    {
        MarkPreviousATLSlotsAsUsed();

        LeanTween.value(this.gameObject, this.blueProgressBarLastPosition,this.blueProgressBarEndPoint.position.x, duration)
            .setOnUpdate((float value) =>
            {
                UpdateProgress(this.repulseAndDefendPointer, this.repulseAndDefendProgressBar, value, blueProgressBarStartPointX, blueProgressBarLastPosition, blueProgressBarLength);
                UpdateProgress(this.activeSkillPointer, this.activeSkillProgressBar, value, yellowProgressBarStartPointX, yellowProgressBarLastPosition, yellowProgressBarLength);
            })
            .setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
            {
                this.repulseAndDefendPointer.gameObject.SetActive(false);
                this.activeSkillPointer.gameObject.SetActive(false);

                for (int i = 0; i < this.theATLSlots.Length; i++)
                {
                    this.theATLSlots[ i ].Show( ATLSlotV2.ATLCurrentStatus.Used );
                }
            });
    }

    public void PlayProgressBarAnimation( Image pointer, Image progressBar, ATLSlotV2 atlSlot, float duration )
    {
        float _atlSlotStartPoint = atlSlot.GetStartingPointX();
        float _atlSlotMiddlePoint = atlSlot.GetMiddlePointX();

        PlayBlueProgressBarAnimation( pointer, progressBar, _atlSlotStartPoint, duration * 0.1f )
            .setEase( LeanTweenType.easeOutQuad ).setOnComplete( () => PlayBlueProgressBarAnimation( pointer, progressBar, _atlSlotMiddlePoint, duration * 0.9f ) );

        pointer.gameObject.SetActive( true );
    }

    public LTDescr PlayBlueProgressBarAnimation( Image pointer, Image progressBar, float targetPoint, float duration )
    {
        return  LeanTween.value( this.gameObject, this.blueProgressBarLastPosition, targetPoint, duration )
                    .setOnUpdate( ( float value ) =>
                    {
                        UpdateProgress( pointer, progressBar, value, blueProgressBarStartPointX, blueProgressBarLastPosition, blueProgressBarLength);
                    } );
    }

    public LTDescr PlayYellowProgressBarAnimation(Image pointer, Image progressBar, float targetPoint, float duration)
    {
        return LeanTween.value(this.gameObject, this.yellowProgressBarLastPosition, targetPoint, duration)
                    .setOnUpdate((float value) =>
                    {
                        UpdateProgress(pointer, progressBar, value, yellowProgressBarStartPointX, yellowProgressBarLastPosition, yellowProgressBarLength);
                    });
    }

    private void UpdateProgress(Image pointer, Image progressBar, float progress, float startPointX, float lastPosition, float progressBarLength)
    {
        float _distance = progress - startPointX;
        progressBar.fillAmount = _distance / progressBarLength;

        Vector3 _pos = pointer.transform.position;
        _pos.x = progress;
        pointer.transform.position = _pos;

        lastPosition = progress;
    }

    private void MarkPreviousATLSlotsAsUsed()
    {
        int _atlIndex = this.lastAtlNumber - 1;
        for (int i = 0; i < this.theATLSlots.Length; i++)
        {
            if (i < _atlIndex)
            {
                this.theATLSlots[ i ].Show( ATLSlotV2.ATLCurrentStatus.Used );
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
