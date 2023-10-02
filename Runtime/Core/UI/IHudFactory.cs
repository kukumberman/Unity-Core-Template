using Game.Enums;

namespace Game.Core.UI
{
    public interface IHudFactory
    {
        HudOrientation Orientation { get; set; }

        void Create(Mediator hud);
    }
}
