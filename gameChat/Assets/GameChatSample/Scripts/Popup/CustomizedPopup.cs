using UnityEngine;
using UnityEngine.UI;

namespace GameChatSample
{
    public class CustomizedPopup : MonoBehaviour
    {
        //해당 팝업(프리팹)이 가진 버튼 배열
        public struct PopupButtonInfo
        {
            public string btnText;
            public popupCallback callback;
        }

        public delegate void popupCallback();
        private popupCallback[] callbacks = null;

        [SerializeField]
        private Text title;

        [SerializeField]
        private Text message;

        [SerializeField]
        protected GameObject[] buttons;


        virtual public void SetPopup(string title, string message, PopupButtonInfo[] btn_info)
        {
            SetText(title, message);
            SetButton(btn_info);
        }

        protected void SetText(string title, string message)
        {
            if (!string.IsNullOrEmpty(title))
                this.title.text = title;
            if (!string.IsNullOrEmpty(message))
                this.message.text = message;
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

        virtual public void ClickButton(int idx)
        {
            if (callbacks != null && callbacks[idx] != null)
            {
                callbacks[idx]();
            }
        }

        public void ClosePopup()
        {
            Destroy(gameObject);
        }
        #endregion

    }
}
