using UnityEngine;
using System.Collections;

public class TextureGenerationScript : MonoBehaviour
{
    public void GenerateTexture()
    {
        int texwidth = 10;
        int texheight = 10;
        Texture2D newtexture = new Texture2D(texwidth, texheight);
        for (int y = 0; y < texheight; y++)
            for (int x = 0; x < texwidth; x++)
            {
                //newtexture.SetPixel(x, y, new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                newtexture.SetPixel(x, y, new Color(Mathf.Sin(x), Mathf.Sin(y), Mathf.Sin(x) * Mathf.Sin(y)));
            }
        newtexture.filterMode = FilterMode.Point;
        newtexture.Apply();
        GetComponent<MeshRenderer>().sharedMaterials[0].mainTexture = newtexture;
    }

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
