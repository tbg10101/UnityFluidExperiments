using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TestSceneController : MonoBehaviour {
    public ComputeShader shader;

    // we need this intermediate render texture because we cannot set enableRandomWrite on 'destination'
    public RenderTexture tempDestination;

    public int kernelNumber = -1;

    public int width;
    public int height;

    public bool sRGB;

    public float speed = 10.0f;

    public Texture2D startTexture;

    public MeshRenderer outputObject;

    private void Start() {
        kernelNumber = shader.FindKernel("CSMain");

        tempDestination = new RenderTexture(
            startTexture.width,
            startTexture.height,
            0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear) {
            enableRandomWrite = true,
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Bilinear,
            antiAliasing = 1,
            autoGenerateMips = true,
            anisoLevel = 1
        };
        tempDestination.Create();

        width = tempDestination.width;
        height = tempDestination.height;
        sRGB = tempDestination.sRGB;

        Graphics.Blit(startTexture, tempDestination);
    }

    private void Update() {
        RenderTexture tempSource = RenderTexture.GetTemporary(
            startTexture.width,
            startTexture.height,
            0,
            tempDestination.graphicsFormat,
            1);

        // copy the result to the source
        Graphics.Blit(tempDestination, tempSource);

        // call the compute shader
        shader.SetVector("Dimensions", new Vector2(tempSource.width, tempSource.height));
        shader.SetFloat("DeltaTime", Time.deltaTime);
        shader.SetFloat("Speed", speed);
        shader.SetTexture(kernelNumber, "Source", tempSource);
        shader.SetTexture(kernelNumber, "Result", tempDestination);
        shader.Dispatch(
            kernelNumber,
            (tempDestination.width + 7) / 8,
            (tempDestination.height + 7) / 8,
            1);

        outputObject.material.mainTexture = tempDestination;

        RenderTexture.ReleaseTemporary(tempSource);
    }

    private void OnDestroy() {
        if (tempDestination != null) {
            tempDestination.Release();
            tempDestination = null;
        }
    }
}
