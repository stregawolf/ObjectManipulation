using UnityEngine;
using System.Collections;

/// <summary>
/// OutlineManipulator acts as an interface for the outline shader
/// </summary>
public class OutlineManipulator : MonoBehaviour {
    private const string OutlineColorName = "_OutlineColor";
    private const string OutlineWidthName = "_Outline";

    public Renderer outlineRenderer;

	protected void Start () {
        if (outlineRenderer == null)
        {
            outlineRenderer = GetComponentInChildren<Renderer>();
        }
	}

    public void SetOutlineColor(Color c)
    {
        if (outlineRenderer != null && outlineRenderer.material.HasProperty(OutlineColorName))
        {
            outlineRenderer.material.SetColor(OutlineColorName, c);
        }
    }

    public void SetOutlineWidth(float width)
    {
        if (outlineRenderer != null && outlineRenderer.material.HasProperty(OutlineWidthName))
        {
            outlineRenderer.material.SetFloat(OutlineWidthName, width);
        }
    }
}
