using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StairTrigger : MonoBehaviour
{
    public Transform firstFloorPoint;
    public Transform secondFloorPoint;
    public GameObject player;
    public GameObject messageUI;
    public Image fadeImage;
    public FloorVisibilityController floorVisibilityController;

    public bool isThisSecondFloorTrigger = false; // set this per trigger
    public bool setInitialFloor = false;
    private bool isPlayerInside = false;
    private bool isTeleporting = false;
    private bool currentFloorIsSecond = false; // tracks Scrap's current floor

    private void Start()
    {

        if (setInitialFloor)
            PlayerFloorManager.CurrentFloor = currentFloorIsSecond ? 2 : 1;

        if (messageUI != null)
            messageUI.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        currentFloorIsSecond = isThisSecondFloorTrigger;

        // Only the designated trigger sets the initial furniture visibility
        if (setInitialFloor && floorVisibilityController != null)
        {
            floorVisibilityController.SetFloor(currentFloorIsSecond ? 2 : 1);
        }
    }

    public static class PlayerFloorManager
    {
        public static int CurrentFloor = 2; // Start on second floor by default
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = true;
        messageUI?.SetActive(true);

        // Update floor state based on which trigger this is
        currentFloorIsSecond = isThisSecondFloorTrigger;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = false;
        messageUI?.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInside && !isTeleporting && Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(TeleportWithFade());
        }
    }

    private IEnumerator TeleportWithFade()
    {
        isTeleporting = true;

        yield return StartCoroutine(Fade(0f, 1f));
        yield return new WaitForSeconds(0.2f);

        // Determine target floor
        Vector3 targetPos = currentFloorIsSecond ? firstFloorPoint.position : secondFloorPoint.position;
        Debug.Log("Teleporting to: " + targetPos);

        // Disable CharacterController or Rigidbody if needed
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Move player
        player.transform.position = targetPos;

        // Re-enable physics
        if (cc != null) cc.enabled = true;
        if (rb != null) rb.isKinematic = false;

        // Update floor state
        currentFloorIsSecond = !currentFloorIsSecond;
        PlayerFloorManager.CurrentFloor = currentFloorIsSecond ? 2 : 1;
        Debug.Log("Now on " + (currentFloorIsSecond ? "second" : "first") + " floor");

        // Call floor visibility update
        if (floorVisibilityController != null)
        {
            floorVisibilityController.SetFloor(currentFloorIsSecond ? 2 : 1);
        }

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(Fade(1f, 0f));
        yield return new WaitForSeconds(1f);

        isTeleporting = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        float time = 0f;
        Color c = fadeImage.color;

        while (time < 0.5f)
        {
            c.a = Mathf.Lerp(from, to, time / 0.5f);
            fadeImage.color = c;
            time += Time.deltaTime;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}
