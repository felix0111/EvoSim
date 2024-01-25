using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CollisionWatcher : MonoBehaviour {

    public List<GameObject> Colliders = new ();

    [HideInInspector] public UnityEvent<GameObject, CollisionEvent> OnEnter = new ();
    [HideInInspector] public UnityEvent<GameObject, CollisionEvent> OnExit = new ();

    void OnTriggerEnter2D(Collider2D collider) {
        Colliders.Add(collider.gameObject);
        OnEnter.Invoke(collider.gameObject, CollisionEvent.Enter);
    }

    void OnTriggerExit2D(Collider2D collider) {
        Colliders.Remove(collider.gameObject);
        OnExit.Invoke(collider.gameObject, CollisionEvent.Exit);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Colliders.Add(collision.gameObject);
        OnEnter.Invoke(collision.gameObject, CollisionEvent.Enter);
    }

    void OnCollisionExit2D(Collision2D collision) {
        Colliders.Remove(collision.gameObject);
        OnExit.Invoke(collision.gameObject, CollisionEvent.Exit);
    }

    private void OnOtherDisabled(GameObject collider) {
        Colliders.Remove(collider);
        OnExit.Invoke(collider, CollisionEvent.Disable);
    }

    public bool IsColliding(params int[] layer) {
        if(layer.Length == 0) return false;

        for (int i = 0; i < Colliders.Count; i++) {
            for (int j = 0; j < layer.Length; j++) {
                if (Colliders[i].layer == layer[j]) return true;
            }
        }

        return false;
    }

    //OnDisable triggers this objects OnExit for every collider
    //also triggers OnExit on every collider, passing this gameobject as the exiting collider
    public void OnDisable() {
        for (int i = 0; i < Colliders.Count; i++) {
            OnExit.Invoke(Colliders[i].gameObject, CollisionEvent.Exit);
            Colliders[i].gameObject.GetComponent<CollisionWatcher>().OnOtherDisabled(gameObject);
        }

        Colliders.Clear();
    }

    public void OnDestroy() {
        for (int i = 0; i < Colliders.Count; i++) {
            OnExit.Invoke(Colliders[i].gameObject, CollisionEvent.Exit);
            Colliders[i].gameObject.GetComponent<CollisionWatcher>().OnOtherDisabled(gameObject);
        }

        Colliders.Clear();
    }

    public bool Contains(string tag) => Colliders.Exists(o => o.CompareTag(tag));

    public bool TryGet<T>(out T component) where T : MonoBehaviour {
        for (int i = 0; i < Colliders.Count; i++) {
            if (Colliders[i].TryGetComponent(out T comp)) {
                component = comp;
                return true;
            }
        }

        component = null;
        return false;
    }

    public T First<T>() where T : MonoBehaviour {
        for (int i = 0; i < Colliders.Count; i++) {
            if (Colliders[i].TryGetComponent(out T comp)) return comp;
        }

        return null;
    }

    public int All<T>(T[] array) where T : MonoBehaviour {

        T comp;
        int arrayStep = 0;

        for (int i = 0; i < Colliders.Count; i++) {
            comp = Colliders[i].GetComponent<T>();

            if (array.Length == arrayStep) break;
            if (comp != null) {
                array[arrayStep] = comp;
                arrayStep++;
            }
        }
        
        //fill rest of array with null
        for (int i = 0; i < array.Length - arrayStep; i++) {
            array[arrayStep + i] = null;
        }

        return arrayStep;
    }

}

public enum CollisionEvent {
    Enter, Exit, Disable
}