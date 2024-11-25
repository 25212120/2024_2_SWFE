using UnityEngine;
using Photon.Pun;

public class ResourceTracker : MonoBehaviourPunCallbacks
{
    private string resourceName;

    // ResourceManager에 알릴 초기화 함수
    public void Initialize(string resourceName)
    {
        this.resourceName = resourceName;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(resourceName))
        {
            Debug.Log($"Resource {resourceName} is being destroyed. Notifying ResourceManager...");

            // 마스터 클라이언트에 파괴 이벤트 전송
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("NotifyResourceDestroyed", RpcTarget.MasterClient, resourceName);
        }
    }

    [PunRPC]
    public void NotifyResourceDestroyed(string resourceName)
    {
        // 마스터 클라이언트에서 처리
        if (PhotonNetwork.IsMasterClient)
        {
            ResourceManager.Instance.HandleResourceCollection(resourceName);
        }
    }
}
