using UnityEngine;

public class PowerUpEngine : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public int maxPowerUpsPerLevel = 3;
    public float powerUpDropChance = .2f;

    private int powerUpsSpawnedThisLevel = 0;

    public bool CanSpawnPowerUp()
    {
        return powerUpsSpawnedThisLevel < maxPowerUpsPerLevel;
    }

    public void RegisterPowerUpSpawn()
    {
        powerUpsSpawnedThisLevel++;
    }

    public void ResetForNewLevel()
    {
        powerUpsSpawnedThisLevel = 0;
    }

    public bool ShouldBecomePowerUpBrick()
    {
        return Random.value < powerUpDropChance && CanSpawnPowerUp();
    }
}