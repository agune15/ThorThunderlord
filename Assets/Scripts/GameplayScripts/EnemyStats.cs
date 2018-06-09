using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    public enum EnemyType { Skull, Fenrir }
    public EnemyType enemyType;

    ParticleInstancer particleInstancer;
    AudioPlayer audioPlayer;

    EnemyBehaviour enemyBehaviour = null;

    [SerializeField] EnemyHealthBar enemyHealthBar = null;
    PauseGameplay pauseGameplay = null;

    [SerializeField] Renderer enemyRenderer;
    [SerializeField] Collider enemyCollider;
    [SerializeField] public static Animator enemyAnimator;

    [SerializeField] float life;
    float maxLife;
    
    public bool isSlowed = false;

    public float deadTimer;

    //Shader related
    [Header("Shader related parameters")]
    [SerializeField] Shader defaultShader;
    [SerializeField] Shader outlineShader;

    bool isOutlineFromMouseOver = false;
    string currentShader = "default";


    void Start () {
        particleInstancer = GameObject.FindWithTag("ParticleInstancer").GetComponent<ParticleInstancer>();
        audioPlayer = GetComponent<AudioPlayer>();

        enemyRenderer = this.GetComponentInChildren<Renderer>();
        enemyCollider = this.GetComponent<Collider>();
        enemyAnimator = this.GetComponentInChildren<Animator>();

        defaultShader = enemyRenderer.material.shader;
        outlineShader = Shader.Find("Custom/Mobile Diffuse Outline");

        enemyHealthBar = GameObject.Find("GameplayUI").GetComponent<EnemyHealthBar>();
        pauseGameplay = enemyHealthBar.gameObject.GetComponent<PauseGameplay>();

        switch (enemyType)
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

    public void SetDamage(float damage, bool playSFX)
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

        if (enemyHealthBar.playerAttackingTarget != null && enemyHealthBar.playerAttackingTarget == this.gameObject) enemyHealthBar.DrawEnemyHealthBar(this.gameObject, maxLife, life, enemyType.ToString());

        if (playSFX)
        {
            int playSound = new int();
            if (enemyType == EnemyType.Fenrir) playSound = (Random.Range(0f, 1f) >= 0.5f) ? 1 : 0;
            else if (enemyType == EnemyType.Skull) playSound = (Random.Range(0f, 1f) >= 0.3f) ? 1 : 0;

            switch (enemyType)
            {
                case EnemyType.Skull:
                    if (life > 0)
                    {
                        if (playSound == 1) audioPlayer.PlaySFX(0, 0.5f, Random.Range(0.96f, 1.04f));   //Skull injured sound
                    }
                    else audioPlayer.PlaySFX(1, 0.5f, Random.Range(0.96f, 1.04f));  //Skull death sound
                    break;
                case EnemyType.Fenrir:
                    if (playSound == 1) audioPlayer.PlaySFX(Random.Range(5, 7), 0.5f, Random.Range(0.96f, 1.04f));
                    break;
                default:
                    break;
            }
        }
    }

    //Particle instancing overload
    public void SetDamage(float damage, bool playSFX, Quaternion particleAngle)
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

        SetDamage(damage, playSFX);
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
        if (pauseGameplay.isGamePaused) return;

        enemyHealthBar.DrawEnemyHealthBar(maxLife, life, enemyType.ToString());
        if (CursorManager.GetCurrentCursorTextureName() != "attack")
        {
            CursorManager.SetAndStoreCursor("attack", Vector2.zero, CursorMode.Auto);

            SetShader("outline");

            isOutlineFromMouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        enemyHealthBar.DisableEnemyHealthBar();
        CursorManager.SetAndStoreCursor("default", Vector2.zero, CursorMode.Auto);
        SetShader("default");

        isOutlineFromMouseOver = false;
    }

    public void SetDefaultShader()
    {
        if (isOutlineFromMouseOver) SetShader("default");
    }

    public void SetShader (string desiredShader)
    {
        if (desiredShader == "default")
        {
            enemyRenderer.material.shader = defaultShader;

            currentShader = desiredShader;
        }
        else if (desiredShader == "outline")
        {
            enemyRenderer.material.shader = outlineShader;
            if (enemyType == EnemyType.Fenrir) enemyRenderer.material.SetFloat("_OutlineWidth", 0.0045f);
            else if (enemyType == EnemyType.Skull) enemyRenderer.material.SetFloat("_OutlineWidth", 0.35f);
            enemyRenderer.material.SetColor("_OutlineColor", new Color(1, 0, 0, 1));

            currentShader = desiredShader;
        }
    }

    #endregion
}
