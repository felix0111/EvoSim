using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [HideInInspector] public CollisionWatcher CollisionWatcher;

    public virtual void Awake() {
        if (CollisionWatcher != null) return;   //should theoretically not happen
        CollisionWatcher = gameObject.AddComponent<CollisionWatcher>();

        CollisionWatcher.OnEnter.AddListener(CollisionEnter);
        CollisionWatcher.OnExit.AddListener(CollisionExit);
    }

    protected virtual void CollisionEnter(GameObject go, CollisionEvent collisionEvent) { }

    protected virtual void CollisionExit(GameObject go, CollisionEvent collisionEvent) { }
}