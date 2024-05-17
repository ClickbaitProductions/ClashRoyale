using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    Transform cam;

    void Start()
    {
        cam = Player.localCam.transform;
    }

    void Update()
    {
        Vector3 directionToCamera = cam.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
        transform.rotation = Quaternion.Euler(rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

}
