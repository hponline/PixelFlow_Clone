using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class SpriteToGridEditor : EditorWindow
{
    Texture2D inputTexture;
    int width = 16;
    int height = 16;

    List<Color> palette = new List<Color>();

    Vector2 scroll;

    [MenuItem("Tools/Sprite To Grid")]
    public static void Open()
    {
        GetWindow<SpriteToGridEditor>("Sprite To Grid");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite To Grid Converter", EditorStyles.boldLabel);

        inputTexture = (Texture2D)EditorGUILayout.ObjectField("Sprite", inputTexture, typeof(Texture2D), false);

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        GUILayout.Space(10);
        GUILayout.Label("Palette (0 = Empty, others = colors)");

        if (GUILayout.Button("Add Selected Color"))
        {
            if (inputTexture != null)
            {
                palette.Add(inputTexture.GetPixel(0, 0));
            }
        }

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(100));
        for (int i = 0; i < palette.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            palette[i] = EditorGUILayout.ColorField(palette[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                palette.RemoveAt(i);
                break;
            }
            EditorGUILayout.LabelField("Index: " + (i + 1));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Convert To Grid JSON"))
        {
            Convert();
        }
    }

    void Convert()
    {
        if (inputTexture == null) return;

        Texture2D readable = GetReadableTexture(inputTexture);

        int[] tiles = new int[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixel = readable.GetPixelBilinear(
                    (float)x / width,
                    (float)y / height
                );

                int index = GetClosestPaletteIndex(pixel);
                tiles[y * width + x] = index;
            }
        }

        SaveJSON(tiles);
    }

    int GetClosestPaletteIndex(Color c)
    {
        float bestDist = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < palette.Count; i++)
        {
            float d = ColorDistance(c, palette[i]);
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i + 1; // 0 = empty
            }
        }

        return bestIndex;
    }

    float ColorDistance(Color a, Color b)
    {
        return Mathf.Pow(a.r - b.r, 2) +
               Mathf.Pow(a.g - b.g, 2) +
               Mathf.Pow(a.b - b.b, 2);
    }

    Texture2D GetReadableTexture(Texture2D tex)
    {
        RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height);
        Graphics.Blit(tex, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(tex.width, tex.height);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }

    void SaveJSON(int[] tiles)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("{");
        sb.AppendLine($"\"width\": {width},");
        sb.AppendLine($"\"height\": {height},");
        sb.Append("\"tiles\": [");

        for (int i = 0; i < tiles.Length; i++)
        {
            sb.Append(tiles[i]);
            if (i < tiles.Length - 1)
                sb.Append(",");
        }

        sb.AppendLine("]");
        sb.AppendLine("}");

        string path = EditorUtility.SaveFilePanel(
            "Save Grid JSON",
            "",
            "grid.json",
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString());
        }
    }
}
/*
[System.Serializable]
public struct PaletteEntry
{
    public Color color;
    public ColorType type;
}
public class SpriteToGridEditor : EditorWindow
{
    Texture2D inputTexture;
    int width = 16;
    int height = 16;

    List<PaletteEntry> palette = new List<PaletteEntry>();

    Vector2 scroll;

    [MenuItem("Tools/Sprite To Grid")]
    public static void Open()
    {
        GetWindow<SpriteToGridEditor>("Sprite To Grid");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprite To Grid Converter", EditorStyles.boldLabel);

        inputTexture = (Texture2D)EditorGUILayout.ObjectField("Sprite", inputTexture, typeof(Texture2D), false);

        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        GUILayout.Space(10);
        GUILayout.Label("Palette (Color → Enum Mapping)");

        if (GUILayout.Button("Add Entry"))
        {
            palette.Add(new PaletteEntry
            {
                color = Color.white,
                type = ColorType.None
            });
        }

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(150));

        for (int i = 0; i < palette.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            palette[i] = new PaletteEntry
            {
                color = EditorGUILayout.ColorField(palette[i].color),
                type = (ColorType)EditorGUILayout.EnumPopup(palette[i].type)
            };

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                palette.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Convert To Grid JSON"))
        {
            Convert();
        }
    }

    void Convert()
    {
        if (inputTexture == null)
        {
            Debug.LogError("Texture missing");
            return;
        }

        Texture2D readable = GetReadableTexture(inputTexture);

        int[] tiles = new int[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // ⚠️ Bilinear yerine direkt pixel
                Color pixel = readable.GetPixel(x, y);

                int index = GetClosestPaletteIndex(pixel);
                tiles[y * width + x] = index;
            }
        }

        SaveJSON(tiles);
    }

    int GetClosestPaletteIndex(Color c)
    {
        float bestDist = float.MaxValue;
        ColorType bestType = ColorType.None;

        for (int i = 0; i < palette.Count; i++)
        {
            float d = ColorDistance(c, palette[i].color);

            if (d < bestDist)
            {
                bestDist = d;
                bestType = palette[i].type;
            }
        }

        return (int)bestType;
    }

    float ColorDistance(Color a, Color b)
    {
        return Mathf.Pow(a.r - b.r, 2) +
               Mathf.Pow(a.g - b.g, 2) +
               Mathf.Pow(a.b - b.b, 2);
    }

    Texture2D GetReadableTexture(Texture2D tex)
    {
        RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height);
        Graphics.Blit(tex, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(tex.width, tex.height);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }

    void SaveJSON(int[] tiles)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("{");
        sb.AppendLine($"\"width\": {width},");
        sb.AppendLine($"\"height\": {height},");
        sb.Append("\"tiles\": [");

        for (int i = 0; i < tiles.Length; i++)
        {
            sb.Append(tiles[i]);
            if (i < tiles.Length - 1)
                sb.Append(",");
        }

        sb.AppendLine("]");
        sb.AppendLine("}");

        string path = EditorUtility.SaveFilePanel(
            "Save Grid JSON",
            "",
            "grid.json",
            "json"
        );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, sb.ToString());
            Debug.Log("Saved: " + path);
        }
    }
}
*/
