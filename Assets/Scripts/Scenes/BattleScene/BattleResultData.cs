using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResultData
{
    private List<BattleResultData_GameCharacter> gameCharacterResultDataList = new();

    public class BattleResultData_GameCharacter
    {
        public GameCharacter gameCharacter = null;

        // 基本參數
        public float maximumHealthPoint = 0.0f;
        public float currentHealthPoint = 0.0f;
        public float virtualHealthPoint = 0.0f;
        public float originalStatePoint = 0.0f;
        public float maximumStatePoint = 0.0f;
        public float minimumStatePoint = 0.0f;
        public float currentStatePoint = 0.0f;
        public float maximumStressValue = 0.0f;
        public float currentStressValue = 0.0f;

        // 崩潰狀態
        public int stateBreakStatusRemainingATLs = 0;  // 以太崩潰維持值 (ATL)
        public int stressBreakStatusRemainingATLs = 0; // 負荷崩潰維持值 (ATL)

        // 能量殘響
        public int energyMarkerRemainingATLs = 0;

        // 其他狀態
        public bool isDead = false;

        // 改變參數（技能發動時）
        public float statePointCost = 0.0f;
        public float maximumStatePointIncrease = 0.0f;

        // 改變參數（命中目標時）
        public float actualHealthPointDamage = 0.0f;
        public float virtualHealthPointDamage = 0.0f;
        public float statePointDamage = 0.0f;
        public float stressValueDamage = 0.0f;

        public bool IsInBreakStatus()
        {
            return ( IsInStateBreakStatus() || IsInStressBreakStatus() );
        }

        public bool IsInStateBreakStatus()
        {
            return ( this.stateBreakStatusRemainingATLs > 0 );
        }

        public bool IsInStressBreakStatus()
        {
            return ( this.stressBreakStatusRemainingATLs > 0 );
        }

        public bool HasEnergyMarker()
        {
            return ( this.energyMarkerRemainingATLs > 0 );
        }

        public void SetCurrentHealthPoint( float value, bool needToUpdateVirtualHealthPoint, bool forVirtualDamageOnly = false )
        {
            this.currentHealthPoint = Mathf.Clamp( value, 0.0f, ( forVirtualDamageOnly ) ? this.virtualHealthPoint : this.maximumHealthPoint );

            if (needToUpdateVirtualHealthPoint)
            {
                UpdateVirtualHealthPoint();
            }

            if (this.currentHealthPoint <= 0)
            {
                this.currentHealthPoint = 0;
                this.isDead = true;
            }
        }

        public void SetVirtualHealthPoint( float value )
        {
            this.virtualHealthPoint = Mathf.Clamp( value, 0.0f, this.maximumHealthPoint );
            UpdateVirtualHealthPoint();
        }

        private void UpdateVirtualHealthPoint()
        {
            if (this.virtualHealthPoint < this.currentHealthPoint)
            {
                this.virtualHealthPoint = this.currentHealthPoint;
            }
        }

        public void SetMaximumStatePoint( float value )
        {
            float _lowestMaximumStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetLowestMaximumStatePoint();
            this.maximumStatePoint = ( this.maximumStatePoint < _lowestMaximumStatePoint ) ? _lowestMaximumStatePoint : value;
        }

        public void SetCurrentStatePoint( float value )
        {
            this.currentStatePoint = Mathf.Clamp( value, this.minimumStatePoint, this.maximumStatePoint );
        }

        public void SetCurrentStressValue( float value )
        {
            this.currentStressValue = Mathf.Clamp( value, 0.0f, this.maximumStressValue );
        }

        public float GetVirtualDamage()
        {
            if (this.virtualHealthPoint > this.currentHealthPoint)
            {
                return ( this.virtualHealthPoint - this.currentHealthPoint );
            }

            return 0.0f;
        }
    }

    private void AddNewElementIntoGameCharacterResultDataList( BattleResultData_GameCharacter gameCharacterResultData, bool isNewElement )
    {
        if (isNewElement)
        {
            this.gameCharacterResultDataList.Add( gameCharacterResultData );
        }
    }

    // 回復受到的“虛傷”。
    public BattleResultData AddGameCharacterResultData_VirtualHealthPointDamageRecovered( GameCharacter gameCharacter, float virtualHealthPointDamageRecovered, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint + virtualHealthPointDamageRecovered, false, true );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 回復受到的“實傷”。
    public BattleResultData AddGameCharacterResultData_ActualHealthPointDamageRecovered( GameCharacter gameCharacter, float actualHealthPointDamageRecovered, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint + actualHealthPointDamageRecovered, true );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 降低最大以太值。
    public BattleResultData AddGameCharacterResultData_MaximumStatePointDecrease( GameCharacter gameCharacter, float maximumStatePointDecrease, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetMaximumStatePoint( gameCharacterResultData.maximumStatePoint - maximumStatePointDecrease );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 提升最大以太值。
    public BattleResultData AddGameCharacterResultData_MaximumStatePointIncrease( GameCharacter gameCharacter, float maximumStatePointIncrease, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.maximumStatePointIncrease += maximumStatePointIncrease;
        gameCharacterResultData.SetMaximumStatePoint( gameCharacterResultData.maximumStatePoint + maximumStatePointIncrease );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 降低當前負荷值。
    public BattleResultData AddGameCharacterResultData_StressValueDamageRecovered( GameCharacter gameCharacter, float stressValueDamageRecovered, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStressBreakStatus())
        {
            gameCharacterResultData.SetCurrentStressValue( gameCharacterResultData.currentStressValue - stressValueDamageRecovered );
        }

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 以太值消耗
    public BattleResultData AddGameCharacterResultData_StatePointCost( GameCharacter gameCharacter, float statePointCost, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStateBreakStatus())
        {
            gameCharacterResultData.statePointCost += statePointCost;
            gameCharacterResultData.SetCurrentStatePoint( gameCharacterResultData.currentStatePoint - statePointCost );
        }

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 以太值傷害
    public BattleResultData AddGameCharacterResultData_StatePointDamage( GameCharacter gameCharacter, float statePointDamage, bool isBreakStatusAvailable, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStateBreakStatus())
        {
            gameCharacterResultData.statePointDamage += statePointDamage;
            gameCharacterResultData.SetCurrentStatePoint( gameCharacterResultData.currentStatePoint - statePointDamage );

            if (isBreakStatusAvailable && gameCharacterResultData.currentStatePoint < 0)
            {
                // 陷入以太崩潰狀態。
                gameCharacterResultData.stateBreakStatusRemainingATLs = 1;
                BattleLogicManagerV2.OnGameCharacterBeingInBreakStatus( gameCharacter );
            }
        }

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 負荷值傷害
    public BattleResultData AddGameCharacterResultData_StressValueDamage( GameCharacter gameCharacter, float stressValueDamage, bool isBreakStatusAvailable, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        if (!gameCharacterResultData.IsInStressBreakStatus())
        {
            gameCharacterResultData.stressValueDamage += stressValueDamage;
            gameCharacterResultData.SetCurrentStressValue( gameCharacterResultData.currentStressValue + stressValueDamage );

            if (gameCharacterResultData.currentStressValue >= gameCharacterResultData.maximumStressValue)
            {
                if (isBreakStatusAvailable)
                {
                    // 陷入負荷崩潰狀態。
                    gameCharacterResultData.stressBreakStatusRemainingATLs = 1;
                    BattleLogicManagerV2.OnGameCharacterBeingInBreakStatus( gameCharacter );
                }
                else
                {
                    // 由於不會陷入崩潰狀態，該角色的負荷值只能達到最高 99% 而已。
                    float _stressValueReduction = gameCharacterResultData.currentStressValue - ( gameCharacterResultData.maximumStressValue - 1 );
                    gameCharacterResultData.stressValueDamage -= _stressValueReduction;
                    gameCharacterResultData.currentStressValue -= _stressValueReduction;
                }
            }
        }

        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // HP 傷害（實傷）
    public BattleResultData AddGameCharacterResultData_ActualHealthPointDamage( GameCharacter gameCharacter, float actualHealthPointDamage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.actualHealthPointDamage += actualHealthPointDamage;
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint - actualHealthPointDamage, false );
        gameCharacterResultData.SetVirtualHealthPoint( gameCharacterResultData.virtualHealthPoint - actualHealthPointDamage );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // HP 傷害（虛傷）
    public BattleResultData AddGameCharacterResultData_VirtualHealthPointDamage( GameCharacter gameCharacter, float virtualHealthPointDamage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.virtualHealthPointDamage += virtualHealthPointDamage;
        gameCharacterResultData.SetCurrentHealthPoint( gameCharacterResultData.currentHealthPoint - virtualHealthPointDamage, true );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 更新能量殘響的 ATL 。
    public BattleResultData AddGameCharacterResultData_RenewedEnergyMarkerATLs( GameCharacter gameCharacter, int renewedEnergyMarkerATLs, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.energyMarkerRemainingATLs = renewedEnergyMarkerATLs;
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 消去能量殘響。
    public BattleResultData AddGameCharacterResultData_RemoveEnergyMarker( GameCharacter gameCharacter, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.energyMarkerRemainingATLs = 0;
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 當前以太值回復至最大以太值的100%。
    public BattleResultData AddGameCharacterResultData_RestoreCurrentStatePoint( GameCharacter gameCharacter, float restorationPercentage, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetCurrentStatePoint( gameCharacterResultData.maximumStatePoint * Mathf.Clamp01( restorationPercentage ) );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 尚未回復的虛傷部分全數轉化為實傷。
    public BattleResultData AddGameCharacterResultData_ConvertAllVirtualDamageToActualDamage( GameCharacter gameCharacter, out BattleResultData_GameCharacter gameCharacterResultData )
    {
        gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        gameCharacterResultData.SetVirtualHealthPoint( gameCharacterResultData.currentHealthPoint );
        AddNewElementIntoGameCharacterResultDataList( gameCharacterResultData, _isNewElement );
        return this;
    }

    // 更新狀態。
    public BattleResultData_GameCharacter AddGameCharacterResultData( GameCharacter gameCharacter,
                                                                      int stateBreakStatusRemainingATLs = 0, float maximumStatePoint = 0.0f, float currentStatePoint = 0.0f,
                                                                      int stressBreakStatusRemainingATLs = 0, float currentStressValue = 0.0f )
    {
        BattleResultData_GameCharacter _gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        // 以太
        _gameCharacterResultData.stateBreakStatusRemainingATLs = stateBreakStatusRemainingATLs;
        _gameCharacterResultData.SetMaximumStatePoint( maximumStatePoint );
        _gameCharacterResultData.SetCurrentStatePoint( currentStatePoint );

        // 負荷
        _gameCharacterResultData.stressBreakStatusRemainingATLs = stressBreakStatusRemainingATLs;
        _gameCharacterResultData.SetCurrentStressValue( currentStressValue );

        AddNewElementIntoGameCharacterResultDataList( _gameCharacterResultData, _isNewElement );
        return _gameCharacterResultData;
    }

    public BattleResultData_GameCharacter GetGameCharacterResultData( GameCharacter gameCharacter )
    {
        return GetGameCharacterResultData( gameCharacter, out _ );
    }

    public BattleResultData_GameCharacter GetGameCharacterResultData( GameCharacter gameCharacter, out bool isNewElement )
    {
        isNewElement = false;

        BattleResultData_GameCharacter _gameCharacterResultData = gameCharacterResultDataList.FirstOrDefault( p => p.gameCharacter == gameCharacter );

        if (_gameCharacterResultData == null)
        {
            isNewElement = true;

            _gameCharacterResultData = new BattleResultData_GameCharacter()
            {
                gameCharacter = gameCharacter,
                maximumHealthPoint = gameCharacter.GetMaximumHealthPoint(),
                currentHealthPoint = gameCharacter.GetCurrentHealthPoint(),
                virtualHealthPoint = gameCharacter.GetVirtualHealthPoint(),
                originalStatePoint = gameCharacter.GetOriginalStatePoint(),
                maximumStatePoint = gameCharacter.GetMaximumStatePoint(),
                minimumStatePoint = gameCharacter.GetMinimumStatePoint(),
                currentStatePoint = gameCharacter.GetCurrentStatePoint(),
                maximumStressValue = gameCharacter.GetMaximumStressValue(),
                currentStressValue = gameCharacter.GetCurrentStressValue(),
                stateBreakStatusRemainingATLs = gameCharacter.GetStateBreakStatusRemainingATLs(),
                stressBreakStatusRemainingATLs = gameCharacter.GetStressBreakStatusRemainingATLs(),
                energyMarkerRemainingATLs = gameCharacter.GetEnergyMarkerRemainingATLs()
            };
        }

        gameCharacter.SetTemporaryBattleResultData( _gameCharacterResultData );

        return _gameCharacterResultData;
    }
}
