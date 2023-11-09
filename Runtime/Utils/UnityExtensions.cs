using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityExtensions
{
    public static void DestroyChildrens(this Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            Object.Destroy(child.gameObject);
        }
    }

    public static Texture2D ToTexture2D(this RenderTexture renderTexture)
    {
        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        var previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = previous;
        return texture;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsRGBA(this Color32 color, Color32 other)
    {
        return color.r == other.r && color.g == other.g && color.b == other.b && color.a == other.a;
    }

    public static void ExpandHeight(this TMPro.TMP_Text text)
    {
        var preferredSize = text.GetPreferredValues();

        var textTrasform = text.rectTransform;
        var sizeDelta = textTrasform.sizeDelta;
        sizeDelta.y = preferredSize.y;
        textTrasform.sizeDelta = sizeDelta;
    }

    public static void ExpandWidth(this TMPro.TMP_Text text)
    {
        var preferredSize = text.GetPreferredValues();

        var textTrasform = text.rectTransform;
        var sizeDelta = textTrasform.sizeDelta;
        sizeDelta.x = preferredSize.x;
        textTrasform.sizeDelta = sizeDelta;
    }

    public static List<Transform> GetChildrenDeeply(Transform parent, int depth)
    {
        var result = new List<Transform>();

        result.Add(parent);

        if (depth <= 0)
        {
            return result;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            result.AddRange(GetChildrenDeeply(child, depth - 1));
        }

        return result;
    }
}
