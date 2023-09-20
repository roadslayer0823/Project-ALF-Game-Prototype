using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATLSlotListPanel : MonoBehaviour
{
    [SerializeField] private ATLSlot[] theATLSlots = new ATLSlot[ 0 ];

    private BattleFlowATL[] battleFlowATLs;

    public void Show( BattleFlowATL[] flowATLs, Action onSkillSlotSwipedCallback, Action onATLSlotExecutedCallback )
    {
        for (int i = 0; i < theATLSlots.Length; i++)
        {
            ATLSlot _altSlot = theATLSlots[ i ];

            if (i < flowATLs.Length)
            {
                BattleFlowATL _flowATL = flowATLs[ i ];
                _flowATL.SetATLSlot( _altSlot );
                _flowATL.SetIsATLSlotExecuted( false );
                _altSlot.Initialize( onSkillSlotSwipedCallback, onATLSlotExecutedCallback );
                _altSlot.Show( _flowATL );
            }
            else
            {
                _altSlot.Hide();
            }
        }

        this.battleFlowATLs = flowATLs;
        OnSkillSlotUpdated();

        base.gameObject.SetActive( true );
    }

    public void Hide()
    {
        base.gameObject.SetActive( false );
    }

    public void OnSkillSlotUpdated()
    {
        Dictionary<GameCharacter,int> _characterSkillCounterDictionary = new Dictionary<GameCharacter,int>();

        for (int i = 0; i < this.battleFlowATLs.Length; i++)
        {
            BattleFlowATL _battleFlowATL = this.battleFlowATLs[ i ];
            GameCharacter _character = _battleFlowATL.GetSelectedCharacter();

            if (_character is PlayerCharacter)
            {
                if (_battleFlowATL.GetIsATLSlotExecuted())
                {
                    _battleFlowATL.SetSkillSlotSwipedManually(true);
                    continue;
                }
            }

            int _totalActiveSkills = _character.GetSelectedActiveSkillList().Count;
            int _skillCounter = 0;
            if (_characterSkillCounterDictionary.ContainsKey( _character ))
            {
                _skillCounter = _characterSkillCounterDictionary[ _character ] % _totalActiveSkills;
                _characterSkillCounterDictionary[ _character ] = _skillCounter + 1;
            }
            else
            {
                _characterSkillCounterDictionary.Add( _character, 1 );
            }

            _battleFlowATL.SetSelectedSkill( _battleFlowATL.GetSelectedCharacter().GetSelectedActiveSkillList()[ _skillCounter ] );
            _battleFlowATL.UpdateATLSlotInfo();
        }
    }
}
