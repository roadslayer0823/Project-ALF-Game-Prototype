using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ObjectChangeEventsManager : MonoBehaviour
{
    static ObjectChangeEventsManager()
    {
        ObjectChangeEvents.changesPublished += ChangesPublished;
    }

    static void ChangesPublished(ref ObjectChangeEventStream stream)
    {
        for (int i = 0; i < stream.length; ++i)
        {
            switch (stream.GetEventType(i))
            {
                case ObjectChangeKind.CreateGameObjectHierarchy:
                    stream.GetCreateGameObjectHierarchyEvent(i, out var createGameObjectHierarchyEvent);
                    var newGameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEvent.instanceId) as GameObject;
                    if (newGameObject.tag == "AutoUnpackPrefab")
                    {
                        PrefabUtility.UnpackPrefabInstance(newGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                        Debug.Log($"Create GameObject: {newGameObject.name} in scene: {createGameObjectHierarchyEvent.scene.name}.");
                    }
                    break;
            }
        }
    }
}
