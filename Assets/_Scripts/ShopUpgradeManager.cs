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
        // Check if we haven't reached the max upgrade level and have enough coins.
        if (currentThrusterUpgrade < maxUpgradeLevel && GameManager.instance.GetCoins() >= upgradeCost)
        {
            // Subtract the upgrade cost from coins.
            GameManager.instance.AddCoins(-upgradeCost);
            
            currentThrusterUpgrade++;
            // Increase player's movement speed.
            player.IncreaseMovementSpeed(thrusterSpeedIncrease, rotationSpeedIncrease);

            // Update the slider to reflect the new upgrade level.
            if (thrustersSlider != null)
            {
                thrustersSlider.value = currentThrusterUpgrade;
            }
        }
    }

    // Called from the Lasers button OnClick event.
    public void UpgradeLasers()
    {
        // Check if we haven't reached the max upgrade level and have enough coins.
        if (currentLaserUpgrade < maxUpgradeLevel && GameManager.instance.GetCoins() >= upgradeCost)
        {
            // Subtract the upgrade cost from coins.
            GameManager.instance.AddCoins(-upgradeCost);
            
            currentLaserUpgrade++;
            // Decrease the laser cooldown to increase the firing rate.
            player.DecreaseLaserCooldown(laserCooldownDecrease);

            // Update the slider to reflect the new upgrade level.
            if (lasersSlider != null)
            {
                lasersSlider.value = currentLaserUpgrade;
            }
        }
    }
}
