using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Debris : MonoBehaviour
{
    public Vector3 velocity;
    public float timeout;
    public Material[] eMaterials;
    public Renderer eRenderer;
    private float startTime;
    public virtual void Start()
    {
        this.gameObject.tag = "Debris";
        this.startTime = Time.time;

        if(eRenderer != null && eMaterials != null && eMaterials.Length == 2) {

            var eParticleSystem = eRenderer.GetComponent<ParticleSystem>();
            var shapeModule = eParticleSystem.shape;
            shapeModule.radius = 0.5f;

            var eRenderer2 = UnityEngine.Object.Instantiate(eRenderer.gameObject);
            eRenderer2.transform.parent = transform;

            shapeModule.position = new Vector3(0f, 0f, 0.5f);
            
            eParticleSystem.Play();
            eRenderer2.GetComponent<ParticleSystem>().Play();

            eRenderer.material = eMaterials[0];
            eRenderer2.GetComponent<Renderer>().material = eMaterials[1];
        }
    }

    public virtual void SetSpeed(float s)
    {
        this.velocity = ((new Vector3(Random.value - 0.5f, Random.value - 0.5f, 0).normalized * s) * 2) * Random.Range(0.8f, 1.2f);
    }

    public virtual void Update()
    {
        this.transform.position = this.transform.position + ((this.velocity * Time.deltaTime) * 3);
        if ((this.timeout != 0) && ((this.startTime + this.timeout) < Time.time))
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

}