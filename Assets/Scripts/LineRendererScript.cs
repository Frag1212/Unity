using UnityEngine;
using System.Collections;

public class LineRendererScript : MonoBehaviour
{
    public float LifeTime;
    float TimeRemains = 0;
    LineRenderer lr = null;

    void Start()
    {
        if (TimeRemains == 0)
            TimeRemains = LifeTime;
        lr = gameObject.GetComponent<LineRenderer>();
    }
    public void SetLifeTime(float t)
    {
        TimeRemains = t;
    }
    // Update is called once per frame
    void Update ()
    {
        TimeRemains -= Time.deltaTime;
        if (TimeRemains <= 0)
            Destroy(gameObject);
        else
            lr.material.SetTextureOffset("_MainTex", new Vector2(TimeRemains*2, 1));
    }
}