using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]

public class TiledWall : MonoBehaviour
{
    public float scaleFactor = 5.0f;
    Material mat;
  
    void Start()
    {
        Debug.Log("Start");
        GetComponent<Renderer>().material.mainTextureScale = new Vector3(transform.localScale.x / scaleFactor,1, transform.localScale.z / scaleFactor);
    }
     
    void Update()
    {
        if (transform.hasChanged && Application.isEditor && !Application.isPlaying)
        {
            Debug.Log("The transform has changed!");
            GetComponent<Renderer>().material.mainTextureScale = new Vector3(transform.localScale.x / scaleFactor,1, transform.localScale.z / scaleFactor);
            transform.hasChanged = false;
        }

    }

}
