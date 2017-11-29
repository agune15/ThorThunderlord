using UnityEngine;
using System.Collections;
/*
public class ENEMY_AI : MonoBehaviour
{
    // Base code By BorDr added to by pvtgreg with help from wesleywh 
    // 9/7/13
    //---------------------------------------------------------------------------------------------

    //public float walkRadius;
    public float walkRadius;
    private Transform PlayerT;
    private GameObject Player;
    private float Timer = 1.5f;
    //public bool DoWaypoints;
    public int Range;
    public float MaxTimer = 1.5f;

    public AudioClip HitNoise;
    Animator animator;


    public bool playerInSight;
    public float fieldOfViewAngle;
    public Vector3 lastPlayerSighting;
    public SphereCollider col;

    private Vector3 previousPosition;

    public float curSpeed;


    bool DoWaypoints = true;

    void OnTriggerStay(Collider other)
    {//This is to see if the player is within a certain distance of the AI.
        if(other.gameObject.tag == "Player")
        {//Only do this on ppl tagged "Player"
            Vector3 direction = other.transform.position - this.transform.position;

            float angle = Vector3.Angle(direction, transform.forward);//Draw the angle in front of the AI

            if(angle < fieldOfViewAngle * 0.5f)//This is the angle that the AI can see
            {
                RaycastHit hit;

                if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))//Check if the AI can see the player
                {
                    if(hit.collider.tag == "Player")
                    {
                        playerInSight = true; //AI Character sees the player
                        DoWaypoints = false; //don't walk waypoints anymore
                    }
                    else
                    {
                        DoWaypoints = true;
                        playerInSight = false;
                    }

                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == FindClosestPlayer())//this is calling a function that tries to find the closest gameobject tagged "Player"
        {
            playerInSight = false;
            lastPlayerSighting = FindClosestPlayer().transform.position; //this is used to have the AI move to this location to start looking for the lost player

        }
    }



    GameObject FindClosestPlayer()
    {
        ArrayList gos = new ArrayList();
        gos.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        gos.AddRange(GameObject.FindGameObjectsWithTag("car"));

        GameObject closest = null;//you will return this as the person you find.
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach(GameObject go in gos)//go through all players in map
        {
            var diff = (go.transform.position - position);
            var curDistance = diff.sqrMagnitude;
            if(curDistance < distance)//is this player closer than the last one?
            {
                closest = go;//this is the closest player
                distance = curDistance;//set the closest distance
            }
        }
        return closest.gameObject;//this is the closest player
    }





    // Use this for initialization
    void Start()
    {
        PlayerT = this.transform;


        animator = GetComponent<Animator>();
    }




    // Update is called once per frame
    void Update()
    {

        //The timer system I use is simple. It counts up- when it is at maximum, the enemy can attack.
        //When it is not, it recharges in seconds.



        if(Timer <= MaxTimer)
        {
            Timer += Time.deltaTime;
        }
        //This is the navigation. The enemy will move toward the player while playing it's running
        //animation. If it is in range, it will stop playing the animation.
        if(playerInSight == true)
        {
            Player = FindClosestPlayer();
            PlayerT = Player.transform;

            GetComponent<NavMeshAgent>().destination = PlayerT.position;
        }
        Vector3 curMove = transform.position - previousPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;
        var distance = Vector3.Distance(transform.position, PlayerT.transform.position);
        if(distance < Range && playerInSight == true)//if he is in range to attack
        {
            Attack();
            animator.SetBool("attack", true); //go to the attack state
        }
        else
        {//if not in range
            animator.SetBool("attack", false);//leave attack state
            animator.SetFloat("speed", curSpeed);//go to idle or walk according to your movement speed
        }
        if(curSpeed < 0.5)
        {
            animator.SetBool("idle", true);
        }
        //if (DoWaypoints == true) 
        //{

        //}

        if(DoWaypoints == true)
        {
            float dist = GetComponent<NavMeshAgent>().remainingDistance;
            if(dist != Mathf.Infinity && GetComponent<NavMeshAgent>().pathStatus == NavMeshPathStatus.PathComplete && GetComponent<NavMeshAgent>().remainingDistance == 0)//Arrived.
            {
                Wonder();
            }
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > 3)
        {
            GetComponent<vp_DamageHandler>().Damage(10.0f);
        }
    }

    void Attack()
    {
        //If the player is in range, the attack animation will play, the timer will reset, and 
        //damage will be dealt. The audio clip will be played to add an effect to the attack.
        if(Timer >= MaxTimer)
        {

            Timer = 0;

            PlayerT.GetComponent<vp_PlayerDamageHandler>().Damage(1.0f);
            audio.PlayOneShot(HitNoise);
        }

    }


    void Wonder()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        GetComponent<NavMeshAgent>().destination = finalPosition;
    }




}
*/