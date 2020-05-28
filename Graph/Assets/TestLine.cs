using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLine : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();

    public GameObject inputNodePrefab;
    public GameObject weightPrefab;
    public GameObject outputNodePrefab;

    public GameObject parentCanvas;

    private List<GameObject> inputNodes = new List<GameObject>();
    private List<GameObject> weightNodes = new List<GameObject>();

    private LineRenderer lineRenderer;

    public int nodesCount;

    public void Start()
    {
        CreateInputNodes();
    }

    private void CreateInputNodes()
    {

        float centerY = 0;
        float partCenter = Screen.height / nodesCount / 2;
        float parts = Screen.height / nodesCount;

        for (int i = 0; i < nodesCount; i++)
        {
            inputNodes.Add(Instantiate(inputNodePrefab, parentCanvas.transform));

            centerY = partCenter + i * parts;

            inputNodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(100f, -centerY);
        }

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

        GameObject outputNode = Instantiate(outputNodePrefab, parentCanvas.transform);
        outputNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(600f, outputNode.GetComponent<RectTransform>().anchoredPosition.y);

        for (int i = 0; i < nodesCount; i++)
        {
            DrawLine(new Transform[] { weightNodes[i].transform, outputNode.transform });
        }
    }

    private void DrawLine(Transform[] transforms)
    {
        GameObject line = new GameObject();
        
        lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;

        lineRenderer.material.color = Color.white;

        Vector3[] pointsArray = new Vector3[2];

        for(int i = 0; i < 2; i++)
        {
            Vector3 pointPos = transforms[i].position;
            pointsArray[i] = new Vector3(pointPos.x, pointPos.y, 0f);
        }

        lineRenderer.SetPositions(pointsArray);
    }
}
