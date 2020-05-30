using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamplesTable : MonoBehaviour
{
    private TestLine testLine;

    public GameObject cellPrefab;
    public GameObject cellPanelPrefab;
    public GameObject content;
    public GameObject examplesTitlePanel;

    void Start()
    {
        testLine = gameObject.GetComponent<TestLine>();
    }


    public void DrawExamplesTitle(int inputsCount, int examplesCount)
    {
        examplesTitlePanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(800 / inputsCount, 50);


        for (int i = 1; i <= inputsCount; i++)
        {
            GameObject cellTitle = Instantiate(cellPrefab, examplesTitlePanel.transform);

            if (i == inputsCount)
            {
                cellTitle.GetComponent<Text>().text = "DECISION";
            }
            else
            {
                cellTitle.GetComponent<Text>().text = $"X_{i}";
            }

        }

        for (int i = 0; i < examplesCount; i++)
        {
            GameObject cellPanel = Instantiate(cellPanelPrefab, content.transform);
            cellPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(800 / inputsCount, 50);

            for (int j = 0; j < inputsCount; j++)
            {
                if (j < inputsCount - 1)
                {
                    Instantiate(cellPrefab, cellPanel.transform).GetComponent<Text>().text = testLine.inputValues[i, j].ToString();
                }
                else
                {
                    Instantiate(cellPrefab, cellPanel.transform).GetComponent<Text>().text = testLine.expectedValues[i].ToString();
                }
            }

        }

    }

    public void SelectRow(int index)
    {
        if (index > 0)
        {
            content.transform.GetChild(index - 1).GetComponent<Image>().color = new Color32(149, 149, 149, 226);

        }

        content.transform.GetChild(index).GetComponent<Image>().color = Color.green;
    }

    public void UnselectAllRows()
    {
        for(int i = 0; i < testLine.inputValues.GetLength(0); i++)
        {
            content.transform.GetChild(i).GetComponent<Image>().color = new Color32(149, 149, 149, 226);
        }
    }
}
