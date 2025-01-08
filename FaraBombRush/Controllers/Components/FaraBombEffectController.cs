using FaraBombRush.Enums;
using FaraBombRush.Interfaces;

namespace FaraBombRush.Controllers.Components;

public class FaraBombEffectController : FaraBombComponentBase
{
    private const int EffectCounter = 120;
    private int _counter;
    private FaraBombExplosionPhaseEnum _phase = FaraBombExplosionPhaseEnum.Idle;

    private void Update()
    {
        if (_phase != FaraBombExplosionPhaseEnum.ExplosionNow) return;

        _counter++;
        if (_counter >= EffectCounter) Disable();
    }

    public override void Initialize()
    {
        // エフェクトオブジェクトの表示/非表示制御
        gameObject.SetActive(false);
        Plugin.Logger.Debug("Initializing EffectController");
    }

    public override void Enable()
    {
        gameObject.SetActive(true);
        _phase = FaraBombExplosionPhaseEnum.ExplosionNow;
        _counter = 0;
        base.Enable();
        Plugin.Logger.Debug("FaraBombEffectController enabled");
    }

    public override void Disable()
    {
        gameObject.SetActive(false);
        _phase = FaraBombExplosionPhaseEnum.Exploded;
        base.Disable();
        Plugin.Logger.Debug("FaraBombEffectController disabled");
    }

    public bool ExplosionCompleted()
    {
        return _phase == FaraBombExplosionPhaseEnum.Exploded;
    }
}