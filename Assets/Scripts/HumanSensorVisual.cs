using UnityEngine;
using UnityEngine.UI;

public class HumanSensorVisual : MonoBehaviour {

    [SerializeField] private Human human;   // Reference to the human
    [SerializeField] private Image[] rayPriorityBars;   // Reference to the priority bars UI elements (showing normalized priority value)
    [SerializeField] private Image[] rayBackgroundBars; // Reference top background bars UI elements
    [SerializeField] private Image toleranceCircle; // Reference to the tolerance circle UI element


    public void FixedUpdate() {

        UpdateToleranceCircleVisual();

        for (int i=0; i<7; i++) {
            UpdateRayBarVisual(i);
        }
    }

    // Update the size of the circle drawn to match the tolerance radius of the human
    private void UpdateToleranceCircleVisual() {
        float humanToleranceRadius = human.GetToleranceRadius();
        float sensorRange = human.GetSensorRange();
        float circleDiameter = 2f*sensorRange*humanToleranceRadius;

        toleranceCircle.rectTransform.sizeDelta = new Vector2(circleDiameter, circleDiameter);
    }

    // Update the ray directions and the priority values visuals on the human sensor
    private void UpdateRayBarVisual(int i) {
        // Get the angle of the ray from the positive x axis
        Vector3 cross = Vector3.Cross(Vector3.right, human.directions[i]);
        var angle = Vector3.Angle(Vector3.right, human.directions[i]);
        if (cross.y > 0) {
            angle = 360 - angle;
        }

        // Apply the rotation to the rays UI elements
        rayPriorityBars[i].transform.rotation = Quaternion.Euler(90, 0, angle);
        rayBackgroundBars[i].transform.rotation = Quaternion.Euler(90, 0, angle);

        // Update the filling value to match the priority value
        rayPriorityBars[i].fillAmount = human.priorities[i];

        // Update the color indicator based on priority value
        if (human.priorities[i] > 0.85) {
            rayPriorityBars[i].color = Color.green;
        } else if (human.priorities[i] > 0.49) {
            rayPriorityBars[i].color = Color.yellow;
        } else {
            rayPriorityBars[i].color = Color.red;
        }
    }
}
