using FaraBombRush.Enums;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

internal class FaraBombEffectController : FaraBombComponentBaseController
{
    private AudioSource _audioSource;
    private float _timer;
    private const float Interval = 120.0f;
    private FaraBombExplosionPhaseEnum _phase = FaraBombExplosionPhaseEnum.Idle;

    private void Update()
    {
        if (_phase != FaraBombExplosionPhaseEnum.ExplosionNow) return;

        if (_timer < Interval)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
            Disable();
        }
    }

    protected override void InitializeComponent()
    {
        // エフェクトオブジェクトの表示/非表示制御
        gameObject.SetActive(false);
        _audioSource = gameObject.GetComponentInChildren<AudioSource>();
        _audioSource.volume = 0.5f;
    }

    protected internal override void Enable()
    {
        gameObject.SetActive(true);
        _phase = FaraBombExplosionPhaseEnum.ExplosionNow;
        _timer = 0;
        base.Enable();
    }

    protected internal override void Disable()
    {
        gameObject.SetActive(false);
        _phase = FaraBombExplosionPhaseEnum.Exploded;
        base.Disable();
    }

    internal bool ExplosionCompleted()
    {
        return _phase == FaraBombExplosionPhaseEnum.Exploded;
    }
}