using System.Collections.Generic;
using System.Linq;

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

        // 改變參數（技能發動時）
        public float statePointCost = 0.0f;
        public float maximumStatePointIncrease = 0.0f;

        // 改變參數（命中目標時）
        public float actualHealthPointDamage = 0.0f;
        public float virtualHealthPointDamage = 0.0f;
        public float statePointDamage = 0.0f;
        public float stressValueDamage = 0.0f;

        // 崩潰狀態
        public int stateBreakStatusRemainingATLs = 0;  // 以太崩潰維持值 (ATL)
        public int stressBreakStatusRemainingATLs = 0; // 負荷崩潰維持值 (ATL)

        // 能量殘響
        public int energyMarkerRemainingATLs = 0;

        public bool isDead = false;

        public bool IsInBreakStatus()
        {
            return ( stateBreakStatusRemainingATLs > 0 || stressBreakStatusRemainingATLs > 0 );
        }
    }

    public void AddGameCharacterResultData( GameCharacter gameCharacter,
                                            float statePointCost = 0.0f, float maximumStatePointIncrease = 0.0f,
                                            float actualHealthPointDamage = 0.0f, float virtualHealthPointDamage = 0.0f, float statePointDamage = 0.0f, float stressValueDamage = 0.0f,
                                            bool isBreakStatusAvailable = false, int renewedEnergyMarkerATLs = 0, bool willRemoveEnergyMarker = false )
    {
        BattleResultData_GameCharacter _gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );
        float _minimumStatePoint = gameCharacter.GetMinimumStatePoint();

        if (statePointCost > 0)
        {
            // 以太值消耗
            _gameCharacterResultData.statePointCost += statePointCost;
            _gameCharacterResultData.currentStatePoint -= statePointCost;
        }

        if (maximumStatePointIncrease > 0)
        {
            // 最大以太值提升
            _gameCharacterResultData.maximumStatePointIncrease += maximumStatePointIncrease;
            _gameCharacterResultData.maximumStatePoint += maximumStatePointIncrease;
        }

        if (actualHealthPointDamage > 0)
        {
            // 實傷
            _gameCharacterResultData.actualHealthPointDamage += actualHealthPointDamage;
            _gameCharacterResultData.currentHealthPoint -= actualHealthPointDamage;
            _gameCharacterResultData.virtualHealthPoint -= actualHealthPointDamage;
        }

        if (virtualHealthPointDamage > 0)
        {
            // 虛傷
            _gameCharacterResultData.virtualHealthPointDamage += virtualHealthPointDamage;
            _gameCharacterResultData.currentHealthPoint -= virtualHealthPointDamage;
        }

        if (statePointDamage > 0)
        {
            // 以太值傷害
            _gameCharacterResultData.statePointDamage += statePointDamage;
            _gameCharacterResultData.currentStatePoint -= statePointDamage;

            if (_gameCharacterResultData.currentStatePoint < 0)
            {
                if (isBreakStatusAvailable)
                {
                    // 陷入以太崩潰狀態。
                    _gameCharacterResultData.stateBreakStatusRemainingATLs = 1;
                }
            }
        }

        if (stressValueDamage > 0)
        {
            // 負荷值傷害
            _gameCharacterResultData.stressValueDamage += stressValueDamage;
            _gameCharacterResultData.currentStressValue += stressValueDamage;

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

        if (_gameCharacterResultData.currentHealthPoint <= 0)
        {
            _gameCharacterResultData.currentHealthPoint = 0;
            _gameCharacterResultData.isDead = true;
        }

        if (_gameCharacterResultData.currentStatePoint < _minimumStatePoint)
        {
            _gameCharacterResultData.currentStatePoint = _minimumStatePoint;
        }

        if (_gameCharacterResultData.currentStressValue >= _gameCharacterResultData.maximumStressValue)
        {
            _gameCharacterResultData.currentStressValue = _gameCharacterResultData.maximumStressValue;
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

        if (_isNewElement)
        {
            gameCharacterResultDataList.Add( _gameCharacterResultData );
        }
    }

    public void AddGameCharacterResultData( GameCharacter gameCharacter,
                                            int stateBreakStatusRemainingATLs = 0, float maximumStatePoint = 0.0f, float currentStatePoint = 0.0f,
                                            int stressBreakStatusRemainingATLs = 0, float currentStressValue = 0.0f )
    {
        BattleResultData_GameCharacter _gameCharacterResultData = GetGameCharacterResultData( gameCharacter, out bool _isNewElement );

        // 以太
        _gameCharacterResultData.stateBreakStatusRemainingATLs = stateBreakStatusRemainingATLs;
        _gameCharacterResultData.maximumStatePoint = maximumStatePoint;
        _gameCharacterResultData.currentStatePoint = currentStatePoint;

        // 負荷
        _gameCharacterResultData.stressBreakStatusRemainingATLs = stressBreakStatusRemainingATLs;
        _gameCharacterResultData.currentStressValue = currentStressValue;

        if (_isNewElement)
        {
            gameCharacterResultDataList.Add( _gameCharacterResultData );
        }
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
                currentStressValue = gameCharacter.GetCurrentStressValue()
            };
        }

        return _gameCharacterResultData;
    }
}
