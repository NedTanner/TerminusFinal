using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player
{
    
    public enum PlayerMovementStates
    {
        Idle,
        Attacking,
        Moving,
        Falling,
        Dodge
        
    }
    
    public class PlayerMovement : MonoBehaviour
    {
        public PlayerMovementStates _currentMovementState = PlayerMovementStates.Idle;
        public PlayerMovementStates CurrentMovementState => _currentMovementState;
        
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsFalling = Animator.StringToHash("IsFalling");

        private PlayerInput playerInput;
        public CharacterController characterController;
        private PlayerCombat playerCombat;
        [SerializeField] private Transform playerCameraTransform;
        
        [SerializeField] private Animator animator;

        [field: Header("Turning")]
        [SerializeField] private float turnSmoothing = 0.1f;
        [SerializeField] private float turnSmoothVelocity;
    
        [field: Header("Moving")]
        [SerializeField] private float speed = 6.0f;
    
        public bool Grounded { get; private set; } = true;

        [field: Header("Player Grounded")]
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private float groundStickForce = -2f;
        [SerializeField] private float coyoteTimer = 0.1f;
        [SerializeField] private float groundedOffset = 1f;
        [SerializeField] private float maxCastDistance = 0.3f;
        private float lastGroundedTime;
        private float verticalVelocity;
        private Vector3 groundNormal = Vector3.up;


        [field: Header("Dodge")]
        [SerializeField] private bool canDodge = true;
        [SerializeField] private float dodgeDistance = 10f;
        [SerializeField] private float dodgeTimeRemaining;
        [SerializeField] private float dodgeTime;
        [SerializeField] private Vector3 dodgeDirection;
        [SerializeField] private float dodgeCooldown;
        [SerializeField] private float dodgeGravityModifier;
        [SerializeField] public bool isImmune;
        private float dodgeReadyTimer = 0.01f; // Keep at 0.01, makes a timestamp for dodge cooldown.
        
        [field: Header("Dodge Back")]
        [SerializeField]private float dodgeBackDistance = 2.25f;
        
        private const float Gravity = 9.81f;

        private void GroundedCheck()
        {
            RaycastHit hit;

            Vector3 sphereCastOrigin = transform.position + Vector3.up * groundedOffset;

            Grounded = Physics.SphereCast(sphereCastOrigin, groundedRadius, Vector3.down, out hit, maxCastDistance, groundLayers, QueryTriggerInteraction.Ignore);

            groundNormal = Grounded ? hit.normal : Vector3.up; // Depending on Grounded, uses the angle where the spherecast 'reflects' off the ground, or uses straight up as reference for  angular movement.

            Vector3 endPoint = sphereCastOrigin + Vector3.down * maxCastDistance;

            // Coyote timer my beloved.
            if (Grounded)
            {
                lastGroundedTime = Time.time;
            }

            Grounded = Time.time - lastGroundedTime <= coyoteTimer;
        }
        private void DodgeCheck()
        {   
            // can't attack if dodging
            // dodgeReadyTimer will be Time.time + dodgeCooldown when dodge ends, compare until dodgeCooldown time has passed.
            canDodge = dodgeReadyTimer <= Time.time; 
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
            playerCombat = GetComponent<PlayerCombat>();
            dodgeTimeRemaining = dodgeTime;
        }

        private void Update()
        {
            DodgeCheck();
            GroundedCheck();
            switch (_currentMovementState)
            {
                case PlayerMovementStates.Moving:
                    Moving();
                    break;

                case PlayerMovementStates.Attacking:
                    Attacking();
                    break;

                case PlayerMovementStates.Falling:
                    Falling();
                    break;

                case PlayerMovementStates.Idle:
                    Idle();
                    break;

                case PlayerMovementStates.Dodge:
                    Dodge();
                    break;

                default: Idle();
                    break;
            }
        }
        
        private void Idle()
        {
            animator.SetFloat(Speed, 0.0f);

            // == transitions

            if (playerCombat.isAttacking && Grounded)
            {
                EnterAttack();
            }

            if (!Grounded)
            {
                _currentMovementState = PlayerMovementStates.Falling;
            }
            
            if (Grounded && playerInput.MoveVector != Vector2.zero)
            {
                _currentMovementState = PlayerMovementStates.Moving;
            }

            if (playerInput.Dodge && canDodge && Grounded)
            {
                _currentMovementState = PlayerMovementStates.Dodge;

                animator.SetTrigger("Dodge");
            }
        }

        public void EnterAttack()
        {
            transform.rotation = Quaternion.Euler(0f, playerCameraTransform.eulerAngles.y, 0f);

            _currentMovementState = PlayerMovementStates.Attacking;
        }
        public void Attacking()
        {
            animator.SetFloat(Speed, 0.0f);

            // == transitions!
            if (!playerCombat.isAttacking && Grounded && playerInput.MoveVector != Vector2.zero)
            {
                _currentMovementState = PlayerMovementStates.Moving;
            }

            if (!playerCombat.isAttacking && Grounded && playerInput.MoveVector == Vector2.zero)
            {
                _currentMovementState = PlayerMovementStates.Idle;
            }

            if (!playerCombat.isAttacking && !Grounded)
            {
                _currentMovementState = PlayerMovementStates.Falling;
            }
        }

        private void Falling()
        {
            animator.SetBool(IsFalling, true);
            characterController.Move(GetMoveDir() * Time.deltaTime);
            
            // == transitions
            if (Grounded)
            {
                _currentMovementState = PlayerMovementStates.Idle;
                animator.SetBool(IsFalling, false);
                animator.SetTrigger("Land");
            }

        }

        private void Moving()
        {  
            Vector3 moveDir = GetMoveDir();
            float moveSpeed = GetMoveSpeed();

            float targetAngle = Mathf.Atan2(playerInput.MoveVector.x, playerInput.MoveVector.y) * Mathf.Rad2Deg + playerCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);

            animator.SetFloat(Speed, moveSpeed);
            characterController.Move(moveDir * (speed * moveSpeed * Time.deltaTime));
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // == transitions

            if (playerCombat.isAttacking && Grounded)
            {
                EnterAttack();
            }

            if (!Grounded)
            {
                _currentMovementState = PlayerMovementStates.Falling;
            }
            
            if (Grounded && playerInput.MoveVector == Vector2.zero)
            {
                _currentMovementState = PlayerMovementStates.Idle;
            }

            if (playerInput.Dodge && Grounded && canDodge)
            {
                _currentMovementState = PlayerMovementStates.Dodge;

                animator.SetTrigger("Dodge");
            }
        }
        
        private void Dodge()
        {
            if (playerCombat.isAttacking)
            {
                EnterAttack();
            }

            dodgeTimeRemaining -= Time.deltaTime;
            isImmune = true;

            float targetAngle = Mathf.Atan2(playerInput.MoveVector.x, playerInput.MoveVector.y) * Mathf.Rad2Deg + playerCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (dodgeTimeRemaining <= 0f)
            {
                dodgeReadyTimer = Time.time + dodgeCooldown; // Adds cooldown to Time.time. Sets a 'bookmark' that is [dodgeCooldown] amount of time in the future.
                _currentMovementState = PlayerMovementStates.Idle;
                dodgeTimeRemaining = dodgeTime;  //reset the time remaining for next dodge
                dodgeDirection = Vector3.zero;
                canDodge = false;
                isImmune = false;
                return;
            }

            // if no dodge direction then this is the first frame of this dodge
            // so we need to determine it.  
            if (dodgeDirection == Vector3.zero)
            {
                // if there is no movement input from the player then we dodge backwards
                dodgeDirection = playerInput.MoveVector == Vector2.zero ? -transform.forward : GetMoveDir();
            }
            
            // if we are dodging backwards then only dodge the dodgeBackDistance instead of dodgeDistance
            float distance = dodgeDirection == -transform.forward? dodgeBackDistance : dodgeDistance;
            
            characterController.Move(dodgeDirection.normalized * (distance * Time.deltaTime));
        }

        /**
         returns 0 to 1 depending on movement input
         keyboard always returns 1 if movement key is pressed
         gamepad returns between 0 & 1 depending on how far stick is pressed
         */
        private float GetMoveSpeed()
        {
            return Mathf.Abs(playerInput.MoveVector.magnitude);
        }
        
        private Vector3 GetMoveDir()
        {
            // handles turning and moving/rotating the character model
            float targetAngle = Mathf.Atan2(playerInput.MoveVector.x, playerInput.MoveVector.y) * Mathf.Rad2Deg + playerCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = Vector3.ProjectOnPlane(moveDir, groundNormal); // Aligns movement to the normal of the slope, normal comes from SphereCast in GroundedCheck().



            if (playerInput.MoveVector == Vector2.zero) // Stops player from constantly shimmying from groundStickForce when no inputs are detected.
            {
                if (Grounded)
                {
                    return Vector3.up * verticalVelocity;
                }
                
                else
                {
                    verticalVelocity -= Gravity * Time.deltaTime;
                }

                return new Vector3(0f, verticalVelocity, 0f);
            }

            if (Grounded)
            {
                verticalVelocity = groundStickForce; // Holds player down enough to prevent jitter, hopefully!
            }
            else
            {
                verticalVelocity -= Gravity * Time.deltaTime;
            }
            
            if (dodgeTimeRemaining != dodgeTime) // Allows for some primitive platforming by making player floatier during dodge, can delete later if we don't like how this feels.
            {
                verticalVelocity *= dodgeGravityModifier;
            }

            moveDir.y = verticalVelocity; // I don't know why I couldn't just set moveDir.y directly, and I don't want to know, verticalVelocity works.

            return moveDir;
        }

        public void CallMovementEnumerator(float duration)
        {
            StartCoroutine(AttackForceMovement(duration));
        }
        // For use with the animator, moves player when AnimatorPlayerBridge calls from Animation Event. Did I mention I hate this?
        private IEnumerator AttackForceMovement(float duration)
        {
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / duration; // Percentage completion to gradually reduce movement force

                float movementForce = 0.05f * (1f - t);

                float targetAngle = Mathf.Atan2(playerInput.MoveVector.x, playerInput.MoveVector.y) * Mathf.Rad2Deg + playerCameraTransform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothing);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                moveDir = Vector3.ProjectOnPlane(moveDir, groundNormal);

                characterController.Move(moveDir * movementForce);

                yield return null;
            }
        }
    }
}
