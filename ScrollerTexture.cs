using UnityEngine;

namespace OSK
{
    public class ScrollerTexture : MonoBehaviour
    {
        [Range(-50, 50)]
        public float speeScroll;
        public bool isDirX;

        Material _material;
        float offset;

        void Start()
        {
            _material = GetComponent<Renderer>().material;
        }

        void Update()
        {
            offset += (Time.deltaTime * speeScroll) / 10f;

            if (isDirX)
            {
                _material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
            }
            else
            {
                _material.SetTextureOffset("_MainTex", new Vector2(0, offset));
            }
        }
    }
}
