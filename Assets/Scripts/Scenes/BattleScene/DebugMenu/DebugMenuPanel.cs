using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class DebugMenuPanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private EnemyCharacter enemyCharacter = null;
    [SerializeField] private BattleGameManager battleGameManager = null;

    [Header("References")]
    [SerializeField] private GameObject container = null;
    [SerializeField] private TMP_Dropdown playerStatList = null;
    [SerializeField] private TMP_Dropdown enemyStatList = null;
    [SerializeField] private TMP_InputField playerStatValue = null;
    [SerializeField] private TMP_InputField enemyStatValue = null;
    [SerializeField] private TextMeshProUGUI enemyInputPlaceHolder = null;
    [SerializeField] private TextMeshProUGUI changedSkillInfo = null;
    [SerializeField] private TextMeshProUGUI playerInputPlaceHolder = null;

    //variable name
    private string selectedPlayerStat;
    private string selectedEnemyStat;
    private string newPlayerStatValue;
    private string newEnemyStatValue;

    private const string AUDIO_ID_CLICK = "click";

    //display function
    public void Start()
    {
        InitializeDropDowns(playerStatList);
        InitializeDropDowns(enemyStatList);
    }

    private void InitializeDropDowns(TMP_Dropdown characterList)
    {
        characterList.ClearOptions();

        var stateNames = new List<string>
        {
            "參數",
            "虛傷",
            "當前技能",
            "當前先手回合",
            "當前以太值",
            "當前負荷值",
            "當前生命值"
        };

        characterList.AddOptions(stateNames);
    }


    public void Show()
    {
        this.container.SetActive(true);
    }

    public void Hide()
    {
        this.container.SetActive(false);
        changedSkillInfo.text = null;
    }

    public void ClickToShow()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        Show();
    }

    public void ClickToHide()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        Hide();
    }

    //state value declaration
    public void OnPlayerStateListChange()
    {
        this.selectedPlayerStat = playerStatList.options[playerStatList.value].text;
        if (selectedPlayerStat == "當前技能")
        {
            playerInputPlaceHolder.text = "無法使用此功能";
        }
        else if(selectedPlayerStat == "當前先手回合")
        {
            playerInputPlaceHolder.text = "1=先手，2=後手";
        }
        else
        {
            playerInputPlaceHolder.text = "輸入數值";
        }
    }

    public void OnEnemyStateListChange()
    {
        this.selectedEnemyStat = enemyStatList.options[enemyStatList.value].text;
        if(selectedEnemyStat == "當前技能")
        {
            enemyInputPlaceHolder.text = "輸入格式:技能ID-技能等級,...";
        }
        else if (selectedEnemyStat == "當前先手回合")
        {
            enemyInputPlaceHolder.text = "1=先手，2=後手";
        }
        else
        {
            enemyInputPlaceHolder.text = "輸入數值";
        }
    }

    public void OnPlayerStateValueChange()
    {
        newPlayerStatValue = playerStatValue.text;
    }

    public void OnEnemyStateValueChange()
    {
        newEnemyStatValue = enemyStatValue.text;
    }

    public bool IsPlayerDropdownActive()
    {
        return playerStatList.gameObject.activeSelf;
    }

    public bool IsEnemyDropdownActive()
    {
        return enemyStatList.gameObject.activeSelf;
    }

    //state option
    public void ChangeStateValue(string statNames, string newStatValue, GameCharacter characterObject)
    {
        if (statNames == "當前以太值") //current state point
        {
            float value;
            if (float.TryParse(newStatValue, out value))
            {
                float _difference = value - characterObject.GetCurrentStatePoint();
                if (_difference > 0)
                {
                    characterObject.AddCurrentStatePoint(_difference);
                }
                else if (_difference < 0)
                {
                    characterObject.MinusCurrentStatePoint(Mathf.Abs(_difference), false);
                }
            }
        }
        else if (statNames == "當前負荷值")//current stress point
        {
            float value;
            if (float.TryParse(newStatValue, out value))
            {
                float _difference = value - characterObject.GetCurrentStressValue();
                if (value > 0)
                {
                    characterObject.AddCurrentStressValue(value);
                }
                if (value < 0)
                {
                    characterObject.MinusCurrentStressValue(Mathf.Abs(_difference));
                }
            }

        }
        else if (statNames == "虛傷")//current virtual value
        {
            float value;
            if (float.TryParse(newStatValue, out value))
            {
                if (characterObject.GetCurrentHealthPoint() >= characterObject.GetVirtualHealthPoint())
                {
                    characterObject.MinusCurrentHealthPoint(value);
                }
                else if (characterObject.GetCurrentHealthPoint() <= characterObject.GetVirtualHealthPoint())
                {
                    characterObject.AddCurrentHealthPoint(value);

                    if (characterObject.GetVirtualHealthPoint() == 0)
                    {
                        characterObject.AddVirtualHealthPoint(value);
                    }
                }
            }
        }
        else if (statNames == "當前生命值")//current health
        {
            float value;
            if (float.TryParse(newStatValue, out value))
            {
                float _difference = value - characterObject.GetCurrentHealthPoint();
                if (_difference > 0)
                {
                    characterObject.AddCurrentHealthPoint(_difference);
                    characterObject.AddVirtualHealthPoint(value - characterObject.GetVirtualHealthPoint());
                }
                else if (_difference < 0)
                {
                    characterObject.MinusCurrentHealthPoint(Mathf.Abs(_difference));
                    characterObject.ClearVirtualHealthPoint();
                }
            }
        }

        else if(statNames == "當前先手回合") //current turn
        {
            int value;
            if(int.TryParse(newStatValue, out value))
            {
                bool _isPlayerFirst = false;
                if (characterObject is PlayerCharacter)
                {
                    _isPlayerFirst = (value == 1);
                }
                else if(characterObject is EnemyCharacter)
                {
                    _isPlayerFirst = (value == 2);
                }
                battleGameManager.GetBattleFlowManager().SetIsPlayerFirst(_isPlayerFirst);
                battleGameManager.GetBattleUiManager().GetCharacterInfoPanel().ShowRoundInfoText(_isPlayerFirst);
            }
        }

        else if (statNames == "當前技能" && characterObject is EnemyCharacter)//current enemy skill
        {
            characterObject.GetSelectedActiveSkillList().Clear();
            characterObject.GetSelectedBackendSkillList().Clear();

            string inputString = newStatValue;
            string[] skillGroup = inputString.Split(new string[] { "," }, StringSplitOptions.None);
            List<string> changedSkill = new List<string>();

            foreach (string item in skillGroup)
            {
                string[] secondSplit = item.Split(new char[] { '-' });
                if (secondSplit.Length >= 2)
                {
                    string skillId = secondSplit[0];
                    string skillLevel = secondSplit[1];

                    Debug.Log("skillId: " + skillId);
                    Debug.Log("skilllevel: Level" + skillLevel);

                    CharacterSkill characterSkill = characterObject.GetSkillbySkillId(skillId);
                    characterSkill.SetSelectedSkillLevel(int.Parse(skillLevel));

                    CharacterSkill repulseSkill = null;
                    if (characterSkill.GetCharacterSubskillData().GetRepulseSkillList().Count > 0)
                    {
                        repulseSkill = characterSkill.GetCharacterSubskillData().GetRepulseSkillList()[0];
                    }

                    CharacterSkill derivedSkill = null;
                    if (characterSkill.GetCharacterSubskillData().GetDerivedSkillList().Count > 0)
                    {
                        derivedSkill = characterSkill.GetCharacterSubskillData().GetDerivedSkillList()[0];
                    }

                    if (repulseSkill != null)
                    {
                        characterSkill.GetCharacterSubskillData().SetSelectedRepulseSkill(repulseSkill);
                    }

                    if (derivedSkill != null)
                    {
                        characterSkill.GetCharacterSubskillData().SetSelectedDerivedSkill(derivedSkill);
                    }

                    characterObject.AddSelectedSkill(characterSkill);

                    changedSkill.Add(characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);
                }
            }
            string allChangedSkill = string.Join("\n", changedSkill);

            changedSkillInfo.text = allChangedSkill;
            Debug.Log(allChangedSkill);
            Debug.Log("skill no. : " + characterObject.GetSelectedActiveSkillList().Count);
            Debug.Log("Backend skill no. : " + characterObject.GetSelectedBackendSkillList().Count);
        }
    }

    //change state
    public void OnPlayerButtonClick()
    {
        ChangeStateValue(selectedPlayerStat, newPlayerStatValue, playerCharacter);
    }

    public void OnEnemyButtonClick()
    {
        ChangeStateValue(selectedEnemyStat, newEnemyStatValue, enemyCharacter);
    }
}
