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
            return ( this.stateBreakStatusRemainingATLs > 0 || this.stressBreakStatusRemainingATLs > 0 );
        }

        public bool HasEnergyMarker()
        {
            return ( this.energyMarkerRemainingATLs > 0 );
        }

        public void SetCurrentHealthPoint( float value, bool needToUpdateVirtualHealthPoint )
        {
            this.currentHealthPoint = Mathf.Clamp( value, 0.0f, this.maximumHealthPoint );

            if (needToUpdateVirtualHealthPoint)
            {
                UpdateVirtualHealthPoint();
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
    }

    public BattleResultData_GameCharacter AddGameCharacterResultData( GameCharacter gameCharacter,
                                                                      float virtualHealthPointDamageRecovered = 0.0f, float stressValueDamageRecovered = 0.0f, bool isCurrentStatePointFullyRestored = false,
                                                                      float statePointCost = 0.0f, float maximumStatePointIncrease = 0.0f, float maximumStatePointDecrease = 0.0f,
                                                                      float actualHealthPointDamage = 0.0f, float virtualHealthPointDamage = 0.0f, float statePointDamage = 0.0f, float stressValueDamage = 0.0f,
                                                                      bool isBreakStatusAvailable = false, int renewedEnergyMarkerATLs = 0, bool willRemoveEnergyMarker = false )
    {
        BattleResultData_GameCharacter _gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        // 回復受到的“虛傷”。
        if (virtualHealthPointDamageRecovered > 0)
        {
            _gameCharacterResultData.SetCurrentHealthPoint( _gameCharacterResultData.currentHealthPoint + virtualHealthPointDamageRecovered, true );
        }

        // 降低最大以太值。
        if (maximumStatePointDecrease > 0)
        {
            _gameCharacterResultData.SetMaximumStatePoint( _gameCharacterResultData.maximumStatePoint - maximumStatePointDecrease );
        }

        // 提升最大以太值。
        if (maximumStatePointIncrease > 0)
        {
            _gameCharacterResultData.SetMaximumStatePoint( _gameCharacterResultData.maximumStatePoint + maximumStatePointIncrease );
        }

        // 降低當前負荷值。
        if (stressValueDamageRecovered > 0)
        {
            _gameCharacterResultData.SetCurrentStressValue( _gameCharacterResultData.currentStressValue - stressValueDamageRecovered );
        }

        // 以太值消耗
        if (statePointCost > 0)
        {
            _gameCharacterResultData.statePointCost += statePointCost;
            _gameCharacterResultData.SetCurrentStatePoint( _gameCharacterResultData.currentStatePoint - statePointCost );
        }

        // 最大以太值提升
        if (maximumStatePointIncrease > 0)
        {
            _gameCharacterResultData.maximumStatePointIncrease += maximumStatePointIncrease;
            _gameCharacterResultData.maximumStatePoint += maximumStatePointIncrease;
        }

        // 以太值傷害
        if (statePointDamage > 0)
        {
            _gameCharacterResultData.statePointDamage += statePointDamage;
            _gameCharacterResultData.SetCurrentStatePoint( _gameCharacterResultData.currentStatePoint - statePointDamage );

            if (_gameCharacterResultData.currentStatePoint < 0)
            {
                if (isBreakStatusAvailable)
                {
                    // 陷入以太崩潰狀態。
                    _gameCharacterResultData.stateBreakStatusRemainingATLs = 1;
                }
            }
        }

        // 負荷值傷害
        if (stressValueDamage > 0)
        {
            _gameCharacterResultData.stressValueDamage += stressValueDamage;
            _gameCharacterResultData.SetCurrentStressValue( _gameCharacterResultData.currentStressValue + stressValueDamage );

            if (_gameCharacterResultData.currentStressValue >= _gameCharacterResultData.maximumStressValue)
            {
                if (isBreakStatusAvailable)
                {
                    // 陷入負荷崩潰狀態。
                    _gameCharacterResultData.stressBreakStatusRemainingATLs = 1;
                }
                else
                {
                    // 由於不會陷入崩潰狀態，該角色的負荷值只能達到最高 99% 而已。
                    float _stressValueReduction = _gameCharacterResultData.currentStressValue - ( _gameCharacterResultData.maximumStressValue - 1 );
                    _gameCharacterResultData.stressValueDamage -= _stressValueReduction;
                    _gameCharacterResultData.currentStressValue -= _stressValueReduction;
                }
            }
        }

        // 實傷
        if (actualHealthPointDamage > 0)
        {
            _gameCharacterResultData.actualHealthPointDamage += actualHealthPointDamage;
            _gameCharacterResultData.SetCurrentHealthPoint( _gameCharacterResultData.currentHealthPoint - actualHealthPointDamage, false );
            _gameCharacterResultData.SetVirtualHealthPoint( _gameCharacterResultData.virtualHealthPoint - actualHealthPointDamage );
        }

        // 虛傷
        if (virtualHealthPointDamage > 0)
        {
            _gameCharacterResultData.virtualHealthPointDamage += virtualHealthPointDamage;
            _gameCharacterResultData.SetCurrentHealthPoint( _gameCharacterResultData.currentHealthPoint - virtualHealthPointDamage, true );
        }

        if (_gameCharacterResultData.currentHealthPoint <= 0)
        {
            _gameCharacterResultData.currentHealthPoint = 0;
            _gameCharacterResultData.isDead = true;
        }

        // 更新能量殘響的 ATL 。
        if (renewedEnergyMarkerATLs > 0)
        {
            _gameCharacterResultData.energyMarkerRemainingATLs = renewedEnergyMarkerATLs;
        }

        // 消去能量殘響。
        if (willRemoveEnergyMarker)
        {
            _gameCharacterResultData.energyMarkerRemainingATLs = 0;
        }

        // 當前以太值回復至最大以太值的100%。
        if (isCurrentStatePointFullyRestored)
        {
            _gameCharacterResultData.SetCurrentStatePoint( _gameCharacterResultData.maximumStatePoint );
        }

        if (_isNewElement)
        {
            gameCharacterResultDataList.Add( _gameCharacterResultData );
        }

        return _gameCharacterResultData;
    }

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

        if (_isNewElement)
        {
            gameCharacterResultDataList.Add( _gameCharacterResultData );
        }

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
