using UnityEngine;

// Configures a ParticleSystem to be a quick one-shot burst, then self-destructs.
[RequireComponent(typeof(ParticleSystem))]
public class BurstParticle : MonoBehaviour
{
    [SerializeField] private int burstCount = 25;
    [SerializeField] private float startSpeed = 6f;
    [SerializeField] private float startSize = 0.25f;
    [SerializeField] private float startLifetime = 0.6f;
    [SerializeField] private Color particleColor = Color.white;

    private ParticleSystem ps;

    // Public setter — DeathParticleSpawner uses this to tint the burst per enemy
    public void SetColor(Color newColor)
    {
        particleColor = newColor;

        // If the particle system is already running, update its main module live
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = newColor;
        }
    }

    // Start runs after Awake of the spawner that called SetColor on us
    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // Stop any in-progress playback first — Unity won't let you change
        // duration while the system is running
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Main module — overall particle settings
        var main = ps.main;
        main.loop = false;
        main.duration = 0.5f;
        main.startLifetime = startLifetime;
        main.startSpeed = startSpeed;
        main.startSize = startSize;
        main.startColor = particleColor;
        main.maxParticles = burstCount + 5;
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // Emission — one burst, then nothing
        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, burstCount)
        });

        // Shape — sphere so particles fly in all directions
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // Restart so the burst plays with the new settings
        ps.Clear();
        ps.Play();
    }
}
