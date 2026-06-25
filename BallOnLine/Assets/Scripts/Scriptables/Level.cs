using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    public string levelName;

    public bool isInkLimited = false;
    public float maxInkAmount = 100f;
    public float inkBallAmount = 10f;

    public int FreezeStartCount = 3;
    public int ShieldStartCount = 3;
    public int FreezeRefillCount = 1;
    public int ShieldRefillCount = 1;

    public float magnifyMultiplier = 1.5f;
    public float shrinkMultiplier = 0.5f;

    public float shootingSpeed = 1.0f;
}
