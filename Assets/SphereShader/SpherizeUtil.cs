using UnityEngine;

public static class SpherizeUtil {
    public static float CircumferenceAtLatitude(float radians) {
        float latitudeRadius = Mathf.Cos(radians);
        return 2.0f * Mathf.PI * latitudeRadius;
    }

    public static Vector2 Spherize(Vector2 uv) {
        float latitudeAngle = Mathf.PI /* / 2.0f * 2.0f */ * (uv.y - 0.5f);
        float latitudeCircumference = CircumferenceAtLatitude(latitudeAngle);

        return new Vector2(latitudeCircumference / (2.0f * Mathf.PI) * uv.x, uv.y);
    }
}
