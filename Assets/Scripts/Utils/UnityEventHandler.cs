using UnityEngine;
using UnityEngine.Events;

public class UnityEventHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent callback = null;

    public void TriggerCallback()
    {
        this.callback?.Invoke();
    }
}
