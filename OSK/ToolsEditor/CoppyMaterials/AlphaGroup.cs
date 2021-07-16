using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls the transparency of the entire object (including all children) as `CanvasGroup` does in the UI system.
/// Executes both in play- & edit- modes.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class AlphaGroup : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField]
    private float _alpha = 1f;

    private float _prevAlpha;
    private Renderer[] _renderers;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        UpdateAlpha();
    }

    private void UpdateAlpha()
    {
        if (_prevAlpha == _alpha)
            return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            var renderer = _renderers[i];

            if (renderer is SpriteRenderer spriteRenderer)
            {
                var color = spriteRenderer.color;
                color.a = _alpha;
                spriteRenderer.color = color;
            }
            else if (renderer is MeshRenderer meshRenderer)
            {
                IsCopyMaterial isCopy = renderer.gameObject.GetComponent<IsCopyMaterial>();
                Material[] materials;

                if (isCopy)
                {
                    materials = meshRenderer.sharedMaterials;
                }
                else
                {
                    materials = meshRenderer.sharedMaterials.Select(m => Instantiate(m)).ToArray();

                    for (int j = 0; j < materials.Length; j++)
                    {
                        var material = materials[j];
                        material.name = meshRenderer.sharedMaterials[j].name + " (copy)";
                    }

                    meshRenderer.sharedMaterials = materials;
                    meshRenderer.gameObject.AddComponent<IsCopyMaterial>(); // Create a copy of the renderer materials only once.
                }

                foreach (Material material in materials)
                {
                    if (!material.HasProperty("_Color"))
                        continue;

                    var color = material.color;
                    color.a = _alpha;
                    material.color = color;
                }
            }
        }

        _prevAlpha = _alpha;
    }
}
