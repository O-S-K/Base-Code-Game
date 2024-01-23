using UnityEngine;

namespace OSK
{
    public class ScrollerTexture : MonoBehaviour
    {
         public Vector2 speedScroll;

 	 private Material _material;
 	 private Vector2 offset;

	 private void Start()
 	 {
  	    _material = GetComponent<Renderer>().material;
 	 }

 	 private void Update()
 	 {
  	    offset += (Time.deltaTime * speedScroll) / 10f;
  	    _material.SetTextureOffset("_MainTex", new Vector2(offset.x, offset.y));
 	 }
    }
}
