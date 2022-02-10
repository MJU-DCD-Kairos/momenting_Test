using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameChatUnity;


namespace GameChatSample
{
    public class TranslateSettingPopup : CustomizedPopup
    {
        #region override

        //델리게이터 재정의를 위해, 구조체 또한 재정의
        new public struct PopupButtonInfo
        {
            public string btnText;
            public popupCallback callback;
        }

        new public delegate void popupCallback(string srcLang, string dstLang);
        private popupCallback[] callbacks = null;

        #endregion


        #region Public Fields

        [SerializeField]
        Dropdown SrcLang;

        [SerializeField]
        GameObject rootDstLang;

        #endregion

        virtual public void SetPopup(string title, string message, PopupButtonInfo[] btn_info)
        {
            SetText(title, message);
            SetButton(btn_info);
        }

        //입력받은 info에 대해, 각 내부 버튼 셋팅
        virtual public void SetButton(PopupButtonInfo[] input_info)
        {
            if (input_info != null && input_info.Length > 0)
            {
                if (buttons.Length > 0)
                    this.callbacks = new popupCallback[buttons.Length];

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (input_info[i].callback != null)
                        this.callbacks[i] = input_info[i].callback;

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
            if (callbacks[idx] != null)
            {
                string _srcLang = SrcLang.options[SrcLang.value].text;
                List<string> _dstLangList = new List<string>();

                //DstLang -> rootToggleGroup 순회 => 체크된 dst 목록 merge
                foreach (Transform elem in rootDstLang.transform)
                {
                    Toggle each_toggle = elem.GetComponentInChildren<Toggle>();
                    if (each_toggle.isOn)
                        _dstLangList.Add(elem.gameObject.name.Trim().ToLower());
                }
                string _dstLang = string.Join(",", _dstLangList);

                callbacks[idx](_srcLang, _dstLang);
            }
        }
        #endregion




    }
}

