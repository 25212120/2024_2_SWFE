using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab1; // 마스터 클라이언트용 플레이어 프리팹
    [SerializeField] private GameObject playerPrefab2; // 일반 클라이언트용 플레이어 프리팹
    [SerializeField] private Transform spawnPoint1;    // 마스터 클라이언트 스폰 위치
    [SerializeField] private Transform spawnPoint2;    // 일반 클라이언트 스폰 위치

    private void Start()
    {
        if(GameSettings.IsMultiplayer == false)
        {
            Instantiate(playerPrefab1, spawnPoint1.position, spawnPoint1.rotation);
            Debug.Log("싱글 플레이 모드에서 플레이어가 생성되었습니다.");
            return;
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // 마스터 클라이언트와 일반 클라이언트에 따라 스폰 위치 및 프리팹 선택
            Transform spawnPoint = PhotonNetwork.IsMasterClient ? spawnPoint1 : spawnPoint2;
            GameObject playerPrefab = PhotonNetwork.IsMasterClient ? playerPrefab1 : playerPrefab2;

            // 네트워크로 플레이어 프리팹 생성
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

            Debug.Log($"플레이어 스폰 완료: {PhotonNetwork.NickName} - 위치: {spawnPoint.position}");
        }
        else
        {
            Debug.LogError("Photon에 연결되지 않았거나 방에 입장하지 않았습니다.");
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"새 플레이어 입장: {newPlayer.NickName}");
        // 필요한 추가 로직이 있다면 여기에 작성
    }
}
