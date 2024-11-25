using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{

    [SerializeField] private InputField roomNameInput;
    [SerializeField] private NetworkManager networkManager;

    public void OnCreateRoomButtonClicked()
    {
        string roomName = "Room1";
        if (!string.IsNullOrEmpty(roomName))
        {
            networkManager.CreateRoom(roomName);
        }
    }

    public void OnJoinRoomButtonClicked()
    {
        string roomName = "Room1";
        if (!string.IsNullOrEmpty(roomName))
        {
            networkManager.JoinRoom(roomName);
        }
    }

    public void LoadLobby()
    {
        GameSettings.IsMultiplayer = true;
        SceneManager.LoadScene("LobbyScene");
    }

    public void LoadSoloPlayer()
    {
        GameSettings.IsMultiplayer = false;
        SceneManager.LoadScene("GameScene");
    }





}
