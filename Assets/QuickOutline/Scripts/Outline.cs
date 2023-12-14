//
// Outline.cs
// QuickOutline
//
// Created by Chris Nolet on 3/30/18.
// Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    [SerializeField] bool testing = false;
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    private bool showThickOutline = false;

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public Mode OutlineMode
    {
        get { return outlineMode; }
        set
        {
            outlineMode = value;
            //needsUpdate = true;
        }
    }

    public Color OutlineColor
    {
        get { return outlineColor; }
        set
        {
            outlineColor = value;
            //needsUpdate = true;
        }
    }

    public float OutlineWidth
    {
        get { return outlineWidth; }
        set
        {
            outlineWidth = value;
            //needsUpdate = true;
        }
    }

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    [SerializeField]
    private Mode outlineMode = Mode.OutlineAll;

    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField]
    private Color manualHighlightColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 4f;
    [SerializeField, Range(0f, 10f)]
    private float thickWidth = 4.5f;

    [SerializeField] private float currentOutlineWidth;

    [Header("Optional")]

    [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
        + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
    private bool precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    [Header("Renderers")]
    [SerializeField] private Renderer[] renderers;

    [Header("Masks")]
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    //public int baseMaterialsCount = 1;

    //private bool needsUpdate;
    [Header("Vars")]
    [SerializeField] private bool showPlayerOutline = false; // Flag for player-selected highlights
    [SerializeField] private bool showManualOutline = false; // Flag for manually selected highlights

    void Awake()
    {
        Debug.Log("Awake: " + gameObject);
        // Cache renderers
        //renderers = GetComponentsInChildren<Renderer>();

        // Instantiate outline materials
        outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";

        // Retrieve or generate smooth normals
        LoadSmoothNormals();

        // Apply material properties immediately
        //needsUpdate = true;

        InvisibleOutline();
    }

    void OnEnable()
    {
        foreach (var renderer in renderers)
        {
            // Append outline shaders
            var materials = renderer.sharedMaterials.ToList();

            materials.Add(outlineMaskMaterial);
            materials.Add(outlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    void OnValidate()
    {
        // Update material properties
        //needsUpdate = true;

        // Clear cache when baking is disabled or corrupted
        if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
        {
            bakeKeys.Clear();
            bakeValues.Clear();
        }

        // Generate smooth normals when baking is enabled
        if (precomputeOutline && bakeKeys.Count == 0)
        {
            Bake();
        }
    }

    void Update()
    {
        if (testing)
        {
            UpdateMaterialProperties();
            currentOutlineWidth = 4;
            return;
        }

        if (/*needsUpdate &&*/ showPlayerOutline)
        {
            //Debug.Log("SPO" + showPlayerOutline);
            //needsUpdate = false;
            SetOnOutline();
            UpdateMaterialProperties();
        } else if (showManualOutline)
        {
            outlineColor = manualHighlightColor;
            SetOnOutline();
            UpdateMaterialProperties();
        } else
        {
            InvisibleOutline();
            UpdateMaterialProperties();
        }

        showPlayerOutline = false;
        showManualOutline = false;
    }

    void OnDisable()
    {
        Debug.Log("Disable outline now!!" + gameObject);

        foreach (var renderer in renderers)
        {
            // Remove outline shaders
            var materials = renderer.sharedMaterials.ToList();

            Debug.Log("renderer: " + renderer.gameObject + " - " + materials.Count);

            materials.Remove(outlineMaskMaterial);
            materials.Remove(outlineFillMaterial);

            Debug.Log("After: " + renderer.gameObject + " - " + materials.Count);

            renderer.materials = materials.ToArray();
        }
    }

    void OnDestroy()
    {
        // Destroy material instances
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }

    void Bake()
    {
        // Generate smooth normals for each mesh
        var bakedMeshes = new HashSet<Mesh>();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            // Skip duplicates
            if (!bakedMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Serialize smooth normals
            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

            bakeKeys.Add(meshFilter.sharedMesh);
            bakeValues.Add(new ListVector3() { data = smoothNormals });
        }
    }

    void LoadSmoothNormals()
    {
        // Retrieve or generate smooth normals
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            // Skip if smooth normals have already been adopted
            if (!registeredMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Retrieve or generate smooth normals
            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

            // Store smooth normals in UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            // Combine submeshes
            var renderer = meshFilter.GetComponent<Renderer>();

            if (renderer != null)
            {
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }
        }

        // Clear UV3 on skinned mesh renderers
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            // Skip if UV3 has already been reset
            if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
            {
                continue;
            }

            // Clear UV3
            skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

            // Combine submeshes
            CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {
            // Skip single vertices
            if (group.Count() == 1)
            {
                continue;
            }

            // Calculate the average normal
            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }

            smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        // Skip meshes with a single submesh
        if (mesh.subMeshCount == 1)
        {
            return;
        }

        // Skip if submesh count exceeds material count
        if (mesh.subMeshCount > materials.Length)
        {
            return;
        }

        // Append combined submesh
        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    void UpdateMaterialProperties()
    {
        if (showPlayerOutline)
        {
            outlineFillMaterial.SetColor("_OutlineColor", outlineColor); // Use outlineColor when showing player-selected outline
        } else if (showManualOutline)
        {
            outlineFillMaterial.SetColor("_OutlineColor", manualHighlightColor); // Use manualHighlightColor for manually selected outline
        } else
        {
            // Set outline color to default or hide it when neither type is selected
            outlineFillMaterial.SetColor("_OutlineColor", Color.clear);
        }

        // Apply properties according to mode

        switch (outlineMode)
        {
            case Mode.OutlineAll:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);

                if (showThickOutline)
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth + thickWidth);
                }
                else
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth);
                }
            
                break;

            case Mode.OutlineVisible:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            
                if (showThickOutline)
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth + thickWidth);
                }
                else
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth);
                }

                break;

            case Mode.OutlineHidden:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
            
                if (showThickOutline)
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth + thickWidth);
                }
                else
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth);
                }

                break;

            case Mode.OutlineAndSilhouette:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
            
                if (showThickOutline)
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth + thickWidth);
                }
                else
                {
                    outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth);
                }

                break;

            case Mode.SilhouetteOnly:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                break;
        }

    }

    public void InvisibleOutline()
    {
        currentOutlineWidth = 0f;
        //needsUpdate = true;
    }

    public void SetOnOutline()
    {
        currentOutlineWidth = outlineWidth;
        //needsUpdate = true;
    }

    public void ShowOutline(Color playerColor, bool thick)
    {
        //Debug.Log("YÄÄÄ");

        outlineColor = playerColor;
        showPlayerOutline = true;

        showThickOutline = thick;
    }

    public void ShowManualOutline()
    {
        outlineColor = manualHighlightColor;
        showManualOutline = true;
        showPlayerOutline = false;
    }

    public void HideOutline()
    {
        showPlayerOutline = false;

        InvisibleOutline();
        UpdateMaterialProperties();
    }

    public void HideManualOutline()
    {
        showManualOutline = false;
    }

}