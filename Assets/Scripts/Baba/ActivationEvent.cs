using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ActivationEvent", menuName = "Scriptable Objects/ActivationEvent")]
public class ActivationEvent : ScriptableObject
{
    private UnityEvent onTrigger = new UnityEvent();

    public void Setup() => onTrigger.Invoke();

    public void AddListener(UnityAction listener) => onTrigger.AddListener(listener);
    public void RemoveListener(UnityAction listener) => onTrigger.RemoveListener(listener);
}
