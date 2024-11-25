using UnityEngine;
using Photon.Pun;

public class ResourceTracker : MonoBehaviourPunCallbacks
{
    private string resourceName;

    // ResourceManager�� �˸� �ʱ�ȭ �Լ�
    public void Initialize(string resourceName)
    {
        this.resourceName = resourceName;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(resourceName))
        {
            Debug.Log($"Resource {resourceName} is being destroyed. Notifying ResourceManager...");

            // ������ Ŭ���̾�Ʈ�� �ı� �̺�Ʈ ����
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("NotifyResourceDestroyed", RpcTarget.MasterClient, resourceName);
        }
    }

    [PunRPC]
    public void NotifyResourceDestroyed(string resourceName)
    {
        // ������ Ŭ���̾�Ʈ���� ó��
        if (PhotonNetwork.IsMasterClient)
        {
            ResourceManager.Instance.HandleResourceCollection(resourceName);
        }
    }
}
