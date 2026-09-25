using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class Interactabule : MonoBehaviour
{
    //add or remove an InteractionEvent component to this gameobject
    public bool useEvents;
    //message displayed to player when looking at an interactibule.
    public string promptMessage;

    //this function will be called from our player

    public void BaseInteract()
    {
        if (useEvents)
        
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        
        Interact();
    }

    protected virtual void Interact()
    {
        //we wont have any code written in this function
        //this is a template function to be overridden by our subclasses
    }
}
