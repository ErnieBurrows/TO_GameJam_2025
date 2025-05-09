using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public List<Renderer> renderers;
    private Color color = Color.white;
    private List<Material> materials;
    
    private void Awake()
    {
        renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool val)
    {
        if (val)
        {
            foreach (var material in materials)
            {
                material.EnableKeyword("_EMISSION");            // Give the material the ability to use emission color
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
        
    }
}
