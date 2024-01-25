using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyHandler {

    private readonly EntityScript _entity;

    //Energy Consumption
    public float EnergyConsumption { get; private set; }
    public bool DisableEnergyConsumption = false;

    //Digestion
    public float ActiveEnergy {
        get => _activeEnergy;
        private set {
            _activeEnergy = Mathf.Clamp(value, 0f, _entity.ScaledMaxEnergy);
            if (_activeEnergy <= 0f) _entity.Kill(true);
        }
    }

    //stomach
    public float AmountInStomach => _meatInStomach + _plantInStomach;
    public float MeatContent => _meatInStomach;
    public float PlantContent => _plantInStomach;

    public float ReproductionEnergy {
        get => _reproductionEnergy;
        set {
            _reproductionEnergy = value;
            if (!(_reproductionEnergy >= _entity.ScaledEnergyToReproduce)) return;

            _entity.Reproduce();
            _reproductionEnergy = 0f;
            IsReproducing = false;
        }
    }

    public bool IsReproducing;

    private float _activeEnergy, _meatInStomach, _plantInStomach, _reproductionEnergy;
    private float _smoothingValue = 0f;

    public EnergyHandler(EntityScript entity) {
        _entity = entity;

        ActiveEnergy = SimulationScript.Instance.CoSh.MinActiveEnergy;

        _meatInStomach = 0f;
        _plantInStomach = 0f;
        _reproductionEnergy = 0f;
    }

    //returns amount that actually got stored
    public float ToStomach(float amount, bool isMeat) {
        float freeSpace = _entity.ScaledStomachContent - AmountInStomach;

        //if food amount doesnt fit, remove rest
        float rest = amount - freeSpace;
        if (rest >= 0f) amount -= rest;

        if (isMeat) {
            _meatInStomach += amount;
        } else {
            _plantInStomach += amount;
        }

        return amount;
    }

    public void RemoveActiveEnergy(float energy) {
        ActiveEnergy -= energy;
    }

    private void RecalculateEnergyConsumption() {
        float temp = _entity.ScaledBaseEnergyLoss;

        //take energy if attacking/producing pheromones
        if (_entity.Network.OutputValues[(int)ActionNeuron.ActionAttack] > SimulationScript.Instance.CoSh.ActionThreshold) temp += SimulationScript.Instance.CoSh.AttackEnergyConsumption;
        if (_entity.Network.OutputValues[(int)ActionNeuron.ActionPheromone] > SimulationScript.Instance.CoSh.ActionThreshold) temp += SimulationScript.Instance.CoSh.PheromoneEnergyConsumption;

        //movement to energy loss
        temp += _entity.AimedMovementDir.magnitude * SimulationScript.Instance.CoSh.MoveEnergyConsumption;

        //take energy for reproduction
        if (_entity.Network.OutputValues[(int)ActionNeuron.ActionReproduce] > SimulationScript.Instance.CoSh.ActionThreshold || IsReproducing) {
            IsReproducing = true;
            float reproductionFactor = _entity.ScaledEnergyToReproduce / SimulationScript.Instance.CoSh.TimeToReproduce;
            temp += reproductionFactor;
            ReproductionEnergy += reproductionFactor * Time.deltaTime;
        }

        //take energy for healing
        if (_entity.Health < _entity.ScaledMaxHealth && ActiveEnergy >= SimulationScript.Instance.CoSh.MinEnergyToHeal) {
            temp += SimulationScript.Instance.CoSh.HealingRate;
            _entity.Health += SimulationScript.Instance.CoSh.HealingRate * Time.deltaTime;
        }

        //multiply by factor
        temp *= SimulationScript.Instance.CoSh.EnergyConsumptionMultiplier;

        //smooth out
        EnergyConsumption = Mathf.SmoothDamp(EnergyConsumption, temp, ref _smoothingValue, 1.5f);
    }

    public void ConsumeEnergy() {
        if (DisableEnergyConsumption) return;

        RecalculateEnergyConsumption();
        ActiveEnergy -= EnergyConsumption * Time.deltaTime;
    }

    public void Digest() {
        float actualMeatDigestion = _meatInStomach < _entity.ScaledDigestionSpeed ? _meatInStomach : _entity.ScaledDigestionSpeed;
        float actualPlantDigestion = _plantInStomach < _entity.ScaledDigestionSpeed ? _plantInStomach : _entity.ScaledDigestionSpeed;

        actualMeatDigestion *= Time.deltaTime;
        actualPlantDigestion *= Time.deltaTime;

        _meatInStomach -= actualMeatDigestion;
        _plantInStomach -= actualPlantDigestion;

        actualMeatDigestion = _entity.Gene.Diet == EntityDiet.Carnivore ? actualMeatDigestion * SimulationScript.Instance.CoSh.MeatToEnergyFactor : 0f;
        actualPlantDigestion = _entity.Gene.Diet == EntityDiet.Herbivore ? actualPlantDigestion * SimulationScript.Instance.CoSh.PlantToEnergyFactor : 0f;

        ActiveEnergy = Mathf.Clamp(ActiveEnergy + actualMeatDigestion + actualPlantDigestion, 0f, _entity.ScaledMaxEnergy);
    }
}