using System.Collections;
using UnityEngine;

public class ButtonSkillManager : MonoBehaviour
{
    public static ButtonSkillManager Instance;

    public static int freezeAmount = 3;
    public static int shieldAmount = 3;

    public GameObject ball;

    private Rigidbody2D ballRb;
    private GameObject shieldObject;
    public bool isShieldActive = false;
    public int shieldTime = 5; // Kalkanýn aktif kalma süresi (saniye)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Component'leri her seferinde aratmamak için Start'ta bir kere tanýmlýyoruz (Performans için)
        ballRb = ball.GetComponent<Rigidbody2D>();
        shieldObject = ball.transform.Find("Shield").gameObject;
    }

    //Freeze Button
    public void OnFreezePointerDown()
    {
        if (freezeAmount > 0)
        {
            UpdateFreezeAmount(-1); // Hakký 1 azalt
            ballRb.constraints = RigidbodyConstraints2D.FreezeAll; // Topu dondur
        }
    }

    // Bu metot butondan PARMAK ÇEKÝLDÝÐÝ AN çalýþacak
    public void OnFreezePointerUp()
    {
        // Topun fiziksel hareketini geri veriyoruz.
        // Eðer topun Z ekseninde dönmesini istemiyorsan RigidbodyConstraints2D.FreezeRotationZ yapmalýsýn.
        ballRb.constraints = RigidbodyConstraints2D.None;
        ballRb.WakeUp(); // Topu zorla uyandýrýp fiziði tekrar hesaplamasýný saðlar
    }

    //Shield Button
    public void ShieldButton()
    {
        if (shieldAmount > 0 && !isShieldActive)
        {
            UpdateShieldAmount(-1); // Hakký 1 azalt
            StartCoroutine(ShieldRoutine()); // 5 saniyelik sayacý baþlat
        }
    }
    private IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        shieldObject.SetActive(true); // Kalkaný aç

        // 5 saniye boyunca bekle (Oyun duraklatýlýrsa sayma da duraklatýlýr)
        yield return new WaitForSeconds(shieldTime);

        // 5 saniye dolduktan sonra eðer top hala sahada duruyorsa kalkaný kapat
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }

        isShieldActive = false;
    }

    //Skill Amount Update Methods
    public void UpdateFreezeAmount(int amount)
    {
        freezeAmount += amount;
        if (freezeAmount < 0) freezeAmount = 0;
        UIManager.Instance.txtFreezeAmount.text = "x" + freezeAmount.ToString();
    }
    public void UpdateShieldAmount(int amount)
    {
        shieldAmount += amount;
        if (shieldAmount < 0) shieldAmount = 0;
        UIManager.Instance.txtShieldAmount.text = "x" + shieldAmount.ToString();
    }
}
