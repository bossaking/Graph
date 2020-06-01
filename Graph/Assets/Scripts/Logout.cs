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
            logText.text = "Na wejścia są podawane wartości z przykładu";
        }
        else if(stage == 1)
        {
            logText.text = "Wartość wejścia neuronu jest mnożona przez odpowiednią wagę";
        }
        else if(stage == 2)
        {
            logText.text = "Wartości, otrzymane w poprzednim kroku się sumują";
        }
        else if(stage == 3)
        {
            logText.text = "Do otrzymanego wyniku dodaję się BIAS i sprawdza się, jak suma odnosi się do wartości prógowej";
        }
        else
        {
            if(testLine.ratio == -1)
            {
                logText.text = "Suma jest mniejsza od wartości prógowej, więc na wyjście idzie 0";
            }
            else if(testLine.ratio == 1)
            {
                logText.text = "Suma jest większa od wartości prógowej, więc na wyjście idzie 1";
            }
            else
            {
                logText.text = "Suma równa się wartości prógowej, więc na wyjście idzie 0";
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
