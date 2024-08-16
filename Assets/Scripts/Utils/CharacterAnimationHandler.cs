using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using AnimationData = DatabaseManager.AnimationData;

public class CharacterAnimationHandler : MonoBehaviour
{
    private string codeType;
    private AnimationData.CodeType selectedCodeType;

    [SerializeField] private Toggle isFront;
    [SerializeField] private Toggle needToRecord;
    [SerializeField] private TMP_InputField subSkillId;
    [SerializeField] private TMP_InputField type;
    [SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private Transform[] containers;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Animator skillEffectBackAnimator;
    [SerializeField] private Animator skillEffectFrontAnimator;
    [SerializeField] private Animator visualEffectBackAnimator;
    [SerializeField] private Animator visualEffectFrontAnimator;

    [SerializeField] private AudioDatabase audioDatabase;

    private AnimatorOverrideController playerAnimatorOverrideController;
    private AnimatorOverrideController skillEffectBackAnimatorOverrideController;
    private AnimatorOverrideController skillEffectFrontAnimatorOverrideController;
    private AnimatorOverrideController visualEffectBackAnimatorOverrideController;
    private AnimatorOverrideController visualEffectFrontAnimatorOverrideController;
    private AnimationData.CodeType codeTypeForLastATL;

    private float actionAnimationLength = 0;
    private float effectAnimationLength = 0;
    private float audioLength = 0;

    void Awake()
    {
        this.playerAnimatorOverrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController)
        {
            name = "PlayerOverrideAnimator"
        };
        this.skillEffectBackAnimatorOverrideController = new AnimatorOverrideController(skillEffectBackAnimator.runtimeAnimatorController)
        {
            name = "SkillEffect_BackOverrideAnimator"
        };
        this.skillEffectFrontAnimatorOverrideController = new AnimatorOverrideController(skillEffectFrontAnimator.runtimeAnimatorController)
        {
            name = "SkillEffect_FrontOverrideAnimator"
        };
        this.visualEffectBackAnimatorOverrideController = new AnimatorOverrideController(visualEffectBackAnimator.runtimeAnimatorController)
        {
            name = "VisualEffect_BackOverrideAnimator"
        };
        this.visualEffectFrontAnimatorOverrideController = new AnimatorOverrideController(visualEffectFrontAnimator.runtimeAnimatorController)
        {
            name = "VisualEffect_FrontOverrideAnimator"
        };

        this.playerAnimator.runtimeAnimatorController = playerAnimatorOverrideController;
        this.skillEffectBackAnimator.runtimeAnimatorController = skillEffectBackAnimatorOverrideController;
        this.skillEffectFrontAnimator.runtimeAnimatorController = skillEffectFrontAnimatorOverrideController;
        this.visualEffectBackAnimator.runtimeAnimatorController = visualEffectBackAnimatorOverrideController;
        this.visualEffectFrontAnimator.runtimeAnimatorController = visualEffectFrontAnimatorOverrideController;

        AudioManager.Instance.SetUpAudioDatabase(this.audioDatabase);

        List<string> values = Enum.GetValues(typeof(AnimationData.CodeType)).Cast<AnimationData.CodeType>().Select(v => v.ToString()).ToList();
        this.dropdown.AddOptions(values);
    }

    public void FlipContainer()
    {
        for(int i = 0; i < this.containers.Length; i ++)
        {
            containers[i].transform.localScale.Set(-1, 0, 0);
        }
    }

    public void FlipVisualEffectContainer()
    {
        containers[0].transform.localScale.Set(-1, 0, 0);
        containers[4].transform.localScale.Set(-1, 0, 0);
    }

    public AnimationData.CodeType GetLastATLCodeType()
    {
        return this.codeTypeForLastATL;
    }

