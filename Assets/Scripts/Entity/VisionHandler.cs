using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class VisionHandler {

    public Vector2 AvgPlantPosition, AvgMeatPosition, AvgEntityPosition;
    public float AvgPlantDistance, AvgMeatDistance, AvgEntityDistance;

    public bool SameSpeciesInView;

    public Collider2D InFrontCollider;
    public List<Collider2D> InVisionCone = new (64);
    private Collider2D[] _colliderBuffer = new Collider2D[64];
    private int _colliderBufferSize = 0;
    private RaycastHit2D[] _raycastBuffer = new RaycastHit2D[1];

    private ContactFilter2D _visionFilter;

    private readonly EntityScript _entity;

    public VisionHandler(EntityScript entity, int layerMask) {
        _entity = entity;
        _visionFilter = new ContactFilter2D {
            layerMask = layerMask
        };
    }

    public void UpdateVision(Vector2 direction, float fovAngle, bool raycastCheck, bool overlap) {
        if(overlap) _colliderBufferSize = Physics2D.OverlapCircle(_entity.transform.position, _entity.Gene.ViewDistance + _entity.Radius, _visionFilter, _colliderBuffer);

        InVisionCone.Clear();

        for (int i = 0; i < _colliderBufferSize; i++) {
            if (!_colliderBuffer[i].isActiveAndEnabled || _colliderBuffer[i] == _entity.Collider || !IsInFOV(direction, _colliderBuffer[i].transform.position, fovAngle, out float colliderAngle)) continue;

            if (raycastCheck) {
                int x = Physics2D.Raycast(_entity.transform.position, (Quaternion.Euler(0f, 0f, colliderAngle) * _entity.transform.up).normalized, _visionFilter, _raycastBuffer, _entity.Gene.ViewDistance + _entity.Radius);
                
                //Debug.DrawRay(_entity.transform.position, (Quaternion.Euler(0f, 0f, colliderAngle) * _entity.transform.up).normalized * (_entity.Gene.ViewDistance + _entity.Radius), Color.red);
                //if (x == 0) Debug.LogWarning("Could not raycast vision! " + _entity.gameObject.name + " : " + _colliderBuffer[i].name);
                
                //don't add collider if view is obstructed
                if (x == 0 || _raycastBuffer[0].collider != _colliderBuffer[i]) continue;
            }
            InVisionCone.Add(_colliderBuffer[i]);
        }

        ProcessVisionCone();

        RaycastHit2D hit = Physics2D.Raycast((Vector2)_entity.transform.position, (Vector2)_entity.transform.up, Mathf.Max(SimulationScript.Instance.CoSh.MaxEatDistance, SimulationScript.Instance.CoSh.MaxAttackDistance) + _entity.Radius, _visionFilter.layerMask);
        InFrontCollider = hit.collider;
    }

    private void ProcessVisionCone() {
        int plantCount = 0;
        int meatCount = 0;
        int entityCount = 0;

        AvgPlantPosition = Vector2.zero;
        AvgMeatPosition = Vector2.zero;
        AvgEntityPosition = Vector2.zero;
        AvgPlantDistance = float.MaxValue;
        AvgMeatDistance = float.MaxValue;
        AvgEntityDistance = float.MaxValue;

        SameSpeciesInView = false;

        for (int i = 0; i < InVisionCone.Count; i++) {
            if (InVisionCone[i].TryGetComponent(out FoodScript fs)) {
                if (fs.IsMeat) {
                    AvgMeatPosition += (Vector2)fs.transform.position;
                    meatCount++;
                } else {
                    AvgPlantPosition += (Vector2)fs.transform.position;
                    plantCount++;
                }
            } else if (InVisionCone[i].TryGetComponent(out EntityScript es)) {
                AvgEntityPosition += (Vector2)es.transform.position;
                if (es.Network.SpeciesID == _entity.Network.SpeciesID) SameSpeciesInView = true;
                entityCount++;
            }
        }

        if (plantCount != 0f) {
            AvgPlantPosition /= plantCount;
            AvgPlantDistance = Vector2.Distance(_entity.transform.position, AvgPlantPosition);
        } else {
            AvgPlantPosition = _entity.transform.position;
        }

        if (meatCount != 0f) {
            AvgMeatPosition /= meatCount;
            AvgMeatDistance = Vector2.Distance(_entity.transform.position, AvgMeatPosition);
        } else {
            AvgMeatPosition = _entity.transform.position;
        }

        if (entityCount != 0f) {
            AvgEntityPosition /= entityCount;
            AvgEntityDistance = Vector2.Distance(_entity.transform.position, AvgEntityPosition);
        } else {
            AvgEntityPosition = _entity.transform.position;
        }
    }

    private bool IsInFOV(Vector2 mainVisionDirection, Vector2 colliderPos, float fovAngle, out float colliderAngle) {
        colliderAngle = Vector2.SignedAngle(mainVisionDirection, (colliderPos - (Vector2)_entity.transform.position).normalized);

        if (Mathf.Abs(colliderAngle) <= fovAngle) return true;
        return false;
    }
    
}
