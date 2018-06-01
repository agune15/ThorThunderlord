using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstancer : MonoBehaviour {

    public GameObject[] particleSystems;
    public List<ParticleAndQuantity> particleSystemsToPool;

    Dictionary<string, GameObject> particlePrefabs = new Dictionary<string, GameObject>();
    Dictionary<string, Queue<GameObject>> particlePrefabsToPool = new Dictionary<string, Queue<GameObject>>();
    
    [SerializeField] List<ParticleAndName> instanciatedParticleSystems = new List<ParticleAndName>();


    private void Start()
    {
        foreach (GameObject particle in particleSystems)
        {
            particlePrefabs.Add(particle.name, particle);
        }

        foreach (ParticleAndQuantity particle in particleSystemsToPool)
        {
            Queue<GameObject> particlePool = new Queue<GameObject>();

            for (int i = 0; i < particle.quantity; i++)
            {
                GameObject particleInstance = Instantiate(particle.prefab);
                particleInstance.GetComponent<ParticlePrefabBehaviour>().particleTag = particle.prefab.name;
                particleInstance.SetActive(false);
                particlePool.Enqueue(particleInstance);
            }

            particlePrefabsToPool.Add(particle.prefab.name, particlePool);
        }
    }

    public void InstanciateParticleSystem (string particleName, Vector3 particlePosition, Quaternion particleRotation)
    {
        GameObject particleInstance = Instantiate(particlePrefabs[particleName], particlePosition, particleRotation, this.transform); //Se le asigna un ParentTranfsorm para que se instancie en la escena correcta
        particleInstance.transform.parent = null;
        
        instanciatedParticleSystems.Add(new ParticleAndName(particleInstance, particleInstance.name));
    }

    //Overload to instanciate as a child of a certain transform
    public void InstanciateParticleSystem(string particleName, Transform parentTransform, Vector3 particleLocalPosition, Quaternion particleLocalRotation)
    {
        GameObject particleInstance = Instantiate(particlePrefabs[particleName].gameObject, Vector3.zero, Quaternion.identity, parentTransform);
        particleInstance.transform.localPosition = particleLocalPosition;
        particleInstance.transform.localRotation = particleLocalRotation;
        
        instanciatedParticleSystems.Add(new ParticleAndName(particleInstance, particleInstance.name));
    }

    public void PoolParticleSystem (string particleName, Vector3 particlePosition, Quaternion particleRotation)
    {
        GameObject particleToPool = particlePrefabsToPool[particleName].Dequeue();
        particleToPool.SetActive(true);
        particleToPool.transform.position = particlePosition;
        particleToPool.transform.rotation = particleRotation;
    }

    public void UnpoolParticleSystem (string particleTag, GameObject particleObject)
    {
        particlePrefabsToPool[particleTag].Enqueue(particleObject);
        particleObject.SetActive(false);
    }

    public void DestroyParticleSystem (string particleName)
    {
        int particleIndex = instanciatedParticleSystems.FindIndex(particle => particle.name == particleName);

        GameObject particleReference = instanciatedParticleSystems[particleIndex].particle;

        instanciatedParticleSystems.Remove(instanciatedParticleSystems[particleIndex]);
        
        Destroy(particleReference);
    }

    //Overload to destroy particle instance with GameObject as input value
    public void DestroyParticleSystem (GameObject particleInstance)
    {
        int particleIndex = instanciatedParticleSystems.FindIndex(particle => particle.particle == particleInstance);

        GameObject particleReference = instanciatedParticleSystems[particleIndex].particle;

        instanciatedParticleSystems.Remove(instanciatedParticleSystems[particleIndex]);

        Destroy(particleReference);
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

[System.Serializable]
public class ParticleAndName
{
    public string name;
    public GameObject particle;

    public ParticleAndName (GameObject particleObject, string particleName)
    {
        name = particleName;
        particle = particleObject;
    }
}