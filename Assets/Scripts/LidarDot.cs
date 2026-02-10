using UnityEngine;
using System.Collections;


public class LidarDot : MonoBehaviour
{
    public float lifetime = 5f;
    private Material matInstance;

    void Start()
    {
        matInstance = GetComponent<Renderer>().material;
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        float elapsed = 0f;
        Color startColor = matInstance.color;

        while (elapsed < lifetime)
        {
            float t = elapsed / lifetime;
            matInstance.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
