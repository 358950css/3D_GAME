using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Patrol, //巡邏
    Chase, //追蹤
    Attack,
    Dead,
    Idle //待機狀態
}

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float patrolRadius = 30f;
    public float walkSpeed = 0.5f;
    public float runSpeed = 2f;
    public float chaseDistance = 10f;
    public float attackDistance = 3f; //敵人的攻擊距離，會在該範圍內停下
    public int enemy_value = 10; //殺死該敵人能獲得多少錢

    private Vector3 origin;
    private Vector3 targetPoint;
    private Animator animator;

    public EnemyState currentState = EnemyState.Patrol;

    private Coroutine backToPatrolCoroutine;

    public ControllerUI playerUI; // 拖玩家物件進 Inspector

    public System.Action<EnemyController> OnEnemyDeath;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform; //尋找物件類型為Player的物件
            else
                Debug.LogError("找不到標籤為 Player 的物件！");
        }

        if (playerUI == null)
        {
            //在場景內尋找tag為Player的物件，且抓取該物件的ControllerUI腳本
            playerUI = GameObject.FindWithTag("Player").GetComponent<ControllerUI>(); 
        }

        //_UI = GetComponent<ControllerUI>();
        origin = transform.position;
        animator = GetComponent<Animator>();
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        //如果死亡就不要再有任何動作
        if (currentState == EnemyState.Dead) return;
        
        //如果偵測到玩家死亡，不要一直鞭屍
        if (playerUI != null && playerUI.isDead_player)
        {
            if (currentState != EnemyState.Patrol && backToPatrolCoroutine == null)
            {
                currentState = EnemyState.Idle;
                backToPatrolCoroutine = StartCoroutine(IdleBeforePatrol());
            }
        return; // 直接結束這幀，不要執行追擊/攻擊邏輯
        }

        if (currentState != EnemyState.Attack)
        {
            Debug.Log("isattack被重置");
            animator.ResetTrigger("isattack");
        }

        Debug.Log("進入狀態喔是：" + currentState);

        float distance = Vector3.Distance(transform.position, player.position);

        // 狀態切換
        if (distance <= attackDistance)
        {
            StopBackToPatrol(); // 取消延遲回巡邏
            currentState = EnemyState.Attack;
            // 延遲觸發攻擊動畫
            //StartCoroutine(DelayedAttack(0.5f));

        }
        else if (distance <= chaseDistance)
        {
            StopBackToPatrol(); // 取消延遲回巡邏
            currentState = EnemyState.Chase;
        }
        else
        {
            // 這裡不直接切換，而是等2秒
            if (currentState != EnemyState.Patrol && backToPatrolCoroutine == null)
            {
                Debug.Log("等兩秒前");
                currentState = EnemyState.Idle;
                backToPatrolCoroutine = StartCoroutine(IdleBeforePatrol());
            }
            //currentState = EnemyState.Patrol;
        }

        Debug.Log("狀態是：" + currentState);

        // 狀態執行
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Idle:
                Idle();
                break;
        }
    }

    void Patrol() //由於巡邏是一開始StartCoroutine(PatrolRoutine());就會無限確認的狀態，所以這邊只留著用來等待擴充
    {
        animator.SetBool("isrun", false);
    }

    void Chase()
    {
        animator.SetBool("iswalk", false);
        Debug.Log("等兩秒是true");
        animator.SetBool("isrun", true);

        // 計算目標位置 (XZ平面)
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);

        // 計算移動方向
        Vector3 moveDirection = targetPos - transform.position;
        moveDirection.y = 0f; // 保持XZ平面

        // 移動
        if (moveDirection.magnitude > 0.01f)
        {
            transform.position += moveDirection.normalized * runSpeed * Time.deltaTime;

            // 面向移動方向
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

    }

    void Attack()
    {
        Debug.Log("isattack正在被呼叫");
        animator.SetBool("iswalk", false);
        animator.SetBool("isrun", false);
        animator.SetBool("isidle", false);
        animator.SetTrigger("isattack"); // 觸發攻擊動畫
    }

    public void Dead()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            box.enabled = false; // 關閉該 BoxCollider
        }
        OnEnemyDeath?.Invoke(this); //呼叫訂閱OnEnemyDeath的所有函式
        animator.SetBool("iswalk", false);
        animator.SetBool("isrun", false);
        animator.SetTrigger("isdie");
        currentState = EnemyState.Dead;
        Debug.Log("進入狀態死亡2");
    }

    public void Hurt()
    {
        animator.SetBool("iswalk", false);
        animator.SetBool("isrun", false);
        animator.SetTrigger("ishurt"); //使用受傷動畫
    }

    void Idle()
    {
        animator.SetBool("iswalk", false);
        animator.SetBool("isrun", false);
        animator.SetBool("isidle", true);
    }

    IEnumerator IdleBeforePatrol() //玩家離開追蹤範圍後，先待機在巡邏
    {
        yield return new WaitForSeconds(2f);

        // 等待結束後切換到 Patrol
        currentState = EnemyState.Patrol;
        backToPatrolCoroutine = null;
    }

    IEnumerator PatrolRoutine()
    {
        while (true) // 一直重複執行（直到物件被Destroy）
        {
            targetPoint = GetRandomPointInCircle();
            Debug.Log("正在巡邏中2是：" + targetPoint);
            while (Vector3.Distance(transform.position, targetPoint) > 0.1f)
            {
                if (currentState != EnemyState.Patrol)
                {
                    yield return null; // 如果不是巡邏狀態，就什麼都不做，下一幀再檢查
                    continue;
                }

                Debug.Log("正在巡邏中3是：" + currentState);
                animator.SetBool("iswalk", true);
                animator.SetBool("isidle", false);
                transform.position = Vector3.MoveTowards(transform.position, targetPoint, walkSpeed * Time.deltaTime);
                transform.LookAt(new Vector3(targetPoint.x, transform.position.y, targetPoint.z));
                yield return null;
            }

            Debug.Log("已到達目標點：" + targetPoint);
            animator.SetBool("iswalk", false);
            animator.SetBool("isidle", true);
            yield return new WaitForSeconds(2f);
        }
    }

    Vector3 GetRandomPointInCircle() //生成巡邏目標點
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        return origin + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    void StopBackToPatrol() //
    {
        if (backToPatrolCoroutine != null)
        {
            StopCoroutine(backToPatrolCoroutine);
            backToPatrolCoroutine = null;
        }
    }
}
