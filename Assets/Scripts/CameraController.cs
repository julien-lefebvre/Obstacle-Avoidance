using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 2.0f;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start() {
        // Store the initial camera position and rotation.
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update() {
        // Get the WASD user input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Camera movement direction, keep Y value constant
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Convert the local movement to world space.
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection.y = 0; // Ignore vertical movement.

        // Apply the movement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Mouse Input for Camera Rotation. (Left mouse button click)
        if (Input.GetMouseButton(0)) {
            
            // Get mouse movement input
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Rotate around Y-axis
            transform.Rotate(Vector3.up * mouseX * rotationSpeed);

            // Rotate around X-axis
            transform.Rotate(Vector3.left * mouseY * rotationSpeed);
            
            // Lock the Z-axis rotation.
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }

        // Reset camera to the initial position and rotation with 'R' key
        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }
    }
}
