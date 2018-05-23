using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {

    public GameObject[] particleSystems;
    public List<ParticleAndQuantity> particleSystemsToPool;

    public Dictionary<string, GameObject> particlePrefabs = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> particlePrefabsToPool = new Dictionary<string, GameObject>();

    //public Dictionary<string, Queue<GameObject>> particlePoolDictionary;

    private void Start()
    {
        foreach (GameObject particle in particleSystems)
        {
            particlePrefabs.Add(particle.name, particle);
        }
    }

    public void InstanciateParticleSystem (string particleName, Vector3 particlePosition, Quaternion particleRotation)
    {
        //int particlePrefabIndex = particlePrefabs.FindIndex(particle => particle.particleName == particleName);
        GameObject particleInstance = Instantiate(particlePrefabs[particleName], particlePosition, particleRotation, this.transform); //Se le asigna un ParentTranfsorm para que se instancie en la escena correcta
        particleInstance.transform.parent = null;
    }

    //Overload to instanciate as a child of a certain transform
    public void InstanciateParticleSystem(string particleName, Transform parentTransform, Vector3 particleLocalPosition, Quaternion particleLocalRotation)
    {
        //int particlePrefabIndex = particleSystems.FindIndex(particle => particle.particleName == particleName);
        GameObject particleInstance = Instantiate(particlePrefabs[particleName].gameObject, parentTransform.position, parentTransform.rotation, parentTransform);
        particleInstance.transform.localPosition = particleLocalPosition;
        particleInstance.transform.localRotation = particleLocalRotation;
    }
}

[System.Serializable]
public class ParticleAndQuantity
{
    public GameObject prefab;
    public int quantity;

    public ParticleAndQuantity (GameObject particlePrefab, int particleQuantity)
    {
        prefab = particlePrefab;
        quantity = particleQuantity;
    }
}