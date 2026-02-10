using UnityEngine;

public class LidarScanner : MonoBehaviour
{
    public GameObject lidarDotPrefab;
    public int rayCount = 100; // number of rays per pulse
    public float maxDistance = 20f;

    public KeyCode triggerKey = KeyCode.L;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            FireLidarPulse();
        }
    }

    void FireLidarPulse()
    {
        for (int i = 0; i < rayCount; i++)
        {
            float angle = i * (360f / rayCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, maxDistance))
            {
                // Spawn dot at hit point
                Instantiate(lidarDotPrefab, hit.point + hit.normal * 0.01f, Quaternion.identity);
            }
        }
    }
}
