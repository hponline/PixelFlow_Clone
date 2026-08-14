using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Level Creator Tool ana penceresi. Sadece UI + kullanıcı etkileşimi;
/// tüm hesaplama TextureGridSampler / TileCounter / TurretDistributionPlanner /
/// LevelJsonExporter içinde yapılır, burada tekrar edilmez.
/// </summary>
public class LevelCreatorWindow : EditorWindow
{
    [MenuItem("Tools/Level Creator")]
    static void Open()
    {
        GetWindow<LevelCreatorWindow>("Level Creator");
    }

    // --- Kaynak ---
    Texture2D sourceTexture;
    TileDatabase tileDatabase;

    // --- Grid ayarları ---
    float colorTolerance = 50f;
    int gridWidth = 20;
    int gridHeight = 20;

    // --- Üretilen veri ---
    TileType[,] generatedGrid;
    Dictionary<TileType, int> tileCounts;

    // --- Mermi preset'leri ---
    List<int> bulletPresets = new() { 20, 40 };
    int newPresetValue = 30;

    // --- Turret taslakları / linkleme ---
    List<TurretDraft> turretDrafts;
    int selectedDraftIndex = -1;

    // --- Export ---
    string exportFolder = "Assets/_Game/Levels";
    string exportFileName = "level_01";

    Vector2 scrollPos;

    const float MAX_PREVIEW_SIZE = 300f;

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        DrawSourceSection();
        EditorGUILayout.Space(10);
        DrawGridSettingsSection();
        EditorGUILayout.Space(10);
        DrawGridPreviewSection();
        EditorGUILayout.Space(10);
        DrawBulletPresetsSection();
        EditorGUILayout.Space(10);
        DrawTurretListSection();
        EditorGUILayout.Space(10);
        DrawExportSection();

