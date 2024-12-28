using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class RoundedImage : MonoBehaviour
{
    private static readonly int RadiusProperty = Shader.PropertyToID("_Radius");
    private static readonly int SmoothnessProperty = Shader.PropertyToID("_Smoothness");

    [SerializeField, Range(0f, 0.5f)]
    private float cornerRadius = 0.1f;

    [SerializeField, Range(0f, 0.1f)]
    private float smoothness = 0.01f;

    private Material roundedMaterial;
    private Image imageComponent;

    public float CornerRadius
    {
        get => cornerRadius;
        set
        {
            cornerRadius = Mathf.Clamp(value, 0f, 0.5f);
            UpdateMaterialProperties();
        }
    }

    public float Smoothness
    {
        get => smoothness;
        set
        {
            smoothness = Mathf.Clamp(value, 0f, 0.1f);
            UpdateMaterialProperties();
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        UpdateMaterialProperties();
    }

    private void OnValidate()
    {
        Initialize();
        UpdateMaterialProperties();
    }

    private void Initialize()
    {
        if (imageComponent == null)
            imageComponent = GetComponent<Image>();

        if (roundedMaterial == null)
        {
            Shader shader = Shader.Find("UI/RoundedCorners");
            if (shader == null)
            {
                Debug.LogError("RoundedCorners shader not found. Please ensure the shader is included in your project.");
                return;
            }

            roundedMaterial = new Material(shader);
            imageComponent.material = roundedMaterial;
        }
    }

    private void UpdateMaterialProperties()
    {
        if (roundedMaterial != null)
        {
            roundedMaterial.SetFloat(RadiusProperty, cornerRadius);
            roundedMaterial.SetFloat(SmoothnessProperty, smoothness);
        }
    }

    private void OnDestroy()
    {
        if (roundedMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(roundedMaterial);
            }
            else
            {
                DestroyImmediate(roundedMaterial);
            }
        }
    }
}
