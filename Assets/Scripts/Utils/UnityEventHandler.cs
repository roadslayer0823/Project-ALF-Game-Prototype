using UnityEngine;
using UnityEngine.Events;

public class UnityEventHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent<string> callback = null;

    public void TriggerCallback( string parameterValue )
    {
        this.callback?.Invoke( parameterValue );
    }

    public UnityEvent<string> GetCallback()
    {
        return this.callback;
    }
}
