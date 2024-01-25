using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EasyNNFramework.NEAT;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpeciesMenu : MonoBehaviour {

    //ui
    public RectTransform ChartRect;
    public GameObject ChartQuadPrefab;

    //chart
    public ChartData _chartData = new ChartData();
    private Dictionary<int, Color> _colorMap = new Dictionary<int, Color>(); //colormap for differentiating species

    public void LogSpecies(int timeStep) {

        for (int i = 0; i < SimulationScript.Instance.Neat.Species.Count; i++) {
            var species = SimulationScript.Instance.Neat.Species.ElementAt(i);
            float speciesPercentage = (float)species.Value.AllNetworks.Count / 80f;

            string dataInfo = "Species " + species.Key + "<br>Amount: " + species.Value.AllNetworks.Count;
            _chartData.AddDataPointAtStep(timeStep, new ChartDataPoint(species.Key, speciesPercentage, dataInfo));

        }
    }

    public void OpenMenu() {
        gameObject.SetActive(true);
        UpdateMenu();
    }

    public void UpdateMenu() {
        //cleanup
        foreach (RectTransform transform in ChartRect) {
            Destroy(transform.gameObject);
        }

        //x-axis
        int startingTimestep = _chartData.DataPointsPerStep.Keys.OrderBy(o => o).First();
        int endTimeStep = _chartData.DataPointsPerStep.Keys.OrderBy(o => o).Last();
        float timeStepDistance = ChartRect.rect.width / (endTimeStep - startingTimestep);
        if (endTimeStep <= 1) return;


        for (int currentStep = startingTimestep, count = 0; currentStep < endTimeStep; currentStep++, count++) {

            float lastPercentage = 0f;
            float lastNextStepPercentage = 0f;

            Vector2[] dataPointVertex = new Vector2[4];

            //foreach datapoint at current timestep, add quad from current to next timestep
            foreach (ChartDataPoint currentPoint in _chartData.DataPointsPerStep[currentStep]) {
                //add to colormap if not existing
                if (!_colorMap.ContainsKey(currentPoint.DataID)) _colorMap.Add(currentPoint.DataID, Random.ColorHSV());

                //if next point exists, draw normal quad
                if (_chartData.TryGetDataFromStep(currentStep + 1, currentPoint.DataID, out var nextPoint)) {
                    dataPointVertex[0] = new Vector2(timeStepDistance * count, (currentPoint.Percentage + lastPercentage) * ChartRect.rect.height);
                    dataPointVertex[1] = new Vector2(timeStepDistance * count, lastPercentage * ChartRect.rect.height);
                    dataPointVertex[2] = new Vector2(timeStepDistance * (count + 1), lastNextStepPercentage * ChartRect.rect.height);
                    dataPointVertex[3] = new Vector2(timeStepDistance * (count + 1), (nextPoint.Percentage + lastNextStepPercentage) * ChartRect.rect.height);

                    lastPercentage += currentPoint.Percentage;
                    lastNextStepPercentage += nextPoint.Percentage;

                } else { //if next data point does not exist, merge quad
                    dataPointVertex[0] = new Vector2(timeStepDistance * count, (currentPoint.Percentage + lastPercentage) * ChartRect.rect.height);
                    dataPointVertex[1] = new Vector2(timeStepDistance * count, lastPercentage * ChartRect.rect.height);
                    dataPointVertex[2] = new Vector2(timeStepDistance * (count + 1), lastNextStepPercentage * ChartRect.rect.height);
                    dataPointVertex[3] = new Vector2(timeStepDistance * (count + 1), lastNextStepPercentage * ChartRect.rect.height);

                    lastPercentage += currentPoint.Percentage;
                    lastNextStepPercentage += 0f;
                }

                CreateCustomQuad(dataPointVertex, _colorMap[currentPoint.DataID], currentPoint);
            }

            //add transition quad for new data points in next timestep
            var newDataPoints = _chartData.GetNewDataAtStep(currentStep + 1);
            foreach (ChartDataPoint newDataPoint in newDataPoints) {
                //it can happen that last update call already checked the new upcoming data
                if (!_colorMap.ContainsKey(newDataPoint.DataID)) _colorMap.Add(newDataPoint.DataID, Random.ColorHSV());

                dataPointVertex[0] = new Vector2(timeStepDistance * count, lastPercentage * ChartRect.rect.height);
                dataPointVertex[1] = new Vector2(timeStepDistance * count, lastPercentage * ChartRect.rect.height);
                dataPointVertex[2] = new Vector2(timeStepDistance * (count + 1), lastNextStepPercentage * ChartRect.rect.height);
                dataPointVertex[3] = new Vector2(timeStepDistance * (count + 1), (newDataPoint.Percentage + lastNextStepPercentage) * ChartRect.rect.height);

                lastPercentage += 0f;
                lastNextStepPercentage += newDataPoint.Percentage;

                CreateCustomQuad(dataPointVertex, _colorMap[newDataPoint.DataID], newDataPoint);
            }
        }
    }

    private UICustomQuadScript CreateCustomQuad(Vector2[] points, Color color, ChartDataPoint point) {
        GameObject quad = Instantiate(ChartQuadPrefab, ChartRect.transform);
        Array.Copy(points, quad.GetComponent<UICustomQuadScript>().VertexPositions, 4);
        quad.GetComponent<UICustomQuadScript>().color = color;

        var tooltip = quad.GetComponent<HoverScript>();
        tooltip.TooltipText = point.AdditionalInfo;

        return quad.GetComponent<UICustomQuadScript>();
    }
}

public class ChartData {

    public Dictionary<int, List<ChartDataPoint>> DataPointsPerStep = new();

    public void AddDataPointAtStep(int step, ChartDataPoint point) {

        if (!DataPointsPerStep.ContainsKey(step)) {
            DataPointsPerStep.Add(step, new List<ChartDataPoint>());
            if (DataPointsPerStep.Count > 100) DataPointsPerStep.Remove(DataPointsPerStep.Keys.OrderBy(o => o).First());
        }

        if (DataPointsPerStep[step].Any(o => o.DataID == point.DataID)) throw new Exception("Data point with ID already exists at timestep!");

        DataPointsPerStep[step].Add(point);
        DataPointsPerStep[step] = DataPointsPerStep[step].OrderBy(o => o.DataID).ToList();
    }

    public bool TryGetDataFromStep(int step, int dataID, out ChartDataPoint dataPoint) {
        if (DataPointsPerStep.TryGetValue(step, out List<ChartDataPoint> points)) {
            dataPoint = points.FirstOrDefault(o => o.DataID == dataID);
            return points.Any(o => o.DataID == dataID);
        } else {
            throw new Exception("There is no step " + step);
        }
    }

    public List<ChartDataPoint> GetNewDataAtStep(int step) {
        if(step == 0) return new List<ChartDataPoint>();

        List<ChartDataPoint> newData = new List<ChartDataPoint>();

        foreach (ChartDataPoint point in DataPointsPerStep[step]) {
            if(!TryGetDataFromStep(step - 1, point.DataID, out ChartDataPoint _)) newData.Add(point);
        }

        return newData;
    }

}

public struct ChartDataPoint {

    public float Percentage;
    public int DataID;
    public string AdditionalInfo;

    public ChartDataPoint(int dataID, float percentage, string info) {
        Percentage = percentage;
        DataID = dataID;
        AdditionalInfo = info;
    }

}