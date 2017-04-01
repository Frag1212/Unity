using UnityEngine;
using System.Collections;

public class DamageScript : MonoBehaviour
{

    public GameObject ParticleSystemDebrisPrefab;
    public float MaxHealth = 100f;
    float Health = 100f;

    void Start ()
    {
        Health = MaxHealth;
    }

    void Update()
    {
        if (transform.position.y < -100)
            Destroy(gameObject);
    }

    public bool TakeDamage(float damage, Vector3 Point, Vector3 ricochetVector)//true if dies
    {
        if (ParticleSystemDebrisPrefab != null)
        {
            GameObject pso = Instantiate(ParticleSystemDebrisPrefab, Point, Quaternion.FromToRotation(new Vector3(0, 0, 1), ricochetVector)) as GameObject;
            ParticleSystem ps = pso.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                /*ps.startSize = 2 * damage / MaxHealth;
                if (ps.startSize > 2f)
                    ps.startSize = 2f;*/
                //ParticleSystem.Burst b = new ParticleSystem.Burst(0f,10,10);
                ParticleSystem.Burst[] b = new ParticleSystem.Burst[1];
                b[0] = new ParticleSystem.Burst(0f, (short)(damage / 2), (short)(damage / 2));
                ps.emission.SetBursts(b);
            }
        }
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
        else
        {
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            if (mr != null)//Todo more visible color change
            {
                float t = (Health / MaxHealth);
                mr.material.color = new Color(t, t, t);
            }
        }
        return Health <= 0;
    }

}
