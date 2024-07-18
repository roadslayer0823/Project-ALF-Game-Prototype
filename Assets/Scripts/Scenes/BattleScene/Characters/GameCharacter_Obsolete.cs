using System;
using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;
using AnimationEvent = BattleAnimationManager.AnimationEvent;

public partial class GameCharacter : MonoBehaviour
{
    [Obsolete] private CharacterSkill currentObservedSkill = null;
    [Obsolete] private int currentSkillStatIncrement = 0;
    [Obsolete] private bool isAbleToUseSkill = false;

    [Obsolete] private int counterAttacks = 0;
    [Obsolete] private List<PopUpDisplayInfo> popUpDisplayInfoList = new();

    // Break Status
    [Obsolete] private int breakStatusRemainingATLs = 0;
    [Obsolete] private bool isBreakStatusCausedByStatePoint = false;
    [Obsolete] private bool isBreakStatusCausedByStressValue = false;

    // Energy Marker
    [Obsolete] private bool hasJustAddedEnergyMarker = false;

    [Obsolete]
    public float AddCurrentHealthPoint( float amount )
    {
        return AddCurrentHealthPoint( amount, this.maximumHealthPoint );
    }

    [Obsolete]
    public float RecoverCurrentHealthPoint( float amount )
    {
        return AddCurrentHealthPoint( amount, this.virtualHealthPoint );
    }

    [Obsolete]
    public float AddCurrentHealthPoint( float amount, float maximumAmount )
    {
        if (amount > 0)
        {
            float _lastValue = this.currentHealthPoint;

            this.currentHealthPoint = Mathf.Clamp( this.currentHealthPoint + amount, 0.0f, maximumAmount );

            if (this.virtualHealthPoint < this.currentHealthPoint)
            {
                this.virtualHealthPoint = this.currentHealthPoint;
            }

            this.onCharacterInfoUpdated?.Invoke();

            return ( this.currentHealthPoint - _lastValue );
        }

        return 0.0f;
    }

