using FaraBombRush.Configs;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

// 基底クラスの追加（オプショナル）
internal abstract class FaraBombComponentBaseController : MonoBehaviour
{
    protected PluginConfig Config;

    protected internal void Initialize(PluginConfig config)
    {
        Config = config;
        InitializeComponent();
    }

    protected abstract void InitializeComponent();

    protected internal virtual void Enable()
    {
        enabled = true;
    }

    protected internal virtual void Disable()
    {
        enabled = false;
    }
}