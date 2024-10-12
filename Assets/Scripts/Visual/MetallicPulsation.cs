using UnityEngine;

public class MetallicPulsation : MonoBehaviour
{
    private Material unitMaterial;
    private float pulseSpeed = 1.0f;
    private float pulseTimer = 0f;
    private float startMetallic = 0f;
    private float maxMetallic = 0.4f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            unitMaterial = renderer.material;
        }
    }

    void Update()
    {
        if (unitMaterial != null)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float metallicValue = Mathf.Lerp(startMetallic, maxMetallic, Mathf.PingPong(pulseTimer, 1));
            unitMaterial.SetFloat("_Metallic", metallicValue);
        }
    }


    public void StartPulsating()
    {
        pulseTimer = 0f;
    }

    public void StopPulsating()
    {
        if (unitMaterial != null)
        {
            unitMaterial.SetFloat("_Metallic", startMetallic);
        }
    }
}
