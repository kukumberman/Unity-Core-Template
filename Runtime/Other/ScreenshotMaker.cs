using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class ScreenshotMaker : MonoBehaviour
{
    public enum PathType
    {
        Auto,
        DataPath,
        PersistentDataPath,
    }

    public sealed class SerializedCanvasProperties
    {
        public RenderMode RenderMode;
        public Camera WorldCamera;
        public float PlaneDistance;
        public int SortingOrder;
        public int SortingLayerId;

        public void ApplyTo(Canvas canvas)
        {
            canvas.renderMode = RenderMode;
            canvas.worldCamera = WorldCamera;
            canvas.planeDistance = PlaneDistance;
            canvas.sortingOrder = SortingOrder;
            canvas.sortingLayerID = SortingLayerId;
        }

        public static SerializedCanvasProperties From(Canvas canvas)
        {
            return new SerializedCanvasProperties
            {
                RenderMode = canvas.renderMode,
                WorldCamera = canvas.worldCamera,
                PlaneDistance = canvas.planeDistance,
                SortingOrder = canvas.sortingOrder,
                SortingLayerId = canvas.sortingLayerID,
            };
        }
    }

    [SerializeField]
    private PathType _pathType = PathType.Auto;

    [SerializeField]
    private Vector2Int _size;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private KeyCode _key = KeyCode.Keypad5;

    [SerializeField]
    private Canvas[] _canvases;

    private RenderTexture _renderTexture;
    private Texture2D _screenshot;

    private readonly SerializedCanvasProperties _targetCanvasProps = new();
    private readonly Dictionary<Canvas, SerializedCanvasProperties> _canvasOriginalProps = new();

    private void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            MakeScreenshot();
        }
    }

    private void Initialize()
    {
        var layers = SortingLayer.layers;
        var lastLayer = layers[layers.Length - 1];

        _targetCanvasProps.RenderMode = RenderMode.ScreenSpaceCamera;
        _targetCanvasProps.WorldCamera = _camera;
        _targetCanvasProps.PlaneDistance = _camera.nearClipPlane + 1e-2f;
        _targetCanvasProps.SortingLayerId = 999;
        _targetCanvasProps.SortingLayerId = lastLayer.id;
    }

    private void CreateTextures()
    {
        _renderTexture = new RenderTexture(_size.x, _size.y, 24);
        _screenshot = new Texture2D(_renderTexture.width, _renderTexture.height);
    }

    private void CleanupTextures()
    {
        _renderTexture.Release();

        if (Application.isPlaying)
        {
            Destroy(_renderTexture);
            Destroy(_screenshot);
        }
        else
        {
            DestroyImmediate(_renderTexture);
            DestroyImmediate(_screenshot);
        }
    }

    private void ModifyCanvases()
    {
        foreach (var canvas in _canvases)
        {
            var originalProperties = SerializedCanvasProperties.From(canvas);
            _canvasOriginalProps.Add(canvas, originalProperties);

            _targetCanvasProps.ApplyTo(canvas);
        }
    }

    private void RestoreCanvases()
    {
        foreach (var canvas in _canvases)
        {
            var originalProperties = _canvasOriginalProps[canvas];
            originalProperties.ApplyTo(canvas);
        }

        _canvasOriginalProps.Clear();
        _canvasOriginalProps.TrimExcess();
    }

    [ContextMenu(nameof(MakeScreenshot))]
    private void MakeScreenshot()
    {
        Initialize();

        CreateTextures();

        var previousActiveRenderTexture = RenderTexture.active;
        var previousCameraTargetTexture = _camera.targetTexture;

        ModifyCanvases();

        _camera.targetTexture = _renderTexture;
        _camera.Render();

        RenderTexture.active = _renderTexture;
        Rect rect = new Rect(0, 0, _screenshot.width, _screenshot.height);
        _screenshot.ReadPixels(rect, 0, 0);
        _screenshot.Apply();

        RestoreCanvases();

        RenderTexture.active = previousActiveRenderTexture;
        _camera.targetTexture = previousCameraTargetTexture;

        WriteToFile(_screenshot);

        CleanupTextures();
    }

    private void WriteToFile(Texture2D texture)
    {
        string fileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".png";
        string path = ResolveBasePath(_pathType);
        string directory = Path.Combine(path, "Screenshots");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string savePath = Path.Combine(directory, fileName);
        File.WriteAllBytes(savePath, texture.EncodeToPNG());
    }

    private static string ResolveBasePath(PathType type)
    {
        if (type == PathType.Auto)
        {
            return Application.isEditor ? Application.dataPath : Application.persistentDataPath;
        }

        if (type == PathType.DataPath)
        {
            return Application.dataPath;
        }

        if (type == PathType.PersistentDataPath)
        {
            return Application.persistentDataPath;
        }

        throw new NotImplementedException();
    }
}
