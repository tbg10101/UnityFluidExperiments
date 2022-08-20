static const float PI = 3.14159265359f;

float CircumferenceAtLatitude(float radians) {
    float latitudeRadius = cos(radians);
    return 2.0f * PI * latitudeRadius;
}

float2 SpherizeUv(float2 uv) {
    float latitudeAngle = PI /* / 2.0f * 2.0f */ * (uv.y - 0.5f);
    float latitudeCircumference = CircumferenceAtLatitude(latitudeAngle);

    return float2(latitudeCircumference / (2.0f * PI) * uv.x, uv.y);
}

uint2 SpherizedPixel(uint2 pixel, int width, int hight) {
    float2 uv = float2(
        (float)pixel.x / (width - 1),
        (float)pixel.y / (hight - 1));

    float2 uvSpherized = SpherizeUv(uv);

    int xSpherized = ceil((width - 1) * uvSpherized.x);

    return uint2(xSpherized, pixel.y);
}

// TODO add de-spherization
// TODO the de-sperization might be done using a clever UV map?

// TODO add fluid sim functions
// TODO add a way to get neighbors in sphereized space (x-axis is easy [one on each side] but there may be multiple y-axis neighboors on each side)
// TODO does the distance to each neighbors affect transmission of dye and velocity?

// TODO we can do multiple layers of dye with the same velocity to simulate different cloud layers (one for the base, one for upper-level clouds)
