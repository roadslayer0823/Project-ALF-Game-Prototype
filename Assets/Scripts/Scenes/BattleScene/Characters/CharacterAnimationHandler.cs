using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using AnimationData = DatabaseManager.AnimationData;
using RangeType = DatabaseManager.Subskill.RangeType;

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

    private GameCharacter gameCharacter;
    private AnimatorOverrideController playerAnimatorOverrideController;
    private AnimatorOverrideController skillEffectBackAnimatorOverrideController;
    private AnimatorOverrideController skillEffectFrontAnimatorOverrideController;
    private AnimatorOverrideController visualEffectBackAnimatorOverrideController;
    private AnimatorOverrideController visualEffectFrontAnimatorOverrideController;
    private AnimationData.CodeType codeTypeForLastATL;

    private float actionAnimationLength = 0;
    private float effectAnimationLength = 0;
    private float audioLength = 0;

    public void Initialize(GameCharacter gameCharacter)
    {
        this.gameCharacter = gameCharacter;

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

        if (this.dropdown != null)  
        {
            List<string> values = Enum.GetValues( typeof( AnimationData.CodeType ) ).Cast<AnimationData.CodeType>().Select( v => v.ToString() ).ToList();
            this.dropdown.AddOptions( values );
        }    
    }

    public void FlipContainer(bool isFlipped)
    {
        float scaleX = isFlipped ? -1 : 1;
        Vector3 newScale = new Vector3(scaleX, 1.0f, 1.0f);

        for(int i = 0; i < this.containers.Length; i ++)
        {
            this.containers[i].transform.localScale = newScale;
        }
    }

    public void FlipVisualEffectContainer(bool isFlipped)
    {
        float scaleX = isFlipped ? -1 : 1;
        Vector3 newScale = new Vector3(scaleX, 1.0f, 1.0f);
        this.containers[0].transform.localScale = newScale;
        this.containers[3].transform.localScale = newScale;
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

    public bool CheckIfAnyAnimationCodeTypeForCurrentSkillHasKeyword(string keyword)
    {
        List<string> _codeTypeList = new();
        bool _isPlayer = this.gameCharacter.GetIsPlayer();
        string _currentSubskillId = this.gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Id;
        int _animationType = AnimationParameterData.ConvertToAnimationType(this.gameCharacter);

        List<AnimationData> _animationDataList = DatabaseManager.Instance.GetAnimationDataList();
        for (int i = 0; i < _animationDataList.Count; i++)
        {
            AnimationData _animationData = _animationDataList[ i ];
            if (_animationData.IsPlayer == _isPlayer
                && _animationData.SubskillIdsArray.Contains( _currentSubskillId )
                && ( _animationData.Type != 0 || _animationData.Type == _animationType ))
            {
                _codeTypeList.Add( _animationData.CodeString );
            }
        }

        for (int i = 0; i < _codeTypeList.Count; i++)
        {
            if (_codeTypeList[i].Contains(keyword))
            {
                Debug.Log(_codeTypeList[i] + " Contain keyword: " + keyword);
                return true;
            }
        }

        return false;
    }

    public class AnimationParameterData
    {
        private bool isSkillEffectFront = false;
        private bool needToRecord = false;
        private AnimationData.CodeType codeType;
        private string subskillId = "";
        private int type = 0;

        public AnimationParameterData (bool isSkillEffectFront, bool needToRecord, AnimationData.CodeType codeType, string subskillId = "", int type = 0)
        {
            this.isSkillEffectFront = isSkillEffectFront;
            this.needToRecord = needToRecord;
            this.codeType = codeType;
            this.subskillId = subskillId;
            this.type = type;
        }

        public static int ConvertToAnimationType( GameCharacter gameCharacter )
        {
            RangeType _gameCharacterCurrentSkillRangeType = gameCharacter.GetCurrentSkillRangeType();

            if (_gameCharacterCurrentSkillRangeType == RangeType.melee)
            {
                return 1;
            }
            else if (_gameCharacterCurrentSkillRangeType == RangeType.ranged)
            {
                return 2;
            }

            return 0;
        }

        public bool GetIsSkillEffectFront()
        {
            return this.isSkillEffectFront;
        }

        public bool GetNeedToRecord()
        {
            return this.needToRecord;
        }

        public AnimationData.CodeType GetCodeType()
        {
            return this.codeType;
        }

        public string GetSubSkillId()
        {
            return this.subskillId;
        }

        public int GetDataType()
        {
            return this.type;
        }
    }

    public void LoadAndPlayAnimation(AnimationParameterData animationParameterData)
    {
        LoadAndPlayAnimation(animationParameterData.GetIsSkillEffectFront(),animationParameterData.GetNeedToRecord(),
            animationParameterData.GetCodeType(),animationParameterData.GetSubSkillId(),animationParameterData.GetDataType());
    }

    public void LoadAndPlayAnimation(bool isSkillEffectFront, bool needToRecord, AnimationData.CodeType codeType, string subskillId = "", int type = 0)
    {
        ResetAnimation();

        Debug.Log("codeType: " + codeType);
        Debug.Log("subskillId: " + subskillId);
        Debug.Log("type: " + type);
        AnimationClip _animationClip = null;
        AudioClip _audioClip = null;
        AnimationData _animationData = DatabaseManager.Instance.GetAnimationData(codeType,subskillId,type);
        actionAnimationLength = 0;
        effectAnimationLength = 0;
        audioLength = 0;

        if(_animationData == null)
        {
            Debug.Log("Failed to get animation data. Change type and load again");
            if (type == 1) type = 2;
            else if (type == 2) type = 1;
            _animationData = DatabaseManager.Instance.GetAnimationData(codeType, subskillId, type);
            Debug.Log("codeType: " + codeType);
            Debug.Log("subskillId: " + subskillId);
            Debug.Log("type: " + type);
            if (_animationData == null)
            {
                Debug.LogError("Even changed type still cannot found animation data from database");
            }
        }

        FlipContainer(_animationData.IsFlipped);

        for (int i = 0; i < 2; i++)
        {
            string _stateName = "Animation_" + i;
            this.playerAnimatorOverrideController[ _stateName ] = null;
            this.skillEffectFrontAnimatorOverrideController[ _stateName ] = null;
            this.skillEffectBackAnimatorOverrideController[ _stateName ] = null;

            string _parameterName = "animation_" + i;
            this.playerAnimator.SetBool( _parameterName, false );
            this.skillEffectFrontAnimator.SetBool( _parameterName, false );
            this.skillEffectBackAnimator.SetBool( _parameterName, false );
        }

        //action
        if(_animationData.ActionsArray != null)
        {
            string[] _actionClipArray = _animationData.ActionsArray;
            for (int i = 0; i < _actionClipArray.Length; i++)
            {
                Debug.Log("_actionClipArray[" + i + "]: " + _actionClipArray[i]);
                _animationClip = Resources.Load<AnimationClip>("Animations/Battle/Actions/" + _actionClipArray[i]);
                this.playerAnimatorOverrideController["Animation_" + i] = _animationClip;
                actionAnimationLength += _animationClip.length;

                // reset
                this.playerAnimator.SetBool("animation_" + i, i == (_actionClipArray.Length - 1));
            }
            this.playerAnimator.SetTrigger("trigger");
        }
        else
        {
            Debug.Log("ActionsArray is empty");
        }
        

        //skill effect
        if(_animationData.EffectsArray != null)
        {
            string[] _effectClipArray = _animationData.EffectsArray;
            Animator _skillEffectAnimator = isSkillEffectFront ? this.skillEffectFrontAnimator : this.skillEffectBackAnimator;
            for (int i = 0; i < _effectClipArray.Length; i++)
            {
                Debug.Log("_effectClipArray[" + i + "]: " + _effectClipArray[i]);
                _animationClip = Resources.Load<AnimationClip>("Animations/Battle/SkillEffects/" + _effectClipArray[i]);

                ( ( isSkillEffectFront ) ? this.skillEffectFrontAnimatorOverrideController
                                         : this.skillEffectBackAnimatorOverrideController )
                                         [ "Animation_" + i ] = _animationClip;

                effectAnimationLength += _animationClip.length;

                _skillEffectAnimator.SetBool("animation_" + i, i == _effectClipArray.Length - 1);
            }

            _skillEffectAnimator.SetBool( "reset", true );
            _skillEffectAnimator.SetTrigger("trigger");
        }
        else
        {
            Debug.Log("EffectsArray is empty");
        }
        

        //audio
        if(_animationData.AudiosArray != null)
        {
            string[] _audioArray = _animationData.AudiosArray;

            if (_audioArray.Length > 0)
            {
                _audioClip = Resources.Load<AudioClip>( "Audios/Battle/Audios/" + _audioArray[ 0 ] );
                AudioManager.Instance.PlaySoundEffect( _audioClip );
                Debug.Log("_audioArray: " + _audioArray[0]);
                audioLength += _audioClip.length;
            }
        }
        else
        {
            Debug.Log("AudiosArray is empty");
        }

        if (needToRecord)
        {
            this.codeTypeForLastATL = codeType;
        }
    }

    public class VisualEffectParameterData
    {
        private bool isFlipped = false;
        private bool isSkillEffectFront = false;
        private string visualEffectName = "";
        private string visualEffectAudioId = "";

        public VisualEffectParameterData (bool isFlipped, bool isSkillEffectFront, string visualEffectName, string visualEffectAudioId)
        {
            this.isFlipped = isFlipped;
            this.isSkillEffectFront = isSkillEffectFront;
            this.visualEffectName = visualEffectName;
            this.visualEffectAudioId = visualEffectAudioId;
        }

        public bool GetIsFlipped()
        {
            return this.isFlipped;
        }

        public bool GetIsSkillEffectFront()
        {
            return this.isSkillEffectFront;
        }

        public string GetVisualEffectName()
        {
            return this.visualEffectName;
        }

        public string GetVisualEffectAudioId()
        {
            return this.visualEffectAudioId;
        }
    }

    public void LoadAndPlayVisualEffect(VisualEffectParameterData visualEffectParameterData)
    {
        LoadAndPlayVisualEffect(visualEffectParameterData.GetIsFlipped(),visualEffectParameterData.GetIsSkillEffectFront(), visualEffectParameterData.GetVisualEffectName(), visualEffectParameterData.GetVisualEffectAudioId());
    }

    public void LoadAndPlayVisualEffect(bool isFlipped,bool isSkillEffectFront, string visualEffectName, string visualEffectAudioId)
    {
        Debug.Log("visualEffectName: " + visualEffectName);
        Debug.Log("visualEffectAudioId: " + visualEffectAudioId);

        Animator _visualEffectAnimator = isSkillEffectFront ? this.visualEffectFrontAnimator : this.visualEffectBackAnimator;

        // visual effect
        FlipVisualEffectContainer(isFlipped);
        AnimationClip _animationClip = Resources.Load<AnimationClip>("Animations/Battle/VisualEffects/" + visualEffectName);

        ((isSkillEffectFront) ? this.visualEffectFrontAnimatorOverrideController
                                        : this.visualEffectBackAnimatorOverrideController)
                                        ["Animation_0"] = _animationClip;

        _visualEffectAnimator.SetBool( "reset", true );
        _visualEffectAnimator.SetTrigger("trigger");
        AudioManager.Instance.PlaySoundEffect(visualEffectAudioId);
    }

    public void ResetAnimation()
    {
        //Debug.Log( "Reset Animation" );

        this.playerAnimator.StopPlayback();
        this.skillEffectBackAnimator.StopPlayback();
        this.skillEffectFrontAnimator.StopPlayback();
        this.visualEffectBackAnimator.StopPlayback();
        this.visualEffectFrontAnimator.StopPlayback();

        Transform[] _transforms = this.GetComponentsInChildren<Transform>( true );
        for (int i = 0; i < _transforms.Length; i++)
        {
            Transform _transform = _transforms[ i ];
            if (_transform != this.transform)
            {
                _transform.localScale = Vector3.one;
            }
        }

        SpriteRenderer[] _spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>( true );
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[ i ].sprite = null;
        }
    }

    public void GoToResetState()
    {
        //Debug.Log( "Go To Reset State" );

        this.playerAnimator.Play( "Reset" );
        this.skillEffectBackAnimator.Play( "Reset" );
        this.skillEffectFrontAnimator.Play( "Reset" );
        this.visualEffectBackAnimator.Play( "Reset" );
        this.visualEffectFrontAnimator.Play( "Reset" );
    }

    public void OnClick()
    {
        this.selectedCodeType = (AnimationData.CodeType)Enum.Parse(typeof(AnimationData.CodeType), codeType);
        LoadAndPlayAnimation(this.isFront.isOn,this.needToRecord, this.selectedCodeType, this.subSkillId.text, int.Parse(this.type.text));

        //Debug.Log("actionAnimationLength: " + actionAnimationLength + "\neffectAnimationLength: " + effectAnimationLength + "\naudioLength: " + audioLength); ;
        //Debug.Log("GetAnimationClipTotalLength(): " + GetAnimationClipTotalLength());
    }

    // compare action, effect and audio length
    public float GetAnimationClipTotalLength()
    {
        return new float[] { actionAnimationLength, effectAnimationLength, audioLength }.Max();
    }

    public void OnValueChanged()
    {
        this.codeType = this.dropdown.options[dropdown.value].text;
        Debug.Log("Selected type: " + codeType);
    }

    public Animator GetPlayerAnimator()
    {
        return this.playerAnimator;
    }

    public void TriggerCallback( string parameterValue )
    {
        if (parameterValue == "reset")
        {
            this.playerAnimator.SetBool( "idle", false );
            this.playerAnimator.SetBool( "prepare", false );
        }
    }
}
