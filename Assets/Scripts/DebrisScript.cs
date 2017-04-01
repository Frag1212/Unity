using UnityEngine;
using System.Collections;

public class DebrisScript : MonoBehaviour
{
    float TimeRemains = 5f;
    bool firstupdate = true;
    public Vector3 StartForce;
    //public bool customMaterial;

    void Update()
    {
        if (firstupdate)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if(rb != null && StartForce.magnitude != 0)
                rb.AddForce(StartForce);//ForceMode.Impulse
            else
                Destroy(gameObject);
            firstupdate = false;
        }
        TimeRemains -= Time.deltaTime;
        if (TimeRemains <= 0)
            Destroy(gameObject);
    }

    /*void OnDestroy()
    {
        if (customMaterial)
            Destroy(gameObject.GetComponent<Renderer>().sharedMaterial);
    }*/
}