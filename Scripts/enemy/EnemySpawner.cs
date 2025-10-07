using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("設定")]
    public GameObject enemyPrefab;      // 要生成的敵人 Prefab
    public int maxEnemies = 10;         // 最大生成敵人數量
    public float spawnRadius = 20f;     // 生成範圍半徑
    public float respawnDelay = 30f;    // 死亡後生成冷卻時間

    private List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        // 初始化補滿到 maxEnemies
        SpawnToMax();
        StartCoroutine(MonitorEnemies());
    }

    private void SpawnToMax()
    {
        int count = CountEnemiesInRange();
        int toSpawn = maxEnemies - count;

        for (int i = 0; i < toSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    private GameObject SpawnEnemy()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = transform.position.y;

        GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        var controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
            controller.OnEnemyDeath += HandleEnemyDeath;

        enemies.Add(enemy);
        return enemy;
    }

    private void HandleEnemyDeath(EnemyController enemy) //如果敵人死亡，就要呼叫該腳本，讓MonitorEnemies去補敵人數量
    {
        enemies.Remove(enemy.gameObject);
    }

    private int CountEnemiesInRange()
    {
        enemies.RemoveAll(e => e == null); // 清理已消失敵人
        return enemies.Count;
    }


    private IEnumerator MonitorEnemies()
    {
        while (true)
        {
            int currentCount = CountEnemiesInRange();
            if (currentCount < maxEnemies)
            {
                // 等待 respawnDelay 再生成一個
                yield return new WaitForSeconds(respawnDelay);
                
                // 再次確認數量，避免生成超過 maxEnemies
                if (CountEnemiesInRange() < maxEnemies)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                yield return null; // 每幀檢查一次也可以換成 WaitForSeconds(1f) 減少負擔
            }
        }
    }
}
