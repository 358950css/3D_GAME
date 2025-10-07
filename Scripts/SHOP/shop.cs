using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class shop : MonoBehaviour
{
    public static shop Instance; // 單例方便呼叫（也可以用事件）
    public bool isUIOpen { get; private set; } = false; //get是公開的getter任何外部程式都可以讀取，private set是私有的setter，只有這個腳本內可以修改這個值。

    [System.Serializable] class ShopItem
    {
        public string ID;
        public string Action_name;
        public int Price;
        public bool IsPurchased = false;
    }

    public GameObject buy_success_Panel;
    public GameObject no_money_Panel; //金錢不足視窗

    [SerializeField] List<ShopItem> ShopItemsList;

    GameObject ItemTemplate;
    GameObject g;
    [SerializeField] Transform ShopScrollView;
    Button buyBtn;

    [SerializeField] private PlayerSkillInventory _payerskill;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        Debug.Log("商城打開");
        gameObject.SetActive(false); // 一開始隱藏
    }

    void Start()
    {
        ItemTemplate = ShopScrollView.GetChild (0).gameObject;

        int len = ShopItemsList.Count;
        for (int i = 0; i < len; i++){
            g = Instantiate (ItemTemplate, ShopScrollView);
            g.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = ShopItemsList[i].Price.ToString(); //這邊是給價格
            //g.transform.GetChild(1).GetComponent <Button> ().interactable = !ShopItemsList[i].IsPurchased; //這邊是是否購買
            g.transform.GetChild(2).GetComponent<TMP_Text>().text = ShopItemsList[i].Action_name.ToString(); //這邊是招式名稱
            buyBtn = g.transform.GetChild(1).GetComponent <Button> ();
            buyBtn.interactable = !ShopItemsList[i].IsPurchased;
            buyBtn.AddEventListener (i,OnShopItemBtnClicked);//(i,ShopItemsList[i].ID,OnShopItemBtnClicked);
        }
        Destroy (ItemTemplate);
    }

    public void OpenShop()
    {
        isUIOpen = true;
        Debug.Log("商城打開了");
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None; // 解鎖滑鼠
        Cursor.visible = true;                     // 顯示滑鼠
    }

    public void CloseShop()
    {
        isUIOpen = false;
        Debug.Log("商城關閉了");
        gameObject.SetActive(false);

        // 隱藏滑鼠並鎖定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CloseBuySuccessPanel()
    {
        buy_success_Panel.SetActive(false);
    }

    public void ClosNoMoneyPanel()
    {
        no_money_Panel.SetActive(false);
    }

    void OnShopItemBtnClicked(int ItemIndex){
        //var item = ShopItemsList.Find(x => x.ID == ID);
        //item.IsPurchased = true;
        var item = ShopItemsList[ItemIndex];
        if (Player_money.Instance.money < item.Price){ //如果要購買技能的金額小於玩家身上的金錢就不能購買
            no_money_Panel.SetActive(true); // 顯示金額不足提示面板
            return;
        }

        Debug.Log("item.Price是：" + item.Price);

        ShopScrollView.GetChild (ItemIndex).GetChild(1).GetComponent<Button>().interactable = false;

        int skillID = int.Parse(item.ID); //int.Parse(ID);  // 假設ID都是整數字串
        Debug.Log("ItemIndex是：" + skillID);
        _payerskill.UnlockSkill(skillID);
        Player_money.Instance.DeductMoney(item.Price);
        buy_success_Panel.SetActive(true); // 顯示購買成功提示面板
        //Debug.Log("ItemIndex是：" + skillID);
    }
}
