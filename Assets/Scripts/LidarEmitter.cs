using System.Collections;
using UnityEngine;

public class LidarEmitter : MonoBehaviour
{
    public Transform lidarOrigin;
    public GameObject lidarDotPrefab;
    public GameObject rayPrefab; // prefab with LineRenderer
    public float scanInterval = 0.05f;
    public float rayLength = 20f;
    public float dotLifetime = 5f;
    public int raysPerScan = 100;
    public float scanAngle = 60f;
    public Gradient rayGradient;
    public AudioSource lidarAudio;
    public bool lidarEnabled = false;
    [SerializeField] private BatteryBar batteryBar;
    [SerializeField] private float batteryCostPerScan = 0.5f;

    void Start()
    {
        // Ensure it's off at the beginning
        lidarEnabled = false;
    }

    void Update()
    {
        if (!lidarEnabled) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (batteryBar != null && batteryBar.UseBattery(batteryCostPerScan))
            {
                StartCoroutine(PerformLidarScan());
            }
            else
            {
                Debug.Log("Not enough battery to perform LIDAR scan.");
            }
        }
    }
    


    IEnumerator PerformLidarScan()
    {
        for (int i = 0; i < raysPerScan; i++)
        {
            Vector3 scanDirection = GetRandomDirectionInFront();
            if (Physics.Raycast(lidarOrigin.position, scanDirection, out RaycastHit hit, rayLength))
            {
                // Spawn lidar dot
                GameObject dot = Instantiate(lidarDotPrefab, hit.point, Quaternion.identity);
                float distance = Vector3.Distance(lidarOrigin.position, hit.point);
                float brightness = Mathf.Clamp01(1 - (distance / rayLength));
                dot.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.green * brightness);
                Destroy(dot, dotLifetime);

                // Visualize ray
                SpawnRayVisual(lidarOrigin.position, hit.point);

                // Play sound blip
                if (lidarAudio != null)
                {
                    lidarAudio.pitch = Random.Range(0.95f, 1.05f); // Optional variation
                    lidarAudio.PlayOneShot(lidarAudio.clip);
                }
            }

            yield return new WaitForSeconds(scanInterval);
        }
    }



    void SpawnRayVisual(Vector3 start, Vector3 end)
    {
        GameObject ray = Instantiate(rayPrefab);
        LineRenderer lr = ray.GetComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.colorGradient = rayGradient;

        StartCoroutine(FadeAndDestroyLine(lr, 0.2f));
    }

    IEnumerator FadeAndDestroyLine(LineRenderer lr, float duration)
    {
        float time = 0f;
        Gradient startGradient = lr.colorGradient;

        GradientColorKey[] colorKeys = startGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(0f, 1f)
        };

        while (time < duration)
        {
            float t = time / duration;
            Gradient g = new Gradient();
            g.SetKeys(colorKeys, new GradientAlphaKey[] {
                new GradientAlphaKey(1f - t, 0f),
                new GradientAlphaKey(1f - t, 1f)
            });
            lr.colorGradient = g;
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(lr.gameObject);
    }

    Vector3 GetRandomDirectionInFront()
    {
        float angle = Random.Range(-scanAngle / 2f, scanAngle / 2f);
        Vector3 direction = Quaternion.Euler(0, angle, 0) * lidarOrigin.forward;
        direction += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.05f, 0.05f), 0f); // add variation
        return direction.normalized;
    }
}
