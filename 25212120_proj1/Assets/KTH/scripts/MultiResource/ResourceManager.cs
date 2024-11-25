using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResourceManager : MonoBehaviourPunCallbacks
{
    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<string, ResourceData> resourceStates = new Dictionary<string, ResourceData>();

    // ���ҽ� ���
    public void RegisterResources(GameObject tileObject)
    {
        for (int i = 0; i < tileObject.transform.childCount; i++)
        {
            Transform child = tileObject.transform.GetChild(i);

            if (child.CompareTag("Resource"))
            {
                string resourceName = $"{tileObject.name}_{child.name}";

                if (!resourceStates.ContainsKey(resourceName))
                {
                    resourceStates[resourceName] = new ResourceData
                    {
                        worldPosition = child.position,
                        resourceName = resourceName,
                        isCollected = false
                    };

                    // ResourceTracker �߰�
                    ResourceTracker tracker = child.gameObject.AddComponent<ResourceTracker>();
                    tracker.Initialize(resourceName);

                    Debug.Log($"Resource registered: {resourceName}");
                }
            }
        }
    }

    public void HandleResourceCollection(string resourceName)
    {
        if (resourceStates.TryGetValue(resourceName, out var resource))
        {
            resourceStates.Remove(resourceName); // ��Ͽ��� ����
            Debug.Log($"Resource collected and removed: {resourceName}");

            // ��� Ŭ���̾�Ʈ�� ����ȭ ����
            photonView.RPC("SyncResourceState", RpcTarget.All, resourceName, true);
        }
        else
        {
            Debug.LogWarning($"Resource {resourceName} not found or already collected.");
        }
    }


    [PunRPC]
    public void SyncResourceState(string resourceName, bool isCollected)
    {
        if (resourceStates.ContainsKey(resourceName))
        {
            resourceStates[resourceName].isCollected = isCollected;
        }

        GameObject resourceObject = GameObject.Find(resourceName);
        if (resourceObject != null)
        {
            resourceObject.SetActive(!isCollected);
        }
    }
}

[System.Serializable]
public class ResourceData
{
    public Vector3 worldPosition;
    public string resourceName;
    public bool isCollected;
}
