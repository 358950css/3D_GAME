using UnityEngine;
using UnityEngine.UI;
using System;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param1, Action<T> OnClick)
    {
        button.onClick.AddListener (delegate(){
            OnClick(param1);
            Debug.Log("param1是：" + param1);
        });
    }
}
