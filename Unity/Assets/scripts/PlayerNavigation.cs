
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavigation : MonoBehaviour {

    [Header("life")]

    public int life;
    public bool dead = false;
    public LifeBarUI lifeBar;

    public Camera cam;
    public NavMeshAgent agent;
    Vector3 newposition;

    public Animator anim;



	// Use this for initialization
	void Start ()
    {
       cam = Camera.main;
       agent = GetComponent<NavMeshAgent>();
       lifeBar.Init(life);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Quaternion transRot = Quaternion.LookRotation(newposition - this.transform.position, Vector3.up);
            Physics.Raycast(ray, out hit);
            if(hit.transform)
            {
                Debug.Log(hit.transform.name);
                Debug.DrawLine(this.transform.position, hit.point, Color.red, 1);
                agent.SetDestination(hit.point);
                anim.SetBool("movement", true);

            }
        }
        if (agent.isStopped) anim.SetBool("movement", false);
        
        

	}
    public void SetDamage(int damage)
    {
        life -= damage;

        if(life <= -1)
        {
            dead = true;
            life = 0;
        }
        lifeBar.UpdateBar(life);
    }
}
