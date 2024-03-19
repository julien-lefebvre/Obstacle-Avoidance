using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour {
    [SerializeField] Transform chairPrefab;
    [SerializeField] Transform humanPrefab;
    [SerializeField] Transform goalPrefab;
    private static List<GameObject> agentsGameObjects = new List<GameObject>();
    private static float spawnDistanceThreshold = 4;    // Minimum distance needed with other agents to be able to spawn
    private static float minX = -20f;   
    private static float maxX = 20f;
    private static float minZ = -12f;
    private static float maxZ = 12f;
    private static float humanY = 1.5f;
    private static float chairY = 0.89f;
    private static int maxIterationsToFindSpawn = 50;   // Default maximum number of attempts at finding a suitable spawn point
    private int missingAgentsCount = 0; // Counts the number of agents that could not be spawned in

    private void Start() {
        // Retrieve the simulation settings
        GameObject simSettingsObject = GameObject.FindWithTag("Settings");
        SimSettings simSettings = simSettingsObject.GetComponent<SimSettings>();
        int numChairs = simSettings.numberOfChairs;
        int numHumans = simSettings.numberOfHumans;

        // Spawn the goal point
        Vector3 goalPos = FindSpawnPoint(maxIterationsToFindSpawn);
        if (goalPos != Vector3.up) {
            GameObject goal = Instantiate(goalPrefab, goalPos, Quaternion.identity).gameObject;
            agentsGameObjects.Add(goal);
        }

        // Spawn the humans
        for (int i = 0; i < numHumans; i++) {
            Vector3 spawnPoint = FindSpawnPoint(maxIterationsToFindSpawn);
            if (spawnPoint != Vector3.up) {
                GameObject human = Instantiate(humanPrefab, spawnPoint, Quaternion.identity).gameObject;
                agentsGameObjects.Add(human);
            } else {
                missingAgentsCount++;
            }
        }
    
        // Spawn the chairs
        for (int i = 0; i < numChairs; i++) {
            Vector3 spawnPoint = FindSpawnPoint(maxIterationsToFindSpawn);
            if (spawnPoint != Vector3.up) {
                spawnPoint.y = chairY;
                GameObject chair = Instantiate(chairPrefab, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0)).gameObject;
                agentsGameObjects.Add(chair);
            } else {
                missingAgentsCount++;
            }
        }

        // Show warning if some agents could not be spawned
        if (missingAgentsCount > 0) {
            Debug.LogWarning("Too many agents in the scene, " + missingAgentsCount + " agents could not be placed while avoiding overlapping.");
        }
    }

    // Tries to find an appropriate spawn point in the scene
    public static Vector3 FindSpawnPoint(int maxIterations) {
        Vector3 point = SampleRandomSpawnPoint();   // Sample a point in the scene
        bool success = false;
        int i=0;
        // Try a maximum of <maxIterations> times 
        while (i < maxIterations) {
            i++;
            if (CanSpawnAtPoint(point)) {
                success = true;
                break;
            } else {
                point = SampleRandomSpawnPoint();
            }
        }

        if (success) {
            return point;
        } else {
            return Vector3.up;  // Vector.up is used to represent there was no valid point found.
        }
    }

    // Returns a random point within the boundaries of the scene
    private static Vector3 SampleRandomSpawnPoint() {
        float posX = Random.Range(minX, maxX);
        float posZ = Random.Range(minZ, maxZ);
        Vector3 point = new(posX, humanY, posZ);
        return point;
    }

    // Returns whether or not a certain point is clear to spawn an agent (i.e. if it would be overlapping within a certain threshold)
    private static bool CanSpawnAtPoint(Vector3 point) {
        foreach (GameObject agent in agentsGameObjects) {
            Vector3 objPosition = agent.transform.position;

            // Calculate the distance between the spawn point and the object's position.
            float distance = Vector3.Distance(point, objPosition);

            // Check if the object is within the spawnDistanceThreshold.
            if (distance < spawnDistanceThreshold) {
                // An object is within the threshold, so return false.
                return false;
            }
        }

        // No objects found within the threshold, so return true.
        return true;
    }
}
