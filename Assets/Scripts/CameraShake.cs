using System.Collections;
using UnityEngine;

// Camera shake — call CameraShake.Instance.Shake(intensity, duration) from anywhere
// Adds offsets to the camera position then smoothly returns to normal
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 currentShakeOffset = Vector3.zero;

    void Awake()
    {
        Instance = this;
    }

    // Public API — call this to shake the camera
    public void Shake(float intensity = 0.3f, float duration = 0.3f)
    {
        // Stop any existing shake before starting a new one
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Random offset within a circle, scaled by remaining intensity
            float remainingPercent = 1f - (elapsed / duration);
            currentShakeOffset = (Vector3)Random.insideUnitCircle * intensity * remainingPercent;
            elapsed += Time.deltaTime;
            yield return null; // wait one frame
        }
        currentShakeOffset = Vector3.zero;
    }

    // Other scripts can read this to apply the offset (CameraFollow uses it)
    public Vector3 GetCurrentOffset()
    {
        return currentShakeOffset;
    }
}
