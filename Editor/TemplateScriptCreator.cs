using System.IO;
using UnityEditor;

public static class TemplateScriptCreator
{
    private const string ScriptTemplateDirectory =
        "Packages/Unity-Core-Template/Editor/ScriptTemplates/";
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
        var path = ScriptTemplateDirectory + templateName;
        var filename = path.Split("-")[^1];
        filename = Path.GetFileNameWithoutExtension(filename);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, filename);
    }
}
