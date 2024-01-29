using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Skill = DatabaseManager.Skill;

public class SkillInfoPanel : MonoBehaviour
{
    [SerializeField] private RectTransform skillInformation;

    [Header("SkillTabButtons")]
    [SerializeField] private Button attackSkillSelectionTabButton = null;
    [SerializeField] private Button repulseSkillSelectionTabButton = null;
    [SerializeField] private Button derivedSkillSelectionTabButton = null;
    [SerializeField] private Button counterSkillSelectionTabButton = null;
    [SerializeField] private Image attackSkillSelectionTabBackgroundImage = null;
    [SerializeField] private Image repulseSkillSelectionTabBackgroundImage = null;
    [SerializeField] private Image derivedSkillSelectionTabBackgroundImage = null;
    [SerializeField] private Image counterSkillSelectionTabBackgroundImage = null;

    [Header("SkillInfoDetails")]
    [SerializeField] private Image skillPortrait;
    [SerializeField] private TextMeshProUGUI skillType;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI attackDamageValue;
    [SerializeField] private TextMeshProUGUI statePointCostTitle;
    [SerializeField] private TextMeshProUGUI statePointCostValue;
    [SerializeField] private TextMeshProUGUI strengthValue;
    [SerializeField] private TextMeshProUGUI accuracyValue;
    [SerializeField] private TextMeshProUGUI evasionValue;
    [SerializeField] private TextMeshProUGUI stressDamageValue;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI evasionStressValue;
    [SerializeField] private TextMeshProUGUI statePointDamageValue;
    [SerializeField] private TextMeshProUGUI speedValue;
    [SerializeField] private RectTransform tagListRectTransform;
    [SerializeField] private TextMeshProUGUI tagRange;
    [SerializeField] private TextMeshProUGUI tagEffectType;

    [Header("ObservedSkillInfo")]
    [SerializeField] private GameObject observedSkillInfo = null;
    [SerializeField] private ObservedSkillBox observedSkillBoxPrefab = null;
    [SerializeField] private RectTransform observedSkillListContent = null;
    [SerializeField] private TextMeshProUGUI noRecordFoundText = null;
}
