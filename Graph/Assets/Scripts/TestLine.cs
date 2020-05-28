﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TestLine : MonoBehaviour
{
    public GameObject inputNodePrefab;
    public GameObject weightPrefab;
    public GameObject outputNodePrefab;

    public GameObject parentCanvas;

    private List<GameObject> inputNodes = new List<GameObject>();
    private List<GameObject> weightNodes = new List<GameObject>();

    public Text biasValueText;
    public Text stepsValueText;
    public Text logInfo;

    private LineRenderer lineRenderer;

    public int nodesCount;

    private int[,] inputValues;
    private int[] expectedValues;
    private int[] actualValues;

    double bias;
    int error;
    int steps;

    Random random = new Random();
    public void Start()
    {
        float centerY = 0;
        float partCenter = Screen.height / nodesCount / 2;
        float parts = Screen.height / nodesCount;

        CreateInputNodes(centerY, partCenter, parts);
        CreateWeightNodes(centerY, partCenter, parts);
        CreateOutputNode();
        ResetValues();
    }

    public void ResetValues()
    {
        inputValues = new int[,] { { 1, 1, 1, 0 }, { 1, 1, 1, 1 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 1, 1 } };
        expectedValues = new int[] { 0, 0, 1, 1, 1, 0 };
        actualValues = new int[] { -1, -1, -1, -1, -1, -1 };
        bias = Math.Round((new Random().NextDouble() * 2 - 1), 2);
        biasValueText.text = bias.ToString();
        error = 0;
        steps = 0;
        stepsValueText.text = steps.ToString();
        logInfo.text = "Ready to learning";

        for (int i = 0; i < nodesCount; i++)
        {
            weightNodes[i].GetComponentInChildren<Text>().text = Math.Round((random.NextDouble() * 2 - 1), 2).ToString();
        }
    }

    #region Create Nodes
    private void CreateInputNodes(float centerY, float partCenter, float parts)
    {

        for (int i = 0; i < nodesCount; i++)
        {
            inputNodes.Add(Instantiate(inputNodePrefab, parentCanvas.transform));
            centerY = partCenter + i * parts;
            inputNodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -centerY);
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

        ResetValues();
    }

    private void CreateOutputNode()
    {
        GameObject outputNode = Instantiate(outputNodePrefab, parentCanvas.transform);
        outputNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(600f, outputNode.GetComponent<RectTransform>().anchoredPosition.y);

        for (int i = 0; i < nodesCount; i++)
        {
            DrawLine(new Transform[] { weightNodes[i].transform, outputNode.transform });
        }
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

    public void Learning()
    {
        Text weightText;
        do
        {
            for (int i = 0; i < inputValues.GetLength(0); i++)
            {
                actualValues[i] = ActivationFunctionBinaryStep(i);

                error = CalculateError(expectedValues[i], actualValues[i]);

                if (error != 0)
                {

                    for (int j = 0; j < inputValues.GetLength(1); j++)
                    {
                        weightText = weightNodes[j].GetComponentInChildren<Text>();
                        weightText.text = (inputValues[i, j] * error + double.Parse(weightText.text)).ToString();
                    }

                    bias += error;
                }

                biasValueText.text = bias.ToString();
            }
            steps++;
            stepsValueText.text = steps.ToString();

        } while (!CompareActualExpectedValues(actualValues, expectedValues));

        logInfo.text = $"Complete after {steps} steps";
    }



    public int ActivationFunctionBinaryStep(int i)
    {
        double outputValue = 0;

        for (int j = 0; j < inputValues.GetLength(1); j++)
        {
            outputValue += inputValues[i, j] * double.Parse(weightNodes[j].GetComponentInChildren<Text>().text);
        }

        return (outputValue + bias > 0) ? 1 : 0;
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

}
