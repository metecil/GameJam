using UnityEngine;

public class PowerUpPanelUI : MonoBehaviour
{
    // References to the child UI elements.
    [SerializeField] private GameObject pressF;
    [SerializeField] private GameObject laserSpray;
    [SerializeField] private GameObject invincible;

    // Enum to define power-up types.
    public enum PowerUpType
    {
        LaserSpray,
        Invincible
    }

    private void Start()
    {
        // Ensure all UI elements are disabled by default.
        HidePowerUpPanel();
    }

    /// <summary>
    /// Shows the power-up UI panel with the corresponding power-up indicator.
    /// PressF is always active when a power-up is ready.
    /// </summary>
    /// <param name="type">The type of power-up that is active.</param>
    public void ShowPowerUp(PowerUpType type)
    {
        if(pressF != null)
            pressF.SetActive(true);

        // Activate only the corresponding power-up indicator.
        if (type == PowerUpType.LaserSpray)
        {
            if (laserSpray != null)
                laserSpray.SetActive(true);
            if (invincible != null)
                invincible.SetActive(false);
        }
        else if (type == PowerUpType.Invincible)
        {
            if (invincible != null)
                invincible.SetActive(true);
            if (laserSpray != null)
                laserSpray.SetActive(false);
        }
    }

    /// <summary>
    /// Hides the power-up panel UI.
    /// </summary>
    public void HidePowerUpPanel()
    {
        if (pressF != null)
            pressF.SetActive(false);
        if (laserSpray != null)
            laserSpray.SetActive(false);
        if (invincible != null)
            invincible.SetActive(false);
    }
}
