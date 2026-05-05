using UnityEngine;
using TMPro;

// Floating "+10" text that drifts up and fades out, then self-destructs
public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 1f;

    private float elapsed = 0f;
    private Color startColor;

    void Awake()
    {
        // Auto-grab the TMP text if not assigned (it should be on this same GameObject)
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null) startColor = textMesh.color;
    }

    public void SetValue(int value)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textMesh.text = "+" + value;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Drift upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out over the lifetime
        if (textMesh != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        // Self-destruct when done
        if (elapsed >= lifetime) Destroy(gameObject);
    }
}
