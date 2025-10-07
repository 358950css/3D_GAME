using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class ControllerMovement3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _runSpeed = 5f; //跑步速度
    [SerializeField] private float _sprintSpeed = 8f; //衝刺速度
    private float _currentSpeed; //目前移動速度


    [SerializeField] private float _turnSpeed = 10f; //角色旋轉速度
    [SerializeField] private GameObject _maincamera; //相機物件
    [SerializeField] private float _mouseSensitivity = 10f; //滑鼠靈敏度
    [SerializeField] private float _cameraDistance = 20f; // 攝影機與角色的距離
    [SerializeField] private float _gravity = -9.81f; // 重力值
    [SerializeField] private float _groundCheckDistance = 0.2f; // 檢查是否在地面的距離
    //[SerializeField] private LayerMask _groundLayer; // 地面圖層

    private bool _hasMoveInput;
    private Vector3 _moveInput; //用來儲存玩家的移動方向

    private CharacterController _characterController; //抓取角色控制器
    //private ControllerCombat _combat; //攻擊控制器

    private Animator _animator; //抓取 Animator

    private float _cameraPitch = 0f; //攝影機的俯仰角度

    private float _cameraYaw = 0f; // 用來追蹤 Y 軸旋轉

    private Vector3 _velocity; // 用來儲存角色的速度（包括重力）

    FlowerSystem fs;

    //private bool _isAttacking = false; // 用來追蹤是否正在攻擊
    public bool _ismove = false; // 用來追蹤是否正在移動
    private bool canMove = true; //是否禁止移動
    public bool is_dead = false; //是否禁止移動
    public bool IsDefense => _animator.GetBool(isDefense); //這邊是專門給ControllerUI讀取是否防禦
    public bool IsIdle => _animator.GetBool(isIdle); //這邊是專門給ControllerUI讀取是否防禦

    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsSprint = Animator.StringToHash("isSprint");
    private static readonly int isIdle = Animator.StringToHash("isIdle");
    private static readonly int isDefense = Animator.StringToHash("isDefense");

    public bool isUninterruptible = false; // 是否霸體，技能不會被打斷

    private void Start()
    {
        if (_maincamera == null)
        {
            _maincamera = Camera.main.gameObject; // 自動抓取主攝影機
        }

        _characterController = GetComponent<CharacterController>();
        //_combat = GetComponent<ControllerCombat>();
        _animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked; // 隱藏鼠標
    }

    public void SetMoveInput(Vector3 moveInput, bool isSprinting) //用來設定角色移動，參數由PlayerController的Update傳遞
    {
        if (shop.Instance != null && shop.Instance.isUIOpen) //如果開啟商城，就進入idle狀態
        {
            _animator.SetBool(IsRunning, false);
            _animator.SetBool(IsSprint, false);
            return; // UI 開啟時不處理
        } 
        if (IsDefense) return;
        //抓取玩家是否有按按鍵
        _hasMoveInput = moveInput.magnitude > 0.1f; //如果玩家按鍵的值大於0.1就代表有按鍵(會變true，如果小於的話會變成false)
        _moveInput = _hasMoveInput ? moveInput : Vector3.zero; //如果有按鍵就將玩家按鍵的值傳給_moveInput，否則就傳Vector3.zero

        // 更新動畫參數
        if (_animator != null)
        {
            _animator.SetBool(IsRunning, _hasMoveInput);
            _animator.SetBool(IsSprint, isSprinting);
        }

        //如果傳入的isSprinting參數是true，速度就改成8f，反之5f
        _currentSpeed = isSprinting ? _sprintSpeed : _runSpeed;
    }

    private void Update()
    {
        
        // 檢查角色是否靜止以及滑鼠左鍵是否被按下
        /*Debug.Log("_ismove是：" + _ismove);
        if (_ismove == false && Input.GetMouseButtonDown(0))
        {
            if (_moveInput.magnitude >= 0.99f)
            {
                return;
            }
        }*/
    }

    public void SetMove(bool allow) //當防禦時，要呼叫該函式禁止移動
    {
        Debug.Log("防禦時canMove是：" + canMove);
        canMove = allow;
        if (!canMove)
        {
            _moveInput = Vector3.zero; // 馬上停止
        }
        //_combat.isAttacking = false;
    }

    private void FixedUpdate()
    {

        Debug.Log("canMove是：" + canMove);
        if (is_dead) return;
        if (shop.Instance != null && shop.Instance.isUIOpen) return; // UI 開啟時不處理
        if (!canMove)
        {
            // 禁止移動的時候只受重力影響
            _velocity.y += _gravity * Time.fixedDeltaTime;
            _characterController.Move(_velocity * Time.fixedDeltaTime);
            return;
        }

        if (_moveInput.magnitude < 0.1f)
        {
            Debug.Log("沒有移動");
            _animator.SetBool(isIdle, true);
            _ismove = false; // 如果沒有移動輸入，設置 _ismove 為 false
            _moveInput = Vector3.zero; // 如果玩家沒有按鍵就將 _moveInput 設為 0

            // 當角色靜止時，讓角色跟隨攝影機的方向進行轉向
            Vector3 cameraForward = _maincamera.transform.forward; // 攝影機的正前方
            cameraForward.y = 0f; // 忽略垂直方向，僅考慮水平面
            cameraForward.Normalize(); // 確保方向向量的長度為 1

            Quaternion targetRotation = Quaternion.LookRotation(cameraForward); // 計算目標旋轉
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime); // 平滑旋轉角色
        }

        else
        {
            Debug.Log("正在移動");
            _animator.SetBool(isIdle, true);
            _ismove = true;
            // 計算角色的目標旋轉方向，基於攝影機的方向
            Vector3 cameraForward = _maincamera.transform.forward; // 攝影機的正前方
            cameraForward.y = 0f; // 忽略垂直方向，僅考慮水平面
            cameraForward.Normalize(); // 確保方向向量的長度為 1

            Vector3 cameraRight = _maincamera.transform.right; // 攝影機的右方
            cameraRight.y = 0f; // 忽略垂直方向
            cameraRight.Normalize();

            // 根據玩家輸入計算移動方向
            Vector3 desiredMoveDirection = (cameraForward * _moveInput.z + cameraRight * _moveInput.x).normalized; // 計算角色在當前幀內的移動向量

            // 移動角色
            _characterController.Move(desiredMoveDirection * _currentSpeed * Time.fixedDeltaTime); // 根據計算的方向移動角色

            // 旋轉角色，使其面向移動方向
            if (desiredMoveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection); // 計算目標旋轉方向
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime); // 平滑旋轉角色
            }
        }

        // 模擬重力
        _velocity.y += _gravity * Time.fixedDeltaTime; // 垂直速度受重力影響
        _characterController.Move(_velocity * Time.fixedDeltaTime); // 將重力應用到角色
    }

    public void Dead()
    {
        Debug.Log("玩家死亡1");
        is_dead = true;
        _animator.SetBool(IsRunning, false);
        _animator.SetBool(IsSprint, false);
        _animator.SetBool(isIdle, false);
        _animator.ResetTrigger("ishurt");  //避免依舊在執行受傷動畫，先重置
        _animator.SetTrigger("isdie");
        Debug.Log("玩家死亡2");
    }

    public void Hurt()
    {

        // 如果正在技能的霸體狀態，就直接忽略受傷動畫
        if (isUninterruptible) 
        {
            isUninterruptible = false;
            return;
        }

        if (is_dead) return;
        Debug.Log("玩家受傷");
        _animator.SetBool(IsRunning, false);
        _animator.SetBool(IsSprint, false);
        _animator.SetBool(isIdle, false);
        _animator.SetTrigger("ishurt"); //使用受傷動畫
    }

    /*private void OnTriggerEnter(Collider other)
    {
        // 檢查碰撞的物件是否為 NPC
        if (other.CompareTag("NPC"))
        {
            Debug.Log("觸發對話系統"); // 在控制台輸出訊息
            // 觸發對話系統
            var fs = FlowerManager.Instance.CreateFlowerSystem("default", false);
            fs.SetupDialog();
            fs.ReadTextFromResource("intro");
        }
    }*/
}