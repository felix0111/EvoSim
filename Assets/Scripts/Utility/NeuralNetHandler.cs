using System;
using NeuraSuite.NeatExpanded;
using UnityEngine;

public class NeuralNetHandler {

    private readonly EntityScript _entity;
    private readonly Transform _entityTransform;
    private int[] _isCollidingLayers;

    public NeuralNetHandler(EntityScript entityScript) {
        _entity = entityScript;
        _entityTransform = entityScript.transform;
        _isCollidingLayers = new[] { LayerMask.NameToLayer("Food"), LayerMask.NameToLayer("Entity") };
    }

    public void GetInputNeuronValues(float[] inputValuesBuffer) {
        
        //reset input buffer
        for (int i = 0; i < inputValuesBuffer.Length; i++) {
            inputValuesBuffer[i] = 0f;
        }

        //vision
        float maxPossibleAngle = SimulationScript.Instance.CoSh.MaxVisionAngle + SimulationScript.Instance.CoSh.MaxFieldOfView / 2f;
        inputValuesBuffer[(int)InputNeuron.AnglePlants] = Vector2.SignedAngle(_entityTransform.up, _entity.VisionHandler.AvgPlantPosition - (Vector2)_entityTransform.position) / maxPossibleAngle;
        inputValuesBuffer[(int)InputNeuron.DistancePlants] = 1f - Mathf.InverseLerp(0f, _entity.Gene.ViewDistance, _entity.VisionHandler.AvgPlantDistance - _entity.Radius);

        inputValuesBuffer[(int)InputNeuron.AngleMeats] = Vector2.SignedAngle(_entityTransform.up, _entity.VisionHandler.AvgMeatPosition - (Vector2)_entityTransform.position) / maxPossibleAngle;
        inputValuesBuffer[(int)InputNeuron.DistanceMeats] = 1f - Mathf.InverseLerp(0f, _entity.Gene.ViewDistance, _entity.VisionHandler.AvgMeatDistance - _entity.Radius);

        inputValuesBuffer[(int)InputNeuron.AngleEntities] = Vector2.SignedAngle(_entityTransform.up, _entity.VisionHandler.AvgEntityPosition - (Vector2)_entityTransform.position) / maxPossibleAngle;
        inputValuesBuffer[(int)InputNeuron.DistanceEntities] = 1f - Mathf.InverseLerp(0f, _entity.Gene.ViewDistance, _entity.VisionHandler.AvgEntityDistance - _entity.Radius);
        inputValuesBuffer[(int)InputNeuron.SameSpeciesInView] = _entity.VisionHandler.SameSpeciesInView ? 1f : 0f;

        //self information
        inputValuesBuffer[(int)InputNeuron.StomachFullness] = Mathf.InverseLerp(0f, _entity.ScaledStomachContent, _entity.EnergyHandler.AmountInStomach);
        inputValuesBuffer[(int)InputNeuron.Energy] = Mathf.InverseLerp(0f, _entity.ScaledMaxEnergy, _entity.EnergyHandler.ActiveEnergy);
        inputValuesBuffer[(int)InputNeuron.Health] = Mathf.InverseLerp(0f, _entity.ScaledMaxHealth, _entity.Health);
        inputValuesBuffer[(int)InputNeuron.Age] = Mathf.InverseLerp(0f, SimulationScript.Instance.CoSh.MaxAge, _entity.Age);
        inputValuesBuffer[(int)InputNeuron.IsPregnant] = _entity.IsPregnant ? 1f : 0f;

        //pheromone information
        PheromoneScript ps = _entity.Smell();
        if (ps != null) {
            inputValuesBuffer[(int)InputNeuron.PheromoneAngle] = Vector2.SignedAngle(_entityTransform.up, ps.Direction) / 180f;
            inputValuesBuffer[(int)InputNeuron.PheromoneR] = ps.PheromoneColor.r;
            inputValuesBuffer[(int)InputNeuron.PheromoneG] = ps.PheromoneColor.g;
            inputValuesBuffer[(int)InputNeuron.PheromoneB] = ps.PheromoneColor.b;
        }
        
        //misc
        inputValuesBuffer[(int)InputNeuron.IsColliding] = _entity.CollisionWatcher.IsColliding(_isCollidingLayers) ? 1f : 0f;
        inputValuesBuffer[(int)InputNeuron.Bias] = 1f;
        inputValuesBuffer[(int)InputNeuron.Random] = UnityEngine.Random.Range(-1f, 1f);
        inputValuesBuffer[(int)InputNeuron.Oscillator] = Mathf.Sin(2 * Mathf.PI * _entity.PassedTime * _entity.Gene.OscillatorFrequency);
    }

