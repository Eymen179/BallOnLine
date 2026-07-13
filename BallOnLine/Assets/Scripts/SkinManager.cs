using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    [Header("All Ball Skins")]
    public ShopItemSO[] allBallSkins; // Tüm Top SO dosyalarýný buraya sürükle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Oyun içindeki herhangi bir scriptin aktif skin'i öðrenmesi için
    public ShopItemSO GetEquippedBallSkin()
    {
        string equippedID = PlayerPrefs.GetString("EquippedBallSkin", "Ball1");

        foreach (var skin in allBallSkins)
        {
            if (skin.itemID == equippedID) return skin;
        }

        if (allBallSkins.Length > 0) return allBallSkins[0];
        return null;
    }
}