using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_money : MonoBehaviour
{
    public static Player_money Instance; // 單例模式，方便其他腳本存取
    public int money;

    [Header("UI 元件")]
    public TMP_Text moneyText; // 拖 UI 的 Text 元件進來

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMoneyUI(); // 開場顯示初始金錢
    }
 
    public void AddMoney(int amount) //獲得金錢
    {
        money += amount;
        Debug.Log($"獲得金錢：{amount}，目前總金錢：{money}");
        UpdateMoneyUI();
    }

    public void DeductMoney(int amount) //扣除金錢
    {
        money -= amount;
        Debug.Log($"扣得金錢：{amount}，目前總金錢：{money}");
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{money}";
        }
    }
}
