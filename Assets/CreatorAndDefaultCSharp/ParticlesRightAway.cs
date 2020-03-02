using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class ParticlesRightAway : MonoBehaviour
{
    public virtual void Start()
    {
        ParticleSystem myParticleSystem = this.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = myParticleSystem.main;
        ParticleSystem.MinMaxCurve origStartLifetime = mainModule.startLifetime;
        ParticleSystem.MinMaxCurve startLifetime = new ParticleSystem.MinMaxCurve(0);
        startLifetime.constantMin = 0;
        startLifetime.constantMax = origStartLifetime.constantMax;
        mainModule.startLifetime = startLifetime;
        ParticleSystem.EmissionModule emissionModule = myParticleSystem.emission;
        float burstSize = emissionModule.rateOverTime.constantMin * startLifetime.constantMax;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
        bursts[0] = new ParticleSystem.Burst(0f, (short)Mathf.RoundToInt(burstSize), (short)Mathf.RoundToInt(burstSize));
        emissionModule.SetBursts(bursts);
    }

}