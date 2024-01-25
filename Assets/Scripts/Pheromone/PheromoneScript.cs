using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneScript : Interactable {

    public CircleCollider2D CircleCollider2D;
    public ParticleSystem ParticleSystem;   

    public int SenderID;
    public Vector2 Direction;
    public Color Color;

    public Color PheromoneColor => ParticleSystem.main.startColor.color;

    //called once directly after Instantiating
    public override void Awake() {
        base.Awake();

        CircleCollider2D = GetComponent<CircleCollider2D>();
        ParticleSystem = GetComponent<ParticleSystem>();
    }

    void Start() {
        CircleCollider2D.radius = SimulationScript.Instance.CoSh.PheromoneSmellDistance;
        var m = ParticleSystem.main;
        m.startLifetime = SimulationScript.Instance.CoSh.PheromoneDegradeTime - m.duration;
        m.startColor = Color;

        StartCoroutine(DegradeRoutine());
    }

    IEnumerator DegradeRoutine() {
        yield return new WaitForSeconds(SimulationScript.Instance.CoSh.PheromoneDegradeTime);
        Destroy(gameObject);
    }
    
}
