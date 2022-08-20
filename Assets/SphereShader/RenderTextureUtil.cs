using UnityEngine;

public static class RenderTextureUtil {
    public static RenderTexture GenerateRenderTexture(int width, int height) {
        RenderTexture rt = new RenderTexture(
            width,
            height,
            0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear) {
            enableRandomWrite = true,
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Point,
            antiAliasing = 1,
            autoGenerateMips = true,
            anisoLevel = 1
        };

        rt.Create();

        return rt;
    }

    public static void CleanUpRenderTexture(ref RenderTexture rt) {
        if (rt == null) return;

        rt.Release();
        rt = null;
    }
}
