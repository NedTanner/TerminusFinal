using Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

public class EvilEnemyHealthBar : MonoBehaviour
{
    [SerializeField] private int viewableDistance = 10;
    private VisualElement _rootVe;
    private ProgressBar _healthBar;
    private IDamageable _owner;
    private Transform _playerCamera;
    private Transform _player;
    private UIDocument _ui;

    private void Start()
    {
        _playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _owner = GetComponentInParent<IDamageable>();
        _rootVe = GetComponent<UIDocument>().rootVisualElement;
        _healthBar = _rootVe.Q<ProgressBar>("healthBar");
    }

    private bool PlayerInRange()
    {
        return Vector3.Distance(_player.position, transform.position) <= viewableDistance;
    }
    
    private void Update()
    {
        // only show if we are alive, have taken damage and the player is close 
        if (_owner.IsAlive() && _owner.CurrentHealth != _owner.MaxHealth && PlayerInRange())
        {
            _rootVe.visible = true;
            // face the player
            Quaternion look = Quaternion.LookRotation(transform.position - _playerCamera.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 1);

            _healthBar.lowValue = 0;
            _healthBar.highValue = _owner.MaxHealth;
            _healthBar.value = _owner.CurrentHealth;
        }
        else
        {
            _rootVe.visible = false;
        }
    }
}