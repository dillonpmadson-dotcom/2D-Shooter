using UnityEngine;

// Centralized sound playback. All sound clips are generated PROCEDURALLY at startup
// using simple wave math — no external audio files needed.
// Other scripts call SoundManager.Instance.PlayShoot() etc. to make sounds.
public class SoundManager : MonoBehaviour
{
    // Singleton — easy global access
    public static SoundManager Instance { get; private set; }

    [SerializeField] private float volume = 0.3f;

    [Header("Background Music")]
    [Tooltip("Drop an audio clip here to use as background music. Leave empty for procedural ambient.")]
    [SerializeField] private AudioClip backgroundMusic;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.4f;

    // The procedurally-generated clips
    private AudioClip shootClip;
    private AudioClip enemyDeathClip;
    private AudioClip explosionClip;
    private AudioClip pickupClip;
    private AudioClip playerHurtClip;

    // The AudioSource that plays our SFX
    private AudioSource sfxSource;
    private AudioSource musicSource;

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

        // Add a separate AudioSource for looping background music
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        musicSource.volume = musicVolume;

        // Use the imported clip if assigned, else fall back to procedural ambient
        musicSource.clip = backgroundMusic != null ? backgroundMusic : MakeAmbientLoop();
        musicSource.Play();

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

    // Builds a cosmic ambient track — chord progression with arpeggios and pads.
    // It's not Hans Zimmer, but it actually sounds like SPACE music.
    private AudioClip MakeAmbientLoop()
    {
        int sampleRate = 44100;
        float duration = 32f; // 32-second loop = enough variation to not feel repetitive
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        // Chord progression — Am, F, C, G (8 seconds each)
        // Each chord is defined by its root + 5th + octave (frequencies in Hz)
        float[][] chords = new float[][]
        {
            new float[] { 110f, 164.81f, 220f },  // A minor
            new float[] { 87.31f, 130.81f, 174.61f }, // F major
            new float[] { 130.81f, 196f, 261.63f },   // C major
            new float[] { 98f, 146.83f, 196f }        // G major
        };

        float chordDuration = duration / chords.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            int chordIndex = Mathf.Min((int)(t / chordDuration), chords.Length - 1);
            float[] chord = chords[chordIndex];

            // Pad layer — sine waves on the chord notes with a 2-sec swell envelope
            // (so notes don't drone for the full 8 seconds — they breathe)
            float padBeat = 2f; // pad pulses every 2 seconds
            float padBeatProgress = (t % padBeat) / padBeat;
            // Bell-shaped envelope: rises then falls within each 2-second window
            float padEnvelope = Mathf.Sin(padBeatProgress * Mathf.PI);

            float pad = 0f;
            foreach (float freq in chord)
            {
                pad += Mathf.Sin(t * 2f * Mathf.PI * freq);
            }
            pad = (pad / chord.Length) * padEnvelope;

            // Arpeggio layer — picks one note per beat (4 beats per chord)
            float beatDuration = chordDuration / 4f;
            int beatInChord = (int)((t % chordDuration) / beatDuration);
            float arpeggioFreq = chord[beatInChord % chord.Length] * 2f; // octave up
            float beatProgress = ((t % chordDuration) % beatDuration) / beatDuration;
            // Pluck envelope — sharp attack, slow decay
            float pluckEnvelope = Mathf.Exp(-beatProgress * 4f);
            float arpeggio = Mathf.Sin(t * 2f * Mathf.PI * arpeggioFreq) * pluckEnvelope * 0.3f;

            // Slow LFO for the pad volume — creates the "breathing" feel
            float padSwell = 0.6f + 0.4f * Mathf.Sin(t * Mathf.PI * 0.1f);

            // Mix the layers
            samples[i] = (pad * padSwell * 0.25f) + (arpeggio * 0.2f);
        }

        AudioClip clip = AudioClip.Create("cosmic_ambient", sampleCount, 1, sampleRate, false);
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
