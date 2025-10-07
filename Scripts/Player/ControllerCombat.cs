using UnityEngine;

public class ControllerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float isAttackCooldown = 0.5f; // 攻擊冷卻時間
    [SerializeField] private float blockMoveSpeedMultiplier = 0.5f; // 防禦時移動速度降低比例
    //[SerializeField] private ControllerUI _UI; //直接於inspector拉UI腳本
    [SerializeField] private ControllerWeapon weaponHitbox; //於inspector拉控制武器的腳本，用於動畫event內呼叫與取消武器的碰撞體

    private Animator _animator;
    private ControllerMovement3D _movement; //引用掛載於同物件的ControllerMovement3D腳本
    private PlayerSkillInventory _playerskill; //引用掛載於同物件的PlayerSkillInventory腳本


    public bool isAttacking = false;
    private bool isDefending = false;
    private float lastisAttackTime = -999f;

    private static readonly int isAttackTrigger = Animator.StringToHash("isAttack");
    private static readonly int isDefense = Animator.StringToHash("isDefense");
    private static readonly int isMove_1Trigger = Animator.StringToHash("isMove_1"); //招式1(按Z)
    private static readonly int isMove_2Trigger = Animator.StringToHash("isMove_2"); //招式2(按X)

    [Header("技能2設定")]
    public GameObject fireballPrefab;     // 火球物件預置體
    public Transform firePoint;           // 火球發射點（通常放在玩家手上或身前）
    public float spawnDistance = 5f;      // 火球生成時向前偏移距離

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<ControllerMovement3D>();
        _playerskill = GetComponent<PlayerSkillInventory>();
    }

    private void Update()
    {
        HandleisAttack();
        HandleBlock();
        HandleMove1();
        HandleMove2();
    }

    private void HandleisAttack() //揮劍攻擊
    {
        if (Input.GetMouseButtonDown(0)) //左鍵
        {
            Debug.Log("進行攻擊的isAttackin是：" + isAttacking);
            bool isMoving = _movement._ismove; //先檢查是否在移動，如果再移動就不執行攻擊動畫
            Debug.Log("進行攻擊的isMoving是：" + isMoving);
            
            if (!isMoving && !isAttacking && Time.time - lastisAttackTime >= isAttackCooldown)
            {
                Debug.Log("進行攻擊2");
                isAttacking = true;
                lastisAttackTime = Time.time;

                if (_animator != null)
                {
                    _animator.SetTrigger(isAttackTrigger); //使用攻擊動畫
                    _movement.SetMove(false); //停下且禁止移動
                    weaponHitbox.SetDamage(20); // 普通攻擊維持20
                }
            }
        }
    }

    private void HandleMove1() //招式1
    {
        if (_movement.is_dead) return;
        if (Input.GetKeyDown(KeyCode.Z)) // 當按下 Z
        {
            if (!_playerskill.skills[1]) return; //如果PlayerSkillInventory內字典的招式1是false就直接跳出
            if (_animator != null)
            {
                _animator.SetTrigger(isMove_1Trigger);
                _movement.SetMove(false); // 播放技能時先停下移動

                _movement.isUninterruptible = true; //進入霸體狀態，使用技能期間不會被攻擊打斷
                weaponHitbox.SetDamage(100); //傷害提升至60(原本20)
            }
        }
    }

    private void HandleMove2() //招式2
    {
        if (_movement.is_dead) return;
        if (Input.GetKeyDown(KeyCode.X)) // 當按下 X
        {
            if (!_playerskill.skills[2]) return; //如果PlayerSkillInventory內字典的招式1是false就直接跳出
            if (_animator != null)
            {
                _animator.SetTrigger(isMove_2Trigger);
                _movement.SetMove(false); // 播放技能時先停下移動

                //_movement.isUninterruptible = true; //進入霸體狀態，使用技能期間不會被攻擊打斷
                //weaponHitbox.SetDamage(60); //傷害提升至60(原本20)
            }
        }
    }

    private void CastFireball()
    {
        if (!_playerskill.skills[2]) return; //如果PlayerSkillInventory內字典的招式1是false就直接跳出
        if (fireballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("⚠️ Fireball prefab 或 firePoint 未設定！");
            return;
        }

        Vector3 spawnPos = firePoint.position + firePoint.forward * spawnDistance;
        GameObject fireball = Instantiate(fireballPrefab, spawnPos, firePoint.rotation);
    }

    private void HandleBlock() //舉盾防禦
    {
        if (_movement.is_dead) return;
        if (Input.GetMouseButton(1)) //右鍵
        {
            if (!isDefending)
            {
                Debug.Log("防禦開始");
                isDefending = true;
                _animator.SetBool(isDefense, isDefending);
                _movement.SetMove(false); //停下且禁止移動
            }
        }
        else if (isDefending) // 鬆開右鍵 → 結束防禦
        {
            Debug.Log("防禦結束");
            isDefending = false;
            _animator.SetBool(isDefense, false);
            _movement.SetMove(true); //恢復移動
            isAttacking = false;
        }
    }

    public void EndisAttack()
    {
        bool isMoving = _movement._ismove; //先檢查是否在移動，如果再移動就不執行攻擊動畫
        _movement.SetMove(true); //恢復移動
        Debug.Log("isMoving是：" + isMoving);
        Debug.Log("攻擊結束");
        isAttacking = false;
    }

    public void EnableWeaponHitbox() //打開武器的碰撞體
    {
        weaponHitbox.EnableHitbox();
    }

    public void DisableWeaponHitbox() //取消武器的碰撞體
    {
        weaponHitbox.DisableHitbox();
    }
}
