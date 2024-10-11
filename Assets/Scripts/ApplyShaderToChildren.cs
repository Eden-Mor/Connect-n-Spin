using UnityEngine;

public class ApplyShaderToChildren : MonoBehaviour
{
    // Material you want to assign to the children
    public Material shaderMaterial;

    void Start()
    {
        // Get all child objects with a Renderer component
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in childRenderers)
        {
            // Apply the material to each child
            renderer.material = shaderMaterial;
        }
    }
}
