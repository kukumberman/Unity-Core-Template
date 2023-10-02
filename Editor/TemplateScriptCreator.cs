using System.IO;
using UnityEditor;
using UnityEngine;

public static class TemplateScriptCreator
{
    private const string ScriptTemplatesDirectory = "ScriptTemplates";
    private const string Script_1 = "01-C# Script__Core__UI-MyHudView.cs.txt";
    private const string Script_2 = "02-C# Script__Core__UI Empty-MyHudView.cs.txt";
    private const string ItemDirectory = "Tools/ScriptTemplates/";

    [MenuItem(ItemDirectory + Script_1)]
    private static void CreateScript_1()
    {
        CreateScriptAsset(Script_1);
    }

    [MenuItem(ItemDirectory + Script_2)]
    private static void CreateScript_2()
    {
        CreateScriptAsset(Script_2);
    }

    private static void CreateScriptAsset(string templateName)
    {
        var scriptAssetPath = AssetDatabase.GUIDToAssetPath(
            AssetDatabase.FindAssets($"t:Script {nameof(TemplateScriptCreator)}")[0]
        );

        var scriptAssetAbsolutePath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", scriptAssetPath)
        );

        var templatesDirectory = Path.Combine(
            Path.GetDirectoryName(scriptAssetAbsolutePath),
            ScriptTemplatesDirectory
        );

        var templateFileAbsolutePath = Path.Combine(templatesDirectory, templateName);

        var filename = templateFileAbsolutePath.Split("-")[^1];
        filename = Path.GetFileNameWithoutExtension(filename);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateFileAbsolutePath, filename);
    }
}
