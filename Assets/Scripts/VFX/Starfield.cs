using UnityEngine;

// Generates a procedural starfield — random tiny stars scattered across a big area.
// With bloom on, these glow like real stars in space.
public class Starfield : MonoBehaviour
{
    [Header("How Many")]
    [SerializeField] private int starCount = 400;

    [Header("Area (centered on this transform)")]
    [SerializeField] private float fieldWidth = 100f;
    [SerializeField] private float fieldHeight = 100f;

    [Header("Star Look")]
    [SerializeField] private Sprite starSprite;          // Drag a circle sprite here, or leave blank to auto-find
    [SerializeField] private float minSize = 0.05f;
    [SerializeField] private float maxSize = 0.15f;
    [SerializeField] private int sortingOrder = -10;     // Behind everything
    [SerializeField] private Color tintA = new Color(1, 1, 1, 0.6f);
    [SerializeField] private Color tintB = new Color(0.7f, 0.85f, 1f, 0.5f); // bluish

    void Start()
    {
        // If no sprite assigned, generate a small white circle texture procedurally
        if (starSprite == null)
        {
            starSprite = CreateCircleSprite(32);
        }

        for (int i = 0; i < starCount; i++)
        {
            CreateStar();
        }
    }

    // Builds a soft white circle sprite at runtime (no external asset needed)
    private Sprite CreateCircleSprite(int diameter)
    {
        Texture2D tex = new Texture2D(diameter, diameter);
        tex.filterMode = FilterMode.Bilinear;

        float radius = diameter / 2f;
        Vector2 center = new Vector2(radius, radius);

        Color[] pixels = new Color[diameter * diameter];
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                // Smooth falloff from center to edge
                float alpha = Mathf.Clamp01(1f - dist / radius);
                pixels[y * diameter + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f), diameter);
    }

    private void CreateStar()
    {
        // Spawn a new GameObject at a random spot inside the field
        GameObject star = new GameObject("Star");
        star.transform.parent = transform;
        star.transform.localPosition = new Vector3(
            Random.Range(-fieldWidth / 2f, fieldWidth / 2f),
            Random.Range(-fieldHeight / 2f, fieldHeight / 2f),
            0f
        );

        // Random size for variety
        float size = Random.Range(minSize, maxSize);
        star.transform.localScale = new Vector3(size, size, 1f);

        // Add a SpriteRenderer with the star sprite
        SpriteRenderer sr = star.AddComponent<SpriteRenderer>();
        sr.sprite = starSprite;
        sr.sortingOrder = sortingOrder;

        // Mix between two tints so some stars look bluish, some white
        sr.color = Random.value < 0.5f ? tintA : tintB;
    }
}
