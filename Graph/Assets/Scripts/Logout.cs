using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logout : MonoBehaviour
{
    public GameObject logPrefab;
    public Transform logsPanelContent;
    public ScrollRect logsPanelScroll;

    TestLine testLine;

    void Start()
    {
        testLine = gameObject.GetComponent<TestLine>();    
    }

    public void WriteLog(int stage)
    {
        GameObject log = Instantiate(logPrefab, logsPanelContent);
        Text logText = log.GetComponentInChildren<Text>();


        if(stage == 0)
        {
            logText.text = "Na wejścia podawane są wartości z przykładu";
        }
        else if(stage == 1)
        {
            logText.text = "Wartości wejścia neuronu mnożone są przez odpowiednie wagi";
        }
        else if(stage == 2)
        {
            logText.text = "Wartości otrzymane w poprzednim kroku sumują się";
        }
        else if(stage == 3)
        {
            logText.text = "Do otrzymanego wyniku dodawany jest BIAS oraz sprawdzane jest, jak suma odnosi się do wartości progowej";
        }
        else if(stage == 4)
        {
            if(testLine.ratio == -1)
            {
                logText.text = "Suma jest mniejsza od wartości progowej, więc na wyjściu pojawi się 0";
            }
            else if(testLine.ratio == 1)
            {
                logText.text = "Suma jest większa od wartości progowej, więc na wyjściu pojawi się 1";
            }
            else
            {
                logText.text = "Suma jest równa wartości progowej, więc na wyjściu pojawi się 0";
            }
            
        }
        else if(stage == 5)
        {
            if(testLine.error != 0)
            {
                logText.text = "Wartość wyjściowa nie jest zgodna z wartością treningową w tabeli. Wagi oraz BIAS ulegają zmianie";
            }
            else
            {
                logText.text = "Wartość wyjściowa jest zgodna z wartością treningową w tabeli. Wagi oraz BIAS nie ulegają zmianie";
            }
        }

        Canvas.ForceUpdateCanvases();
        logsPanelScroll.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    public void ClearLogs()
    {
        for(int i = 0; i < logsPanelContent.childCount; i++)
        {
            Destroy(logsPanelContent.GetChild(i).gameObject);
            //Debug.Log(i);
        }
    }
}
