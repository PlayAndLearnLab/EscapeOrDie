using UnityEngine;

public class LookX1 : MonoBehaviour
{
    [SerializeField]
    private float _sensitivity = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float _xmouseX = Input.GetAxis("Mouse X");

        Vector3 newRotation = transform.localEulerAngles;
        newRotation.y += _xmouseX * _sensitivity;
        transform.localEulerAngles = newRotation;

    }
}
