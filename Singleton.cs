using UnityEngine;


namespace OSK
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        Debug.LogError(typeof(T).Name + " == Null");
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            T[] Ts = FindObjectsOfType<T>();
            if (Ts.Length > 1)
            {
                Debug.Log("Singleton = " + Ts.Length);
                Destroy(this);
            }
        }
    }
}