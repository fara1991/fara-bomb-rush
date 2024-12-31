namespace FaraBombRush.Interfaces;

public interface IFaraBombComponent
{
    void Initialize();
    bool IsEnabled { get; }
    void Enable();
    void Disable();
}