    private Coroutine _buffer;
    public void ComputeOutputs(in float[] outputValuesBuffer) {

        //interpret moving neurons
        _entity.AimedMovementDir = outputValuesBuffer[(int)ActionNeuron.ActionMoveForward] * _entityTransform.up + outputValuesBuffer[(int)ActionNeuron.ActionMoveRight] * _entityTransform.right;

        //interpret vision
        _entity.VisionAngle = outputValuesBuffer[(int)ActionNeuron.ActionVisionAngle] * SimulationScript.Instance.CoSh.MaxVisionAngle;

        //rotation neuron
        _entity.AimedRotationDir = outputValuesBuffer[(int)ActionNeuron.ActionRotate];
        
        //pheromone neuron
        if (outputValuesBuffer[(int)ActionNeuron.ActionPheromone] >= ConstantSheet.ActionThreshold) {
            _entity.ProducePheromone();
        }

        //attack
        if (outputValuesBuffer[(int)ActionNeuron.ActionAttack] >= ConstantSheet.ActionThreshold) {
            _entity.Attack();
        }

        //eating
        if (outputValuesBuffer[(int)ActionNeuron.ActionEat] >= ConstantSheet.ActionThreshold) {
            _entity.Eat();
        }

        /*
        //digesting
        if (outputValuesBuffer[(int)ActionNeuron.ActionDigest] >= ConstantSheet.ActionThreshold) {
            _entity.EnergyHandler.Digest();
        }*/

        //want to reproduce
        if (outputValuesBuffer[(int)ActionNeuron.ActionReproduce] >= ConstantSheet.ActionThreshold && !_entity.IsPregnant && _entity.Age >= SimulationScript.Instance.CoSh.MinAgeToReproduce) {

            if (SimulationScript.Instance.CoSh.SexualReproduction) {
                //use when sexual reproduction
                var touching = _entity.CollisionWatcher.First<EntityScript>();
                if (touching == null || touching.IsPregnant) return;

                _entity.SexualPartner = new SerializableEntity(touching);
                _entity.IsPregnant = true;
            } else {
                //use when asexual reproduction
                _entity.SexualPartner = new SerializableEntity(_entity);
                _entity.IsPregnant = true;
            }
        }

    }

    public static ActivationFunction GetNeuronFunction(ActionNeuron an) {
        switch (an) {
            case ActionNeuron.ActionAttack: return ActivationFunction.SIGMOID;
            case ActionNeuron.ActionEat: return ActivationFunction.SIGMOID;
            case ActionNeuron.ActionMoveForward: return ActivationFunction.TANH;
            case ActionNeuron.ActionMoveRight: return ActivationFunction.TANH;
            case ActionNeuron.ActionRotate: return ActivationFunction.TANH;
            case ActionNeuron.ActionVisionAngle: return ActivationFunction.TANH;
            case ActionNeuron.ActionReproduce: return ActivationFunction.SIGMOID;
            //case ActionNeuron.ActionDigest: return ActivationFunction.SIGMOID;
            case ActionNeuron.ActionPheromone: return ActivationFunction.SIGMOID;
            default: return ActivationFunction.IDENTITY;
        }
    }

    public static int GetNeuronID(string name) {
        if (int.TryParse(name, out int id)) return id;
        if (Enum.TryParse(name, true, out ActionNeuron an)) return (int)an + Enum.GetValues(typeof(InputNeuron)).Length;
        if (Enum.TryParse(name, true, out InputNeuron input)) return (int)input;

        throw new Exception("Could not find ID for neuron: " + name);
    }

    public static string GetNeuronName(int id) {
        if (Enum.IsDefined(typeof(ActionNeuron), id - Enum.GetValues(typeof(InputNeuron)).Length)) return ((ActionNeuron)id - Enum.GetValues(typeof(InputNeuron)).Length).ToString();
        if (Enum.IsDefined(typeof(InputNeuron), id)) return ((InputNeuron)id).ToString();

        return id.ToString();
    }

    public static void GetNeuronTemplates(out Neuron[] ins, out Neuron[] outs) {
        int inputNeuronCount = Enum.GetValues(typeof(InputNeuron)).Length;
        int actionNeuronCount = Enum.GetValues(typeof(ActionNeuron)).Length;

        ins = new Neuron[inputNeuronCount];
        outs = new Neuron[actionNeuronCount];

        for (int i = 0; i < inputNeuronCount; i++) {
            ins[i] = new Neuron(i, ActivationFunction.IDENTITY, NeuronType.Input);
        }

        for (int i = 0; i < actionNeuronCount; i++) {
            outs[i] = new Neuron(i + inputNeuronCount, GetNeuronFunction((ActionNeuron)i), NeuronType.Action);
        }
    }
}

public enum InputNeuron {
    StomachFullness, Energy, Health, Age, IsColliding, IsPregnant, //self information
    AnglePlants, AngleMeats, AngleEntities, DistancePlants, DistanceMeats, DistanceEntities, SameSpeciesInView, //target information
    PheromoneR, PheromoneG, PheromoneB, PheromoneAngle, //pheromone
    Bias, Random, Oscillator //misc
}

public enum ActionNeuron {
    ActionAttack,
    ActionEat,
    ActionMoveForward,
    ActionMoveRight,
    ActionRotate,
    ActionReproduce,
    ActionVisionAngle,
    //ActionDigest,
    ActionPheromone //pheromone
}