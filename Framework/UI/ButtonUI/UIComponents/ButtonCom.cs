using UnityEngine;
using UnityEngine.Events;

public class ButtonCom : MonoBehaviour
{
    [SerializeField] private UnityEvent m_OnClick = new UnityEvent();

    public UnityEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    private void OnTriggerEnter(Collider other)
    {
        m_OnClick?.Invoke();
    }

#if UNITY_EDITOR
    private void OnMouseDown()
    {
        m_OnClick?.Invoke();
    }
#endif
}