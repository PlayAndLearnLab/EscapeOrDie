using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public int nodeIndex;  // Index of the corresponding node in the manager
    private bool hasEntered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasEntered && other.CompareTag("Player"))
        {
            hasEntered = true;

            NodeMapManager mapManager = FindObjectOfType<NodeMapManager>();
            if (mapManager != null)
            {
                mapManager.RevealNode(nodeIndex);
            }
        }
    }
}
