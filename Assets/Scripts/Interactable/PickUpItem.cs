using UnityEngine;

public class PickUpItem : Interactable
    
{
    public GameObject text;

    public AudioSource LevelUp;

    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    void PickUp()
    {
        Debug.Log("Picking up Item");

        text.SetActive(true);

        //Add our item to the colectionable menu. 

        LevelUp.Play();

        Destroy(gameObject);
    }



}
