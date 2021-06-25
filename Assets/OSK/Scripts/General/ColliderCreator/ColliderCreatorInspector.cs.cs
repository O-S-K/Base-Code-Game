#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColliderCreator))]
[CanEditMultipleObjects]
public class ColliderCreatorInspector : UnityEditor.Editor
{
    private GUIContent generateContent;
    private GUIContent clearAllContent;

    public void OnEnable()
    {
        generateContent = new GUIContent(" Generate Colliders", EditorGUIUtility.IconContent("d_CreateAddNew").image);	
        clearAllContent = new GUIContent(" Clear Colliders", EditorGUIUtility.IconContent("P4_DeletedLocal").image);
    }

    public override void OnInspectorGUI()
    {
        var colliderCreator = target as ColliderCreator;
        var transform = colliderCreator.transform;

        if (transform.childCount < 1)
        {
            GUILayout.Label("Child objects are not setup.");
            if (GUILayout.Button("Setup Child Objects", GUILayout.Width(150), GUILayout.Height(30)))
            {
                var boxContainer = new GameObject("_BOXCOLLIDERS");
                boxContainer.transform.localPosition = Vector3.zero;
                boxContainer.transform.localScale = Vector3.one;
                boxContainer.transform.SetParent(transform);

                var point1 = new GameObject("Point (1)");
                point1.transform.SetParent(transform);
                point1.transform.localPosition = Vector3.zero;
                point1.transform.localScale = Vector3.one + Vector3.up * 3;

                var point2 = new GameObject("Point (2)");
                point2.transform.SetParent(transform);
                point2.transform.localPosition = Vector3.forward * 5;
                point2.transform.localScale = Vector3.one + Vector3.up * 3;
            }
        }
        else
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(generateContent, GUILayout.Height(30)))
            {
                GenerateColliders();
            }
            if (GUILayout.Button(clearAllContent, GUILayout.Height(30)))
            {
                ClearAllColliders();
            }
            GUILayout.EndHorizontal();
        }
    }

    private void GenerateColliders()
    {
        var colliderCreator = target as ColliderCreator;
        var transform = colliderCreator.transform;
        var boxContainerTransform = transform.GetChild(0);

        ClearAllColliders();

        // create capsule AND box colliders.
        for (int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            // create capsule collider.
            if (colliderCreator.generateCapsuleColliders)
            {
                var capsuleCollider = child.gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.radius = 1;
                capsuleCollider.height = 1;
            }

            // create box colliders for inbetween children.
            if (colliderCreator.generateBoxColliders && i < transform.childCount - 1)
            {
                var nextChild = transform.GetChild(i + 1);

                var position = (child.position + nextChild.position) / 2f;
                var size = new Vector3((child.localScale.x + nextChild.localScale.x) / 2f, (child.localScale.y + nextChild.localScale.y) / 2f, Vector3.Distance(child.position, nextChild.position));
                var rotation = Quaternion.LookRotation(nextChild.position - child.position);

                var boxChild = new GameObject("Box " + i, typeof(BoxCollider));
                boxChild.transform.SetParent(boxContainerTransform);
                boxChild.transform.position = position;
                boxChild.transform.localScale = size;
                boxChild.transform.rotation = rotation;
            }
        }
    }

    private void ClearAllColliders()
    {
        var colliderCreator = target as ColliderCreator;
        var transform = colliderCreator.transform;
        var boxContainerTransform = transform.GetChild(0);

        // delete all existing box colliders.
        for (int i = boxContainerTransform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(boxContainerTransform.GetChild(i).gameObject);
        }

        // delete all capsule colliders of children.
        for (int i = 1; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var capsuleCollider = child.GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                DestroyImmediate(capsuleCollider);
            }
        }
    }
}
#endif