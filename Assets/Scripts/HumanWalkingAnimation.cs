using UnityEngine;

public class HumanWalkingAnimation : MonoBehaviour {

    [SerializeField] private float floatHeight = 0.1f;
    [SerializeField] private float floatSpeed = 20.0f;
    private float phaseOffsetRange = 2.0f * Mathf.PI;
    private Vector3 parentPosition;
    private float phaseOffset;

    private void Start() {
        parentPosition = transform.parent.position;
        phaseOffset = Random.Range(0.0f, phaseOffsetRange);
    }
    
    private void FixedUpdate() {
        parentPosition = transform.parent.position;

        // If human is walking, make it float up and down following a sinus
        if(transform.parent.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0.5) {
            float verticalOffset = Mathf.Sin((Time.time + phaseOffset) * floatSpeed) * floatHeight;
            transform.position = parentPosition + new Vector3(0.0f, verticalOffset, 0.0f);
        }
        
    }
}
