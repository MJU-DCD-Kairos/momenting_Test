using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class csvReader : MonoBehaviour
{

    //csv파일을 외부에서 인스펙터에서 직접 참조할 수 있도록 생성
    public TextAsset csvfile;

    //오늘의 질문을 넣어줄 UI텍스트 오브젝트를 인스펙터로 참조받기위한 선언
    public Text Question;
    public Text answerA;
    public Text answerB;

    //CSV파일의 행 개수를 인스펙터상에서 입력하기 위한 퍼블릭 변수 선언
    public int tableSize;
    
    //각 값을 보유할 클래스 생성
    [System.Serializable]
    public class TodayQuestion
    {
        public int Num;
        public string QuestionT;
        public string answerA;
        public string answerB;
    }

    //리스트를 보유할 클래스 생성
    [System.Serializable]
    public class TQList
    {
        public TodayQuestion[] TQL; 
    }

    //각 클래스를 기반으로 배열 변수 생성
    public TQList todayQuestionList = new TQList();

    
    // Start is called before the first frame update
    void Start()
    {
        RedaCSV();
        PrintQuestionT();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //본격적으로 CSV파일을 파싱해서 배열정보로 생성하는 함수 작성
    void RedaCSV(){
        //참조한 CSV파일을 ,와 엔터단위로 파싱
        string[] CSVdata = csvfile.text.Split(new string[] {",", "\n"}, StringSplitOptions.None);

        todayQuestionList.TQL = new TodayQuestion[tableSize];

        for(int i = 0; i<tableSize-1; i++)
        {
            todayQuestionList.TQL[i] = new TodayQuestion();
            todayQuestionList.TQL[i].Num = i+1;
            todayQuestionList.TQL[i].QuestionT = (CSVdata[4*(i+1)+1]);
            todayQuestionList.TQL[i].answerA = (CSVdata[4*(i+1)+2]);
            todayQuestionList.TQL[i].answerB = (CSVdata[4*(i+1)+3]);
        }

        
    }

    void PrintQuestionT()
    {
        int tqlNum = UnityEngine.Random.Range(1,109);
        
        Debug.Log(tqlNum);
        Debug.Log(todayQuestionList.TQL[tqlNum].QuestionT);
        Debug.Log(todayQuestionList.TQL[tqlNum].answerA);
        Debug.Log(todayQuestionList.TQL[tqlNum].answerB);

        Question.text = "Q."+ todayQuestionList.TQL[tqlNum].QuestionT;
        answerA.text = todayQuestionList.TQL[tqlNum].answerA;
        answerB.text = todayQuestionList.TQL[tqlNum].answerB;
    }
}
