using UnityEngine;

public class GameExit : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();

        // This line is just for the editor (won't affect builds)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
