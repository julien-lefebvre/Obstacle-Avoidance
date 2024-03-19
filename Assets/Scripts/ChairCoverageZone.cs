using UnityEngine;

public class ChairCoverageZone : MonoBehaviour {
    [SerializeField] private float movementSpeed = 1f;  // Default chair movement speed
    [SerializeField] private Rigidbody rb;
    private Transform target; // Stores a pointer to the human the chair is currently targeting
    private Vector3 initialPosition;  // Spawn position of the chair
    
    private void Start() {
        initialPosition = transform.position;
    }

    private void FixedUpdate() {
        Vector3 movementVector = Vector3.zero;
        // Check if chair has a target, and if it has not gone too far beyond its spawn point
        if (target && (initialPosition - transform.position).magnitude < 3) {
            // If so, chair move towards the projected position of the target
            Vector3 humanDirection = target.gameObject.GetComponent<Human>().directions[3];
            movementVector = (target.position + 3*humanDirection - transform.position).normalized;
        } 
        // Otherwise, if its not already at its position, make the chair go back to its spot
        else if ((initialPosition - transform.position).magnitude > 0.5) {
            movementVector = (initialPosition - transform.position).normalized;
        }
        
        // Apply the calculated movement vectors
        movementVector *= movementSpeed;
        movementVector.y = 0;
        rb.velocity = movementVector;
    }

    // Assigns the target variable to the human that enters its coverage zone
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Human")) {
            target = other.transform;
        }
    }

    // Removes the target if they leave its coverage zone
    private void OnTriggerExit(Collider other) {
        target = null;
    }
}

