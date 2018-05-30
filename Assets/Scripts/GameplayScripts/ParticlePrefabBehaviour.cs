using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePrefabBehaviour : MonoBehaviour {

    public enum ParticleEndAction { Destroy, Deactivate }
    public ParticleEndAction particleEndAction;

    private ParticleInstancer particleInstancer = null;

    [HideInInspector] public string particleTag;

    [SerializeField] List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    float activeCount;

    private void Start()
    {
        if (GetComponent<ParticleSystem>() != null) particleSystems.Add(GetComponent<ParticleSystem>());
        if (GetComponentsInChildren<ParticleSystem>().Length > 0) particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());

        particleInstancer = GameObject.FindWithTag("ParticleInstancer").GetComponent<ParticleInstancer>();
    }

    private void Update()
    {
        activeCount = 0;

        foreach (ParticleSystem particle in particleSystems)
        {
            if (particle.IsAlive()) activeCount++;
        }

        if (activeCount < 1)
        {
            switch (particleEndAction)
            {
                case ParticleEndAction.Destroy:
                    particleInstancer.DestroyParticleSystem(gameObject);
                    break;
                case ParticleEndAction.Deactivate:
                    particleInstancer.UnpoolParticleSystem(particleTag, gameObject);
                    break;
                default:
                    break;
            }
        }
    }

    public void ResetParticleSystem ()
    {
        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Clear();
            particle.Play();
        }
    }

    private void OnEnable()
    {
        ResetParticleSystem();
    }
}
