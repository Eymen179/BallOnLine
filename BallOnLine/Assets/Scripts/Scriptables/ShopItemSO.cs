using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Scriptable Objects/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    public string itemID; // Her top için benzersiz bir ID (Örn: "Ball_01")
    public Sprite shopImage; // Markette (ImgBall) görünecek görsel
    public Material shopItemMaterial; // Satýn alýnýnca oyun içindeki topa atanacak materyal
    public Color shopItemColor;
    public int requiredStars; // Kilidin açýlmasý için gereken yýldýz
    public int coinCost; // Satýn alma bedeli
}