using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    [SerializeField] private Image redOverlayImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    public void ShowDamageEffect()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        redOverlayImage.color = new Color(1, 0, 0, 0.5f); // red with 50% alpha

        float elapsed = 0f;
        Color startColor = redOverlayImage.color;
        Color endColor = new Color(1, 0, 0, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            redOverlayImage.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            yield return null;
        }

        redOverlayImage.color = endColor;
    }
}
