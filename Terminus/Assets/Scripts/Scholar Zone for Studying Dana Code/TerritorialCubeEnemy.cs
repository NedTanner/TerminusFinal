using Interfaces;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{/*
    public class TerritorialCubeEnemy : MonoBehaviour, IDamageable
    {
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int DoDab = Animator.StringToHash("DoDab");
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int DoAttack = Animator.StringToHash("DoOneSwing");
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private GameObject _player;
        private PlayerHealth _playerHealth;
        [SerializeField] private EnemyState defaultState = EnemyState.Idle;
        [SerializeField] private int maxHealth = 50;
        private VisionSensor  _visionSensor;
        private PlayerProximityDetectorSensor _playerProximityDetectorSensor;
        private EnemyWeapon _weapon;
        private int _currentHealth;
        private readonly float _turnSpeed = 3.0f;
        private EnemyState _currentState;
        private float _timeOfLastAttack;
        [SerializeField] private float attackDelaySeconds = 2;
        [SerializeField] private float cheerDelaySeconds = 2;

        [Header("Territory")]
        [SerializeField] private Transform territory;
        [SerializeField] private Transform home;
        [SerializeField] private float territoryRadius;
        [SerializeField] private float chaseRange = 1;
        
        private float _timeOfPlayerDeath = 0.5f;
        private float _timeOfLastCheer = 0.5f;

        private float _stunduration = 2.0f;
        private float _timeOfStun;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _visionSensor = GetComponent<VisionSensor>();
            _playerProximityDetectorSensor = GetComponent<PlayerProximityDetectorSensor>();
            _weapon = GetComponentInChildren<EnemyWeapon>();
            _playerHealth = _player.GetComponent<PlayerHealth>();
        }

        private void Start()
        {
            _currentState = defaultState;
            _currentHealth = maxHealth;
            
        }
    
        private void Update()
        {
            PlayerInTerritory();
            switch (_currentState)
            {
                case EnemyState.Chase:  DoChase(); break;
                case EnemyState.Attacking: DoAttacking(); break;
                case EnemyState.Dead: DoDeath(); break;
                case EnemyState.Cheering: DoCheer(); break;
                case EnemyState.ReturnHome: GoHome(); break;
                case EnemyState.Stunned: DoStunned(); break;
                case EnemyState.Idle:
                case EnemyState.Searching:
                default: DoIdle(); break;
            }
        }

        private void GoHome()
        {
            // arrived home?
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                _currentState = EnemyState.Idle;    
            }

            // go home or to territory
            _navMeshAgent.SetDestination(home ? home.position : territory.position);
            _navMeshAgent.isStopped = false;

            // if player is in territory & not next to me: chase
            if (PlayerInTerritory() && !AmNextToTarget())
            {
                _currentState = EnemyState.Chase;
            }            
            
            // if we can see player & next to player: attack
            if (CanSeePlayer() && AmNextToTarget())
            {
                _currentState = EnemyState.Attacking;
            }
            
            CheckForPlayerDeath();
        }

        private bool CanSeePlayer()
        {
            return _visionSensor.VisibleObjects.Exists(go => go.CompareTag("Player"));
        }
        
        private bool AmNextToTarget()
        {
            if (!_navMeshAgent.isStopped)
            {
                return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
            }
            return _playerProximityDetectorSensor.NearPlayer;
        }

        private void DoIdle()
        {
            _navMeshAgent.isStopped = true;

            if (CanSeePlayer())
            {
                FaceTarget(_player.transform);
            }
            
            if (CanSeePlayer() && !AmNextToTarget() && PlayerInTerritory())
            {
                _currentState = EnemyState.Chase;
            }
            
            if (CanSeePlayer() && AmNextToTarget())
            {
                _currentState = EnemyState.Attacking;
            }

            CheckForPlayerDeath();
        }

        private void CheckForPlayerDeath()
        {
            if (!_playerHealth.IsAlive() && _timeOfPlayerDeath < 1)
            {
                _timeOfPlayerDeath = Time.time;
                _currentState = EnemyState.Cheering;
            }
        }

        private bool PlayerInChaseRange()
        {
             return Vector3.Distance(transform.position, _player.transform.position) <= chaseRange;
        }
        
        private void DoChase()
        {
            // animator.SetFloat("Speed", 0.5f);
            _navMeshAgent.SetDestination(_player.gameObject.transform.position);
            _navMeshAgent.isStopped = false;
            if (!PlayerInTerritory() && !AmNextToTarget() && !PlayerInChaseRange())
            {
                _currentState = EnemyState.ReturnHome;
                return;
            }            
            if (CanSeePlayer() && AmNextToTarget())
            {
                _currentState = EnemyState.Attacking;
            }
            CheckForPlayerDeath();
        }
        
        private void DoAttacking()
        {
            _navMeshAgent.isStopped = true;
            // animator.SetFloat("Speed", 0.0f);
            FaceTarget(_player.transform);
            
            if (!AmNextToTarget() && PlayerInTerritory())
            {
                _currentState = EnemyState.Chase;
                return;
            }

            if (!AmNextToTarget() && !PlayerInTerritory())
            {
                _currentState = EnemyState.ReturnHome;
                return;
            }
            
            if (CanAttack() && _playerHealth.IsAlive())
            {
                // time send last attack ended > 2 seconds
                _timeOfLastAttack = Time.time;
                
                _animator.SetTrigger(DoAttack);
            }

            CheckForPlayerDeath();
        }

        private bool CanAttack()
        {
            return Time.time - _timeOfLastAttack > attackDelaySeconds;
        }
        
        private void DoDeath()
        {
            _navMeshAgent.isStopped = true;
            _animator.SetBool(IsDead, true);
        }

        private void DoCheer()
        {
            _navMeshAgent.isStopped = true;
            
            if (_timeOfLastCheer < 1 && ((Time.time - _timeOfPlayerDeath) > cheerDelaySeconds))
            {
                _timeOfLastCheer = Time.time;
                _animator.SetTrigger(DoDab);
            }
        }
        
        public bool TakeDamage(int amount, PainTypes painType = PainTypes.Hit)
        {
            //if already dead: bail 
            if (!IsAlive()) return false;
            
            _currentHealth -= amount;
            if (_currentHealth <= 0)
            {
                _currentState = EnemyState.Dead;
                return true;
            }

            _weapon.DamageActive = false;
            _animator.SetTrigger(Hit);
            _currentState = EnemyState.Chase;
            return true;
        }

        public bool IsAlive()
        {
            return _currentHealth > 0;
        }

        public int CurrentHealth => _currentHealth;

        public int MaxHealth => maxHealth;

        private void FaceTarget(Transform target)
        {
            // determine vector to the target
            var vectorToTarget = target.position - transform.position;
            // rotate towards
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, vectorToTarget, Time.fixedDeltaTime/_turnSpeed, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }


        private bool PlayerInTerritory()
        {
            return Vector3.Distance(territory.transform.position, _player.transform.position) <= territoryRadius;
        }

        public void Stun()
        {
            if (!IsAlive()) return;
            _timeOfStun = Time.time;
            _currentState = EnemyState.Stunned;
            _navMeshAgent.isStopped = true;
            _animator.SetTrigger(Hit);
        }

        private void DoStunned()
        {
            _navMeshAgent.isStopped = true;
            if (Time.time - _timeOfStun >= _stunduration)
            {
                _currentState = EnemyState.Chase;
            }
        }
    } */
}