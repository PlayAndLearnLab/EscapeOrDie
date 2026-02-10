using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

public class CiderController : MonoBehaviour
{
    public float radarRange = 10f;
    public float retrieveRange = 1f;
    public LayerMask cheeseLayer;
    public LineRenderer webLineRenderer;
    public Transform webOrigin;
    private AudioSource pingAudio;
    public GameObject inventoryFullWarningUI;

    [SerializeField]
    private bool radarEnabled = false;
    public GameObject radarIconUI;
    [SerializeField] private BatteryBar batteryBar;
    [SerializeField] private float batteryCostPerScan = 0.5f;

    private List<Transform> detectedCheese = new List<Transform>();
    private Transform nearestCheese;
    public LayerMask batteryLayer;
    public LayerMask keyLayer;

    void Start()
    {
        webLineRenderer.startWidth = 0.05f;
        webLineRenderer.endWidth = 0.01f;
        webLineRenderer.textureMode = LineTextureMode.Tile;
        pingAudio = GetComponent<AudioSource>();

        if (webLineRenderer.material != null)
        {
            webLineRenderer.material.mainTextureScale = new Vector2(4f, 1f);
        }
    }

    void Update()
    {
        if (radarEnabled && Input.GetKeyUp(KeyCode.R))
        {
            if (batteryBar != null && batteryBar.UseBattery(batteryCostPerScan))
            {
                ScanForCheese();
            }
            else
            {
                Debug.Log("Not enough battery to use Radar");
            }
        }

        if (nearestCheese != null && Vector3.Distance(transform.position, nearestCheese.position) <= retrieveRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(RetrieveCheese(nearestCheese));
            }
        }
    }

    public void EnableRadar()
    {
        radarEnabled = true;

        if (radarIconUI != null)
        {
            radarIconUI.SetActive(true);
            Debug.Log("Radar UI enabled");
        }

        Debug.Log("Radar acquired!");
    }

    void ScanForCheese()
    {
        detectedCheese.Clear();

        Collider[] cheeseHits = Physics.OverlapSphere(transform.position, radarRange, cheeseLayer);
        Collider[] batteryHits = Physics.OverlapSphere(transform.position, radarRange, batteryLayer);
        Collider[] keyHits = Physics.OverlapSphere(transform.position, radarRange, keyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        // Handle Cheese
        foreach (Collider hit in cheeseHits)
        {
            detectedCheese.Add(hit.transform);
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = hit.transform;
            }
            ShowRadarIcon(hit.transform);
        }

        // Handle Batteries
        foreach (Collider hit in batteryHits)
        {
            detectedCheese.Add(hit.transform);
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = hit.transform;
            }
            ShowRadarIcon(hit.transform);
        }

        // Handle Keys
        foreach (Collider hit in keyHits)
        {
            detectedCheese.Add(hit.transform);
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = hit.transform;
            }
            ShowRadarIcon(hit.transform);
        }

        // Audio ping
        if ((cheeseHits.Length > 0 || batteryHits.Length > 0 || keyHits.Length > 0) && pingAudio != null)
        {
            pingAudio.Play();
        }

        nearestCheese = closest;
        StartCoroutine(HideRadarIconsAfterDelay(10f));
    }

    void ShowRadarIcon(Transform obj)
    {
        Transform icon = obj.transform.Find("Add-OnMinimapIcon");
        if (icon != null)
        {
            icon.gameObject.SetActive(true);
            Transform ping = icon.Find("PingWave");
            if (ping != null) ping.gameObject.SetActive(true);
        }
    }

    IEnumerator HideRadarIconsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform cheese in detectedCheese)
        {
            Transform icon = cheese.Find("Add-OnMinimapIcon");
            if (icon != null)
            {
                icon.gameObject.SetActive(false);
                Transform ping = icon.Find("PingWave");
                if (ping != null) ping.gameObject.SetActive(false);
            }
        }

        detectedCheese.Clear();
    }

    IEnumerator RetrieveCheese(Transform cheese)
    {
        Collider cheeseCollider = cheese.GetComponent<Collider>();
        if (cheeseCollider != null) cheeseCollider.enabled = false;

        Rigidbody rb = cheese.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 start = cheese.position;
        Vector3 end = webOrigin.position;

        if (webLineRenderer != null && webOrigin != null)
        {
            webLineRenderer.positionCount = 2;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            Vector3 newPos = Vector3.Lerp(start, end, t);
            cheese.position = newPos;

            if (webLineRenderer != null)
            {
                webLineRenderer.SetPosition(0, webOrigin.position);
                webLineRenderer.SetPosition(1, cheese.position);
            }

            yield return null;
        }

        cheese.position = end;

        if (webLineRenderer != null)
        {
            yield return new WaitForSeconds(0.2f);
            webLineRenderer.positionCount = 0;
        }

        InventorySystem inventory = FindFirstObjectByType<InventorySystem>();
        if (inventory != null)
        {
            bool added = false;

            if (cheese.CompareTag("Battery"))
            {
                added = inventory.AddBattery();
            }
            else if (cheese.CompareTag("Cheese"))
            {
                added = inventory.AddCheese();
            }
            else if (cheese.CompareTag("Bag"))
            {
                if (!inventory.HasBag())
                {
                    inventory.AddBag();

                    // Enable transfer button in cabinet
                    CabinetUIManager cabinet = FindObjectOfType<CabinetUIManager>();
                    if (cabinet != null)
                        cabinet.EnableTransferButton();

                    Debug.Log("Bag collected! Transfer button enabled.");
                }
                added = true;
            }
            else if (cheese.CompareTag("Key"))
            {
                //InventorySystem inventory = FindFirstObjectByType<InventorySystem>();
                if (inventory != null)
                {
                    Debug.Log("Attempting to add key to inventory...");
                    added = inventory.AddKey();
                    if (added)
                    {
                        Debug.Log("Key successfully added!");
                        Destroy(cheese.gameObject); // only destroy if successfully added
                    }
                    else
                    {
                        Debug.LogWarning("Key could not be added to inventory.");
                    }
                }
            }


            if (!added)
            {
                if (inventoryFullWarningUI != null)
                    StartCoroutine(ShowInventoryFullWarning());

            }

            Destroy(cheese.gameObject);
            nearestCheese = null;
        }
        cheese.GetComponent<Collider>().enabled = true;
        Rigidbody cheeseRb = cheese.GetComponent<Rigidbody>();
        if (cheeseRb != null) cheeseRb.isKinematic = false;

        yield break;
    }


    private IEnumerator ShowInventoryFullWarning()
    {
        inventoryFullWarningUI.SetActive(true);
        yield return new WaitForSeconds(5f);
        inventoryFullWarningUI.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radarRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, retrieveRange);
    }
}