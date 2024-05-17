using Fusion;
using UnityEngine;

public class Singleton : MonoBehaviour
{

    public static Singleton Instance;

    public NetworkRunner runner;

    public GameObject viewCam;

    [Header("Towers")]
    public Transform leftBlueTower;
    public Transform rightBlueTower;
    public Transform leftRedTower;
    public Transform rightRedTower;

    void Start()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;
    }

}
