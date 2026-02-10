using UnityEngine;

public class PingWave : MonoBehaviour
{
    public float duration = 1f;
    public float maxScale = 2f;
    public float startAlpha = 0.6f;

    private SpriteRenderer sr;
    private float timer;

    void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        timer = 0f;
        transform.localScale = Vector3.zero;
        SetAlpha(startAlpha);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        // Scale up
        float scale = Mathf.Lerp(0f, maxScale, t);
        transform.localScale = new Vector3(scale, scale, scale);

        // Fade out
        float alpha = Mathf.Lerp(startAlpha, 0f, t);
        SetAlpha(alpha);

        if (timer >= duration)
        {
            timer = 0f;
            transform.localScale = Vector3.zero;
            SetAlpha(startAlpha);
        }
    }

    void SetAlpha(float a)
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }
}
