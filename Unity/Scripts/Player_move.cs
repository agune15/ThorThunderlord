using System.Collections;
using UnityEngine;

public class Player_move : MonoBehaviour
{

    [SerializeField][Range(1, 20)]
    private float speed = 10;                                   //How fast the player moves
    private Vector3 targetPosition;                             //Where we want to travell
    private bool isMoving;                                      //check if we are moving or not

    const int RIGHT_MOUSE_BUTTON = 1;                            //el butó


	// Use this for initialization
	void Start ()
    {

        targetPosition = transform.position;                    //set the target position to where we are at the start
        isMoving = false;                                       //set out move toggle

	}
	
	// Update is called once per frame
	void Update ()
    {

        //if player clicked on the screen find out where
        if (Input.GetMouseButton(RIGHT_MOUSE_BUTTON))
            SetTargetPosition();

        //if we are still moving, then move the player
        if (isMoving)
            MovePlayer();


	}

    //Set the target position we will travell to
    void SetTargetPosition()
    {

        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float point = 0f;

        if (plane.Raycast(ray, out point))
            targetPosition = ray.GetPoint(point);

        //set the player to move
        isMoving = true;

    }

    //Moves The player in the right direction and also rotates them to look at the target position
    //When the player gets to the target, stop them from moving
    void MovePlayer()
    {

        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        //if we are at the target position then stop moving 
        if (transform.position == targetPosition)
            isMoving = false;

        //dibujar la linea
        Debug.DrawLine(transform.position, targetPosition, Color.red);


    }
}
