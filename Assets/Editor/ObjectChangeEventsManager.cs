using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ObjectChangeEventsManager
{
    private const string TAG_AUTO_UNPACK_PREFAB = "AutoUnpackPrefab";

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
                    if (newGameObject.CompareTag( TAG_AUTO_UNPACK_PREFAB ))
                    {
                        PrefabUtility.UnpackPrefabInstance(newGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                        Debug.Log($"Automatically unpack the prefab instance named \"{newGameObject.name}\" in the scene named \"{createGameObjectHierarchyEvent.scene.name}\".");
                    }
                    break;
            }
        }
    }
}
