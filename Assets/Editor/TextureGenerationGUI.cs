using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TextureGenerationScript))]
public class TextureGenerationGUI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Regenerate"))
        {
            TextureGenerationScript t = (TextureGenerationScript)target;
            t.GenerateTexture();
        }
    }
}
