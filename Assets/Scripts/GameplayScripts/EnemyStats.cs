using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public enum EnemyType { Skull, Fenrir }
    public EnemyType enemyType;

    ParticleInstancer particleInstancer;

    SkullBehaviour skullBehaviour = null;
    EnemyBehaviour enemyBehaviour = null;

    [SerializeField] EnemyHealthBar enemyHealthBar = null;

    [SerializeField] Renderer enemyRenderer;
    [SerializeField] Collider enemyCollider;
    [SerializeField] public static Animator enemyAnimator;

    [SerializeField] float life;
    float maxLife;
    
    public bool isSlowed = false;

    public float deadTimer;


    // Use this for initialization
    void Start () {
        particleInstancer = GameObject.FindWithTag("ParticleInstancer").GetComponent<ParticleInstancer>();

        enemyRenderer = this.GetComponentInChildren<Renderer>();
        enemyCollider = this.GetComponent<Collider>();
        enemyAnimator = this.GetComponentInChildren<Animator>();

        enemyHealthBar = GameObject.Find("GameplayUI").GetComponent<EnemyHealthBar>();

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

        enemyBehaviour = this.GetComponent<EnemyBehaviour>();
        enemyBehaviour.SetLife(life);
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
        life = 30;
        maxLife = life;
    }

    void FenrirInit()
    {
        life = 300;
        maxLife = life;
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

                    enemyBehaviour.SetLife(life);
                    break;
                case EnemyType.Fenrir:
                    life -= damage / 0.75f;

                    enemyBehaviour.SetLife(life);
                    break;
                default:
                    break;
            }
        }

        //Debug.Log(enemyType.ToString() + " received damage");

        if (enemyHealthBar.playerAttackingTarget != null && enemyHealthBar.playerAttackingTarget == this.gameObject) enemyHealthBar.DrawEnemyHealthBar(this.gameObject, maxLife, life, enemyType.ToString());
    }

    //Particle instancing overload
    public void SetDamage(float damage, Quaternion particleAngle)
    {
        if (life > 0)
        {
            switch (enemyType)
            {
                case EnemyType.Skull:
                    if (damage > 0) particleInstancer.InstanciateParticleSystem("Enemy_Blood", transform.position + new Vector3(0, 1, 0), particleAngle);
                    break;
                case EnemyType.Fenrir:
                    if (damage > 0) particleInstancer.InstanciateParticleSystem("Enemy_Blood", transform.position + new Vector3(0, 2, -0.5f), particleAngle);
                    break;
                default:
                    break;
            }
        }

        SetDamage(damage);
    }

    public void SetSlowSpeed()
    {
        isSlowed = true;
        enemyBehaviour.SetSlow(true);
    }

    public void SetInitSpeed()
    {
        isSlowed = false;
        enemyBehaviour.SetSlow(false);
    }

    public static void SetFrozen ()
    {
        enemyAnimator.enabled = false;
    }

    #endregion

    #region Gets

    public float GetLife()
    {
        return life;
    }
    
    public float GetMaxLife()
    {
        return maxLife;
    }

    public EnemyType GetEnemyType ()
    {
        return enemyType;
    }

    #endregion

    #region Others

    private void OnMouseOver()
    {
        enemyHealthBar.DrawEnemyHealthBar(maxLife, life, enemyType.ToString());
    }

    private void OnMouseExit()
    {
        enemyHealthBar.DisableEnemyHealthBar();
    }

    #endregion
}
