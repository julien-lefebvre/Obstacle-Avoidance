using UnityEngine;

public class Human : MonoBehaviour {
    [SerializeField] private float movementSpeed = 2.0f;    // Default movement speed for humans 
    private GameObject goal;    // Reference to the goal zone object
    private Rigidbody rb;
    private float sensorRange = 2.0f;   // Range of the rays casted 
    private int rayCount = 7;   // Number of rays casted
    private float toleranceRadius = 0.8f;   // Initial size of the tolerance circle
    private int pathUpdateInterval = 10;  // Update human direction every 10 frames
    private int frameCounter = 0;
    private float[] basePriorities = {0.97f, 0.98f, 0.99f, 1f, 0.99f, 0.98f, 0.97f};
    [HideInInspector] public float[] priorities = {0.97f, 0.98f, 0.99f, 1f, 0.99f, 0.98f, 0.97f}; // List storing the priorities for each of the 7 rays from the sensor
    [HideInInspector] public Vector3[] directions = new Vector3[7]; // List storing the directions for each of the 7 rays from the sensor
    private Vector3 movementVector = Vector3.zero;


    private void Start() {
        rb = GetComponent<Rigidbody>();
        goal = GameObject.FindWithTag("Goal");
        GameObject simSettingsObject = GameObject.FindWithTag("Settings");
        SimSettings simSettings = simSettingsObject.GetComponent<SimSettings>();
        if (simSettings.showHumanSensors) {
            GameObject sensorVisuals = transform.GetChild(transform.childCount-1).gameObject;
            sensorVisuals.SetActive(true);
        }
    }

    private void FixedUpdate() {
        // If the goal is active
        if (goal.activeInHierarchy) {
            frameCounter++;
            // If we should update the pathing at this frame
            if (frameCounter >= pathUpdateInterval) {
                frameCounter = 0;

                Vector3 goalPosition = goal.transform.position; // Get the goal position
                int highestPriorityIndex = 3;   // Initialize the highest priority index to the forward direction (index 3)

                // Set all the priorities to the base priorities
                for (int i = 0; i < 7; i++) {
                    priorities[i] = basePriorities[i];
                }
                
                // For each of the 7 directions, calculate the priority value based on the raycast results
                for (int i = 0; i < rayCount; i++) {
                    float angle = -75 + 25 * i; // Angle of the ray direction
                    Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * (goalPosition-transform.position).normalized;
                    directions[i] = rayDirection;   // Update the direction list

                    if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, sensorRange)) {
                        if (hit.collider.CompareTag("Chair") || hit.collider.CompareTag("Human") || hit.collider.CompareTag("Wall")) {
                            // If the ray hit one of chairs, humans, or wall, compute the priority value 
                            float priorityReduction = 1f / (1+hit.distance);    // Priority reduction value based on the hit distance
                            priorities[i] -= priorityReduction; // Update the priority for this direction
                            // Propagate priority reduction to neighbours
                            if (i>0) priorities[i-1] -= priorityReduction * .5f;    // 50% to immediate neighbours
                            if (i<6) priorities[i+1] -= priorityReduction * .5f;
                            if (i>1) priorities[i-2] -= priorityReduction * .25f;   // 25% to second neighbours
                            if (i<5) priorities[i+2] -= priorityReduction * .25f;
                        }
                    }
                }

                // Fin the direction with the highest priority
                for (int i = 0; i < 7; i++) {
                    if (priorities[i] > priorities[highestPriorityIndex] + 0.1) {
                        highestPriorityIndex = i; 
                    }
                }

                // If highest priority is less than tolerance, wait and slowly reduce tolerance radius
                if (priorities[highestPriorityIndex] < toleranceRadius) {
                    movementVector = directions[highestPriorityIndex] * 0;
                    toleranceRadius -= Time.fixedDeltaTime;
                } 
                // Otherwise, move in direction of highest priority
                else {
                    movementVector = directions[highestPriorityIndex] * movementSpeed;
                }

            }

            // Apply the calculated velocity vector
            rb.velocity = movementVector;
        }
    }

    // Public getter for the sensorRange
    public float GetSensorRange() {
        return sensorRange;
    }

    // Public getter for the tolerance value
    public float GetToleranceRadius() {
        return toleranceRadius;
    }
}
