using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab1; // ������ Ŭ���̾�Ʈ�� �÷��̾� ������
    [SerializeField] private GameObject playerPrefab2; // �Ϲ� Ŭ���̾�Ʈ�� �÷��̾� ������
    [SerializeField] private Transform spawnPoint1;    // ������ Ŭ���̾�Ʈ ���� ��ġ
    [SerializeField] private Transform spawnPoint2;    // �Ϲ� Ŭ���̾�Ʈ ���� ��ġ

    private void Start()
    {
        if(GameSettings.IsMultiplayer == false)
        {
            Instantiate(playerPrefab1, spawnPoint1.position, spawnPoint1.rotation);
            Debug.Log("�̱� �÷��� ��忡�� �÷��̾ �����Ǿ����ϴ�.");
            return;
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // ������ Ŭ���̾�Ʈ�� �Ϲ� Ŭ���̾�Ʈ�� ���� ���� ��ġ �� ������ ����
            Transform spawnPoint = PhotonNetwork.IsMasterClient ? spawnPoint1 : spawnPoint2;
            GameObject playerPrefab = PhotonNetwork.IsMasterClient ? playerPrefab1 : playerPrefab2;

            // ��Ʈ��ũ�� �÷��̾� ������ ����
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

            Debug.Log($"�÷��̾� ���� �Ϸ�: {PhotonNetwork.NickName} - ��ġ: {spawnPoint.position}");
        }
        else
        {
            Debug.LogError("Photon�� ������� �ʾҰų� �濡 �������� �ʾҽ��ϴ�.");
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"�� �÷��̾� ����: {newPlayer.NickName}");
        // �ʿ��� �߰� ������ �ִٸ� ���⿡ �ۼ�
    }
}
