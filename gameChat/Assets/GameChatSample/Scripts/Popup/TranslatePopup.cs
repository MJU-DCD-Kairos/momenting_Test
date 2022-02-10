using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameChatUnity;

namespace GameChatSample
{
    public class TranslatePopup : CustomizedPopup
    {
        #region override
        //델리게이터 재정의를 위해, 구조체 또한 재정의
        new public struct PopupButtonInfo
        {
            public string btnText;
            public popupCallback callback;
        }

        new public delegate void popupCallback(string input);
        private popupCallback[] callbacks = null;
        #endregion


        #region Public Fields

        //번역된 객체 리스트의 루트노드
        [SerializeField]
        private GameObject rootTranslationUnit;

        public string selectedMsg = "";

        #endregion






        virtual public void SetPopup(string title, string message, PopupButtonInfo[] btn_info, Translation[] translations)
        {
            SetText(title, message);
            SetButton(btn_info);

            foreach (Translation elem in translations)
            {
                //각 UnitTranslation 생성한다음 배치
                GameObject prefab_translation = Resources.Load<GameObject>("UnitTranslation");
                prefab_translation.name = elem.lang;

                GameObject instance = Instantiate(prefab_translation) as GameObject;
                instance.transform.SetParent(rootTranslationUnit.transform);
                instance.transform.localScale = new Vector3(1, 1, 1);

                instance.GetComponent<SampleUnitTranslation>().SetParent(gameObject);
                instance.GetComponent<SampleUnitTranslation>().SetContents(elem);

                instance.GetComponentInChildren<Toggle>().isOn = false;
                instance.GetComponentInChildren<Toggle>().group = GetComponent<ToggleGroup>();

            }
        }



        virtual public void SetButton(PopupButtonInfo[] input_info)
        {
            if (input_info != null && input_info.Length > 0)
            {
                if (buttons.Length > 0)
                    this.callbacks = new popupCallback[buttons.Length];

                for (int i = 0; i < buttons.Length; i++)
                {

                    if (input_info[i].callback != null)
                    {
                        this.callbacks[i] = input_info[i].callback;
                    }

                    if (!string.IsNullOrEmpty(input_info[i].btnText)
                    && buttons[i].transform.GetChild(0) != null
                    && buttons[i].transform.GetChild(0).GetComponent<Text>() != null)
                    {
                        Text btn_text = buttons[i].transform.GetChild(0).GetComponent<Text>();
                        btn_text.text = input_info[i].btnText;
                    }
                }
            }
        }




        #region UIButton.onClick
        override public void ClickButton(int idx)
        {
            if (callbacks[idx] != null
                && !string.IsNullOrEmpty(selectedMsg))
            {
                callbacks[idx](selectedMsg);
            }
        }

        //public void SetTranslation(Translation translation)
        //{
        //    curTranslation = translation;
        //}
        #endregion



    }
}
