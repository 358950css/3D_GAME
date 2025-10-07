using UnityEngine;
using UnityEngine.UI;

public class ControllerUI : MonoBehaviour
{
    [Header("血條設定")]
    public Image Bar;
    public float health;
    public float maxHealth = 100f;

    [Header("金錢設定")]
    public float money;

    public bool destroyOnDeath = true; // 是否死亡後刪除物件（玩家不刪，敵人要刪）

    private Animator _animator;
    private EnemyController _controller;
    private ControllerMovement3D _movement;

    private bool isDead = false;   // 檢查是否已經死亡
    public bool isDead_player = false;   // 檢查玩家是否已經死亡

    public enum DamageType
    {
        Enemy,
        Player,
    }

    private void Start()
    {
        _controller = GetComponent<EnemyController>(); //給敵人物件使用
        _movement = GetComponent<ControllerMovement3D>(); //給玩家物件使用
        _animator = GetComponent<Animator>();
        health = maxHealth; // 開場時血量全滿
    }

    private void Update()
    {
        BarFiller();
    }

    private void BarFiller()
    {
        if (Bar != null)
            Bar.fillAmount = health / maxHealth;
    }

    public void AddHealth(float amount = 10f)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void ReduceHealth(float amount, DamageType source)
    {
        Debug.Log("攻擊種類是：" + source);
        if (_movement != null) //如果有值代表這是玩家物件
        {
            bool is_defense = _movement.IsDefense;  // 通知 EnemyController 進入死亡狀態動畫
            if (is_defense) 
            {
                Debug.Log("防禦成功" + is_defense);
                amount = 0;
                return;
            }
        }
        if (_controller != null) 
        {
            if (source == DamageType.Enemy){
                Debug.Log("是友軍攻擊無效");
                return;
            }
            _controller.Hurt();  // 通知 EnemyController 進入死亡狀態動畫
        }
        if (_movement != null) 
        {
            _movement.Hurt();  // 通知 ControllerMovement3D 進入死亡狀態動畫
        }
        Debug.Log($"{gameObject.name} 扣血 {amount}");
        health -= amount;
        //_animator.SetTrigger(ishurt); //使用受傷動畫
        if (health <= 0)
        {
            Debug.Log("進入狀態死亡0");
            health = 0;
            Die();
            return;
        }
    }

    private void Die()
    {
        if (isDead) return;  // 已經死亡就不再執行
        isDead = true;
        Debug.Log($"{gameObject.name} 死亡");
        Debug.Log("進入狀態死亡1");
        if (_controller != null) 
        {
            Player_money.Instance.AddMoney(_controller.enemy_value);
            _controller.Dead();  // 通知 EnemyController 進入死亡狀態動畫
        }
        if (_movement != null) 
        {
            Debug.Log("玩家進入狀態死亡1");
            _movement.Dead();  // 通知 ControllerMovement3D 進入死亡狀態動畫
            isDead_player = true;
        }

        if (destroyOnDeath)
        {

            //Destroy(gameObject); // 敵人死亡直接刪除
        }
    }
}