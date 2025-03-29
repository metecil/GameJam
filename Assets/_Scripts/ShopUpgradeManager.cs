using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopUpgradeManager : MonoBehaviour
{
    [Header("Reference to the PlayerController")]
    [SerializeField] private PlayerController player;

    [Header("UI Sliders for Visual Upgrades")]
    [SerializeField] private Slider thrustersSlider;  // Visual indicator for thrusters upgrades
    [SerializeField] private Slider lasersSlider;     // Visual indicator for lasers upgrades
    [SerializeField] private float lastInvincibilityTime;

    [Header("Upgrade Settings")]
    public int maxUpgradeLevel = 3;
    private int currentThrusterUpgrade = 0;
    private int currentLaserUpgrade = 0;
    private int invincibilityCost = 25;
    [SerializeField] private PowerUpPanelUI powerUpPanelUI;


    // Each upgrade costs 5 coins.
    [SerializeField] private int upgradeCost = 5;

    // Cost for the Laser Spray Power-Up.
    [SerializeField] private int laserSprayCost = 15;

    // Amount by which each upgrade affects the player's attributes.
    [SerializeField] private float thrusterSpeedIncrease = 100.0f;    // Increase in movement speed per upgrade
    [SerializeField] private float rotationSpeedIncrease = 30.0f;     // Increase in rotation speed per upgrade
    [SerializeField] private float laserCooldownDecrease = 0.5f;      // Decrease in laser cooldown per upgrade

    // Variables to help prevent duplicate clicks.
    private float lastThrusterUpgradeTime = -Mathf.Infinity;
    private float lastLaserUpgradeTime = -Mathf.Infinity;
    private float lastLaserSprayTime = -Mathf.Infinity;

    // Time window (in unscaled seconds) within which additional clicks are ignored.
    private const float upgradeCooldown = 0.2f;

    void Start()
    {
        // Configure the slider ranges and initial values.
        if (thrustersSlider != null)
        {
            thrustersSlider.minValue = 0;
            thrustersSlider.maxValue = maxUpgradeLevel;
            thrustersSlider.value = currentThrusterUpgrade;
        }
        if (lasersSlider != null)
        {
            lasersSlider.minValue = 0;
            lasersSlider.maxValue = maxUpgradeLevel;
            lasersSlider.value = currentLaserUpgrade;
        }
    }

    // Called from the Thrusters button OnClick event.
    public void UpgradeThrusters()
    {
        // Prevent multiple triggers within a short time window.
        if (Time.unscaledTime - lastThrusterUpgradeTime < upgradeCooldown)
            return;
        lastThrusterUpgradeTime = Time.unscaledTime;

        // Check if we haven't reached the max upgrade level and have enough coins.
        if (currentThrusterUpgrade < maxUpgradeLevel && GameManager.instance.GetCoins() >= upgradeCost)
        {
            // Subtract exactly 5 coins.
            GameManager.instance.AddCoins(-upgradeCost);

            currentThrusterUpgrade++;
            // Increase player's movement and rotation speed.
            player.IncreaseMovementSpeed(thrusterSpeedIncrease, rotationSpeedIncrease);

            // Update the slider to reflect the new upgrade level.
            if (thrustersSlider != null)
            {
                thrustersSlider.value = currentThrusterUpgrade;
            }
        }

        // Immediately clear the current selection to reset button state.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Called from the Lasers button OnClick event.
    public void UpgradeLasers()
    {
        // Prevent multiple triggers within a short time window.
        if (Time.unscaledTime - lastLaserUpgradeTime < upgradeCooldown)
            return;
        lastLaserUpgradeTime = Time.unscaledTime;

        // Check if we haven't reached the max upgrade level and have enough coins.
        if (currentLaserUpgrade < maxUpgradeLevel && GameManager.instance.GetCoins() >= upgradeCost)
        {
            // Subtract exactly 5 coins.
            GameManager.instance.AddCoins(-upgradeCost);
            currentLaserUpgrade++;
            // Decrease the laser cooldown to increase firing rate.
            player.DecreaseLaserCooldown(laserCooldownDecrease);

            // Update the slider to reflect the new upgrade level.
            if (lasersSlider != null)
            {
                lasersSlider.value = currentLaserUpgrade;
            }
        }

        // Immediately clear the current selection to reset button state.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Called from the Laser Spray Power-Up button OnClick event.
    // This method unlocks the power-up for later use via the F key.
    public void ActivateLaserSprayPowerUp()
    {
        // Prevent multiple triggers within a short time window.
        if (Time.unscaledTime - lastLaserSprayTime < upgradeCooldown)
            return;
        lastLaserSprayTime = Time.unscaledTime;

        // Check if the player already has the Laser Spray power-up.
        if (player.HasLaserSprayPowerUp)
        {
            Debug.Log("Laser Spray Power-Up is already unlocked.");
        }
        else if (GameManager.instance.GetCoins() >= laserSprayCost)
        {
            // Deduct the coins and unlock the power-up.
            GameManager.instance.AddCoins(-laserSprayCost);
            player.UnlockLaserSpray();
            powerUpPanelUI.ShowPowerUp(PowerUpPanelUI.PowerUpType.LaserSpray);
        }
        else
        {
            Debug.Log("Not enough coins for Laser Spray Power-Up!");
        }

        // Immediately clear the current selection to reset button state.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    // Called from the Invincibility Power-Up button OnClick event.
    public void ActivateInvincibilityPowerUp()
    {
        if (Time.unscaledTime - lastInvincibilityTime < upgradeCooldown)
            return;
        lastInvincibilityTime = Time.unscaledTime;

        if (player.HasInvincibilityPowerUp)
        {
            Debug.Log("Invincibility Power-Up is already unlocked.");
        }
        else if (GameManager.instance.GetCoins() >= invincibilityCost)
        {
            GameManager.instance.AddCoins(-invincibilityCost);
            player.UnlockInvincibility();
            powerUpPanelUI.ShowPowerUp(PowerUpPanelUI.PowerUpType.Invincible);
        }
        else
        {
            Debug.Log("Not enough coins for Invincibility Power-Up!");
        }

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
}
