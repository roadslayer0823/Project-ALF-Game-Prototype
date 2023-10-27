using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    AttackerWins,
    DefenderWins,
    Draw
}

public class DebugBattleDialogMenu : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject container;

    private Action<ResultType> onResultCallBack = null;

    public void Initialize(Action<ResultType> resultCallBack)
    {
        this.onResultCallBack = resultCallBack;
    }

    public void Show()
    {
        this.container.SetActive(true);
    }

    public void Hide()
    {
        this.container.SetActive(false);
    }

    public void ClickToSelectAttacker()
    {
        this.onResultCallBack?.Invoke(ResultType.AttackerWins);
        Hide();
    }

    public void ClickToSelectDefender()
    {
        this.onResultCallBack?.Invoke(ResultType.DefenderWins);
        Hide();
    }

    public void ClickToSelectDraw()
    {
        this.onResultCallBack?.Invoke(ResultType.Draw);
        Hide();
    }
}
