using System.Collections;
using UnityEngine;

public class LidarRayVisual : MonoBehaviour
{
    public Transform lidarOrigin;
    public int rayCount = 50;
    public float range = 20f;
    public float delayBeforeFade = 0.05f;
    public float rayDuration = 0.2f;
    public GameObject lidarDotPrefab;
    public Material rayMaterial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) // Just for testing
        {
            StartCoroutine(FireLidarRays());
        }
    }

    IEnumerator FireLidarRays()
    {
        for (int i = 0; i < rayCount; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            direction.y = Mathf.Abs(direction.y); // keep rays pointing outward
            RaycastHit hit;

            if (Physics.Raycast(lidarOrigin.position, direction, out hit, range))
            {
                // Spawn LIDAR dot
                GameObject dot = Instantiate(lidarDotPrefab, hit.point, Quaternion.identity);
                float distance = Vector3.Distance(lidarOrigin.position, hit.point);
                float brightness = 1f - (distance / range);
                Color dotColor = Color.green * brightness;
                dot.GetComponent<Renderer>().material.SetColor("_EmissionColor", dotColor);
                Destroy(dot, 5f); // auto destroy after 5 seconds

                // Create LIDAR ray
                GameObject rayGO = new GameObject("LidarRay");
                LineRenderer lr = rayGO.AddComponent<LineRenderer>();
                lr.material = new Material(rayMaterial);
                lr.startColor = lr.endColor = Color.green * brightness;
                lr.startWidth = 0.02f;
                lr.endWidth = 0.005f;
                lr.SetPosition(0, lidarOrigin.position);
                lr.SetPosition(1, hit.point);
                Destroy(rayGO, rayDuration);
            }

            yield return new WaitForSeconds(delayBeforeFade);
        }
    }
}

