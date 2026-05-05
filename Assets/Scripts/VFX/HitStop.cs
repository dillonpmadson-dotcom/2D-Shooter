using System.Collections;
using UnityEngine;

// Briefly freezes time on impacts. Like a punctuation mark for big moments.
// Call HitStop.Instance.Freeze(durationInSeconds) from anywhere.
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // duration in REAL seconds (will pause the game for this long)
    public void Freeze(float duration = 0.08f)
    {
        StopAllCoroutines();
        StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        Time.timeScale = 0f;
        // WaitForSecondsRealtime ignores timeScale — uses wall-clock time
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
