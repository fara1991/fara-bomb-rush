using FaraBombRush.Configs;

namespace FaraBombRush.Interfaces;

public interface IFaraBombComponent
{
    bool IsEnabled { get; }
    void InitializeComponent();
    void Enable();
    void Disable();
}