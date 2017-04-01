using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour
{

    public GameObject DeathCameraPrefab;

    public void OnPlayerDeath ()
    {
        GameObject DeathCamera = null;

        if (DeathCameraPrefab != null)
        {
            DeathCamera = Instantiate(DeathCameraPrefab, transform.position, transform.rotation) as GameObject;
        }

        if (DeathCamera != null)
        {
            DeathCamera.SetActive(true);
            DeathCamera.transform.position = transform.position;
            DeathCamera.transform.rotation = transform.rotation;
            DeathCamera.GetComponent<Camera>().enabled = true;
        }
    }
}
