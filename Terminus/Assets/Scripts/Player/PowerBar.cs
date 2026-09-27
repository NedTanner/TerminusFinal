using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public Image powerBar;
    [SerializeField] PlayerPower playerPower;


    public void UpdatePowerBar()
    {
        GameObject playerCharacter = GameObject.FindWithTag("Player");
        playerPower = playerCharacter.GetComponent<PlayerPower>();
        float currentPower = playerPower.CurrentPower;
        float maxPower = playerPower.MaxPower;
        powerBar.fillAmount = currentPower / maxPower;
    }
}
