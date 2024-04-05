using UnityEngine;

public class PopUpDisplayInfoV2 : MonoBehaviour
{
    [SerializeField] private DisplayInfoV2 healthPointDamageDisplay = null;
    [SerializeField] private DisplayInfoV2 maxStatePointUpDisplay = null;
    [SerializeField] private DisplayInfoV2 statePointDamageDisplay = null;
    [SerializeField] private DisplayInfoV2 stressValueDamageDisplay = null;
    [SerializeField] private DisplayInfoV2 stressValueDownDisplay = null;

    public static void SpawnPopUpDisplayInfoV2(PopUpDisplayInfoV2 popUpDisplayInfoV2, Vector3 pivotPosition, bool isLeft,
                                               float healthPointDamage = 0.0f, float maxStatePointUp = 0.0f, float statePointDamage = 0.0f,
                                               float stressValueDamage = 0.0f, float stressValueDown = 0.0f)
    {
        GameObject _popUpDisplayInfoV2Obj = Instantiate(popUpDisplayInfoV2.gameObject);
        _popUpDisplayInfoV2Obj.transform.position = pivotPosition + new Vector3(0,2,0);

        PopUpDisplayInfoV2 _popUpDisplayInfoV2 = _popUpDisplayInfoV2Obj.GetComponent<PopUpDisplayInfoV2>();
        _popUpDisplayInfoV2.healthPointDamageDisplay.DisplayPopUpNumber( healthPointDamage, false);
        _popUpDisplayInfoV2.maxStatePointUpDisplay.DisplayPopUpNumber( maxStatePointUp, isLeft );
        _popUpDisplayInfoV2.statePointDamageDisplay.DisplayPopUpNumber( statePointDamage, isLeft );
        _popUpDisplayInfoV2.stressValueDamageDisplay.DisplayPopUpNumber( stressValueDamage, isLeft );
        _popUpDisplayInfoV2.stressValueDownDisplay.DisplayPopUpNumber( stressValueDown, isLeft );

        Destroy(_popUpDisplayInfoV2Obj, 1.2f);
    }
}
