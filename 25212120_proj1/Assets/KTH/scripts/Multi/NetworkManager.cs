using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "1.0"; // Photon 버전
    [SerializeField] private byte maxPlayersPerRoom = 2; // 최대 플레이어 수

    private void Start()
    {
        // 씬 동기화 활성화
        PhotonNetwork.AutomaticallySyncScene = true;

        // Photon 서버 연결
        Debug.Log("Connecting to Photon Server...");
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name cannot be empty!");
            return;
        }

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom
        };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void JoinRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("Room name cannot be empty!");
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        
        UpdatePlayerCount();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined the room.");
        Debug.Log($"Current Player Count: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");

        UpdatePlayerCount();

        // 인원이 다 찼을 때 게임 씬으로 전환
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("Room is full. Triggering scene load...");
            LoadGameScene();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the room.");
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            Debug.Log($"Player Count Updated: {playerCount}/{maxPlayers}");
        }
    }

    private void LoadGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("MasterClient is triggering the scene load...");
            Debug.Log($"PhotonNetwork.AutomaticallySyncScene: {PhotonNetwork.AutomaticallySyncScene}");
            PhotonNetwork.LoadLevel("main");
        }
        else
        {
            Debug.LogWarning("Non-MasterClient attempted to load the scene. Ignoring.");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to create room: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }

    private void Update()
    {
    }
}
