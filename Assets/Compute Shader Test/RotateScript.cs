using UnityEngine;

public class RotateScript : MonoBehaviour {
    public Vector3 angularVelocity;

    private void Update() {
        transform.Rotate(Time.deltaTime * angularVelocity);
    }
}
