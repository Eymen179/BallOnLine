using System.Collections;
using UnityEngine;

public class ButtonSkillManager : MonoBehaviour
{
    public static ButtonSkillManager Instance;

    public static int freezeCount = 3;
    public static int shieldCount = 3;

    public GameObject ball;

    private Rigidbody2D ballRb;
    private GameObject shieldObject;
    public bool isShieldActive = false;
    public int shieldTime = 5;

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
        ballRb = ball.GetComponent<Rigidbody2D>();
        shieldObject = ball.transform.Find("Shield").gameObject;

        // Baþlangýç deðerlerini yöneticiden çekiyoruz
        freezeCount = LevelManager.Instance.currentLevel.FreezeStartCount;
        shieldCount = LevelManager.Instance.currentLevel.ShieldStartCount;

        UIManager.Instance.txtFreezeAmount.text = "x" + freezeCount.ToString();
        UIManager.Instance.txtShieldAmount.text = "x" + shieldCount.ToString();
    }

    // --- Diðer metotlarýn ayný þekilde kalýyor (OnFreezePointerDown, OnFreezePointerUp, ShieldButton vs.) ---

    public void OnFreezePointerDown()
    {
        if (freezeCount > 0)
        {
            UpdateFreezeCount(-1);
            ballRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void OnFreezePointerUp()
    {
        ballRb.constraints = RigidbodyConstraints2D.None;
        ballRb.WakeUp();
    }

    public void ShieldButton()
    {
        if (shieldCount > 0 && !isShieldActive)
        {
            UpdateShieldCount(-1);
            StartCoroutine(ShieldRoutine());
        }
    }

    private IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        shieldObject.SetActive(true);

        yield return new WaitForSeconds(shieldTime);

        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }

        isShieldActive = false;
    }

    public void UpdateFreezeCount(int amount)
    {
        freezeCount += amount;
        if (freezeCount < 0) freezeCount = 0;
        UIManager.Instance.txtFreezeAmount.text = "x" + freezeCount.ToString();
    }

    public void UpdateShieldCount(int amount)
    {
        shieldCount += amount;
        if (shieldCount < 0) shieldCount = 0;
        UIManager.Instance.txtShieldAmount.text = "x" + shieldCount.ToString();
    }
}