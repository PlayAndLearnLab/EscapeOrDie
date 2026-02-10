using UnityEngine;
using System.Collections;

public class MapNodeRevealer : MonoBehaviour
{
    public static MapNodeRevealer Instance;

    [Tooltip("Assign each room node UI manually")]
    public GameObject[] roomNodes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RevealNode(string roomID)
    {
        foreach (GameObject node in roomNodes)
        {
            if (node.name.Contains(roomID))
            {
                StartCoroutine(FadeInNode(node));
                Debug.Log("Revealed map node: " + roomID);
                return;
            }
        }
        Debug.LogWarning("Room node not found: " + roomID);
    }

    private IEnumerator FadeInNode(GameObject node)
    {
        node.SetActive(true);

        CanvasGroup cg = node.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.LogWarning("Missing CanvasGroup on node: " + node.name);
            yield break;
        }

        float duration = 1.5f;
        float time = 0f;

        cg.alpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }
}