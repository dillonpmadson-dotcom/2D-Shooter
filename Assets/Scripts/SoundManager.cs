using UnityEngine;

// Centralized sound playback. All sound clips are generated PROCEDURALLY at startup
// using simple wave math — no external audio files needed.
// Other scripts call SoundManager.Instance.PlayShoot() etc. to make sounds.
public class SoundManager : MonoBehaviour
{
    // Singleton — easy global access
    public static SoundManager Instance { get; private set; }

    [SerializeField] private float volume = 0.3f;

    // The procedurally-generated clips
    private AudioClip shootClip;
    private AudioClip enemyDeathClip;
    private AudioClip explosionClip;
    private AudioClip pickupClip;
    private AudioClip playerHurtClip;

    // The AudioSource that plays our SFX
    private AudioSource sfxSource;

    void Awake()
    {
        // Singleton setup — only one allowed in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Add an AudioSource to play SFX
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Build all the procedural clips up front (cheap)
        shootClip       = MakeBlip(freqStart: 1200f, freqEnd: 400f, duration: 0.10f, type: WaveType.Square);
        enemyDeathClip  = MakeBlip(freqStart: 600f,  freqEnd: 100f, duration: 0.20f, type: WaveType.Triangle);
        pickupClip      = MakeBlip(freqStart: 600f,  freqEnd: 1400f,duration: 0.20f, type: WaveType.Square);
        explosionClip   = MakeNoiseBurst(duration: 0.5f);
        playerHurtClip  = MakeBlip(freqStart: 200f,  freqEnd: 80f,  duration: 0.25f, type: WaveType.Square);
    }

    // ===== Public API — call these from anywhere =====

    public void PlayShoot()       { sfxSource.PlayOneShot(shootClip, volume * 0.5f); }
    public void PlayEnemyDeath()  { sfxSource.PlayOneShot(enemyDeathClip, volume); }
    public void PlayExplosion()   { sfxSource.PlayOneShot(explosionClip, volume); }
    public void PlayPickup()      { sfxSource.PlayOneShot(pickupClip, volume); }
    public void PlayPlayerHurt()  { sfxSource.PlayOneShot(playerHurtClip, volume); }

    // ===== Procedural sound generation =====

    private enum WaveType { Sine, Square, Triangle, Sawtooth }

    // Creates a "blip" — a tone that sweeps from one frequency to another
    private AudioClip MakeBlip(float freqStart, float freqEnd, float duration, WaveType type)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float phase = 0f;
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            // Frequency sweep from start to end
            float freq = Mathf.Lerp(freqStart, freqEnd, t);
            // Volume envelope — fades out so it doesn't click
            float envelope = 1f - t;

            phase += freq / sampleRate;
            if (phase > 1f) phase -= 1f;

            float sample = WaveSample(phase, type) * envelope;
            samples[i] = sample;
        }

        AudioClip clip = AudioClip.Create("blip", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Creates an explosion — a burst of white noise that fades out
    private AudioClip MakeNoiseBurst(float duration)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float envelope = 1f - t;
            samples[i] = (Random.value * 2f - 1f) * envelope;
        }

        AudioClip clip = AudioClip.Create("explosion", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Returns one sample of the chosen wave at the given phase (0 to 1)
    private float WaveSample(float phase, WaveType type)
    {
        switch (type)
        {
            case WaveType.Sine:     return Mathf.Sin(phase * 2f * Mathf.PI);
            case WaveType.Square:   return phase < 0.5f ? 1f : -1f;
            case WaveType.Triangle: return 1f - 4f * Mathf.Abs(phase - 0.5f);
            case WaveType.Sawtooth: return 2f * phase - 1f;
            default: return 0f;
        }
    }
}
