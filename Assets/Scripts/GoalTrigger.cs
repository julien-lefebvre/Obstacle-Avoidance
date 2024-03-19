using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GoalTrigger : MonoBehaviour {
    private TextMeshProUGUI timerText;  // UI text element displaying the 10s timer
    private bool isCleared = false;
    private float goalTimeLimit = 10f;  // Time limit for humans to reach the goal before it changes
    private float spawnDelay = 0.5f;    // Delay before spawning the new goal 
    private float timeSinceLastReached = 0f;
    private GameObject currentCrown;    // The crown worn by the human winner

    private void Start() {
        // Retrieve the timer text element
        GameObject timer = GameObject.FindWithTag("Timer");
        timerText = timer.GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        if (!isCleared) {
            // Update the timer
            timeSinceLastReached += Time.deltaTime;
            timerText.text = "Timer: " + timeSinceLastReached.ToShortString();
            // Check if no humans have reached the goal for a certain amount of time.
            if (timeSinceLastReached >= goalTimeLimit) {
                currentCrown?.SetActive(false); // Remove the crown
                ClearGoal();   // Reset the goal
            }
        }
    }

    // Checks when a human enters the goal zone
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Human")) {
            if (!isCleared) {
                currentCrown?.SetActive(false);
                currentCrown = other.transform.GetChild(0).GetChild(0).gameObject;
                currentCrown.SetActive(true);
                ClearGoal();
            }
        }
    }

    // Clears the goal and calls for it to respawn elsewhere after a delay
    private void ClearGoal() {
        // Clear the goal.
        isCleared = true;
        gameObject.SetActive(false);
        timeSinceLastReached = 0; // Reset the timer.

        // Respawn the goal after a delay using Invoke.
        Invoke("RespawnGoal", spawnDelay);
    }

    // Try and find a new valid spawn point, and move the goal there
    private void RespawnGoal() {
        // Respawn the goal at a new position.
        Vector3 newPosition = AgentSpawner.FindSpawnPoint(100);
        if (newPosition != Vector3.up) {
            transform.position = newPosition;
            gameObject.SetActive(true);
        } else {
            Debug.LogError("The goal could not be placed. The spawner tried 100 times but could not find a spot without overlapping other agents. Restart the simulation and consider using a smaller number of agents.");
        }

        isCleared = false;
    }
}
