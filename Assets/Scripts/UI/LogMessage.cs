using TMPro;
using UnityEngine;
using static GameConstants;

/// <summary>A single in-world log line that hides itself after a while. Shown through Utils.SendLogMessage.</summary>
public class LogMessage : MonoBehaviour
{
    [SerializeField] TextMeshPro tmpro;
    float lifespan;

    /// <summary>Shows the text as the newest (last) message and restarts the countdown.</summary>
    public void Display(string text)
    {
        lifespan = MessageLifespan;
        tmpro.text = text;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0) gameObject.SetActive(false);
    }
}
