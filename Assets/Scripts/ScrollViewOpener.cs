using UnityEngine;

public class ScrollViewOpener : MonoBehaviour
{
    public GameObject scrollView; // Assign in Inspector

    public void ShowScrollView()
    {
        scrollView.SetActive(true);
    }

    public void HideScrollView()
    {
        scrollView.SetActive(false);
    }
}

