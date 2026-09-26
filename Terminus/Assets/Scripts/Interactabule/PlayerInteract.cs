using Player;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private PlayerInput playerInput;
        public Camera cam;
        private InputAction interactAction;
        public Transform InteractPoint;
        
        private float distance = 1.5f;
        [SerializeField] private InputActionAsset playerControls;
        [SerializeField]
        private LayerMask mask;
       
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerInput = GetComponent<PlayerInput>();

        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray(InteractPoint.transform.position, cam.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * distance);
            RaycastHit hitinfo; // variable to store our collision information.
            if (Physics.Raycast(ray, out hitinfo, distance, mask))
            {
                if (hitinfo.collider.GetComponent<Interactabule>() != null)
                {
                    Interactabule interactabule = hitinfo.collider.GetComponent<Interactabule>();
                    Debug.Log(hitinfo.collider.GetComponent<Interactabule>().promptMessage);


                    if (playerInput.Interact)
                    {
                        interactabule.BaseInteract();
                    }


                }
            }
        }

       
    }
}
