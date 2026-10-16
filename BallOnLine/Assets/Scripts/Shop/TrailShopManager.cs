using System.Collections.Generic;
using UnityEngine;

public class TrailShopManager : MonoBehaviour
{
    public static TrailShopManager Instance;

    [Header("Trail Shop References")]
    public List<ShopItemUI> allTrailItems;

    private string currentEquippedTrailID;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Varsayýlan olarak "Trail1" seçili gelir
        currentEquippedTrailID = PlayerPrefs.GetString("EquippedTrailSkin", "Trail1");
        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        int totalStars = StarManager.Instance != null ? StarManager.Instance.totalStars : 0;
        int totalCoins = CoinManager.Instance != null ? CoinManager.Instance.coinAmount : 0;

        foreach (var itemUI in allTrailItems)
        {
            if (itemUI == null || itemUI.itemData == null) continue;

            ShopItemSO data = itemUI.itemData;
            bool isOwned = PlayerPrefs.GetInt("SkinOwned_" + data.itemID, 0) == 1;

            // Ýlk trail her zaman oyuncuya açýk ve ücretsizdir
            if (data.itemID == "Trail1") isOwned = true;

            if (isOwned)
            {
                if (currentEquippedTrailID == data.itemID) itemUI.Setup(ShopItemState.Equipped_LightBlue);
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
        if (data.itemID == "Trail1") isOwned = true;

        if (isOwned)
        {
            EquipTrail(data);
        }
        else
        {
            int totalCoins = CoinManager.Instance.coinAmount;
            if (totalCoins >= data.coinCost)
            {
                CoinManager.Instance.UpdateCoinAmount(-data.coinCost);
                PlayerPrefs.SetInt("SkinOwned_" + data.itemID, 1);
                EquipTrail(data);

                if (ShopMenuManager.Instance != null && ShopMenuManager.Instance.txtTotalCoins != null)
                {
                    ShopMenuManager.Instance.txtTotalCoins.text = CoinManager.Instance.coinAmount.ToString();
                }

                // Coin azaldýðý için diðer market sekmesindeki (Ball) satýn alma durumlarýný da yenile
                if (BallShopManager.Instance != null)
                {
                    BallShopManager.Instance.RefreshShopUI();
                }
            }
        }
    }

    private void EquipTrail(ShopItemSO data)
    {
        currentEquippedTrailID = data.itemID;
        PlayerPrefs.SetString("EquippedTrailSkin", data.itemID);
        PlayerPrefs.Save();

        RefreshShopUI();
    }
}