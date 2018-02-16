using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public enum EnemyType { Skull, Fenrir }
    public EnemyType enemyType;

    SkullBehaviour skullBehaviour = null;
    EnemyBehaviour enemyBehaviour = null;

    [SerializeField] Renderer enemyRenderer;
    [SerializeField] Collider enemyCollider;
    [SerializeField] public static Animator enemyAnimator;

    [SerializeField] float life;
    
    public bool isSlowed = false;

    public float deadTimer;



    // Use this for initialization
    void Start () {
        enemyRenderer = this.GetComponentInChildren<Renderer>();
        enemyCollider = this.GetComponent<Collider>();
        enemyAnimator = this.GetComponentInChildren<Animator>();

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
        life = 60;
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

        Debug.Log("heey");
    }

    #endregion

    #region Gets

    public float GetLife()
    {
        return life;
    }

    public EnemyType GetEnemyType ()
    {
        return enemyType;
    }

    #endregion
}