        EditorGUILayout.EndScrollView();
    }

    #region Source

    void DrawSourceSection()
    {
        EditorGUILayout.LabelField("Kaynak", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);
        tileDatabase = (TileDatabase)EditorGUILayout.ObjectField("Tile Database", tileDatabase, typeof(TileDatabase), true);

        if (sourceTexture != null)
            EditorGUILayout.LabelField("Texture Boyutu", $"{sourceTexture.width} x {sourceTexture.height} px");
    }

    #endregion

    #region Grid Settings

    void DrawGridSettingsSection()
    {
        EditorGUILayout.LabelField("Grid Ayarları", EditorStyles.boldLabel);

        colorTolerance = EditorGUILayout.Slider("Color Tolerance", colorTolerance, 0f, 255f);
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Auto Aspect"))
            ApplyAutoAspect();

        GUI.enabled = sourceTexture != null && tileDatabase != null && gridWidth > 0 && gridHeight > 0;
        if (GUILayout.Button("Generate From Texture"))
            GenerateFromTexture();
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        if (sourceTexture == null)
            EditorGUILayout.HelpBox("Source Texture atanmamış.", MessageType.Warning);
        else if (tileDatabase == null)
            EditorGUILayout.HelpBox("Tile Database atanmamış.", MessageType.Warning);
    }

    void ApplyAutoAspect()
    {
        if (sourceTexture == null || gridWidth <= 0) return;
        float ratio = sourceTexture.height / (float)sourceTexture.width;
        gridHeight = Mathf.Max(1, Mathf.RoundToInt(gridWidth * ratio));
    }

    void GenerateFromTexture()
    {
        var palette = tileDatabase.tiles;

        generatedGrid = TextureGridSampler.Sample(sourceTexture, gridWidth, gridHeight, palette, colorTolerance);
        tileCounts = TileCounter.Count(generatedGrid);

        // Grid değiştiği için eski turret planı artık geçersiz — kullanıcı yeniden üretmeli
        turretDrafts = null;
        selectedDraftIndex = -1;
    }

    #endregion

    #region Grid Preview

    void DrawGridPreviewSection()
    {
        if (generatedGrid == null) return;

        EditorGUILayout.LabelField("Grid Önizleme", EditorStyles.boldLabel);

        int width = generatedGrid.GetLength(0);
        int height = generatedGrid.GetLength(1);
        float cellSize = Mathf.Clamp(MAX_PREVIEW_SIZE / Mathf.Max(width, height), 2f, 20f);

        Rect area = GUILayoutUtility.GetRect(width * cellSize, height * cellSize);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Rect cellRect = new Rect(
                    area.x + x * cellSize,
                    area.y + (height - 1 - z) * cellSize, // z yukarı doğru artsın diye ters çiziyoruz
                    cellSize,
                    cellSize);

                EditorGUI.DrawRect(cellRect, GetPreviewColor(generatedGrid[x, z]));
            }
        }

        EditorGUILayout.Space(4);

        if (tileCounts != null)
        {
            foreach (var entry in tileCounts.OrderBy(e => e.Key))
                EditorGUILayout.LabelField(entry.Key.ToString(), entry.Value.ToString());
        }
    }

    Color GetPreviewColor(TileType type)
    {
        if (type == TileType.None) return new Color(0.15f, 0.15f, 0.15f);

        if (tileDatabase != null)
        {
            foreach (var tile in tileDatabase.tiles)
            {
                if (tile != null && tile.type == type)
                    return tile.sampleColor;
            }
        }

        return Color.magenta; // eşleşme bulunamadı — görsel uyarı
    }

    #endregion

    #region Bullet Presets

    void DrawBulletPresetsSection()
    {
        EditorGUILayout.LabelField("Auto Shooter Generation", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Bullet Counts:");

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < bulletPresets.Count; i++)
        {
            EditorGUILayout.LabelField(bulletPresets[i].ToString(), GUILayout.Width(40));
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                bulletPresets.RemoveAt(i);
                break; // liste değişti, bu frame'de döngüyü bırak
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        newPresetValue = EditorGUILayout.IntField(newPresetValue, GUILayout.Width(60));
        if (GUILayout.Button("Add", GUILayout.Width(50)))
        {
            if (newPresetValue > 0 && !bulletPresets.Contains(newPresetValue))
                bulletPresets.Add(newPresetValue);
        }
        EditorGUILayout.EndHorizontal();

        GUI.enabled = generatedGrid != null && bulletPresets.Count > 0;
        if (GUILayout.Button("Generate Shooters"))
            GenerateShooters();
        GUI.enabled = true;

        if (generatedGrid == null)
            EditorGUILayout.HelpBox("Önce grid üretilmeli.", MessageType.Info);
    }

    void GenerateShooters()
    {
        turretDrafts = TurretDistributionPlanner.Plan(tileCounts, bulletPresets);
        selectedDraftIndex = -1;
    }

    #endregion

    #region Turret List / Linking

    void DrawTurretListSection()
    {
        if (turretDrafts == null) return;

        EditorGUILayout.LabelField($"Turretler ({turretDrafts.Count})", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Linklemek için bir turret'a, sonra linklenecek diğerine tıkla.", MessageType.None);

        for (int i = 0; i < turretDrafts.Count; i++)
        {
            var draft = turretDrafts[i];

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            Rect swatchRect = GUILayoutUtility.GetRect(16, 16, GUILayout.Width(16));
            EditorGUI.DrawRect(swatchRect, GetPreviewColor(draft.Color));

            EditorGUILayout.LabelField($"#{draft.Id}  {draft.Color}  ammo:{draft.Ammo}", GUILayout.Width(180));

            if (draft.LinkedTo != -1)
            {
                EditorGUILayout.LabelField($"↔ #{draft.LinkedTo}", GUILayout.Width(60));
                if (GUILayout.Button("Unlink", GUILayout.Width(60)))
                    UnlinkDraft(draft.Id);
            }
            else
            {
                string label = selectedDraftIndex == i ? "Seçili (iptal için tekrar tıkla)" : "Link";
                if (GUILayout.Button(label))
                    OnTurretRowClicked(i);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    void OnTurretRowClicked(int index)
    {
        if (selectedDraftIndex == -1)
        {
            selectedDraftIndex = index;
        }
        else if (selectedDraftIndex == index)
        {
            selectedDraftIndex = -1;
        }
        else
        {
            LinkDrafts(selectedDraftIndex, index);
            selectedDraftIndex = -1;
        }
    }

    void LinkDrafts(int indexA, int indexB)
    {
        var draftA = turretDrafts[indexA];
        var draftB = turretDrafts[indexB];

        if (draftA.LinkedTo != -1) UnlinkDraft(draftA.Id);
        if (draftB.LinkedTo != -1) UnlinkDraft(draftB.Id);

        draftA.LinkedTo = draftB.Id;
        draftB.LinkedTo = draftA.Id;
    }

    void UnlinkDraft(int id)
    {
        var draft = turretDrafts.FirstOrDefault(d => d.Id == id);
        if (draft == null) return;

        int partnerId = draft.LinkedTo;
        draft.LinkedTo = -1;

        var partner = turretDrafts.FirstOrDefault(d => d.Id == partnerId);
        if (partner != null) partner.LinkedTo = -1;
    }

    #endregion

    #region Export

    void DrawExportSection()
    {
        EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);

        exportFolder = EditorGUILayout.TextField("Klasör", exportFolder);
        exportFileName = EditorGUILayout.TextField("Dosya Adı", exportFileName);

        GUI.enabled = generatedGrid != null && turretDrafts != null;
        if (GUILayout.Button("Export Level"))
        {
            string path = LevelJsonExporter.Export(generatedGrid, turretDrafts, exportFolder, exportFileName);
            Debug.Log($"Level export edildi: {path}");
        }
        GUI.enabled = true;

        if (generatedGrid == null)
            EditorGUILayout.HelpBox("Önce grid üretilmeli.", MessageType.Info);
        else if (turretDrafts == null)
            EditorGUILayout.HelpBox("Önce turretler üretilmeli (Generate Shooters).", MessageType.Info);
    }

    #endregion
}