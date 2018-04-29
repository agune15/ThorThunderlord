using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePrefabBehaviour : MonoBehaviour {

    [SerializeField] List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    float activeCount;

    private void Start()
    {
        particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
    }

    private void Update()
    {
        activeCount = 0;

        foreach (ParticleSystem particle in particleSystems)
        {
            if (particle.IsAlive()) activeCount++;
        }

        if (activeCount < 1) Destroy(this.gameObject);
    }
}
