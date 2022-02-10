using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVManager : MonoBehaviour
{

    public string todayQ = "";
    public string todayA_a = "";
    public string todayA_b = "";
    // Start is called before the first frame update
    void Start()
    {
        List<Dictionary<string,object>> todayQuestionData = CSVReader.Read("TodayQuestionList");

        
        //for(var i=0; i< 3; i++){
		//	Debug.Log("index " + (i).ToString() + " : " + todayQuestionData[i]["TodayQuestionNum"] + " " + todayQuestionData[i]["todayQ"] + " " + todayQuestionData[i]["todayA"]);
		//}
        todayQ = (string)todayQuestionData[0]["todayQ"];
		Debug.Log(todayQ);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
