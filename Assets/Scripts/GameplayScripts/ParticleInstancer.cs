using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {

    public GameObject[] particleSystems;
    public List<ParticleAndName> particlePrefabs = new List<ParticleAndName>();

    //public List<ParticleAndName> particleInstances = new List<ParticleAndName>();

    private void Start()
    {
        foreach (GameObject particleSystem in particleSystems)
        {
            ParticleAndName particle = new ParticleAndName(particleSystem.name, particleSystem);
            particlePrefabs.Add(particle);
        }
    }

    public void InstanciateParticleSystem (string particleName, Vector3 particlePosition, Quaternion particleRotation)
    {
        int particlePrefabIndex = particlePrefabs.FindIndex(particle => particle.particleName == particleName);
        GameObject particleInstance = Instantiate(particlePrefabs[particlePrefabIndex].particlePrefab, particlePosition, particleRotation, this.transform);
        particleInstance.transform.parent = null;

        //particleInstances.Add()
    }

    //Instanciate Gameobject
    //DestroyGameobject
}

public class ParticleAndName
{
    public string particleName;
    public GameObject particlePrefab;

    public ParticleAndName (string name, GameObject prefab)
    {
        particleName = name;
        particlePrefab = prefab;
    }
}
