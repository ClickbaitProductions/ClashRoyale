using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System.Collections.Generic;
using Fusion.Sockets;
using System;
using System.Collections;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] NetworkPrefabRef playerPrefab;
    [SerializeField] Transform knight;

    bool clicked = false;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        if (playerRef == runner.LocalPlayer)
        {
            Singleton.Instance.runner = runner;
        }

        if (runner.IsServer)
        {
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, playerRef);
            networkPlayerObject.GetComponent<Player>().Host_Initialize(playerRef, networkPlayerObject);
        } else
        {
            Invoke("Client_InitializePlayers", 1f);
        }
    }

    void Client_InitializePlayers()
    {
        int n = 0;
        foreach (Player player in FindObjectsOfType<Player>())
        {
            n++;
            player.Client_Initialize();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        if (Player.playersMap.TryGetValue(playerRef, out NetworkObject networkObject))
        {
            Player.RemovePlayer(networkObject, playerRef);
            runner.Despawn(networkObject);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();
        Vector3 mouseWorldPos = Vector3.one * 100;

        if (Player.localPlayer != null)
        {
            float[] spawnRangeZ = Player.blueSpawnRangeZ;
            if (Player.localPlayer.playerType == PlayerType.Red)   // Runs only on host
                spawnRangeZ = Player.redSpawnRangeZ;

            Ray ray = Player.localCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 300, Player.localPlayer.groundLayerMask))
                mouseWorldPos = hit.point;

            if (clicked && (mouseWorldPos.x < Player.spawnRangeX[0] || mouseWorldPos.x > Player.spawnRangeX[1] ||
                mouseWorldPos.z < spawnRangeZ[0] || mouseWorldPos.z > spawnRangeZ[1] || mouseWorldPos.y > 0.1f))
                clicked = false;

            data.playerType = Player.localPlayer.playerType;
        } else
        {
            clicked = false;
        }
        
        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, clicked);

        mouseWorldPos.y = 0;
        data.clickPos = mouseWorldPos;

        input.Set(data);

        if (clicked)
            clicked = false;
    }

    void Update()
    {
        clicked = clicked | Input.GetMouseButtonDown(0);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    private NetworkRunner _runner;

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

}