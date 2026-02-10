using UnityEngine;

public class RadarPickup : MonoBehaviour
{
    public GameObject cider;

    private void Start()
    {
        //this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // or "Cider" if you're specific
        {
            CiderController controller = cider.GetComponent<CiderController>();
            if (controller != null)
            {
                controller.EnableRadar();
            }

            

            Destroy(gameObject); // Remove the pickup from the scene
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }


}

