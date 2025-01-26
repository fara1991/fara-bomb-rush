using FaraBombRush.Configs;
using UnityEngine;

namespace FaraBombRush.Interfaces;

// 基底クラスの追加（オプショナル）
public abstract class FaraBombComponentBase : MonoBehaviour, IFaraBombComponent
{
    protected PluginConfig _pluginConfig;

    public void Initialize(PluginConfig pluginConfig)
    {
        _pluginConfig = pluginConfig;
        InitializeComponent();
    }

    public abstract void InitializeComponent();

    public bool IsEnabled => enabled;

    public virtual void Enable()
    {
        enabled = true;
    }

    public virtual void Disable()
    {
        enabled = false;
    }
}