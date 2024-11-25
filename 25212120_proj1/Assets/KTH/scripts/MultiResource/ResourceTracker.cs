using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
    private string resourceName;
    private string chunkName;
    private string tileName;

    public void Initialize(string resourceName, string chunkName, string tileName)
    {
        this.resourceName = resourceName;
        this.chunkName = chunkName;
        this.tileName = tileName;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(resourceName) && ResourceManager.Instance != null)
        {
            Debug.Log($"Resource {resourceName} is being destroyed. Notifying ResourceManager...");
            ResourceManager.Instance.OnResourceDestroyed(resourceName, chunkName, tileName);
        }
    }
}
