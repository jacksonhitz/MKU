using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering
{
    /// <summary>
    /// Custom shader GUI for the Instanced Lit Combined shader
    /// </summary>
    public class InstancedLitShaderGUI : ShaderGUI
    {
        // Properties
        private MaterialProperty transparencyModeProp;
        private MaterialProperty cutoffProp;
        private MaterialProperty ditherThresholdProp;
        private MaterialProperty mainTexProp;
        private MaterialProperty colorProp;
        private MaterialProperty bumpMapProp;
        private MaterialProperty bumpScaleProp;
        private MaterialProperty cullModeProp;

        // Keywords for transparency modes
        private static readonly string[] transparencyKeywords =
        {
            "_TRANSPARENCY_OFF",
            "_TRANSPARENCY_CUTOUT",
            "_TRANSPARENCY_DITHER",
        };

        // Material properties
        private const string PROP_TRANSPARENCY_MODE = "_TransparencyMode";
        private const string PROP_CUTOFF = "_Cutoff";
        private const string PROP_DITHER_THRESHOLD = "_DitherThreshold";
        private const string PROP_MAIN_TEX = "_MainTex";
        private const string PROP_COLOR = "_Color";
        private const string PROP_BUMP_MAP = "_BumpMap";
        private const string PROP_BUMP_SCALE = "_BumpScale";
        private const string PROP_CULL = "_Cull";

        // Hidden properties for rendering setup
        private const string PROP_SRC_BLEND = "_SrcBlend";
        private const string PROP_DST_BLEND = "_DstBlend";
        private const string PROP_ZWRITE = "_ZWrite";

        // Labels
        private static readonly GUIContent transparencyModeLabel = new GUIContent(
            "Transparency Mode",
            "How transparency is handled - impacts batching and rendering"
        );
        private static readonly GUIContent[] transparencyModeNames =
        {
            new GUIContent("Opaque"),
            new GUIContent("Alpha Cutout"),
            new GUIContent("Dithered"),
        };

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            // Find all shader properties
            FindProperties(properties);

            Material material = materialEditor.target as Material;

            // Header
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Instanced Lit Shader", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            // Main properties section
            EditorGUILayout.LabelField("Main Properties", EditorStyles.boldLabel);
            materialEditor.TexturePropertySingleLine(
                new GUIContent("Albedo", "Base color and transparency"),
                mainTexProp,
                colorProp
            );
            materialEditor.TextureScaleOffsetProperty(mainTexProp);

            // Normal map
            materialEditor.TexturePropertySingleLine(
                new GUIContent("Normal Map", "Normal map texture"),
                bumpMapProp,
                bumpMapProp.textureValue != null ? bumpScaleProp : null
            );

            // Transparency mode dropdown
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Transparency Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            TransparencyMode mode = (TransparencyMode)transparencyModeProp.floatValue;
            mode = (TransparencyMode)
                EditorGUILayout.Popup(transparencyModeLabel, (int)mode, transparencyModeNames);

            if (EditorGUI.EndChangeCheck())
            {
                transparencyModeProp.floatValue = (float)mode;
                SetupTransparencyMode(material, mode);
            }

            // Show the right properties based on transparency mode
            switch (mode)
            {
                case TransparencyMode.AlphaCutout:
                    materialEditor.ShaderProperty(
                        cutoffProp,
                        new GUIContent("Alpha Cutoff", "Threshold for alpha cutout")
                    );
                    break;

                case TransparencyMode.Dithered:
                    materialEditor.ShaderProperty(
                        ditherThresholdProp,
                        new GUIContent(
                            "Dither Threshold",
                            "Controls the intensity of dithered transparency"
                        )
                    );
                    break;
            }

            // Advanced properties section
            EditorGUILayout.Space();
            materialEditor.ShaderProperty(
                cullModeProp,
                new GUIContent("Culling", "Face culling mode")
            );

            materialEditor.EnableInstancingField();

            // Apply changes
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var target in materialEditor.targets)
                {
                    Material mat = (Material)target;
                    SetupMaterialKeywords(mat);
                }
            }
        }

        // Find all material properties
        private void FindProperties(MaterialProperty[] properties)
        {
            transparencyModeProp = FindProperty(PROP_TRANSPARENCY_MODE, properties);
            cutoffProp = FindProperty(PROP_CUTOFF, properties);
            ditherThresholdProp = FindProperty(PROP_DITHER_THRESHOLD, properties);
            mainTexProp = FindProperty(PROP_MAIN_TEX, properties);
            colorProp = FindProperty(PROP_COLOR, properties);
            bumpMapProp = FindProperty(PROP_BUMP_MAP, properties);
            bumpScaleProp = FindProperty(PROP_BUMP_SCALE, properties);
            cullModeProp = FindProperty(PROP_CULL, properties);
        }

        /// <summary>
        /// Setup material based on selected transparency mode
        /// </summary>
        private void SetupTransparencyMode(Material material, TransparencyMode mode)
        {
            // Set up keywords for the shader
            foreach (var keyword in transparencyKeywords)
            {
                material.DisableKeyword(keyword);
            }

            material.EnableKeyword(transparencyKeywords[(int)mode]);

            // Set rendering properties based on transparency mode
            switch (mode)
            {
                case TransparencyMode.Opaque:
                    material.SetOverrideTag("RenderType", "Opaque");
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.renderQueue = (int)RenderQueue.Geometry;
                    break;

                case TransparencyMode.AlphaCutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.renderQueue = (int)RenderQueue.AlphaTest;
                    break;

                case TransparencyMode.Dithered:
                    material.SetOverrideTag("RenderType", "Opaque");
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.renderQueue = (int)RenderQueue.Geometry;
                    break;
            }
        }

        /// <summary>
        /// Set up all material keywords based on current settings
        /// </summary>
        public static void SetupMaterialKeywords(Material material)
        {
            // Get transparency mode
            TransparencyMode transparencyMode = (TransparencyMode)
                material.GetFloat(PROP_TRANSPARENCY_MODE);

            // Setup transparency keywords
            foreach (var keyword in transparencyKeywords)
            {
                material.DisableKeyword(keyword);
            }
            material.EnableKeyword(transparencyKeywords[(int)transparencyMode]);

            // Normal map
            bool hasNormalMap = material.GetTexture(PROP_BUMP_MAP) != null;
            CoreUtils.SetKeyword(material, "_NORMALMAP", hasNormalMap);
        }
    }
}
