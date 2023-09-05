using UnityEngine;
using TMPro;

public class PopUpDisplayInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text displayLabel = null;

    public PopUpDisplayInfo Show( string text, Color textColor, float fontSize, FontStyles fontStyle )
    {
        this.displayLabel.SetText( text );
        this.displayLabel.color = textColor;
        this.displayLabel.fontSize = fontSize;
        this.displayLabel.fontStyle = fontStyle;
        return this;
    }

    public PopUpDisplayInfo MoveUpAndFadeOut( float duration, float fadeOutDuration, float height )
    {
        LeanTween.moveLocalY( this.gameObject, this.transform.localPosition.y + height, duration ).setEaseOutCirc();
        LeanTween.value( this.gameObject, 1.0f, 0.0f, fadeOutDuration ).setDelay( duration - fadeOutDuration )
            .setOnUpdate( UpdateDisplayLabelAlphaValue )
            .setOnComplete( () => { Destroy( this.gameObject ); } );

        return this;
    }

    private void UpdateDisplayLabelAlphaValue( float alphaValue )
    {
        Color _color = displayLabel.color;
        _color.a = alphaValue;
        displayLabel.color = _color;
    }
}
