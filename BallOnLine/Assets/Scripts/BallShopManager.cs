using System.Collections.Generic;
using UnityEngine;

public class BallShopManager : MonoBehaviour
{
    public static BallShopManager Instance;

    [Header("Shop References")]
    public List<ShopItemUI> allBallItems;

    private string currentEquippedBallID;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        currentEquippedBallID = PlayerPrefs.GetString("EquippedBallSkin", "Ball1");
        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        int totalStars = StarManager.Instance != null ? StarManager.Instance.totalStars : 0;
        int totalCoins = CoinManager.Instance != null ? CoinManager.Instance.coinAmount : 0;

        foreach (var itemUI in allBallItems)
        {
            ShopItemSO data = itemUI.itemData;
            bool isOwned = PlayerPrefs.GetInt("SkinOwned_" + data.itemID, 0) == 1;

            if (data.itemID == "Ball1") isOwned = true;

            if (isOwned)
            {
                if (currentEquippedBallID == data.itemID) itemUI.Setup(ShopItemState.Equipped_LightBlue);
                else itemUI.Setup(ShopItemState.Owned_DarkBlue);
            }
            else
            {
                if (totalStars < data.requiredStars) itemUI.Setup(ShopItemState.Locked_Gray);
                else if (totalCoins >= data.coinCost) itemUI.Setup(ShopItemState.CanBuy_Green);
                else itemUI.Setup(ShopItemState.CannotBuy_Red);
            }
        }
    }

    public void OnShopItemClicked(ShopItemUI clickedItem)
    {
        ShopItemSO data = clickedItem.itemData;
        bool isOwned = PlayerPrefs.GetInt("SkinOwned_" + data.itemID, 0) == 1;
        if (data.itemID == "Ball1") isOwned = true;

        if (isOwned)
        {
            EquipSkin(data);
        }
        else
        {
            int totalCoins = CoinManager.Instance.coinAmount;
            if (totalCoins >= data.coinCost)
            {
                CoinManager.Instance.UpdateCoinAmount(-data.coinCost);
                PlayerPrefs.SetInt("SkinOwned_" + data.itemID, 1);
                EquipSkin(data);

                if (ShopMenuManager.Instance != null && ShopMenuManager.Instance.txtTotalCoins != null)
                {
                    ShopMenuManager.Instance.txtTotalCoins.text = CoinManager.Instance.coinAmount.ToString();
                }
            }
        }
    }

    private void EquipSkin(ShopItemSO data)
    {
        currentEquippedBallID = data.itemID;
        PlayerPrefs.SetString("EquippedBallSkin", data.itemID);
        PlayerPrefs.Save();

        RefreshShopUI(); // Renkler güncellenir
    }
}