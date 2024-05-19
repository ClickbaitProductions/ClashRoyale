using Fusion;
using UnityEngine;

public class LookAtCamera : NetworkBehaviour
{

    [SerializeField] bool isAgent = false;

    float zRot;
    bool flipped = false;

    void Start()
    {
        zRot = transform.localEulerAngles.z;
        Player.LocalPlayer.AddListener(OnGotLocalPlayer);
    }

    void OnGotLocalPlayer(Player localPlayer)
    {
        localPlayer.OnCanvasFlip += () =>
        {
            if (!flipped)
            {
                zRot += 180;
                flipped = true;
            }
        };
    }

    void Update()
    {
        Vector3 directionToCamera = Singleton.Instance.camPos - transform.position;
        Quaternion worldRot = Quaternion.LookRotation(directionToCamera);

        Quaternion parentRotation = transform.parent.rotation;
        Quaternion localRotation = Quaternion.Inverse(parentRotation) * worldRot;

        Vector3 localEulerAngles = localRotation.eulerAngles;
        localEulerAngles.y = transform.localEulerAngles.y;
        localEulerAngles.z = zRot;

        //if (!HasStateAuthority && isAgent)
        //{
        //    // If im the client, looking from red's side, the agent's health bar would be off
        //    float offset = -90 - localEulerAngles.x;
        //    localEulerAngles.x += offset * 2;
        //}

        transform.localEulerAngles = localEulerAngles;
    }

}