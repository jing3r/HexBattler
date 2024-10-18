using UnityEngine;

public class HighlightController : MonoBehaviour
{
    public GameObject hexHighlight;
    public GameObject circularHighlight;

    private GameObject activeHexHighlight;
    private GameObject activeCircularHighlight;

    void Start()
    {
        if (hexHighlight != null)
        {
            activeHexHighlight = Instantiate(hexHighlight);
            activeHexHighlight.SetActive(false);
        }

        if (circularHighlight != null)
        {
            activeCircularHighlight = Instantiate(circularHighlight);
            activeCircularHighlight.SetActive(false);
        }
    }

    public void EnableHexHighlight(Vector3 position)
    {
        if (activeHexHighlight != null)
        {
            activeHexHighlight.transform.position = position + Vector3.up * 0.1f;
            activeHexHighlight.SetActive(true);
        }
    }

    public void DisableHexHighlight()
    {
        if (activeHexHighlight != null)
        {
            activeHexHighlight.SetActive(false);
        }
    }

    public void EnableCircularHighlight(Vector3 position)
    {
        if (activeCircularHighlight != null)
        {
            activeCircularHighlight.transform.position = position + Vector3.up * 0.1f;
            activeCircularHighlight.SetActive(true);
        }
    }

    public void DisableCircularHighlight()
    {
        if (activeCircularHighlight != null)
        {
            activeCircularHighlight.SetActive(false);
        }
    }
}
