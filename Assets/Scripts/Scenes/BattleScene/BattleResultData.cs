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
        public bool isEnteringIntoBreakStatus = false;
        public bool isBreakStatusCausedByStatePoint = false;
        public bool isBreakStatusCausedByStressValue = false;
        public int breakStatusAtlNumber = 0;

        // 能量殘響
        public bool hasEnergyMarker = false;
        public int energyMarkerRemainingATLs = 0;

        public bool isDead = false;
    }

    public void AddGameCharacterResultData( GameCharacter gameCharacter,
                                            float statePointCost = 0.0f, float maximumStatePointIncrease = 0.0f,
                                            float actualHealthPointDamage = 0.0f, float virtualHealthPointDamage = 0.0f, float statePointDamage = 0.0f, float stressValueDamage = 0.0f,
                                            bool isBreakStatusAvailable = false, bool hasEnergyMarker = false, int energyMarkerRemainingATLs = 0 )
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

            if (isBreakStatusAvailable)
            {
                if (_gameCharacterResultData.currentStatePoint < 0)
                {
                    // 以太崩潰
                    _gameCharacterResultData.isEnteringIntoBreakStatus = true;
                    _gameCharacterResultData.isBreakStatusCausedByStatePoint = true;
                    _gameCharacterResultData.breakStatusAtlNumber = 1;
                }
            }
        }

        if (stressValueDamage > 0)
        {
            // 負荷值傷害
            _gameCharacterResultData.stressValueDamage += stressValueDamage;
            _gameCharacterResultData.currentStressValue += stressValueDamage;

            if (isBreakStatusAvailable)
            {
                if (_gameCharacterResultData.currentStressValue >= _gameCharacterResultData.maximumStressValue)
                {
                    // 負荷崩潰
                    _gameCharacterResultData.isEnteringIntoBreakStatus = true;
                    _gameCharacterResultData.isBreakStatusCausedByStressValue = true;
                    _gameCharacterResultData.breakStatusAtlNumber = 1;
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

        if (hasEnergyMarker)
        {
            // 能量殘響
            _gameCharacterResultData.hasEnergyMarker = hasEnergyMarker;
            _gameCharacterResultData.energyMarkerRemainingATLs = energyMarkerRemainingATLs;
        }

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
                currentStressValue = gameCharacter.GetCurrentStressValue(),
                isBreakStatusCausedByStatePoint = gameCharacter.GetIsBreakStatusCausedByStatePoint(),
                isBreakStatusCausedByStressValue = gameCharacter.GetIsBreakStatusCausedByStressValue()
            };
        }

        return _gameCharacterResultData;
    }
}
