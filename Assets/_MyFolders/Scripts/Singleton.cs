using UnityEngine;

public class Singleton : MonoBehaviour
{

    public static Singleton Instance;
    
    public GameObject viewCam;

    [HideInInspector] public Vector3 blueCamPos = new Vector3(0, 91, 43.3f);
    [HideInInspector] public Vector3 redCamPos = new Vector3(0, 94.9f, 34.3f);
    public Vector3 camPos;

    [Header("Towers")]
    public Transform leftBlueTower;
    public Transform rightBlueTower;
    public Transform leftRedTower;
    public Transform rightRedTower;
    public Transform[] towerCanvases;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        camPos = blueCamPos;
    }

}