    [Obsolete]
    public void MinusCurrentHealthPoint( float amount )
    {
        if (amount > 0)
        {
            this.currentHealthPoint -= amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    [Obsolete]
    public void AddVirtualHealthPoint(float amount)
    {
        if (amount > 0)
        {
            this.virtualHealthPoint = Mathf.Clamp(this.virtualHealthPoint + amount, 0.0f, this.maximumHealthPoint);
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    [Obsolete]
    public void MinusVirtualHealthPoint(float amount)
    {
        if (amount > 0)
        {
            this.virtualHealthPoint -= amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    [Obsolete]
    public void RecoverHealthPointToVirtualHP()
    {
        this.currentHealthPoint = this.virtualHealthPoint;
        this.onCharacterInfoUpdated?.Invoke();
    }

    [Obsolete]
    public float ClearVirtualHealthPoint()
    {
        float _difference = this.virtualHealthPoint - this.currentHealthPoint;

        this.virtualHealthPoint = this.currentHealthPoint;
        this.onCharacterInfoUpdated?.Invoke();

        return _difference;
    }

    [Obsolete]
    private void SetCurrentStatePoint( float amount )
    {
        this.currentStatePoint = Mathf.Clamp( amount, this.minimumStatePoint, this.maximumStatePoint );
    }

    [Obsolete]
    public void AddCurrentStatePoint( float amount )
    {
        if (amount > 0)
        {
            SetCurrentStatePoint( this.currentStatePoint + amount );
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    [Obsolete]
    public void MinusCurrentStatePoint( float amount, bool onHit, bool isBreakStatusAvailable )
    {
        if (amount > 0)
        {
            SetCurrentStatePoint( this.currentStatePoint - amount );

            if (isBreakStatusAvailable)
            {
                if (BattleLogicManager.IsGameCharacterInBreakStatus( this, onHit ))
                {
                    this.isBreakStatusCausedByStatePoint = true;

                    if (!GetIsInBreakStatus())
                    {
                        EnterIntoBreakStatus( 1 );
                    }

                    AudioManager.Instance.PlaySoundEffect( AUDIO_ID_BREAK );
                }
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    [Obsolete]
    public void SetCurrentStatePointToMaximum()
    {
        this.currentStatePoint = this.maximumStatePoint;
        this.onCharacterInfoUpdated?.Invoke();
    }

    [Obsolete]
    public float AddMaximumStatePoint( float amount )
    {
        if (amount > 0)
        {
            float _lastValue = this.maximumStatePoint;

            this.maximumStatePoint += amount;
            this.onCharacterInfoUpdated?.Invoke();

            return ( this.maximumStatePoint - _lastValue );
        }

        return 0.0f;
    }

    [Obsolete]
    public float MinusMaximumStatePoint( float amount )
    {
        if (amount > 0)
        {
            float _lastValue = this.maximumStatePoint;

            this.maximumStatePoint -= amount;

            float _lowestMaximumStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetLowestMaximumStatePoint();
            if (this.maximumStatePoint < _lowestMaximumStatePoint)
            {
                this.maximumStatePoint = _lowestMaximumStatePoint;
            }

            this.onCharacterInfoUpdated?.Invoke();

            return ( _lastValue - this.maximumStatePoint );
        }

        return 0.0f;
    }

    [Obsolete]
    public void AddCurrentStressValue( float amount, bool isBreakStatusAvailable )
    {
        Debug.Log("AddCurrentStressValue");
        if (amount > 0)
        {
            if (!this.isBreakStatusCausedByStressValue)
            {
                this.currentStressValue += amount;

                if (isBreakStatusAvailable)
                {
                    if (BattleLogicManager.IsGameCharacterInBreakStatus( this ))
                    {
                        this.currentStressValue = this.maximumStressValue;
                        this.isBreakStatusCausedByStressValue = true;

                        if (!GetIsInBreakStatus())
                        {
                            EnterIntoBreakStatus( 1 );
                        }
                    }
                }
                else
                {
                    if (this.currentStressValue >= this.maximumStressValue)
                    {
                        this.currentStressValue = this.maximumStressValue - 1;
                    }
                }
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    [Obsolete]
    public float MinusCurrentStressValue( float amount )
    {
        Debug.Log("MinusCurrentStressValue");
        if (amount > 0)
        {
            float _lastValue = this.currentStressValue;

            this.currentStressValue -= amount;

            if (this.currentStressValue < 0)
            {
                this.currentStressValue = 0.0f;
            }

            this.onCharacterInfoUpdated?.Invoke();

            return ( _lastValue - this.currentStressValue );
        }

        return 0.0f;
    }

    [Obsolete]
    public void MinusBreakStatusRemainingATLs()
    {
        if (this.breakStatusRemainingATLs > 0)
        {
            this.breakStatusRemainingATLs--;

            if (!GetIsInBreakStatus())
            {
                if (this.isBreakStatusCausedByStressValue)
                {
                    this.isBreakStatusCausedByStressValue = false;
                    this.currentStressValue = 0.0f;
                }

                if (this.isBreakStatusCausedByStatePoint)
                {
                    this.isBreakStatusCausedByStatePoint = false;
                    this.maximumStatePoint = this.originalStatePoint;
                    this.currentStatePoint = this.maximumStatePoint;
                }

                this.onCharacterInfoUpdated?.Invoke();
            }
        }
    }

    [Obsolete]
    public bool IsAbleToRepulse( BattleGameManager battleGameManager )
    {
        return IsAbleToRepulse( battleGameManager, out _, out _ );
    }

    [Obsolete]
    public bool IsAbleToRepulse( BattleGameManager battleGameManager, out CharacterSkill repulseSkill, out bool isSpecial )
    {
        repulseSkill = null;
        isSpecial = false;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        Subskill _attackerSubskillData = this.currentAttacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        if (!_attackerSubskillData.IsInterceptable)
        {
            return false;
        }

        if (battleGameManager.GetBattleFlowManager_V2() == null)
        {
            BattleFlowATL _nextATL = battleGameManager.GetBattleFlowManager().GetCurrentRound().GetNextATL( this );
            if (_nextATL != null)
            {
                repulseSkill = _nextATL.GetSelectedSkill().GetCharacterSubskillData().GetSelectedRepulseSkill();
            }
            else
            {
                return false;
            }
        }

        if (repulseSkill != null
            && ( ( int )_attackerSubskillData.EffectType > ( int )repulseSkill.GetCharacterSubskillData().GetSubskillData().EffectType ))
        {
            repulseSkill = null;
        }

        if (repulseSkill == null)
        {
            SkillSlotListPanel _skillSlotListPanel = battleGameManager.GetBattleUiManager().GetSkillSlotListPanel();
            SkillSlot[] _skillSlots = _skillSlotListPanel.GetSkillSlots();

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                CharacterSkill _slotSkill = _skillSlots[ i ].GetSelectedSkill();
                if (_slotSkill != null)
                {
                    CharacterSkill _slotRepulseSkill = _slotSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
                    if (( int )_attackerSubskillData.EffectType <= ( int )_slotRepulseSkill.GetCharacterSubskillData().GetSubskillData().EffectType)
                    {
                        repulseSkill = _slotRepulseSkill;
                        isSpecial = true;
                        break;
                    }
                }
            }
        }

        if (repulseSkill == null)
        {
            return false;
        }

        return true;
    }

    [Obsolete]
    public bool IsAbleToDerive()
    {
        return IsAbleToDerive( out _ );
    }

    [Obsolete]
    public bool IsAbleToDerive( out CharacterSkill derivedSkill )
    {
        derivedSkill = null;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        derivedSkill = this.currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();

        if (derivedSkill == null)
        {
            return false;
        }

        return true;
    }

    [Obsolete]
    public bool IsAbleToCounter()
    {
        return IsAbleToCounter( out _ );
    }

    [Obsolete]
    public bool IsAbleToCounter( out CharacterSkill counterSkill )
    {
        counterSkill = null;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        if (BattleLogicManager.HasGameCharacterReachedCounterAttackLimit( this ))
        {
            return false;
        }

        counterSkill = this.currentSkill.GetCharacterSubskillData().GetSelectedCounterSkill();

        if (counterSkill == null)
        {
            return false;
        }

        return true;
    }

    [Obsolete]
    public bool IsAbleToUseBackendSkill( CharacterSkill backendSkill )
    {
        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        Subskill _backendSubskillData = backendSkill.GetCharacterSubskillData().GetSubskillData();
        if (_backendSubskillData.IsEvadingSkill)
        {
            if (this.currentStatePoint <= 0)
            {
                return false;
            }

            if (this.currentAttacker != null)
            {
                Subskill _attackerSubskillData = this.currentAttacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
                if (( int )_attackerSubskillData.EffectType > ( int )_backendSubskillData.EffectType)
                {
                    return false;
                }
            }
        }

        return true;
    }

    [Obsolete]
    public void EnterIntoBreakStatus( int numberOfATLs )
    {
        this.breakStatusRemainingATLs = numberOfATLs;
        TriggerEvent( AnimationEvent.OnBeingInBreakStatus );
        BattleLogicManager.OnCharacterEnteredIntoBreakStatus( this );
    }

    [Obsolete]
    public void ShowPopUpDisplayInfo( string text, Color textColor )
    {
        GameObject _popUpDisplayInfoObj = Instantiate( this.popUpDisplayInfoPrefab.gameObject );
        _popUpDisplayInfoObj.transform.position = this.pivot.position;

        PopUpDisplayInfo _popUpDisplayInfo = _popUpDisplayInfoObj.GetComponent<PopUpDisplayInfo>();
        _popUpDisplayInfo.Show( text, textColor, 5.0f, TMPro.FontStyles.Bold ).MoveUpAndFadeOut( 2.0f, 0.5f, 1.5f ).SetOnDestroyedCallback( OnPopUpDisplayInfoDestroyed );

        this.popUpDisplayInfoList.Add( _popUpDisplayInfo );
    }

    [Obsolete]
    private void OnPopUpDisplayInfoDestroyed( PopUpDisplayInfo popUpDisplayInfo )
    {
        this.popUpDisplayInfoList.Remove( popUpDisplayInfo );
    }

    [Obsolete]
    public bool HasPopUpDisplayInfo()
    {
        return ( this.popUpDisplayInfoList.Count > 0 );
    }

    [Obsolete]
    public void SetCurrentObservedSkill( CharacterSkill currentObservedSkill )
    {
        this.currentObservedSkill = currentObservedSkill;
    }

    [Obsolete]
    public CharacterSkill GetCurrentObservedSkill()
    {
        return this.currentObservedSkill;
    }

    [Obsolete]
    public void SetCurrentSkillStatIncrement( int currentSkillStatIncrement )
    {
        this.currentSkillStatIncrement = currentSkillStatIncrement;
    }

    [Obsolete]
    public void ResetCurrentSkillStatIncrement()
    {
        this.currentSkillStatIncrement = 0;
        this.onCharacterInfoUpdated?.Invoke();
    }

    [Obsolete]
    public int GetCurrentSkillStatIncrement()
    {
        return this.currentSkillStatIncrement;
    }

    [Obsolete]
    public void SetIsAbleToUseSkill( bool isAbleToUseSkill )
    {
        this.isAbleToUseSkill = isAbleToUseSkill;
        this.onCharacterInfoUpdated?.Invoke();
    }

    [Obsolete]
    public bool GetIsAbleToUseSkill()
    {
        return this.isAbleToUseSkill;
    }

    [Obsolete]
    public void IncreaseCounterAttacks()
    {
        this.counterAttacks++;
    }

    [Obsolete]
    public void ResetCounterAttacks()
    {
        this.counterAttacks = 0;
    }

    [Obsolete]
    public int GetCounterAttacks()
    {
        return this.counterAttacks;
    }

    [Obsolete]
    public int GetBreakStatusRemainingATLs()
    {
        return this.breakStatusRemainingATLs;
    }

    [Obsolete]
    public bool GetIsInBreakStatus()
    {
        return ( this.breakStatusRemainingATLs > 0 );
    }

    [Obsolete]
    public bool GetIsBreakStatusCausedByStatePoint()
    {
        return this.isBreakStatusCausedByStatePoint;
    }

    [Obsolete]
    public bool GetIsBreakStatusCausedByStressValue()
    {
        return this.isBreakStatusCausedByStressValue;
    }

    [Obsolete]
    public void SetEnergyMarkerRemainingATLs( int energyMarkerRemainingATLs )
    {
        this.energyMarkerRemainingATLs = energyMarkerRemainingATLs;
        this.hasJustAddedEnergyMarker = true;
        this.onCharacterInfoUpdated?.Invoke();
    }

    [Obsolete]
    public void MinusEnergyMarkerRemainingATLs()
    {
        if (this.energyMarkerRemainingATLs > 0)
        {
            if (this.hasJustAddedEnergyMarker)
            {
                this.hasJustAddedEnergyMarker = false;
            }
            else
            {
                this.energyMarkerRemainingATLs--;
            }
        }
    }

    [Obsolete]
    public void RemoveEnergyMarker()
    {
        this.energyMarkerRemainingATLs = 0;
        this.onCharacterInfoUpdated?.Invoke();
    }
}
