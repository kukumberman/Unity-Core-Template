using Game.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public sealed class GameView : MonoBehaviour
    {
        [SerializeField] public Camera Camera;
        [SerializeField] public Canvas Canvas;
        [SerializeField] public UIDocument Document;
    }
}