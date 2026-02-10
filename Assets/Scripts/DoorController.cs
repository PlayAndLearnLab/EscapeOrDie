using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openAnimationStateName = "Open"; // This should match the actual state name in Animator

    public bool isOpen = false;

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;

        animator.enabled = true;
        animator.Play(openAnimationStateName);
    }
}


