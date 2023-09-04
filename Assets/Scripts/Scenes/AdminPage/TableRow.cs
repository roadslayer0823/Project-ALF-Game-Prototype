using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TableRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sheetName = null;
    [SerializeField] TextMeshProUGUI version = null;
    [SerializeField] TextMeshProUGUI status = null;

    [Header("Row Settings")]
    [SerializeField] float rowHeight = 100.0f;
    [SerializeField] private RectTransform tableRowRect;

    private void Start()
    {
        this.tableRowRect.sizeDelta = new Vector2(tableRowRect.sizeDelta.x, this.rowHeight);
    }

    public void SetSheetName(string sheetName)
    {
        this.sheetName.SetText(sheetName);
    }

    public void SetVersionNumber(int version)
    {
        this.version.SetText(version.ToString());
    }

    public void SetStatus(string status)
    {
        this.status.SetText(status);
    }
}
