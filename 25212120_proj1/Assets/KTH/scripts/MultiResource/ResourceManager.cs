using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class ResourceManager : MonoBehaviourPunCallbacks
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<string, ResourceData> resourceStates = new Dictionary<string, ResourceData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 이 오브젝트가 씬 전환 시에도 유지되도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class ResourceData
    {
        public Vector3 worldPosition;
        public string chunkName;
        public string tileName;
        public string resourceName;
        public bool isCollected;
    }

    // 리소스 등록
    public void RegisterResources(GameObject tileObject, Vector2Int chunkCoord)
    {
        string chunkName = $"Chunk_{chunkCoord.x}_{chunkCoord.y}";
        string tileName = tileObject.name;

        for (int i = 0; i < tileObject.transform.childCount; i++)
        {
            Transform child = tileObject.transform.GetChild(i);

            if (child.CompareTag("Resource"))
            {
                string resourceName = child.name;

                string resourceKey = $"{chunkName}_{tileName}_{resourceName}";

                if (!resourceStates.ContainsKey(resourceKey))
                {
                    resourceStates[resourceKey] = new ResourceData
                    {
                        worldPosition = child.position,
                        chunkName = chunkName,
                        tileName = tileName,
                        resourceName = resourceName,
                        isCollected = false
                    };

                    // 리소스 오브젝트의 이름을 resourceName으로 설정
                    child.gameObject.name = resourceName;

                    // ResourceTracker 추가 및 초기화
                    ResourceTracker tracker = child.gameObject.GetComponent<ResourceTracker>();
                    if (tracker == null)
                    {
                        tracker = child.gameObject.AddComponent<ResourceTracker>();
                        tracker.Initialize(resourceName, chunkName, tileName);
                    }

                    Debug.Log($"Resource registered: {resourceKey}");
                }
                else
                {
                    // 이미 등록된 리소스라도 ResourceTracker가 있는지 확인하고 없으면 추가
                    ResourceTracker tracker = child.gameObject.GetComponent<ResourceTracker>();
                    if (tracker == null)
                    {
                        tracker = child.gameObject.AddComponent<ResourceTracker>();
                        tracker.Initialize(resourceName, chunkName, tileName);
                    }
                }
            }
        }
    }

    // 리소스 파괴 이벤트 처리
    public void OnResourceDestroyed(string resourceName, string chunkName, string tileName)
    {
        Debug.Log($"ResourceManager received destruction event for {resourceName} in {chunkName}/{tileName}");

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트에서 리소스 처리
            HandleResourceCollection(resourceName, chunkName, tileName);
        }
        else
        {
            // 마스터 클라이언트에 파괴 이벤트 전송
            photonView.RPC("NotifyMasterResourceDestroyed", RpcTarget.MasterClient, resourceName, chunkName, tileName);
        }
    }

    [PunRPC]
    public void HandleResourceCollection(string resourceName, string chunkName, string tileName)
    {
        string resourceKey = $"{chunkName}_{tileName}_{resourceName}";
        if (resourceStates.TryGetValue(resourceKey, out var resource))
        {
            Debug.Log($"Before deletion: Total resources = {resourceStates.Count}");

            // 동기화 호출 (리소스 삭제를 클라이언트에서 처리)
            photonView.RPC("SyncResourceState", RpcTarget.All, resourceName, chunkName, tileName, true);

            // 마스터 클라이언트에서 리스트에서 제거
            resourceStates.Remove(resourceKey);

            Debug.Log($"After deletion: Total resources = {resourceStates.Count}");
            Debug.Log($"Resource collected and removed: {resourceKey}");
        }
        else
        {
            Debug.LogWarning($"Resource {resourceKey} not found or already collected.");
        }
    }

    [PunRPC]
    public void NotifyMasterResourceDestroyed(string resourceName, string chunkName, string tileName)
    {
        Debug.Log($"Master received destruction event for {resourceName} in {chunkName}/{tileName}");
        HandleResourceCollection(resourceName, chunkName, tileName);
    }

    [PunRPC]
    public void SyncResourceState(string resourceName, string chunkName, string tileName, bool isCollected)
    {
        Debug.Log($"SyncResourceState called on client for {resourceName} in {chunkName}/{tileName} with isCollected = {isCollected}");

        try
        {
            // 파라미터 null 체크
            if (string.IsNullOrEmpty(resourceName) || string.IsNullOrEmpty(chunkName) || string.IsNullOrEmpty(tileName))
            {
                Debug.LogError("One or more parameters are null or empty.");
                return;
            }

            // 리소스 상태 업데이트
            string resourceKey = $"{chunkName}_{tileName}_{resourceName}";
            Debug.Log($"resourceKey: {resourceKey}");

            if (resourceStates.ContainsKey(resourceKey))
            {
                resourceStates[resourceKey].isCollected = isCollected;
            }
            else
            {
                Debug.LogWarning($"resourceStates does not contain key: {resourceKey}");
            }

            // 계층 구조를 따라 리소스 오브젝트 찾기
            GameObject chunkObject = GameObject.Find(chunkName);
            if (chunkObject != null)
            {
                Transform tileTransform = chunkObject.transform.Find(tileName);
                if (tileTransform != null)
                {
                    Transform resourceTransform = tileTransform.Find(resourceName);
                    if (resourceTransform != null)
                    {
                        GameObject resourceObject = resourceTransform.gameObject;

                        if (isCollected)
                        {
                            // 리소스를 파괴
                            Debug.Log($"Destroying resource object: {chunkName}/{tileName}/{resourceName}");
                            Destroy(resourceObject);
                            Debug.Log($"Resource object {chunkName}/{tileName}/{resourceName} destroyed successfully.");
                        }
                        else
                        {
                            resourceObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Resource object {resourceName} not found under {chunkName}/{tileName}.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Tile {tileName} not found under {chunkName}.");
                }
            }
            else
            {
                Debug.LogWarning($"Chunk {chunkName} not found in scene.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception in SyncResourceState: {ex.Message}\n{ex.StackTrace}");
        }
    }

}
