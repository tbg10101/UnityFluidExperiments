using System;
using UnityEngine;
using UnityEngine.UI;

public class SphereShaderController : MonoBehaviour {
    public new Camera camera;

    public ComputeShader shader;

    public int ySize = 32;

    private RenderTexture destination;
    private RenderTexture persistedDestination;

    public int hitX = -1;
    public int hitY = -1;

    private MeshRenderer meshRenderer;

    private int cursorPaintMainKernel = -1;

    public Color cursorColor = Color.yellow;

    public Image spherizedTextureDisplay;
    public Image finalTextureDisplay;

    private void Start() {
        ySize = Mathf.Max(
            32,
            Mathf.Min(
                8192,
                Mathf.NextPowerOfTwo(ySize))); // ensures power of 2

        cursorPaintMainKernel = shader.FindKernel("CursorPaint");

        meshRenderer = GetComponent<MeshRenderer>();

        destination = RenderTextureUtil.GenerateRenderTexture(2 * ySize, ySize);
        persistedDestination = RenderTextureUtil.GenerateRenderTexture(2 * ySize, ySize);

        meshRenderer.material.mainTexture = destination;
        finalTextureDisplay.material.mainTexture = persistedDestination;
    }

    private void Update() {
        hitX = hitY = -1;

        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out var hit) && hit.transform.gameObject == gameObject) {
            Vector2 hitTextureCoordinate = hit.textureCoord;

            hitX = (int) (hitTextureCoordinate.x * destination.width);
            hitY = (int) (hitTextureCoordinate.y * destination.height);
        }

        RenderTexture tempSource = RenderTexture.GetTemporary(
            destination.width,
            destination.height,
            0,
            destination.graphicsFormat,
            1);

        Graphics.Blit(persistedDestination, tempSource);

        shader.GetKernelThreadGroupSizes(
            cursorPaintMainKernel,
            out uint threadGroupSizeX,
            out uint threadGroupSizeY,
            out _);

        shader.SetBool("CursorOver", hitX >= 0);
        shader.SetBool("CursorClicked", hitX >= 0 && Input.GetMouseButton(0));
        shader.SetVector("CursorColor", cursorColor);
        shader.SetVector("CursorPosition", new Vector2(Mathf.Max(0.0f, hitX), Mathf.Max(0.0f, hitY)));
        shader.SetInt("Width", persistedDestination.width);
        shader.SetInt("Height", persistedDestination.height);
        shader.SetTexture(cursorPaintMainKernel, "Source", tempSource);
        shader.SetTexture(cursorPaintMainKernel, "Result", destination);
        shader.SetTexture(cursorPaintMainKernel, "PersistedResult", persistedDestination);
        shader.Dispatch(
            cursorPaintMainKernel,
            Math.Max(1, persistedDestination.width / (int)threadGroupSizeX),
            Math.Max(1, persistedDestination.height / (int)threadGroupSizeY),
            1);

        RenderTexture.ReleaseTemporary(tempSource);
    }

    private void OnDestroy() {
        RenderTextureUtil.CleanUpRenderTexture(ref destination);
        RenderTextureUtil.CleanUpRenderTexture(ref persistedDestination);
    }
}
