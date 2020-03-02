using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class InsetCameraMovie : MonoBehaviour
{
    public float speed;
    public virtual void Start()
    {
        foreach (Transform t in this.transform)
        {
            ParticleSystem p = t.GetComponent<ParticleSystem>();
            if (p)
            {
                // p.localVelocity.x *= speed;
                ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = p.velocityOverLifetime;
                velocityOverLifetimeModule.x = velocityOverLifetimeModule.x.constant * this.speed;
                // p.minEnergy /= speed;
                // p.maxEnergy /= speed;
                ParticleSystem.MainModule mainModule = p.main;
                mainModule.startLifetime = mainModule.startLifetime.constant / this.speed;
                // p.minEmission *= speed;
                // p.maxEmission *= speed
                ParticleSystem.EmissionModule emissionModule = p.emission;
                emissionModule.rateOverTime = emissionModule.rateOverTime.constant * this.speed;
                ParticleSystemRenderer pr = (ParticleSystemRenderer) t.GetComponent(typeof(ParticleSystemRenderer));
                if (pr && (pr.renderMode == ParticleSystemRenderMode.Stretch))
                {
                    pr.lengthScale = pr.lengthScale * (this.speed*0.2f);
                }
            }
        }
    }

    public InsetCameraMovie()
    {
        this.speed = 1f;
    }

}