using UnityEngine;
/// <summary>
/// Este script es el encargado de aplicar transparencia al GameObject
/// </summary>
public class TransparentObject : MonoBehaviour
{
    #region atributos
    private Material[] originalMaterials; /// <summary> almacena todos los material del go original/// </summary>
    [SerializeField]private Material transparentMaterial; /// <summary> almacena el material transparente/// </summary>
    private bool canChangeMaterial; /// <summary> determina si es posible cambiar el material/// </summary>
    private SkinnedMeshRenderer[] renderers; /// <summary> almaena todos los skins del go/// </summary>

    #endregion
    #region lifecyle script
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Obtiene todos los SkinnedMeshRenderer dentro del modelo
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        // Guarda los materiales originales (uno por renderer)
        originalMaterials = new Material[renderers.Length];
        // A cada uno de los materiales asigna transparencia
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
            renderers[i].material = transparentMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region logic code

    // Si en algún momento querés restaurar los materiales originales
    public void RestoreOriginalMaterials()
    {
        if (renderers == null || originalMaterials == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }

    #endregion




}
