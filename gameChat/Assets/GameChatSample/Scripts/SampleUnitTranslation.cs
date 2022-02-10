using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameChatUnity;
using GameChatUnity.SimpleJSON;

namespace GameChatSample
{
    public class SampleUnitTranslation : MonoBehaviour
    {
        [SerializeField]
        private TranslatePopup parentPopup;

        [SerializeField]
        private Translation contents;

        public void SetParent(GameObject target)
        {
            parentPopup = target.GetComponentInChildren<TranslatePopup>();
        }

        public void SetContents(Translation translation)
        {
            contents = translation;

            transform.Find("Lang").GetComponent<Text>().text = translation.lang + " : ";
            Color _color = translation.translated ? Color.black : Color.red;
            Text _contentText = transform.Find("Contents").GetComponentInChildren<Text>();
            _contentText.text = translation.message;
            _contentText.color = _color;

            //번역 안됐을 때는, 선택 불가
            if (!translation.translated)
            {
                transform.Find("Toggle").GetComponent<Toggle>().interactable = false;
            }
        }


        public void SelectTranslation()
        {
            bool _flag = GetComponentInChildren<Toggle>().isOn;
            if (_flag)
            {
                parentPopup.selectedMsg = contents.message;
            }
        }

    }

}

