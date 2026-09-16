using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace TextureGeneration
{
    /// <summary>
    /// A single palette entry: the color to place and how likely it is to be picked
    /// compared to the other entries.
    /// </summary>
    [Serializable]
    public struct WeightedColor
    {
        public Color color;

        [Min(0f)]
        public float weight;

        public WeightedColor(Color color, float weight)
        {
            this.color = color;
            this.weight = weight;
        }
    }

    /// <summary>
    /// Editor window that writes PNG files filled with pixels randomly picked from a
    /// weighted color palette.
    /// </summary>
    public class WeightedColorTextureGeneratorWindow : EditorWindow
    {
        private const string DefaultOutputFolder = "Assets/Art/GeneratedTextures";
        private const string DefaultFileNamePrefix = "texture";
        private const int MaxPreviewDimension = 256;
        private const float PreviewHeight = 160f;

        [SerializeField] private List<WeightedColor> palette = new List<WeightedColor>();
        [SerializeField] private Vector2Int sizeInPixels = new Vector2Int(32, 32);
        [SerializeField, Min(1)] private int numberOfFiles = 8;
        [SerializeField] private string outputFolder = DefaultOutputFolder;
        [SerializeField] private string fileNamePrefix = DefaultFileNamePrefix;
        [SerializeField] private bool useFixedSeed = false;
        [SerializeField] private int seed = 0;
        [SerializeField] private bool applyPixelArtImportSettings = true;
        [SerializeField] private Vector2 scrollPosition;

        private SerializedObject serializedWindow;
        private SerializedProperty paletteProperty;
        private SerializedProperty sizeInPixelsProperty;
        private SerializedProperty numberOfFilesProperty;
        private SerializedProperty outputFolderProperty;
        private SerializedProperty fileNamePrefixProperty;
        private SerializedProperty useFixedSeedProperty;
        private SerializedProperty seedProperty;
        private SerializedProperty applyPixelArtImportSettingsProperty;

        private ReorderableList paletteList;
        private GUIStyle rightAlignedMiniLabel;
        private Texture2D previewTexture;
        private int previewSettingsHash;

        [MenuItem("Tools/Wings Of Wrath/Weighted Color Texture Generator")]
        private static void Open()
        {
            var window = GetWindow<WeightedColorTextureGeneratorWindow>();
            window.titleContent = new GUIContent("Texture Generator");
            window.minSize = new Vector2(380f, 480f);
            window.Show();
        }

        private void OnEnable()
        {
            if (palette == null || palette.Count == 0)
            {
                palette = new List<WeightedColor>
                {
                    new WeightedColor(Color.black, 1f),
                    new WeightedColor(Color.white, 1f),
                };
            }

            InitializeSerialization();
        }

        private void OnDisable()
        {
            DestroyPreview();
        }

        private void InitializeSerialization()
        {
            serializedWindow = new SerializedObject(this);
            paletteProperty = serializedWindow.FindProperty(nameof(palette));
            sizeInPixelsProperty = serializedWindow.FindProperty(nameof(sizeInPixels));
            numberOfFilesProperty = serializedWindow.FindProperty(nameof(numberOfFiles));
            outputFolderProperty = serializedWindow.FindProperty(nameof(outputFolder));
            fileNamePrefixProperty = serializedWindow.FindProperty(nameof(fileNamePrefix));
            useFixedSeedProperty = serializedWindow.FindProperty(nameof(useFixedSeed));
            seedProperty = serializedWindow.FindProperty(nameof(seed));
            applyPixelArtImportSettingsProperty = serializedWindow.FindProperty(nameof(applyPixelArtImportSettings));

            paletteList = new ReorderableList(serializedWindow, paletteProperty, true, true, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight + 4f,
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Colors And Weights"),
                drawElementCallback = DrawPaletteElement,
                onAddCallback = AddPaletteEntry,
            };
        }

        private void OnGUI()
        {
            if (serializedWindow == null)
            {
                InitializeSerialization();
            }

            serializedWindow.Update();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);
            paletteList.DoLayoutList();
            EditorGUILayout.LabelField("Weights are relative: 2 and 1 means the same as 66.7% and 33.3%.", EditorStyles.miniLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sizeInPixelsProperty, new GUIContent("Size (px)", "Width and height of every generated texture, in pixels."));
            EditorGUILayout.PropertyField(numberOfFilesProperty, new GUIContent("Number Of Files", "How many PNG files to generate in this batch."));
            DrawOutputFolderField();
            EditorGUILayout.PropertyField(fileNamePrefixProperty, new GUIContent("File Name Prefix", "Files are named prefix_000.png, prefix_001.png, and so on."));
            EditorGUILayout.PropertyField(applyPixelArtImportSettingsProperty, new GUIContent("Pixel Art Import Settings", "Import the generated textures uncompressed and readable, with point filtering and no mipmaps."));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useFixedSeedProperty, new GUIContent("Use Fixed Seed", "Generate the same batch every time instead of a new random one."));

            using (new EditorGUI.DisabledScope(useFixedSeedProperty.boolValue == false))
            {
                EditorGUILayout.PropertyField(seedProperty, new GUIContent("Seed"));
            }

            serializedWindow.ApplyModifiedProperties();
            ClampValues();

            EditorGUILayout.Space();
            DrawPreview();

            EditorGUILayout.Space();
            DrawGenerateButton();

            EditorGUILayout.EndScrollView();
        }

        private void DrawPaletteElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            const float weightWidth = 60f;
            const float shareWidth = 52f;
            const float spacing = 4f;

            SerializedProperty element = paletteProperty.GetArrayElementAtIndex(index);
            SerializedProperty colorProperty = element.FindPropertyRelative(nameof(WeightedColor.color));
            SerializedProperty weightProperty = element.FindPropertyRelative(nameof(WeightedColor.weight));

            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            var colorRect = new Rect(rect.x, rect.y, rect.width - weightWidth - shareWidth - spacing * 2f, rect.height);
            var weightRect = new Rect(colorRect.xMax + spacing, rect.y, weightWidth, rect.height);
            var shareRect = new Rect(weightRect.xMax + spacing, rect.y, shareWidth, rect.height);

            colorProperty.colorValue = EditorGUI.ColorField(colorRect, GUIContent.none, colorProperty.colorValue, true, true, false);
            weightProperty.floatValue = Mathf.Max(0f, EditorGUI.FloatField(weightRect, GUIContent.none, weightProperty.floatValue));
            EditorGUI.LabelField(shareRect, GetShareLabel(weightProperty.floatValue), GetRightAlignedMiniLabel());
        }

        private void AddPaletteEntry(ReorderableList list)
        {
            int index = paletteProperty.arraySize;
            paletteProperty.arraySize++;

            SerializedProperty element = paletteProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative(nameof(WeightedColor.color)).colorValue = Color.HSVToRGB(UnityEngine.Random.value, 0.6f, 0.9f);
            element.FindPropertyRelative(nameof(WeightedColor.weight)).floatValue = 1f;

            list.index = index;
        }

        private string GetShareLabel(float weight)
        {
            float totalWeight = GetSerializedTotalWeight();

            if (totalWeight <= 0f)
            {
                return "-";
            }

            return $"{Mathf.Max(0f, weight) / totalWeight * 100f:0.#}%";
        }

        private float GetSerializedTotalWeight()
        {
            float totalWeight = 0f;

            for (int i = 0; i < paletteProperty.arraySize; i++)
            {
                SerializedProperty weightProperty = paletteProperty.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(WeightedColor.weight));
                totalWeight += Mathf.Max(0f, weightProperty.floatValue);
            }

            return totalWeight;
        }

        private GUIStyle GetRightAlignedMiniLabel()
        {
            if (rightAlignedMiniLabel == null)
            {
                rightAlignedMiniLabel = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleRight,
                };
            }

            return rightAlignedMiniLabel;
        }

        private void DrawOutputFolderField()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(outputFolderProperty, new GUIContent("Output Folder", "Folder the PNG files are written to. Relative paths are resolved from the project root."));

                if (GUILayout.Button("Browse", GUILayout.Width(60f)))
                {
                    string selectedFolder = EditorUtility.OpenFolderPanel("Select Output Folder", GetAbsoluteOutputFolder(), string.Empty);

                    if (string.IsNullOrEmpty(selectedFolder) == false)
                    {
                        outputFolderProperty.stringValue = ToProjectRelativePath(selectedFolder);
                        GUI.FocusControl(null);
                    }
                }
            }
        }

        private void ClampValues()
        {
            sizeInPixels = new Vector2Int(Mathf.Max(1, sizeInPixels.x), Mathf.Max(1, sizeInPixels.y));
            numberOfFiles = Mathf.Max(1, numberOfFiles);
        }

        private void DrawPreview()
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            UpdatePreview();

            Rect previewRect = GUILayoutUtility.GetRect(PreviewHeight, PreviewHeight, GUILayout.ExpandWidth(true));

            if (previewTexture == null)
            {
                EditorGUI.LabelField(previewRect, "Nothing to preview.", EditorStyles.miniLabel);
                return;
            }

            float aspect = (float)previewTexture.width / previewTexture.height;
            float drawHeight = Mathf.Min(previewRect.height, previewRect.width / aspect);
            float drawWidth = drawHeight * aspect;
            var drawRect = new Rect(previewRect.x + (previewRect.width - drawWidth) * 0.5f, previewRect.y, drawWidth, drawHeight);

            GUI.DrawTexture(drawRect, previewTexture, ScaleMode.StretchToFill, true);

            if (previewTexture.width != sizeInPixels.x || previewTexture.height != sizeInPixels.y)
            {
                EditorGUILayout.LabelField($"Showing a {previewTexture.width}x{previewTexture.height} sample of the {sizeInPixels.x}x{sizeInPixels.y} output.", EditorStyles.miniLabel);
            }
        }

        private void UpdatePreview()
        {
            int settingsHash = GetPreviewSettingsHash();

            if (settingsHash == previewSettingsHash && previewTexture != null)
            {
                return;
            }

            previewSettingsHash = settingsHash;
            DestroyPreview();

            if (IsPaletteUsable() == false)
            {
                return;
            }

            int previewWidth = Mathf.Min(sizeInPixels.x, MaxPreviewDimension);
            int previewHeight = Mathf.Min(sizeInPixels.y, MaxPreviewDimension);

            previewTexture = GenerateTexture(previewWidth, previewHeight, useFixedSeed ? seed : 0);
            previewTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        private int GetPreviewSettingsHash()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + sizeInPixels.GetHashCode();
                hash = hash * 31 + (useFixedSeed ? seed : 0);

                for (int i = 0; i < palette.Count; i++)
                {
                    hash = hash * 31 + palette[i].color.GetHashCode();
                    hash = hash * 31 + palette[i].weight.GetHashCode();
                }

                return hash;
            }
        }

        private void DestroyPreview()
        {
            if (previewTexture != null)
            {
                DestroyImmediate(previewTexture);
                previewTexture = null;
            }
        }

        private void DrawGenerateButton()
        {
            string validationError = GetValidationError();
            bool canGenerate = string.IsNullOrEmpty(validationError);

            if (canGenerate)
            {
                EditorGUILayout.HelpBox($"Writes {numberOfFiles} file(s) of {sizeInPixels.x}x{sizeInPixels.y} px into {outputFolder}. Existing files are left alone, new ones continue the numbering.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(validationError, MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(canGenerate == false))
            {
                if (GUILayout.Button("Generate", GUILayout.Height(30f)))
                {
                    GenerateFiles();
                }
            }
        }

        private string GetValidationError()
        {
            if (palette.Count == 0)
            {
                return "Add at least one color to the palette.";
            }

            if (IsPaletteUsable() == false)
            {
                return "At least one color needs a weight greater than zero.";
            }

            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                return "Pick an output folder.";
            }

            if (string.IsNullOrWhiteSpace(fileNamePrefix))
            {
                return "Enter a file name prefix.";
            }

            if (fileNamePrefix.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return "The file name prefix contains characters that are not allowed in a file name.";
            }

            return null;
        }

        private bool IsPaletteUsable()
        {
            return palette.Count > 0 && GetTotalWeight() > 0f;
        }

        private float GetTotalWeight()
        {
            float totalWeight = 0f;

            for (int i = 0; i < palette.Count; i++)
            {
                totalWeight += Mathf.Max(0f, palette[i].weight);
            }

            return totalWeight;
        }

        /// <summary>
        /// Creates a texture where every pixel is independently picked from the palette, each
        /// color showing up in proportion to its weight. The same seed always gives the same texture.
        /// </summary>
        private Texture2D GenerateTexture(int width, int height, int textureSeed)
        {
            float[] cumulativeWeights = BuildCumulativeWeights();
            float totalWeight = cumulativeWeights[cumulativeWeights.Length - 1];

            var random = new System.Random(textureSeed);
            var pixels = new Color32[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                float roll = (float)(random.NextDouble() * totalWeight);
                pixels[i] = palette[PickIndex(cumulativeWeights, roll)].color;
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false, linear: false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };

            texture.SetPixels32(pixels);
            texture.Apply();

            return texture;
        }

        private float[] BuildCumulativeWeights()
        {
            var cumulativeWeights = new float[palette.Count];
            float runningTotal = 0f;

            for (int i = 0; i < palette.Count; i++)
            {
                runningTotal += Mathf.Max(0f, palette[i].weight);
                cumulativeWeights[i] = runningTotal;
            }

            return cumulativeWeights;
        }

        /// <summary>
        /// Returns the first index whose cumulative weight is above <paramref name="roll"/>.
        /// Colors with a weight of zero are never picked.
        /// </summary>
        private static int PickIndex(float[] cumulativeWeights, float roll)
        {
            int low = 0;
            int high = cumulativeWeights.Length - 1;

            while (low < high)
            {
                int middle = (low + high) / 2;

                if (roll < cumulativeWeights[middle])
                {
                    high = middle;
                }
                else
                {
                    low = middle + 1;
                }
            }

            return low;
        }

        private void GenerateFiles()
        {
            string absoluteFolder = GetAbsoluteOutputFolder();

            try
            {
                Directory.CreateDirectory(absoluteFolder);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Texture Generator", $"Could not create the output folder:\n{absoluteFolder}\n\n{exception.Message}", "OK");
                return;
            }

            int baseSeed = useFixedSeed ? seed : Environment.TickCount;
            var createdFiles = new List<string>(numberOfFiles);
            int nextFileIndex = 0;

            try
            {
                for (int i = 0; i < numberOfFiles; i++)
                {
                    bool cancelled = EditorUtility.DisplayCancelableProgressBar("Generating Textures", $"Writing file {i + 1} of {numberOfFiles}", (float)i / numberOfFiles);

                    if (cancelled)
                    {
                        break;
                    }

                    string filePath;

                    do
                    {
                        filePath = Path.Combine(absoluteFolder, $"{fileNamePrefix}_{nextFileIndex:D3}.png");
                        nextFileIndex++;
                    }
                    while (File.Exists(filePath));

                    Texture2D texture = GenerateTexture(sizeInPixels.x, sizeInPixels.y, GetSeedForIndex(baseSeed, i));
                    byte[] pngBytes = texture.EncodeToPNG();
                    DestroyImmediate(texture);

                    File.WriteAllBytes(filePath, pngBytes);
                    createdFiles.Add(filePath);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Texture Generator", $"Generation stopped after {createdFiles.Count} file(s):\n\n{exception.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (createdFiles.Count == 0)
            {
                return;
            }

            AssetDatabase.Refresh();
            ApplyImportSettings(createdFiles);
            RevealOutputFolder(absoluteFolder);

            Debug.Log($"Generated {createdFiles.Count} texture(s) of {sizeInPixels.x}x{sizeInPixels.y} px in {absoluteFolder}");
        }

        /// <summary>
        /// Turns a base seed and a file index into a well spread out seed, so consecutive files
        /// do not end up sharing a random sequence.
        /// </summary>
        private static int GetSeedForIndex(int baseSeed, int index)
        {
            unchecked
            {
                int hash = baseSeed * 486187739 + index * 1327217885;
                hash ^= hash >> 13;
                hash *= 668265263;
                hash ^= hash >> 15;

                // System.Random does not accept int.MinValue on every runtime, so stay positive.
                return hash & 0x7FFFFFFF;
            }
        }

        private void ApplyImportSettings(List<string> createdFiles)
        {
            if (applyPixelArtImportSettings == false)
            {
                return;
            }

            int maxTextureSize = Mathf.Clamp(Mathf.NextPowerOfTwo(Mathf.Max(sizeInPixels.x, sizeInPixels.y)), 32, 16384);

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (string filePath in createdFiles)
                {
                    if (TryGetAssetPath(filePath, out string assetPath) == false)
                    {
                        continue;
                    }

                    if (AssetImporter.GetAtPath(assetPath) is not TextureImporter importer)
                    {
                        continue;
                    }

                    importer.textureType = TextureImporterType.Default;
                    importer.filterMode = FilterMode.Point;
                    importer.wrapMode = TextureWrapMode.Clamp;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = false;
                    importer.isReadable = true;
                    importer.sRGBTexture = true;
                    importer.alphaIsTransparency = true;
                    importer.maxTextureSize = maxTextureSize;
                    importer.SaveAndReimport();
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private void RevealOutputFolder(string absoluteFolder)
        {
            if (TryGetAssetPath(absoluteFolder, out string folderAssetPath))
            {
                var folderAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(folderAssetPath);

                if (folderAsset != null)
                {
                    EditorGUIUtility.PingObject(folderAsset);
                    return;
                }
            }

            EditorUtility.RevealInFinder(absoluteFolder);
        }

        private string GetAbsoluteOutputFolder()
        {
            string folder = string.IsNullOrWhiteSpace(outputFolder) ? DefaultOutputFolder : outputFolder.Trim();

            if (Path.IsPathRooted(folder))
            {
                return Path.GetFullPath(folder);
            }

            return Path.GetFullPath(Path.Combine(GetProjectRoot(), folder));
        }

        private static string ToProjectRelativePath(string absolutePath)
        {
            string fullPath = Path.GetFullPath(absolutePath);
            string projectRoot = GetProjectRoot();

            if (fullPath.StartsWith(projectRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                return fullPath.Substring(projectRoot.Length + 1).Replace('\\', '/');
            }

            return fullPath.Replace('\\', '/');
        }

        /// <summary>
        /// Converts an absolute path into an Assets relative path, or fails when it sits outside the project.
        /// </summary>
        private static bool TryGetAssetPath(string absolutePath, out string assetPath)
        {
            string fullPath = Path.GetFullPath(absolutePath);
            string assetsRoot = Path.GetFullPath(Application.dataPath);

            if (fullPath.StartsWith(assetsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                assetPath = "Assets/" + fullPath.Substring(assetsRoot.Length + 1).Replace('\\', '/');
                return true;
            }

            assetPath = null;
            return false;
        }

        private static string GetProjectRoot()
        {
            DirectoryInfo projectRoot = Directory.GetParent(Application.dataPath);
            return projectRoot != null ? projectRoot.FullName : Application.dataPath;
        }
    }
}
