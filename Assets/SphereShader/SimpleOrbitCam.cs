using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleOrbitCam : MonoBehaviour {
    public float xSensitivity = 100;
    public float ySensitivity = 100;
    public float distanceSensitivity = 1;

    public float x = 0;
    public float y = 0;
    public float distance = 3;

    private Transform cachedTransform;

    private void Awake() {
        cachedTransform = transform;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            x += xSensitivity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            x -= xSensitivity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            y += ySensitivity * Time.deltaTime;
            if (y > 90) {
                y = 90;
            }
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            y -= ySensitivity * Time.deltaTime;
            if (y < -90) {
                y = -90;
            }
        }

        if (Input.GetKey(KeyCode.PageUp)) {
            distance -= distanceSensitivity * Time.deltaTime * distance;
        }

        if (Input.GetKey(KeyCode.PageDown)) {
            distance += distanceSensitivity * Time.deltaTime * distance;
        }

        cachedTransform.localRotation = Quaternion.Euler(new Vector3(y, x, 0.0f));
        cachedTransform.localPosition = cachedTransform.localRotation * new Vector3(0.0f, 0.0f, -distance);
    }
}
