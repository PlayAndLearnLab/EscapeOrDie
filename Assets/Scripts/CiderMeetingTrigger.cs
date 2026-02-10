using UnityEngine;
using System.Collections;
using HeneGames.DialogueSystem;

public class CiderMeetingTrigger : MonoBehaviour
{
    public DialogueManager ciderDialogue;
    public GameObject ciderSceneModel; // The one in the world
    public GameObject ciderBackPrefab; // The one that appears on Scrap's back
    public GameObject RadarCanBeFound;
    public GameObject AddSlotsInventory; //to notify that cider is attached and there are another 2 slots

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player")) // Make sure Scrap has this tag
        {
            triggered = true;
            DialogueUI.instance.StartDialogue(ciderDialogue);
            StartCoroutine(WaitForDialogueThenReplaceCider());
        }
    }

    private IEnumerator WaitForDialogueThenReplaceCider()
    {
        // Wait until dialogue finishes
        while (DialogueUI.instance.IsProcessingDialogue() || DialogueUI.instance.IsTyping())
        {
            yield return null;
        }

        // Hide the scene model of Cider
        ciderSceneModel.SetActive(false);

        // Show radar in the scene
        RadarCanBeFound.SetActive(true);
        Debug.Log("Radar pickup activated");

        // Show the Cider prefab on Scrap's back
        ciderBackPrefab.SetActive(true);

        //add extra slots
        AddSlotsInventory.GetComponent<InventorySystem>().AddCider();


    }
}
