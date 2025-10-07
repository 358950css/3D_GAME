using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 100;
    public float speed = 10f;

    private void Start()
    {
        // 一生成就往前飛
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        // 5 秒後自動銷毀
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            var enemy = other.GetComponent<ControllerUI>();
            if (enemy != null)
            {
                enemy.ReduceHealth(damage, ControllerUI.DamageType.Player);
                Destroy(gameObject); // 擊中後銷毀
            }
        }
    }
}