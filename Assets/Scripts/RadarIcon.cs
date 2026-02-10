using UnityEngine;

public class RadarIcon : MonoBehaviour
{
    public Transform target;
    public RectTransform minimapRect;
    public Transform radarOrigin;
    public float mapScale = 5f;

    void Update()
    {
        if (target == null || radarOrigin == null || minimapRect == null) return;

        Vector3 offset = target.position - radarOrigin.position;
        Vector2 iconPos = new Vector2(offset.x, offset.z) * mapScale;
        ((RectTransform)transform).anchoredPosition = iconPos;
    }
}
