using UnityEngine;

public class LidarPickup : MonoBehaviour
{
    public GameObject uiLidarIcon;  // Icon to show after pickup

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Look for the LidarEmitter on the player or any of its children
            LidarEmitter lidar = other.GetComponentInChildren<LidarEmitter>();
            Debug.Log("Trying to get LidarEmitter from children...");

            if (lidar != null)
            {
                lidar.lidarEnabled = true;
                Debug.Log("LIDAR Enabled!");

                if (uiLidarIcon != null)
                {
                    uiLidarIcon.SetActive(true);
                    Debug.Log("LIDAR UI Icon Enabled!");
                }

                Destroy(gameObject); // Remove the pickup item
            }
            else
            {
                Debug.LogWarning("LidarEmitter component not found in player or its children.");
            }
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }
}
