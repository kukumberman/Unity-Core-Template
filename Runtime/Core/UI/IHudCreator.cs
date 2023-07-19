using Game.Enums;

namespace Game.Core.UI
{
    public interface IHudCreator
    {
        HudOrientation Orientation { get; set; }

        void Create(Mediator hud);
    }
}
