using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerWeapon : MonoBehaviour
{
   [SerializeField] private int damage = 20;
    private Collider hitboxCollider;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false; // 預設關閉
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("武器碰撞1");
        Debug.Log("這個腳本掛在：" + gameObject.tag);
        ControllerUI enemy = other.GetComponent<ControllerUI>();
        ControllerMovement3D _movement = other.GetComponent<ControllerMovement3D>();
        ControllerCombat _combat = other.GetComponent<ControllerCombat>();
        if (enemy != null)
        {
            Debug.Log("武器碰撞2");
            //判斷武器類別是甚麼，來給予攻擊方的類別，避免友軍傷害
            if (gameObject.tag == "player_weapon"){
                enemy.ReduceHealth(damage, ControllerUI.DamageType.Player); // 直接把武器的 damage 當作扣血量
            }
            else if (gameObject.tag == "enemy_weapon"){
                enemy.ReduceHealth(damage, ControllerUI.DamageType.Enemy); // 直接把武器的 damage 當作扣血量
            }
            hitboxCollider.enabled = false; //武器攻擊到就把碰撞體關掉，避免攻擊一次，但扣複數次數血量
        }
        if (_movement != null)
        {
            Debug.Log("被攻擊的是玩家1");
            _movement.SetMove(true); //恢復移動
        }
        if (_combat != null)
        {
            _combat.isAttacking = false;
        }
    }

    public void SetDamage(int newDamage) //調整傷害，當使用技能跟普通攻擊的傷害是不一樣的
    {
        damage = newDamage;
    }

    // 打開武器的碰撞體
    public void EnableHitbox()
    {
        hitboxCollider.enabled = true;
    }

    // 關閉武器的碰撞體
    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
    }
}