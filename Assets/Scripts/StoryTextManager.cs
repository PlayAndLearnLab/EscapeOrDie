using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryTextController : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public Button nextButton;
    public Animator animationToPlay; 
    [TextArea(3, 10)]
    public string[] storyParagraphs;

    public Image fadeImage;
    public float fadeDuration = 1f;


    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        nextButton.onClick.AddListener(NextParagraph);
        ShowParagraph(currentIndex);
    }

    void NextParagraph()
    {
        if (isTransitioning)
            return;

        // If we're on the last paragraph already
        if (currentIndex >= storyParagraphs.Length - 1)
        {
            //Optional: Play a final animation
            if (animationToPlay != null)
            {
                animationToPlay.SetTrigger("OnNext");
            }

            // Load next scene after animation delay (adjust if needed)
            StartCoroutine(LoadNextSceneWithFade("Test"));
            return;
        }

        currentIndex++;

        if (animationToPlay != null)
        {
            animationToPlay.SetTrigger("OnNext");
        }

        StartCoroutine(FadeText(storyParagraphs[currentIndex]));
    }

    void ShowParagraph(int index)
    {
        storyText.text = storyParagraphs[index];
        storyText.alpha = 1f;
    }

    IEnumerator Fade(float targetAlpha)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }



    IEnumerator FadeText(string newText)
    {
        isTransitioning = true;

        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            storyText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        storyText.text = newText;

        // Fade in
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            storyText.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        isTransitioning = false;
    }
    IEnumerator LoadNextSceneWithFade(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Replace "NextSceneName" with the actual scene name
        SceneManager.LoadScene("NextSceneName");
    }
}
