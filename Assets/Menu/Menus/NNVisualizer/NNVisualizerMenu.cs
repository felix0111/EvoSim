using System;
using System.Collections.Generic;
using System.Linq;
using NeuraSuite.NeatExpanded;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class NNVisualizerMenu : MonoBehaviour {

    //ui stuff
    public GameObject NeuronPrefab, ConnectionPrefab, NeuronPlaceholder, ConnectionPlaceholder;
    public TMP_InputField WeightInputField;
    public TMP_Dropdown FunctionDropdown;
    private RectTransform _rectTransform;
    private Dictionary<int, NeuronScript> _neuronDict, _recurrentDict;
    private Dictionary<int, ConnectionScript> _connectionDict;

    private EntityScript _entityScript;

    //neural net modifying
    private int _firstNeuron, _secondNeuron;
    private ConnectionScript _dragConnection;
    public bool IsDragging => _dragConnection != null;

    void Awake() {
        _rectTransform = GetComponent<RectTransform>();

        //fill dropdown
        FunctionDropdown.AddOptions(Enum.GetNames(typeof(ActivationFunction)).ToList());
        FunctionDropdown.SetValueWithoutNotify(0);
        FunctionDropdown.RefreshShownValue();
    }

    void Update() {
        UpdateTexts();
        if(_dragConnection != null) _dragConnection.UpdateConnection(default, _neuronDict[_firstNeuron].transform.position, Input.mousePosition, Color.black); 
    }

    void OnDisable() {
        InterruptDragging();
    }

    public void OpenMenu(EntityScript entity) {
        gameObject.SetActive(true);

        _entityScript = entity;

        InterruptDragging();
        CreateElements();
    }

    public void UpdateEntity(EntityScript entity) {
        _entityScript = entity;

        InterruptDragging();
        CreateElements();
    }

    private void CreateElements() {
        //cleanup
        foreach (Transform tr in NeuronPlaceholder.transform) Destroy(tr.gameObject);
        foreach (Transform tr in ConnectionPlaceholder.transform) Destroy(tr.gameObject);
        _neuronDict = new Dictionary<int, NeuronScript>();
        _recurrentDict = new Dictionary<int, NeuronScript>();
        _connectionDict = new Dictionary<int, ConnectionScript>();


        LayerStructure ls = new (_entityScript.Network);
        int layerCount = ls.LayerArray.Count;

        if (_entityScript.Network.RecurrentConnections.Count > 0) layerCount++;

        float distBetweenLayer = _rectTransform.rect.width / (layerCount + 1);

        //add visual neurons
        for (int i = 0; i < ls.LayerArray.Count; i++) {
            float distBetweenNeurons = _rectTransform.rect.height / (ls.LayerArray[i].Count + 1);

            for (int j = 0; j < ls.LayerArray[i].Count; j++) {
                CreateNeuronVisual(new Vector3(distBetweenLayer * (i + 1), distBetweenNeurons * (j + 1), 0f), ls.LayerArray[i][j], false);
            }
        }

        //add recurrent visualization
        float distBetweenRecurrent = _rectTransform.rect.height / (_entityScript.Network.RecurrentConnections.Count + 1);
        foreach (Connection connection in _entityScript.Network.RecurrentConnections.Values) {
            //if recurrent neuron target already exists, skip
            if (_recurrentDict.ContainsKey(connection.TargetID)) continue;
            CreateNeuronVisual(new Vector3(distBetweenLayer * layerCount, distBetweenRecurrent * (_recurrentDict.Count + 1), 0f), connection.TargetID, true);
        }

        //add visual connections
        foreach (var connectionPair in _entityScript.Network.Connections.Values) {
            CreateConnectionVisual(connectionPair, false);
        }
        foreach (var connectionPair in _entityScript.Network.RecurrentConnections.Values) {
            CreateConnectionVisual(connectionPair, true);
        }
    }

    private NeuronScript CreateNeuronVisual(Vector3 pos, int neuronID, bool isRecurrent) {
        GameObject neuronVisual = Instantiate(NeuronPrefab, pos, Quaternion.identity, NeuronPlaceholder.transform);
        neuronVisual.GetComponent<RectTransform>().anchoredPosition = pos;
        neuronVisual.transform.localScale = Vector3.one * 0.3f;

        NeuronScript ns = neuronVisual.GetComponent<NeuronScript>();
        ns.NeuronID = neuronID;

        if (isRecurrent) {
            _recurrentDict.Add(neuronID, ns);
            ns.GetComponent<UICornerCut>().color = Color.cyan;
        } else {
            _neuronDict.Add(neuronID, ns);
        }

        return ns;
    }

    private ConnectionScript CreateConnectionVisual(Connection connection, bool isRecurrent) {
        _neuronDict.TryGetValue(connection.SourceID, out NeuronScript from);

        NeuronScript to;
        if (isRecurrent) {
            _recurrentDict.TryGetValue(connection.TargetID, out to);
        } else {
            _neuronDict.TryGetValue(connection.TargetID, out to);
        }

        float weight = connection.Weight;

        Color color;
        if (weight == 0) {
            color = Color.black;
        } else if (weight < 0) {
            color = Color.blue;
        } else {
            color = Color.green;
        }

        if (!connection.Activated) color = Color.black;

        GameObject connectionVisual = Instantiate(ConnectionPrefab, ConnectionPlaceholder.transform);

        ConnectionScript cs = connectionVisual.GetComponent<ConnectionScript>();
        cs.UpdateConnection(connection, from.transform.position, to.transform.position, color);

        _connectionDict.Add(connection.InnovationID, cs);

        return cs;
    }

    private void UpdateTexts() {
        foreach (var neuronVisualPosition in _neuronDict) {
            Neuron n = _entityScript.Network.Neurons[neuronVisualPosition.Key];

            neuronVisualPosition.Value.GetComponentInChildren<TMP_Text>().text = MathF.Round(n.Value, 3) + "<br>" + NeuralNetHandler.GetNeuronName(n.ID);

            if (n.Type != NeuronType.Input) neuronVisualPosition.Value.GetComponentInChildren<TMP_Text>().text += "<br>" + Enum.GetName(typeof(ActivationFunction), n.Function);
        }

        foreach (var recurrentVisualPosition in _recurrentDict) {
            Neuron n = _entityScript.Network.Neurons[recurrentVisualPosition.Key];

            recurrentVisualPosition.Value.GetComponentInChildren<TMP_Text>().text = MathF.Round(n.Value, 3) + "<br>" + NeuralNetHandler.GetNeuronName(n.ID);
        }
    }

    public void OnClickNeuron(PointerEventData data, int neuronID) {
        if (data.button == PointerEventData.InputButton.Left) {
            if (_firstNeuron == -1) {
                _firstNeuron = neuronID;

                //create drag connection
                _dragConnection = Instantiate(ConnectionPrefab, ConnectionPlaceholder.transform).GetComponent<ConnectionScript>();
                _dragConnection.GetComponent<Image>().raycastTarget = false;
                _dragConnection.UpdateConnection(default, _neuronDict[_firstNeuron].transform.position, Input.mousePosition, Color.black);
            } else {
                _secondNeuron = neuronID;

                int newInno = SimulationScript.Instance.Neat.NewInnovation(_firstNeuron, _secondNeuron);
                Connection c = _entityScript.Network.AddConnection(newInno, _firstNeuron, _secondNeuron, 1f);

                if (c.InnovationID == -1) {
                    _entityScript.Network.AddNeuron(SimulationScript.Instance.Neat, newInno, default);
                }

                //update neural network
                SimulationScript.Instance.Neat.SpeciateSingle(_entityScript.Network);

                InterruptDragging();

                //update menu
                UpdateEntity(_entityScript);
            }
        } else if (data.button == PointerEventData.InputButton.Right && !IsDragging) {
            _entityScript.Network.Neurons[neuronID].Function = (ActivationFunction)FunctionDropdown.value;

            //update neural network
            SimulationScript.Instance.Neat.SpeciateSingle(_entityScript.Network);

            //update menu
            UpdateEntity(_entityScript);
        }

        
    }

    public void OnClickConnection(PointerEventData data, Connection connection) {
        if (data.button == PointerEventData.InputButton.Left) {
            _entityScript.Network.Connections[connection.InnovationID] = new Connection(connection.InnovationID, connection.SourceID, connection.TargetID, float.Parse(WeightInputField.text));
        } else if (data.button == PointerEventData.InputButton.Right) {
            _entityScript.Network.RemoveConnection(connection.InnovationID);
        }

        //update neural network
        SimulationScript.Instance.Neat.SpeciateSingle(_entityScript.Network);

        //update menu
        UpdateEntity(_entityScript);
    }

    public void InterruptDragging() {
        if (_dragConnection != null) Destroy(_dragConnection.gameObject);
        _dragConnection = null;
        _firstNeuron = -1;
        _secondNeuron = -1;
    }

    public void OnSaveButton() {
        StartCoroutine(Serializer.SaveEntity(new SerializableEntity(_entityScript)));
    }

    public void OnLoadButton() {
        StartCoroutine(Serializer.LoadEntity(o => {
            _entityScript.InitEntity(o);
            SimulationScript.Instance.MenuManager.EntityMenu.UpdateEntity(_entityScript);
        }));
    }

    public void OnMutateButton() {
        _entityScript.Mutate(1);
        UpdateEntity(_entityScript);
    }
}
