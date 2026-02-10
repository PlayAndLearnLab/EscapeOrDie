using UnityEngine;

public class KillerRobotManager : MonoBehaviour
{
    [Header("Robots")]
    public GameObject killerRobot1;
    public GameObject killerRobot2;

    [Header("Robot AI scripts")]
    public KillerRobotAI robotAI1;
    public KillerRobotAI robotAI2;

    private bool robotsSpawned = false;

    void Start()
    {
        if (killerRobot1 != null)
            killerRobot1.SetActive(false);

        if (killerRobot2 != null)
            killerRobot2.SetActive(false);
    }

    public void SpawnRobots(int floorNumber)
    {
        if (robotsSpawned) return; // Only spawn once

        if (killerRobot1 != null)
            killerRobot1.SetActive(true);

        if (killerRobot2 != null)
            killerRobot2.SetActive(true);

        robotsSpawned = true;

        if (robotAI1 != null)
            Debug.Log($"Killer Robot 1 activated on floor {floorNumber}!");

        if (robotAI2 != null)
            Debug.Log($"Killer Robot 2 activated on floor {floorNumber}!");
    }
}
