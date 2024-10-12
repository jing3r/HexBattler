using UnityEngine;

public class PulsatingEffect : MonoBehaviour
{
    public float minScale = 0.95f;
    public float maxScale = 1.05f;
    public float pulseSpeed = 2f;
    private Vector3 originalScale;
    private bool isPulsating = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isPulsating)
        {
            float scaleFactor = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * pulseSpeed, 1));
            transform.localScale = originalScale * scaleFactor;
        }
    }

    public void StartPulsating()
    {
        isPulsating = true;
    }

    public void StopPulsating()
    {
        isPulsating = false;
        transform.localScale = originalScale;
    }
}
