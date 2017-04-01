using UnityEngine;
using System.Collections;

public class SparklesScript : MonoBehaviour
{
    float TimeRemains = 0;
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if(ps == null)
            Destroy(gameObject);
        else
            TimeRemains = ps.startLifetime;
    }

    void Update()
    {
        TimeRemains -= Time.deltaTime;
        if (TimeRemains <= 0)
            Destroy(gameObject);
    }
}
