using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneScript : Interactable {

    private CircleCollider2D _circleCollider2D;

    public int SenderID;
    public Vector2 Direction;
    public Color PheromoneColor;

    //called once directly after Instantiating
    public override void Awake() {
        base.Awake();

        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void StartPheromone() {
        _circleCollider2D.radius = SimulationScript.Instance.CoSh.PheromoneSmellDistance;

        StartCoroutine(DegradeRoutine());
    }

    IEnumerator DegradeRoutine() {
        yield return new WaitForSeconds(SimulationScript.Instance.CoSh.PheromoneDegradeTime);
        SimulationScript.Instance.PheromonePool.DespawnPheromone(this);
    }
    
}