    public bool CheckIfSameAsLastATLCodeType(string[] lastCode)
    {
        string codeTypeForLastATL = this.codeTypeForLastATL.ToString();
        foreach(string subString in lastCode)
        {
            if (codeTypeForLastATL.Contains(subString))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfSameAsLastATLCodeType(string lastCode)
    {
        return CheckIfSameAsLastATLCodeType(new string[] { lastCode });
    }

    public void LoadAndPlayAnimation(bool isSkillEffectFront, bool needToRecord, AnimationData.CodeType codeType, string subskillId = "", int type = 0)
    {
        AnimationClip _animationClip = null;
        AnimationData _animationData = DatabaseManager.Instance.GetAnimationData(codeType,subskillId,type);
        actionAnimationLength = 0;
        effectAnimationLength = 0;
        audioLength = 0;

        if(_animationData == null)
        {
            Debug.Log("Failed to get animation data");
            return;
        }

        //action
        string[] _actionClipArray = _animationData.ActionsArray;
        if(_actionClipArray.Length > 0)
        {
           for(int i = 0; i < _actionClipArray.Length;i++)
           {
                _animationClip = Resources.Load<AnimationClip>("Animations/Battle/Actions/" + _actionClipArray[i]);
                this.playerAnimatorOverrideController["Animation_"+ i] = _animationClip;
                actionAnimationLength += _animationClip.length;
           }
            this.playerAnimator.SetTrigger("trigger");
        }

        //skill effect 
        string[] _effectClipArray = _animationData.EffectsArray;
        if (_effectClipArray != null && _effectClipArray.Length > 0)
        {
            for (int i = 0; i < _effectClipArray.Length; i++)
            {
                _animationClip = Resources.Load<AnimationClip>("Animations/Battle/SkillEffects/" + _effectClipArray[i]);
                if(isSkillEffectFront)
                {
                    this.skillEffectFrontAnimatorOverrideController["Animation_" + i] = _animationClip;
                }
                else
                {
                    this.skillEffectBackAnimatorOverrideController["Animation_" + i] = _animationClip;
                }
            }

            if (isSkillEffectFront)
            {
                this.skillEffectFrontAnimator.SetTrigger("trigger");
            }
            else
            {
                this.skillEffectBackAnimator.SetTrigger("trigger");
            }
            effectAnimationLength += _animationClip.length;
        }

        //audio
        string[] _audioArray = _animationData.AudiosArray;
        if (_audioArray != null && _audioArray.Length > 0)
        {
            AudioManager.Instance.PlaySoundEffect(_audioArray[0]);
            AudioDatabase.AudioData _audioData = this.audioDatabase.GetAudioDataById(_audioArray[0]);
            audioLength += _audioData.GetClip().length;
        }


        if(needToRecord)
        {
            this.codeTypeForLastATL = codeType;
        }
    }

    public void LoadAndPlayVisualEffect(string visualEffectName, string visualEffectAudioId)
    {
        // visual effect
        AnimationClip _animationClip = Resources.Load<AnimationClip>("Animations/Battle/VisualEffects/" + visualEffectName);
        this.skillEffectBackAnimatorOverrideController["Animation_0"] = _animationClip;

        AudioManager.Instance.PlaySoundEffect(visualEffectAudioId);
    }

    public void OnClick()
    {
        this.selectedCodeType = (AnimationData.CodeType)Enum.Parse(typeof(AnimationData.CodeType), codeType);
        LoadAndPlayAnimation(this.isFront.isOn,this.needToRecord, this.selectedCodeType, this.subSkillId.text, int.Parse(this.type.text));

        Debug.Log("actionAnimationLength: " + actionAnimationLength + "\neffectAnimationLength: " + effectAnimationLength + "\naudioLength: " + audioLength); ;
        Debug.Log("GetAnimationClipTotalLength(): " + GetAnimationClipTotalLength());
    }

    // compare action, effect and audio length
    public float GetAnimationClipTotalLength()
    {
        if(actionAnimationLength > effectAnimationLength)
        {
            if(actionAnimationLength > audioLength)
            {
                return actionAnimationLength;
            }
            else
            {
                return audioLength;
            }
        }
        else if(effectAnimationLength > audioLength)
        {
            return effectAnimationLength;
        }
        else
        {
            return audioLength;
        }
    }

    public void OnValueChanged()
    {
        this.codeType = this.dropdown.options[dropdown.value].text;
        Debug.Log("Selected type: " + codeType);
    }
}
