using UnityEngine;

public class PopUpDisplayInfoV2 : MonoBehaviour
{
    [SerializeField] private DisplayInfoV2 actualHealthPointDamage = null;
    [SerializeField] private DisplayInfoV2 maxStatePointUp = null;
    [SerializeField] private DisplayInfoV2 statePointDamage = null;
    [SerializeField] private DisplayInfoV2 stressValueDamage = null;
    [SerializeField] private DisplayInfoV2 stressValueDown = null;

    public static void SpawnPopUpDisplayInfoV2(PopUpDisplayInfoV2 popUpDisplayInfoV2, GameObject pivotPosition, bool isLeft,
                                                float actualHealthPointDamage = 0, float statePointDamageUp = 0, float statePointDamageDown = 0, float stressPointDamageUp = 0, float stressPointDamageDown = 0)
    {
        GameObject _popUpDisplayInfoV2Obj = Instantiate(popUpDisplayInfoV2.gameObject);
        _popUpDisplayInfoV2Obj.transform.position = pivotPosition.transform.position;

        PopUpDisplayInfoV2 _popUpDisplayInfoV2 = _popUpDisplayInfoV2Obj.GetComponent<PopUpDisplayInfoV2>();
        _popUpDisplayInfoV2.actualHealthPointDamage.DisplayPopUpNumber(actualHealthPointDamage, isLeft);
        _popUpDisplayInfoV2.maxStatePointUp.DisplayPopUpNumber(statePointDamageUp, isLeft);
        _popUpDisplayInfoV2.statePointDamage.DisplayPopUpNumber(statePointDamageDown, isLeft);
        _popUpDisplayInfoV2.stressValueDamage.DisplayPopUpNumber(stressPointDamageUp, isLeft);
        _popUpDisplayInfoV2.stressValueDown.DisplayPopUpNumber(stressPointDamageDown, isLeft);
    }

    public void OnEndAnimation()
    {
        Destroy(this.gameObject);
    }
}
