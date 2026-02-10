using UnityEngine;
using System.Collections;

public class CabinetSystem : MonoBehaviour
{
    public CabinetUIManager cabinetUIManager;
    public InventorySystem scrapInventory;
    public AudioSource openSound;
    public Animator cabinetAnimator;
    public GameObject cabinetPromptUI;

    private bool isPlayerNearby = false;
    private bool isCabinetOpen = false;
    private bool isAnimating = false;
    private bool itemsTransferred = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.O))
        {
            if (!cabinetUIManager.IsCabinetLocked() && !isAnimating)
            {
                ToggleCabinet();
            }
        }
    }

    private void ToggleCabinet()
    {
        isCabinetOpen = !isCabinetOpen;
        isAnimating = true;

        cabinetAnimator.enabled = true;
        cabinetAnimator.SetBool("isOpen", isCabinetOpen);

        if (isCabinetOpen)
        {
            openSound?.Play();
            cabinetUIManager.ToggleCabinetUI(true);
            cabinetPromptUI.SetActive(false);
        }
        else
        {
            cabinetUIManager.ToggleCabinetUI(false);
        }

        // Adjust delay based on your actual open/close animation lengths
        StartCoroutine(EndAnimationAfterDelay(1f));
    }

    private IEnumerator EndAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAnimating = false;
        cabinetAnimator.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!isCabinetOpen)
                cabinetPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            cabinetPromptUI.SetActive(false);

            if (isCabinetOpen)
            {
                CloseCabinet();
            }
        }
    }

    public void CloseCabinet()
    {
        if (!isCabinetOpen) return;

        isCabinetOpen = false;
        isAnimating = true;

        cabinetUIManager.ToggleCabinetUI(false);
        cabinetAnimator.enabled = true;
        cabinetAnimator.SetBool("isOpen", false);
        StartCoroutine(EndAnimationAfterDelay(1f));
    }

    public void MarkTransferDone()
    {
        itemsTransferred = true;
    }
}

