namespace FaraBombRush.Interfaces;

public interface IFaraBombComponent
{
    bool IsEnabled { get; }
    void Initialize();
    void Enable();
    void Disable();
}