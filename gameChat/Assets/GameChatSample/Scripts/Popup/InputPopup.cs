using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameChatSample
{
    public class InputPopup : CustomizedPopup
    {
        #region override
        new public struct PopupButtonInfo
        {
            public string btnText;
            public popupCallback callback;
        }

        new public delegate void popupCallback(params string[] input);
        private popupCallback[] callbacks = null;
        #endregion


        #region Public Fields

        [SerializeField]
        protected InputField[] inputText;

        #endregion


        virtual public void SetPopup(string title, string message, PopupButtonInfo[] btn_info, string[] input_info)
        {
            SetText(title, message);
            SetButton(btn_info);
            SetPlaceholder(input_info);
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

        virtual public void SetPlaceholder(params string[] input_info)
        {
            if (input_info == null)
                return;

            for (int i = 0; i < inputText.Length; i++)
            {
                if (i < input_info.Length)
                {
                    Text tmp =  inputText[i].placeholder.GetComponent<Text>();
                    tmp.text = input_info[i];
                }
            }
        }


        #region UIButton.onClick
        override public void ClickButton(int idx)
        {
            if (callbacks[idx] != null)
            {
                string[] input_string = new string[inputText.Length];
                for (int i = 0; i < input_string.Length; i++)
                {
                    input_string[i] = inputText[i].text;
                }

                callbacks[idx](input_string);
            }
        }

        //override public void ClosePopup()
        //{
        //    Destroy(gameObject);
        //}
        #endregion


    }
}


