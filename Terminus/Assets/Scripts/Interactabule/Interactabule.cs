using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class Interactabule : MonoBehaviour
{
    //message displayed to player when looking at an interactibule.
    public string promptMessage;

    //this function will be called from our player

    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        //we wont have any code written in this function
        //this is a template function to be overridden by our subclasses
    }
}
