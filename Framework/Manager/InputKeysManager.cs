using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 按键输入管理器，仅监听注册过的按键。
/// </summary>
public class InputKeysManager : SingletonPatternBase<InputKeysManager>
{
    // 是否启用按键监听
    public bool IsActive { get; private set; }

    // 存储注册的按键
    private readonly HashSet<KeyCode> registeredKeys = new HashSet<KeyCode>();

    public InputKeysManager()
    {
        MonoManager.Instance.AddUpdateListener(CheckKeys);
    }

    /// <summary>
    /// 激活或禁用按键监听。
    /// </summary>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        Debug.Log($"InputKeysManager Active: {isActive}");
    }

    /// <summary>
    /// 注册一个按键，使其被监听。
    /// </summary>
    public void AddListenerKey(KeyCode key)
    {
        registeredKeys.Add(key);
    }

    /// <summary>
    /// 注销一个按键，使其不再被监听。
    /// </summary>
    public void RemoveListenerKey(KeyCode key)
    {
        registeredKeys.Remove(key);
    }

    /// <summary>
    /// 清除所有注册的按键。
    /// </summary>
    public void ClearAllRegisteredKeys()
    {
        registeredKeys.Clear();
    }

    private void CheckKeys()
    {
        if (!IsActive) return;

        // 检查注册的键盘按键
        foreach (var key in registeredKeys)
        {
            CheckKeyCode(key);
        }

        // 检查鼠标按钮
        CheckMouseButtons();
    }

    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
            EventCenterManager.Instance.Dispatch(E_InputCommand.GetKeyDown, key);

        if (Input.GetKeyUp(key))
            EventCenterManager.Instance.Dispatch(E_InputCommand.GetKeyUp, key);

        if (Input.GetKey(key))
            EventCenterManager.Instance.Dispatch(E_InputCommand.GetKey, key);
    }

    private void CheckMouseButtons()
    {
        for (int i = 0; i <= 2; i++)
        {
            if (Input.GetMouseButtonDown(i))
                EventCenterManager.Instance.Dispatch(E_InputCommand.GetMouseButtonDown, i);

            if (Input.GetMouseButtonUp(i))
                EventCenterManager.Instance.Dispatch(E_InputCommand.GetMouseButtonUp, i);

            if (Input.GetMouseButton(i))
                EventCenterManager.Instance.Dispatch(E_InputCommand.GetMouseButton, i);
        }
    }
}