using UnityEngine;

public class BattleBackgroundManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite backgroundPartA = null;
    [SerializeField] private Sprite backgroundPartB = null;

    public void ChangeToBackgroundPartA()
    {
        background.sprite = backgroundPartA;
    }

    public void ChangeToBackgroundPartB()
    {
        background.sprite = backgroundPartB;
    }
}
