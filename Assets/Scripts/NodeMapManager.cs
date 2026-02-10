using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeMapManager : MonoBehaviour
{
    public GameObject nodeMapPanel;          // Panel to show/hide
    public List<GameObject> roomNodes;       // Node icons (linked to rooms)

    private HashSet<int> revealedNodeIndices = new HashSet<int>();

    public void ToggleNodeMapVisibility()
    {
        nodeMapPanel.SetActive(!nodeMapPanel.activeSelf);
    }

    public void RevealNode(int index)
    {
        if (index >= 0 && index < roomNodes.Count)
        {
            if (!revealedNodeIndices.Contains(index))
            {
                revealedNodeIndices.Add(index);
                roomNodes[index].SetActive(true);
            }
        }
    }
}
