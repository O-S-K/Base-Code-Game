using UnityEngine;
using System.Collections.Generic;


namespace OSK
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {
        public static Dictionary<string, List<MonoBehaviour>> ObjPooling =
                  new Dictionary<string, List<MonoBehaviour>>();


        public static Obj CreatePooling<Obj>(Obj _obj) where Obj : MonoBehaviour
        {
            List<MonoBehaviour> listObjects;
            string type = _obj.gameObject.name;

            if (!ObjPooling.ContainsKey(type))
            {
                listObjects = new List<MonoBehaviour>
            {
                Instantiate(_obj)
            };
                ObjPooling.Add(type, listObjects);
                Obj obj = (Obj)listObjects[0];
                return obj;
            }
            else
            {
                listObjects = ObjPooling[type];
                for (int index = 0; index < listObjects.Count; index++)
                {
                    var v = listObjects[index];
                    if (v == null)
                    {
                        listObjects.Remove(v);
                        continue;
                    }
                    if (!v.gameObject.activeSelf)
                    {
                        v.gameObject.SetActive(true);
                        return v as Obj;
                    }
                }

                Obj obj = Instantiate(_obj);
                listObjects.Add(obj);
                ObjPooling[type] = listObjects;
                return obj;
            }
        }
    }
}