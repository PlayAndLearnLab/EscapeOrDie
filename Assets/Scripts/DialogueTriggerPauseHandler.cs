using UnityEngine;
using HeneGames.DialogueSystem;

public class DialogueTriggerPauseHandler : MonoBehaviour
{
    [Header("Assign the specific DialogueManager manually")]
    public DialogueManager dialogueManager;

    [Header("References to pause")]
    public GameObject player;
    public GameObject[] robots;

    private bool dialogueStarted = false;

    // Cache components for re-enabling
    private MonoBehaviour[] playerScripts;
    private MonoBehaviour[][] robotScripts;

    void Start()
    {
        if (dialogueManager != null)
        {
            dialogueManager.startDialogueEvent.AddListener(PauseGame);
            dialogueManager.endDialogueEvent.AddListener(ResumeGameAndDestroy);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} is missing a reference to a DialogueManager.");
        }

        // Cache player scripts (customize this list as needed)
        playerScripts = player.GetComponents<MonoBehaviour>();

        // Cache robot scripts
        robotScripts = new MonoBehaviour[robots.Length][];
        for (int i = 0; i < robots.Length; i++)
        {
            robotScripts[i] = robots[i].GetComponents<MonoBehaviour>();
        }
    }

    void PauseGame()
    {
        if (!dialogueStarted) return;

        // Disable player scripts
        foreach (var script in playerScripts)
        {
            if (script != this) script.enabled = false;
        }

        // Disable robot scripts
        foreach (var robot in robotScripts)
        {
            foreach (var script in robot)
            {
                script.enabled = false;
            }
        }
    }

    void ResumeGameAndDestroy()
    {
        if (!dialogueStarted) return;

        // Re-enable player scripts
        foreach (var script in playerScripts)
        {
            if (script != this) script.enabled = true;
        }

        // Re-enable robot scripts
        foreach (var robot in robotScripts)
        {
            foreach (var script in robot)
            {
                script.enabled = true;
            }
        }

        // Destroy this trigger
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !dialogueStarted)
        {
            dialogueStarted = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dialogueStarted)
        {
            dialogueStarted = true;
        }
    }
}
