using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TestLine : MonoBehaviour
{

    private ExamplesTable examplesTable;

    public GameObject inputNodePrefab;
    public GameObject weightPrefab;
    public GameObject outputNodePrefab;
    public GameObject signalPrefab;

    public GameObject parentCanvas;

    private List<GameObject> inputNodes = new List<GameObject>();
    private List<GameObject> weightNodes = new List<GameObject>();
    private GameObject sumNode;
    private GameObject thresholdNode;
    private GameObject outputNode;
    private List<GameObject> signals = new List<GameObject>();


    public Text biasValueText;
    public Text stepsValueText;
    public Text logInfo;

    private LineRenderer lineRenderer;

    public int nodesCount;
    public float threshold;

    [HideInInspector]
    public int[,] inputValues;
    [HideInInspector]
    public int[] expectedValues;
    private int[] actualValues;
    private double[] weights;
    private double outputValue;

    double bias;
    int error;
    int steps;

    public int stage;

    Random random = new Random();
    public void Start()
    {
        examplesTable = gameObject.GetComponent<ExamplesTable>();

        float centerY = 0;
        float partCenter = Screen.height / nodesCount / 2;
        float parts = Screen.height / nodesCount;

        CreateInputNodes(centerY, partCenter, parts);
        CreateWeightNodes(centerY, partCenter, parts);
        CreateSumNode();
        CreateThresholdNode();
        CreateOutputNode();
        ResetValues();
        examplesTable.DrawExamplesTitle(inputValues.GetLength(1) + 1, inputValues.GetLength(0));
    }

    public void Update()
    {
        if (stage == 1)
        {

            for(int i = 0; i < nodesCount; i++)
            {
                signals[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(signals[i].GetComponent<RectTransform>().anchoredPosition,
                    weightNodes[i].GetComponent<RectTransform>().anchoredPosition, 3 * Time.deltaTime);
            }

        }
        else if (stage == 2)
        {
            for (int i = 0; i < nodesCount; i++)
            {
                signals[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(signals[i].GetComponent<RectTransform>().anchoredPosition,
                    sumNode.GetComponent<RectTransform>().anchoredPosition, 3 * Time.deltaTime);
            }
        }
        else if (stage == 3)
        {
            signals[0].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(signals[0].GetComponent<RectTransform>().anchoredPosition,
                    thresholdNode.GetComponent<RectTransform>().anchoredPosition, 3 * Time.deltaTime);
        }
        else if(stage == 4)
        {
            signals[0].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(signals[0].GetComponent<RectTransform>().anchoredPosition,
                    outputNode.GetComponent<RectTransform>().anchoredPosition, 3 * Time.deltaTime);
        }
    }

    public void ResetValues()
    {

        inputValues = new int[,] { { 1, 1, 1, 0 }, { 1, 1, 1, 1 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 1 } };
        expectedValues = new int[] { 0, 0, 1, 1, 1, 0 };
        actualValues = new int[] { -1, -1, -1, -1, -1, -1 };
        weights = new double[nodesCount];
        bias = Math.Round((new Random().NextDouble() * 2 - 1), 2);
        biasValueText.text = bias.ToString();
        error = 0;
        steps = 0;
        stepsValueText.text = steps.ToString();
        logInfo.text = "Ready to learning";

        for (int i = 0; i < nodesCount; i++)
        {
            weights[i] = Math.Round((random.NextDouble() * 2 - 1), 2);
            weightNodes[i].GetComponentInChildren<Text>().text = weights[i].ToString();
        }

        HidePanels();
        ResetSignalsPositions();
        
    }
    private void ResetSignalsPositions()
    {
        stage = 0;

        for (int i = 0; i < nodesCount; i++)
        {
            signals[i].GetComponent<RectTransform>().anchoredPosition = inputNodes[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    #region Create Nodes
    private void CreateInputNodes(float centerY, float partCenter, float parts)
    {

        for (int i = 0; i < nodesCount; i++)
        {
            inputNodes.Add(Instantiate(inputNodePrefab, parentCanvas.transform));
            signals.Add(Instantiate(signalPrefab, parentCanvas.transform));
            centerY = partCenter + i * parts;
            inputNodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -centerY);
            inputNodes[i].transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            signals[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -centerY);
            signals[i].transform.SetSiblingIndex(i);

        }
 
    }

    private void CreateWeightNodes(float centerY, float partCenter, float parts)
    {
        partCenter = (inputNodes[0].GetComponent<RectTransform>().anchoredPosition.y -
            inputNodes[inputNodes.Count - 1].GetComponent<RectTransform>().anchoredPosition.y) / nodesCount / 2;

        parts = (inputNodes[0].GetComponent<RectTransform>().anchoredPosition.y -
            inputNodes[inputNodes.Count - 1].GetComponent<RectTransform>().anchoredPosition.y) / nodesCount;

        for (int i = 0; i < nodesCount; i++)
        {
            weightNodes.Add(Instantiate(weightPrefab, parentCanvas.transform));

            centerY = partCenter + i * parts - inputNodes[0].GetComponent<RectTransform>().anchoredPosition.y;

            weightNodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(300f, -centerY);

            DrawLine(new Transform[] { inputNodes[i].transform, weightNodes[i].transform });
        }

        
    }

    private void CreateSumNode()
    {
        sumNode = Instantiate(outputNodePrefab, parentCanvas.transform);
        sumNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(600f, sumNode.GetComponent<RectTransform>().anchoredPosition.y);

        for (int i = 0; i < nodesCount; i++)
        {
            DrawLine(new Transform[] { weightNodes[i].transform, sumNode.transform });
        }
    }

    private void CreateThresholdNode()
    {
        thresholdNode = Instantiate(weightPrefab, parentCanvas.transform);
        thresholdNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(800f, thresholdNode.GetComponent<RectTransform>().anchoredPosition.y);

        thresholdNode.transform.GetChild(0).GetComponent<Text>().text = threshold.ToString();

        DrawLine(new Transform[] { sumNode.transform, thresholdNode.transform });
    }

    private void CreateOutputNode()
    {
        outputNode = Instantiate(outputNodePrefab, parentCanvas.transform);
        outputNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000f, outputNode.GetComponent<RectTransform>().anchoredPosition.y);

        outputNode.transform.GetChild(0).GetComponent<Text>().text = string.Empty;

        DrawLine(new Transform[] { thresholdNode.transform, outputNode.transform });
    }

    #endregion

    #region Draw Lines
    private void DrawLine(Transform[] transforms)
    {
        GameObject line = new GameObject();

        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;

        lineRenderer.material.color = Color.white;

        Vector3[] pointsArray = new Vector3[2];

        for (int i = 0; i < 2; i++)
        {
            Vector3 pointPos = transforms[i].position;
            pointsArray[i] = new Vector3(pointPos.x, pointPos.y, 0f);
        }

        lineRenderer.SetPositions(pointsArray);
    }
    #endregion

    #region Lerning

    public void Learning(Button button)
    {
        Text buttonText = button.transform.GetChild(0).GetComponent<Text>();

        if(buttonText.text == "Start")
        {
            buttonText.text = "Stop";
            StartCoroutine(InfoAnimate());
            StartCoroutine(LearningEnum());
        }
        else
        {
            buttonText.text = "Start";
            StopLearning();
        }

    }

    private void StopLearning()
    {
        StopAllCoroutines();
        ResetValues();
        examplesTable.UnselectAllRows();
    }

    private IEnumerator LearningEnum()
    {
        Text weightText;

        do
        {
            steps++;
            stepsValueText.text = steps.ToString();
            for (int i = 0; i < inputValues.GetLength(0); i++)
            {
                HidePanels();
                ClearNodes();
                examplesTable.SelectRow(i);
                ShowInputs(i);
                stage = 1;
                yield return new WaitForSecondsRealtime(1f);

                
                actualValues[i] = ActivationFunctionBinaryStep(i);
                yield return new WaitForSecondsRealtime(2f);

                stage = 2;
                yield return new WaitForSecondsRealtime(1f);

                ShowSumPanel(i);
                
                yield return new WaitForSecondsRealtime(2f);
                stage = 3;
                
                yield return new WaitForSecondsRealtime(1f);
                ShowThresholdPanel();
                yield return new WaitForSecondsRealtime(2f);

                stage = 4;

                yield return new WaitForSecondsRealtime(1f);

                Text outputValueText = outputNode.transform.GetChild(0).GetComponent<Text>();

                if (actualValues[i] == expectedValues[i])
                {
                    outputValueText.color = Color.green;
                }
                else
                {
                    outputValueText.color = Color.red;
                }

                outputValueText.text = actualValues[i].ToString();

                error = CalculateError(expectedValues[i], actualValues[i]);

                if (error != 0)
                {

                    for (int j = 0; j < inputValues.GetLength(1); j++)
                    {
                        weights[j] = inputValues[i, j] * error + weights[j];

                        weightText = weightNodes[j].GetComponentInChildren<Text>();
                        weightText.text = weights[j].ToString();
                    }

                    bias += error;
                }

                biasValueText.text = bias.ToString();
                ResetSignalsPositions();
                yield return new WaitForSecondsRealtime(2f);
            }
            
            
            examplesTable.UnselectAllRows();

        } while (!CompareActualExpectedValues(actualValues, expectedValues));

        logInfo.text = $"Completed after {steps} steps";
        StopAllCoroutines();
        
    }

    public int ActivationFunctionBinaryStep(int i)
    {
        outputValue = 0;

        for (int j = 0; j < inputValues.GetLength(1); j++)
        {
            outputValue += inputValues[i, j] * weights[j];
            ShowWeightsPanels(i, j);
        }

        return (outputValue + bias > threshold) ? 1 : 0;
    }

    public int CalculateError(int expectedValue, int actualValue)
    {
        return expectedValue - actualValue;
    }

    public bool CompareActualExpectedValues(int[] actualValues, int[] expectedValues)
    {
        for (int i = 0; i < actualValues.Length; i++)
        {
            if (actualValues[i] != expectedValues[i])
            {
                return false;
            }
        }

        return true;
    }


    #endregion

    private void ShowInputs(int i)
    {
        for(int j = 0; j < nodesCount; j++)
        {
            inputNodes[j].transform.GetChild(0).GetComponent<Text>().text = inputValues[i, j].ToString();
        }
    }

    private void ShowWeightsPanels(int i, int j)
    {
        GameObject panel = weightNodes[j].transform.GetChild(1).gameObject;
        panel.transform.GetChild(0).GetComponent<Text>().text = $"{ inputValues[i, j] } x { weights[j] }";
        panel.SetActive(true);
    }

    private void ShowSumPanel(int j)
    {
        GameObject panel = sumNode.transform.GetChild(1).gameObject;
        Text panelText = panel.transform.GetChild(0).GetComponent<Text>();
        panelText.text = string.Empty;

        for (int i = 0; i < inputNodes.Count; i++)
        {
            double output = inputValues[j, i] * weights[i];

            panelText.text += output < 0 ? $"({ output })" : $"{ output }";

            if (i < inputNodes.Count - 1)
            {
                panelText.text += " + ";
            }
        }

        sumNode.transform.GetChild(0).GetComponent<Text>().text = outputValue.ToString();
        sumNode.transform.GetChild(0).GetComponent<Text>().fontSize = 23;

        panel.SetActive(true);
    }

    private void ShowThresholdPanel()
    {
        GameObject panel = thresholdNode.transform.GetChild(2).gameObject;
        Text panelText = panel.transform.GetChild(0).GetComponent<Text>();

        if(outputValue + bias > threshold)
        {
            panelText.text = $"{outputValue} + {bias} > {threshold}";
        }
        else if(outputValue + bias < threshold)
        {
            panelText.text = $"{outputValue} + {bias} < {threshold}";
        }
        else
        {
            panelText.text = $"{outputValue} + {bias} = {threshold}";
        }


        panel.SetActive(true);
    }

    private void HidePanels()
    {
        for (int i = 0; i < nodesCount; i++)
        {
            weightNodes[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        sumNode.transform.GetChild(1).gameObject.SetActive(false);
        thresholdNode.transform.GetChild(2).gameObject.SetActive(false);
        sumNode.transform.GetChild(0).GetComponent<Text>().text = "Σ";
        sumNode.transform.GetChild(0).GetComponent<Text>().fontSize = 25;
    }

    private void ClearNodes()
    {
        outputNode.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
    }
    private IEnumerator InfoAnimate()
    {
        logInfo.text = "Learning.";
        yield return new WaitForSecondsRealtime(1f);
        logInfo.text = "Learning..";
        yield return new WaitForSecondsRealtime(1f);
        logInfo.text = "Learning...";
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(InfoAnimate());
    }

}
