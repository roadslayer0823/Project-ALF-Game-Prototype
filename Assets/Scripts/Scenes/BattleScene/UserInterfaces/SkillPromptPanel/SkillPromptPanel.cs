using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillPromptPanel : MonoBehaviour
{
    [SerializeField] private Animator speedEffectAnimator = null;
    [SerializeField] private Animator strengthEffectAnimator = null;

    [SerializeField] private GameObject skillNameGO= null;
    [SerializeField] private TextMeshPro skillNameText = null;
    [SerializeField] private SpriteRenderer speedEffect = null;
    [SerializeField] private SpriteRenderer strengthEffect = null;
    [SerializeField] private SpriteRenderer background = null;

    public void Show(CharacterSkill characterSkill)
    {
        if (characterSkill == null)
        {
            return;
        }

        this.speedEffect.gameObject.SetActive(true);
        this.strengthEffect.gameObject.SetActive(true);
        this.skillNameText.SetText(characterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName);

        ResizeBackgroundBasedOnText();

        int _speed = characterSkill.GetCharacterSubskillData().GetSubskillData().Speed;
        int _strength = characterSkill.GetCharacterSubskillData().GetSubskillData().Strength;

        if (_speed == 3)
        {
            this.speedEffectAnimator.Play("Speed");
        }
        else if (_speed == 4)
        {
            this.speedEffectAnimator.Play("GodSpeed");
        }

        if (_strength == 2)
        {
            this.strengthEffectAnimator.Play("Strength_1");
        }
        else if (_strength == 3)
        {
            this.strengthEffectAnimator.Play("Strength_2");
        }
    }

    private void Hide()
    {
        this.skillNameGO.SetActive(false);
        this.speedEffect.gameObject.SetActive(false);
        this.strengthEffect.gameObject.SetActive(false);
    }

    private void ResizeBackgroundBasedOnText()
    {
        float _skillNameTextWidth = this.skillNameText.GetPreferredValues().x; // get the text width
        Vector2 _backgroundPosition = this.background.transform.position; // get the background position

        this.background.transform.position = new Vector2(-_skillNameTextWidth / 2, _backgroundPosition.y); // move it to the left (the starting point of the text)
        this.background.size = this.skillNameText.GetPreferredValues(); // resize it same with the text
    }

    public void OnSkillPromptPanelAnimationTriggered(string parameterValue)
    {
        if (string.Equals(parameterValue, "ShowSkillName"))
        {
            this.skillNameGO.SetActive(true);
            Debug.Log("Show skill name");
        }
        else
        {
            Hide();
            Debug.Log("Hide skill name");
        }
    }
}
