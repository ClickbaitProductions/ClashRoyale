using Fusion;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Blue,
    Red
}

public class Player : NetworkBehaviour
{
    public static Dictionary<PlayerType, Player> players { get; set; } = new Dictionary<PlayerType, Player>();

    // playersMap is only filled in the host side
    public static Dictionary<PlayerRef, NetworkObject> playersMap { get; set; } = new Dictionary<PlayerRef, NetworkObject>();

    public static float[] spawnRangeX = new float[] { -17.7f, 18.25f };
    public static float[] blueSpawnRangeZ = new float[] { 8.3f, 44 };
    public static float[] redSpawnRangeZ = new float[] { -49, -11f };
    public static Player localPlayer;
    public static Camera localCam;

    [SerializeField] GameObject knightPrefab;
    [SerializeField] Transform playerCam;
    public LayerMask groundLayerMask;

    public PlayerType playerType;

    public static void AddPlayer(Player player, PlayerType playerType, PlayerRef? playerRef = null, NetworkObject playerNetworkObject = null)
    {
        if (!players.ContainsKey(playerType))
            players[playerType] = player;

        if (playerRef != null && !playersMap.ContainsKey(playerRef.Value))
            playersMap[playerRef.Value] = playerNetworkObject;
    }

    public static void RemovePlayer(NetworkObject playerNetworkObject, PlayerRef playerRef)
    {
        Player _player = playerNetworkObject.gameObject.GetComponent<Player>();
        if (_player.Equals(players[PlayerType.Blue]))
            players.Remove(PlayerType.Blue);
        else
            players.Remove(PlayerType.Red);

        playersMap.Remove(playerRef);
    }

    public void Host_Initialize(PlayerRef playerRef, NetworkObject networkPlayerObject)
    {
        if (HasInputAuthority)
        {
            // I'm the first person to join - blue, on host side - local
            AddPlayer(this, PlayerType.Blue, playerRef, networkPlayerObject);
            gameObject.name = "P1 - Blue - Local - Host";
            playerType = PlayerType.Blue;
            localPlayer = this;
            localCam = playerCam.GetComponent<Camera>();
        } else
        {
            // I'm the 2nd person to join - red, on host side - not local
            playerCam.gameObject.SetActive(false);
            
            AddPlayer(this, PlayerType.Red, playerRef, networkPlayerObject);
            players[PlayerType.Red].gameObject.name = "P2 - Red - Not Local - Client";
        }
    }

    public void Client_Initialize()
    {
        if (HasInputAuthority)
        {
            // I'm red on client side - local
            gameObject.name = "P2 - Red - Local - Client";
            gameObject.tag = "Red";

            Vector3 camRot = playerCam.localEulerAngles;
            camRot.z = 180;
            playerCam.localEulerAngles = camRot;

            Vector3 camPos = playerCam.localPosition;
            camPos.y = 94.9f;
            camPos.z = 34.3f;
            playerCam.localPosition = camPos;

            AddPlayer(this, PlayerType.Red);

            playerType = PlayerType.Red;
            localPlayer = this;
            localCam = playerCam.GetComponent<Camera>();
        } else
        {
            // I'm blue on client side - not local
            gameObject.name = "P1 - Blue - Not Local - Client";
            gameObject.tag = "Blue";

            playerCam.gameObject.SetActive(false);

            AddPlayer(this, PlayerType.Blue);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            print("Got input");
            if (HasStateAuthority)
            {
                print("Has authority");
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    SpawnCharacter(data);
                }
            }
        }
    }

    void SpawnCharacter(NetworkInputData data)
    {
        print("Spawning");
        Runner.Spawn(knightPrefab, data.clickPos, Quaternion.identity, Object.InputAuthority, (runner, instance) =>
        {
            // Initialize the agent before synchronizing it
            instance.GetComponent<AgentWalker>().Init(data.playerType);
        });
    }

}
