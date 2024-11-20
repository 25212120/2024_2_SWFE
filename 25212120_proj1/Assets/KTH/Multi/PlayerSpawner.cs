using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab; // �� ���� ����� ������
    [SerializeField] private GameObject player2Prefab; // �� �������� ������
    [SerializeField] private Transform spawnPoint1;     // ���� ��ġ
    [SerializeField] private Transform spawnPoint2;     // ���� ��ġ
    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // Master Client�� Player1Prefab, �ٸ� Ŭ���̾�Ʈ�� Player2Prefab
            GameObject prefabToSpawn = PhotonNetwork.IsMasterClient ? player1Prefab : player2Prefab;
            Transform spawnPoint = PhotonNetwork.IsMasterClient ? spawnPoint1 : spawnPoint2;

            // ��Ʈ��ũ�� ������ ����
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
