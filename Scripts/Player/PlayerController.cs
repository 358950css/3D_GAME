using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private ControllerMovement3D _controllerMovement; //首先要抓取角色移動腳本
    private Vector3 _moveInput;// 設置一個Vector3 去抓取 input

    private void Awake()
    {
        _controllerMovement = GetComponent<ControllerMovement3D>(); //直接從角色裡抓取ControllerMovement3D腳本
    }

    public void OnMove(InputValue value) //這邊是接受玩家按鍵(OnMove是輸入系統的回呼函數，只要玩家有按鍵輸入，或是搖桿移動就會觸發)
    {
        Debug.Log("進行OnMove"); // 在控制台輸出訊息
        Vector2 input = value.Get<Vector2>(); //這邊是接受玩家按鍵的值
        _moveInput = new Vector3(input.x, 0f, input.y); //這邊是將玩家按鍵的值轉換成Vector3
    }

    private void Update()
    {
        if (_controllerMovement == null) return; //如果沒有抓到角色移動腳本就return
        Debug.Log("進行按鍵傳回"); // 在控制台輸出訊息
        bool isSprinting = Keyboard.current.leftShiftKey.isPressed; //檢查設備的Shift鍵有無被按下
        _controllerMovement.SetMoveInput(_moveInput, isSprinting);
    }
}
