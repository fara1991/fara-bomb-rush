using FaraBombRush.Enums;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

internal class FaraBombEffectController : FaraBombComponentBaseController
{
    private const int EffectCounter = 120;
    private AudioSource _audioSource;
    private int _counter;
    private FaraBombExplosionPhaseEnum _phase = FaraBombExplosionPhaseEnum.Idle;

    private void Update()
    {
        if (_phase != FaraBombExplosionPhaseEnum.ExplosionNow) return;

        _counter++;
        if (_counter >= EffectCounter) Disable();
    }

    protected override void InitializeComponent()
    {
        // エフェクトオブジェクトの表示/非表示制御
        gameObject.SetActive(false);
        _audioSource = gameObject.GetComponentInChildren<AudioSource>();
        _audioSource.volume = 0.5f;
        Plugin.Logger.Debug("Initializing EffectController");
    }

    protected internal override void Enable()
    {
        gameObject.SetActive(true);
        _phase = FaraBombExplosionPhaseEnum.ExplosionNow;
        _counter = 0;
        base.Enable();
        Plugin.Logger.Debug("FaraBombEffectController enabled");
    }

    protected internal override void Disable()
    {
        gameObject.SetActive(false);
        _phase = FaraBombExplosionPhaseEnum.Exploded;
        base.Disable();
        Plugin.Logger.Debug("FaraBombEffectController disabled");
    }

    internal bool ExplosionCompleted()
    {
        return _phase == FaraBombExplosionPhaseEnum.Exploded;
    }
}