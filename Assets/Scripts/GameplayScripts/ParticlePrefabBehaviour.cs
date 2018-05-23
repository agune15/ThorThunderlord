using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePrefabBehaviour : MonoBehaviour {

    public enum ParticleEndAction { Destroy, Deactivate }
    public ParticleEndAction particleEndAction;

    [SerializeField] List<ParticleSystem> particleSystems = new List<ParticleSystem>();
    float activeCount;

    private void Start()
    {
        if (GetComponent<ParticleSystem>() != null) particleSystems.Add(GetComponent<ParticleSystem>());
        particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
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
                    Destroy(this.gameObject);
                    break;
                case ParticleEndAction.Deactivate:
                    this.gameObject.SetActive(false);
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
}
