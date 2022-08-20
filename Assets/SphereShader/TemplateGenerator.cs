using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class TemplateGenerator : MonoBehaviour {
    public ComputeShader shader;

    public int ySize = 32;

    private Texture2D cpuSpherizedTemplate;
    private RenderTexture gpuSpherizedTemplate;

    private int generateSpherizedTemplateKernel = -1;
    private uint threadGroupSizeX = 0;
    private uint threadGroupSizeY = 0;

    public Text spherizedCpuTemplateTime;
    public Text spherizedGpuTemplateTime;

    public Image spherizedCpuTemplateDisplay;
    public Image spherizedGpuTemplateDisplay;

    public bool regenerate = false;

    private void Start() {
        // CPU template
        cpuSpherizedTemplate = new Texture2D(2 * ySize, ySize, TextureFormat.ARGB32, false, true) {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Repeat
        };

        spherizedCpuTemplateDisplay.material.mainTexture = cpuSpherizedTemplate;

        GenerateCpuTemplate();

        // GPU template
        generateSpherizedTemplateKernel = shader.FindKernel("GenerateSpherizedTemplate");

        shader.GetKernelThreadGroupSizes(
            generateSpherizedTemplateKernel,
            out threadGroupSizeX,
            out threadGroupSizeY,
            out _);

        gpuSpherizedTemplate = RenderTextureUtil.GenerateRenderTexture(2 * ySize, ySize);

        spherizedGpuTemplateDisplay.material.mainTexture = gpuSpherizedTemplate;

        GenerateGpuTemplate();
    }

    private void Update() {
        if (regenerate) {
            regenerate = false;

            GenerateCpuTemplate();
            GenerateGpuTemplate();
        }
    }

    private void OnDestroy() {
        RenderTextureUtil.CleanUpRenderTexture(ref gpuSpherizedTemplate);
    }

    private void GenerateCpuTemplate() {
        Stopwatch cpuTemplateStopwatch = new Stopwatch();
        cpuTemplateStopwatch.Start();

        for (int y = 0; y < cpuSpherizedTemplate.height; y++) {
            for (int x = 0; x < cpuSpherizedTemplate.width; x++) {
                Vector2 uv = new Vector2(
                    (float)x / (cpuSpherizedTemplate.width - 1),
                    (float)y / (cpuSpherizedTemplate.height - 1));

                Vector2 uvSpherized = SpherizeUtil.Spherize(uv);

                int xSpherized = Mathf.CeilToInt((cpuSpherizedTemplate.width - 1) * uvSpherized.x);

                cpuSpherizedTemplate.SetPixel(xSpherized, y, new Color(uvSpherized.x, uv.y, 0.5f));
            }
        }

        cpuSpherizedTemplate.Apply();

        cpuTemplateStopwatch.Stop();

        spherizedCpuTemplateTime.text = cpuTemplateStopwatch.ElapsedMillisecondsWithFraction().ToString("F4");
    }

    private void GenerateGpuTemplate() {
        Stopwatch gpuTemplateStopwatch = new Stopwatch();
        gpuTemplateStopwatch.Start();

        shader.SetInt("Width", gpuSpherizedTemplate.width);
        shader.SetInt("Height", gpuSpherizedTemplate.height);
        shader.SetTexture(generateSpherizedTemplateKernel, "Result", gpuSpherizedTemplate);
        shader.Dispatch(
            generateSpherizedTemplateKernel,
            Math.Max(1, gpuSpherizedTemplate.width / (int)threadGroupSizeX),
            Math.Max(1, gpuSpherizedTemplate.height / (int)threadGroupSizeY),
            1);

        gpuTemplateStopwatch.Stop();

        spherizedGpuTemplateTime.text = gpuTemplateStopwatch.ElapsedMillisecondsWithFraction().ToString("F4");
    }
}
