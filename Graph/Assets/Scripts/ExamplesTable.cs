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
        
    }


    public void DrawExamplesTitle(int inputsCount, int examplesCount)
    {
        testLine = gameObject.GetComponent<TestLine>();
        float weight = examplesTitlePanel.GetComponent<RectTransform>().sizeDelta.x;
        examplesTitlePanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(weight / inputsCount, 50);


        for (int i = 1; i <= inputsCount; i++)
        {
            GameObject cellTitle = Instantiate(cellPrefab, examplesTitlePanel.transform);

            if (i == inputsCount)
            {
                cellTitle.GetComponent<Text>().text = "DEC.";
            }
            else
            {
                cellTitle.GetComponent<Text>().text = $"X_{i}";
            }

        }

        for (int i = 0; i < examplesCount; i++)
        {
            GameObject cellPanel = Instantiate(cellPanelPrefab, content.transform);
            cellPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(weight / inputsCount, 50);

            for (int j = 0; j < inputsCount; j++)
            {
                GameObject cell = Instantiate(cellPrefab, cellPanel.transform);
                if (j < inputsCount - 1)
                {
                    
                    cell.GetComponent<Text>().text = testLine.inputValues[i, j].ToString();
                }
                else
                {
                    cell.GetComponent<Text>().text = testLine.expectedValues[i].ToString();
                }
            }

        }

    }

    public void SelectRow(int index)
    {
        if (index > 0)
        {
            content.transform.GetChild(index - 1).GetComponent<Image>().color = Color.white;

            for (int i = 0; i < content.transform.GetChild(index - 1).transform.childCount; i++)
            {
                content.transform.GetChild(index - 1).transform.GetChild(i).GetComponent<Text>().fontStyle = FontStyle.Normal;
            }
        }

        content.transform.GetChild(index).GetComponent<Image>().color = Color.green;
        
        for(int i = 0; i < content.transform.GetChild(index).transform.childCount; i++)
        {
            content.transform.GetChild(index).transform.GetChild(i).GetComponent<Text>().fontStyle = FontStyle.Bold;
        }

    }

    public void UnselectAllRows()
    {
        for(int i = 0; i < testLine.inputValues.GetLength(0); i++)
        {
            content.transform.GetChild(i).GetComponent<Image>().color = Color.white;

            for (int j = 0; j < content.transform.GetChild(i).transform.childCount; j++)
            {
                content.transform.GetChild(i).transform.GetChild(j).GetComponent<Text>().fontStyle = FontStyle.Normal;
            }
        }
    }
}
