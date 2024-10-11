using UnityEngine;

[ExecuteInEditMode]
public class CameraShaderEffect : MonoBehaviour
{
    public Material postProcessMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //Removes the smearing that is cached
        Graphics.Blit(src, dest);

        if (postProcessMaterial != null)
        {
            // Apply the shader to the render output
            Graphics.Blit(src, dest, postProcessMaterial);
        }
    }
}
