using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeManager : MonoBehaviour
{
    [Header("Reference to the PlayerController")]
    [SerializeField] private PlayerController player;

    [Header("UI Sliders for Visual Upgrades")]
    [SerializeField] private Slider thrustersSlider;  // Visual indicator for thrusters upgrades
    [SerializeField] private Slider lasersSlider;     // Visual indicator for lasers upgrades

    [Header("Upgrade Settings")]
    public int maxUpgradeLevel = 3;
    private int currentThrusterUpgrade = 0;
    private int currentLaserUpgrade = 0;

    // Each upgrade costs 5 coins.
    [SerializeField] private int upgradeCost = 5;

    // Amount by which each upgrade affects the player's attributes.
    [SerializeField] private float thrusterSpeedIncrease = 100.0f;    // Increase in movement speed per upgrade
    [SerializeField] private float rotationSpeedIncrease = 30.0f;    // Increase in movement speed per upgrade
    [SerializeField] private float laserCooldownDecrease = 0.5f;      // Decrease in laser cooldown per upgrade

    // Variables to help prevent duplicate clicks
    private float lastThrusterUpgradeTime = -Mathf.Infinity;
    private float lastLaserUpgradeTime = -Mathf.Infinity;
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
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
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
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
