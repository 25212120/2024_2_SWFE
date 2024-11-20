using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab; // 방 만든 사람의 프리팹
    [SerializeField] private GameObject player2Prefab; // 방 참가자의 프리팹
    [SerializeField] private Transform spawnPoint1;     // 스폰 위치
    [SerializeField] private Transform spawnPoint2;     // 스폰 위치
    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Master Client는 Player1Prefab, 다른 클라이언트는 Player2Prefab
            GameObject prefabToSpawn = PhotonNetwork.IsMasterClient ? player1Prefab : player2Prefab;
            Transform spawnPoint = PhotonNetwork.IsMasterClient ? spawnPoint1 : spawnPoint2;

            // 네트워크로 프리팹 생성
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
