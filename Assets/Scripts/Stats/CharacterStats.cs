using UnityEngine;

/* Contains all the stats for a character. */

public class CharacterStats : MonoBehaviour
{
    private LevelLogic managerScenes;
    public GameObject sceneLevelManager;
    public HealthThorBar lifeBar;
    public Stat maxHealth;          // Maximum amount of health
    public float currentHealth { get; protected set; }    // Current amount of health

    public Stat damage;
    public Stat armor;

    public event System.Action OnHealthReachedZero;

    public virtual void Awake()
    {
        sceneLevelManager = GameObject.FindWithTag("GameManager");
        managerScenes = sceneLevelManager.GetComponent<LevelLogic>();

        PlayerPrefs.SetInt("ThorDie", 0);
        currentHealth = maxHealth.GetValue();
        lifeBar.UpdateEnergyUI();
    }

    // Start with max HP.
    public virtual void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    // Damage the character
    public void TakeDamage(float damage)
    {
        // Subtract the armor value - Make sure damage doesn't go below 0.
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, float.MaxValue);

        // Subtract damage from health
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        lifeBar.UpdateEnergyUI();

        // If we hit 0. Die.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // Die in some way
        // This method is meant to be overwritten
        managerScenes.LoadEndScene();
        PlayerPrefs.SetInt("ThorDie", 1);
        Debug.Log(transform.name + " died.");
    }


    // Heal the character.
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.GetValue());
    }



}