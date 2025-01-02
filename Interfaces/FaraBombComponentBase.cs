using UnityEngine;

namespace FaraBombRush.Interfaces;

// 基底クラスの追加（オプショナル）
public abstract class FaraBombComponentBase : MonoBehaviour, IFaraBombComponent
{
    public virtual void Initialize()
    {
    }

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