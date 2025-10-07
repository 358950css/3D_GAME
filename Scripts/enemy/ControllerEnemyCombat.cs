using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemyCombat : MonoBehaviour
{

    [SerializeField] private ControllerWeapon weaponHitbox; //於inspector拉控制武器的腳本，用於動畫event內呼叫與取消武器的碰撞體


    public void EnableWeaponHitbox() //打開武器的碰撞體
    {
        weaponHitbox.EnableHitbox();
    }

    public void DisableWeaponHitbox() //取消武器的碰撞體
    {
        weaponHitbox.DisableHitbox();
    }
}
