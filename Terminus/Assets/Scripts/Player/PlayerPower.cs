using UnityEngine;

public class PlayerPower : MonoBehaviour
{
    [SerializeField] private float maxPower = 10f;
    [SerializeField] PowerBar powerBar;
    private float currentPower;
    [SerializeField] public float attackDamage = 5f;
    private PlayerCombat playerCombat;
    [SerializeField] DamageZone damageZone;
    public float MaxPower
    {
        get => maxPower;
        set
        {
            maxPower = value;
            currentPower = Mathf.Clamp(value, 0, maxPower);
        }
    }
    public float CurrentPower
    {
        get => currentPower;
        private set
        {
            currentPower = Mathf.Clamp(value, 0, maxPower);

        }
    }

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
       
        currentPower = maxPower;
    }
    private void Update()
    {
        if (currentPower < maxPower)
        {
            PowerRegen();
        }

    }
    public void UsePower()
    {
        
        float weaponPower = playerCombat.weaponPower;
        currentPower -= weaponPower;
        powerBar.UpdatePowerBar();
        DamageMod();
        if (currentPower < 0)
        {
            currentPower = 0;
        }
        

    }

    public void PowerRegen()
    {


        if (currentPower < maxPower)
        {
            float weaponRegen = playerCombat.weaponRegen;
            currentPower += weaponRegen;
            powerBar.UpdatePowerBar();
            
        }
      
    }

    public void DamageMod()
    {
       
        float powerModifier = currentPower * 10f / 100f;
        float modifiedDamage = powerModifier * attackDamage;
        damageZone.finalDamage = modifiedDamage;
        // print(modifiedDamage);
        
    }
 }