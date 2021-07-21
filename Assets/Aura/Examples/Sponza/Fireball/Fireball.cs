using UnityEngine;

public class Fireball : MonoBehaviour
{
    private ParticleSystem _emitter;
    public AnimationCurve curve;
    public float rotationSpeed;

    private void Start()
    {
        _emitter = GetComponent<ParticleSystem>();
        _emitter.maxParticles = Mathf.CeilToInt(_emitter.emissionRate * _emitter.startLifetime);
    }

    private void LateUpdate()
    {
        Color particleColor;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_emitter.particleCount];
        _emitter.GetParticles(particles);
        for(int i = 0; i < particles.Length; ++i)
        {
            particles[i].size = curve.Evaluate(1.0f - particles[i].remainingLifetime / particles[i].startLifetime) * _emitter.startSize;
            particleColor = particles[i].color;
            particleColor.a = i / (float)_emitter.maxParticles % 1;
            particles[i].color = particleColor;
            particles[i].rotation += (particleColor.a > 0.5f ? 1 : -1) * rotationSpeed * Time.deltaTime / particles[i].startLifetime;
        }

        _emitter.SetParticles(particles, particles.Length);
    }
}
