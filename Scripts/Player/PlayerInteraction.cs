using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            Debug.Log("觸發商城 / 對話系統是");
            // 發事件或直接呼叫 UI 管理器
            shop.Instance.OpenShop();
        }
    }
}