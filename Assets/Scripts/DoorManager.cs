using UnityEngine;
using UnityEngine.UI;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private GameObject popupUI;
    [SerializeField] private Button openDoorButton;

    private DoorController currentDoor = null;

    void Start()
    {
        popupUI.SetActive(false);
        openDoorButton.onClick.AddListener(OpenCurrentDoor);
    }

    void Update()
    {
        // Optional keyboard support
        if (currentDoor != null && Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Pressed O - trying to open door");
            OpenCurrentDoor();
        }
    }

    public void OpenCurrentDoor()
    {
        if (currentDoor != null && !currentDoor.isOpen)
        {
            currentDoor.Open();
            popupUI.SetActive(false);
            currentDoor = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            DoorController door = other.GetComponent<DoorController>();
            if (door != null && !door.isOpen)
            {
                currentDoor = door;
                popupUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            DoorController door = other.GetComponent<DoorController>();
            if (door == currentDoor)
            {
                popupUI.SetActive(false);
                currentDoor = null;
            }
        }
    }
}
