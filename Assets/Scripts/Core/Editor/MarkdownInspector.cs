using UnityEditor;
using UnityEngine;

/// <summary>
/// [CORE - Éditeur] Affiche les fichiers .md mis en forme dans l'Inspector
/// (titres, gras, `code`, listes, tableaux, notes).
/// Ouvre aussi l'énoncé du TP au premier lancement du projet.
/// </summary>
[CustomEditor(typeof(TextAsset))]
public class MarkdownInspector : Editor
{
    const string ReadmePath = "Assets/README_TP.md";

    GUIStyle textStyle;
    GUIStyle boldStyle;
    GUIStyle title1Style;
    GUIStyle title2Style;
    GUIStyle title3Style;
    GUIStyle noteStyle;
    GUIStyle codeStyle;

    [MenuItem("SnackLine/Ouvrir l'énoncé du TP", false, 0)]
    static void OpenReadme()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(ReadmePath);
    }

    [InitializeOnLoadMethod]
    static void OpenReadmeOnFirstLaunch()
    {
        if (Application.isBatchMode)
        {
            return;
        }

        // Une seule fois par projet et par ordinateur
        string key = "SnackLine.ReadmeShown." + Application.dataPath;
        if (EditorPrefs.GetBool(key, false))
        {
            return;
        }
        EditorPrefs.SetBool(key, true);
        EditorApplication.delayCall += OpenReadme;
    }

    public override void OnInspectorGUI()
    {
        TextAsset file = (TextAsset)target;
        bool wasEnabled = GUI.enabled;
        GUI.enabled = true;
        CreateStyles();

        if (AssetDatabase.GetAssetPath(file).EndsWith(".md"))
        {
            DrawMarkdown(file.text);
        }
        else
        {
            // Autres fichiers texte : on affiche le début du texte brut
            string text = file.text;
            if (text.Length > 7000)
            {
                text = text.Substring(0, 7000) + "\n...";
            }
            GUILayout.Label(text, textStyle);
        }

        GUI.enabled = wasEnabled;
    }

    void DrawMarkdown(string markdown)
    {
        string[] lines = markdown.Split('\n');
        bool inCodeBlock = false;
        bool previousWasTable = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.TrimEnd('\r');
            string trimmed = line.Trim();
            bool isTable = trimmed.StartsWith("|");

            if (trimmed.StartsWith("```"))
            {
                inCodeBlock = !inCodeBlock;
            }
            else if (inCodeBlock)
            {
                GUILayout.Label(line, codeStyle);
            }
            else if (trimmed == "")
            {
                GUILayout.Space(6);
            }
            else if (trimmed == "---")
            {
                DrawSeparator();
            }
            else if (trimmed.StartsWith("### "))
            {
                GUILayout.Label(Format(trimmed.Substring(4)), title3Style);
            }
            else if (trimmed.StartsWith("## "))
            {
                GUILayout.Space(8);
                GUILayout.Label(Format(trimmed.Substring(3)), title2Style);
            }
            else if (trimmed.StartsWith("# "))
            {
                GUILayout.Label(Format(trimmed.Substring(2)), title1Style);
            }
            else if (trimmed.StartsWith(">"))
            {
                string note = trimmed.Substring(1).Trim();
                if (note != "")
                {
                    GUILayout.Label(Format(note), noteStyle);
                }
            }
            else if (isTable)
            {
                // La ligne |---|---| sépare l'en-tête du reste : on ne l'affiche pas
                if (trimmed.Contains("---") == false)
                {
                    DrawTableRow(trimmed, previousWasTable == false);
                }
            }
            else if (trimmed.StartsWith("- "))
            {
                string indent = line.Substring(0, line.Length - line.TrimStart().Length);
                GUILayout.Label(indent + "   •  " + Format(trimmed.Substring(2)), textStyle);
            }
            else
            {
                GUILayout.Label(Format(line), textStyle);
            }

            previousWasTable = isTable;
        }
    }

    void DrawTableRow(string row, bool isHeader)
    {
        string[] cells = row.Trim('|').Split('|');
        float width = EditorGUIUtility.currentViewWidth - 40f;
        GUIStyle style = textStyle;
        if (isHeader)
        {
            style = boldStyle;
        }

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < cells.Length; i++)
        {
            // 1re colonne : 40 % de la largeur, les autres se partagent le reste
            float cellWidth = width * 0.4f;
            if (i > 0)
            {
                cellWidth = width * 0.6f / (cells.Length - 1);
            }
            GUILayout.Label(Format(cells[i].Trim()), style, GUILayout.Width(cellWidth));
        }
        EditorGUILayout.EndHorizontal();

        Rect line = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(line, new Color(0.5f, 0.5f, 0.5f, 0.25f));
    }

    void DrawSeparator()
    {
        GUILayout.Space(6);
        Rect line = GUILayoutUtility.GetRect(1f, 2f, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(line, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUILayout.Space(6);
    }

    // Transforme **gras** et `code` en texte enrichi Unity (<b> et <color>)
    static string Format(string text)
    {
        string result = "";
        bool bold = false;
        bool code = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '*' && i + 1 < text.Length && text[i + 1] == '*')
            {
                if (bold)
                {
                    result = result + "</b>";
                }
                else
                {
                    result = result + "<b>";
                }
                bold = !bold;
                i++;
            }
            else if (text[i] == '`')
            {
                if (code)
                {
                    result = result + "</color>";
                }
                else
                {
                    result = result + "<color=#E8A33D>";
                }
                code = !code;
            }
            else
            {
                result = result + text[i];
            }
        }

        if (code)
        {
            result = result + "</color>";
        }
        if (bold)
        {
            result = result + "</b>";
        }
        return result;
    }

    void CreateStyles()
    {
        if (textStyle != null)
        {
            return;
        }

        textStyle = new GUIStyle(EditorStyles.label);
        textStyle.richText = true;
        textStyle.wordWrap = true;
        textStyle.fontSize = 13;

        boldStyle = new GUIStyle(textStyle);
        boldStyle.fontStyle = FontStyle.Bold;

        title1Style = new GUIStyle(textStyle);
        title1Style.fontSize = 22;
        title1Style.fontStyle = FontStyle.Bold;

        title2Style = new GUIStyle(textStyle);
        title2Style.fontSize = 17;
        title2Style.fontStyle = FontStyle.Bold;

        title3Style = new GUIStyle(textStyle);
        title3Style.fontSize = 14;
        title3Style.fontStyle = FontStyle.Bold;

        noteStyle = new GUIStyle(EditorStyles.helpBox);
        noteStyle.richText = true;
        noteStyle.wordWrap = true;
        noteStyle.fontSize = 13;
        noteStyle.padding = new RectOffset(10, 10, 8, 8);

        codeStyle = new GUIStyle(textStyle);
        codeStyle.wordWrap = false;
        codeStyle.normal.textColor = new Color(0.9f, 0.64f, 0.24f);
    }
}
