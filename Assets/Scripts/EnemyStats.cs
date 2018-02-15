using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public enum EnemyType { Skull, Fenrir }
    public EnemyType enemyType;

    SkullBehaviour skullBehaviour = null;

    [SerializeField] Renderer enemyRenderer;
    [SerializeField] Collider enemyCollider;

    [SerializeField] float life;

    public float deadTimer;


    // Use this for initialization
    void Start () {
        enemyRenderer = this.GetComponentInChildren<Renderer>();
        enemyCollider = this.GetComponent<Collider>();

        switch(enemyType)
        {
            case EnemyType.Skull:
                SkullInit();
                break;
            case EnemyType.Fenrir:
                FenrirInit();
                break;
            default:
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (life <= 0)
        {
            if(!enemyRenderer.gameObject.isStatic) enemyRenderer.gameObject.isStatic = true;
            enemyCollider.enabled = false;

            if(deadTimer > 0)
            {
                deadTimer -= Time.deltaTime;
                return;
            }
            
            if (!enemyRenderer.isVisible) this.gameObject.SetActive(false);
        }
	}

    #region Inits

    void SkullInit()
    {
        life = 60;

        skullBehaviour = this.GetComponent<SkullBehaviour>();
        skullBehaviour.SetLife(life);
    }

    void FenrirInit()
    {
        life = 340;
    }

    #endregion

    #region Updates

    #endregion

    #region Sets

    public void SetDamage(float damage)
    {
        if (life > 0)
        {
            switch(enemyType)
            {
                case EnemyType.Skull:
                    life -= damage;

                    skullBehaviour.SetLife(life);
                    break;
                case EnemyType.Fenrir:
                    life -= damage / 0.75f;

                    //fenrirBehaviour.SetLife(life);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region Gets

    public float GetLife()
    {
        return life;
    }

    #endregion
}
