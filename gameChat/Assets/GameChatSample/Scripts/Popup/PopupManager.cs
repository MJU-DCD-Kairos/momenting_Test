using UnityEngine;
using GameChatUnity;

namespace GameChatSample
{
    public class PopupManager : MonoBehaviour
    {
        public enum CommonPopupType
        {
            SMALL_SIZE,
            LARGE_SIZE
        }

        public GameObject ShowPopup(GameObject parent, string popupName)
        {
            GameObject popupGameObject = Resources.Load<GameObject>(popupName);

            if (popupGameObject == null)
            {
                Debug.LogError("GamePot Sample - Could not found " + popupName + " in Resources folder");
            }

            popupGameObject = Instantiate(popupGameObject) as GameObject;
            popupGameObject.transform.parent = parent.transform;
            popupGameObject.transform.localScale = new Vector3(1, 1, 1);
            popupGameObject.transform.localPosition = Vector3.zero;
            popupGameObject.transform.SetAsLastSibling();

            return popupGameObject;
        }


        public  void ShowCustomPopup(GameObject parent, string prefab_name, string title, string message, CustomizedPopup.PopupButtonInfo[] btn_info)
        {
            CustomizedPopup customPopup = ShowPopup(parent, prefab_name).GetComponent<CustomizedPopup>();

            if (customPopup != null)
            {
                customPopup.SetPopup(title, message, btn_info);
            }
        }

        public  void ShowCustomPopup(GameObject parent, string prefab_name, string title, string message, InputPopup.PopupButtonInfo[] btn_info, string[] input_info)
        {
            InputPopup inputPopup = ShowPopup(parent, prefab_name).GetComponent<InputPopup>();

            if (inputPopup != null)
            {
                inputPopup.SetPopup(title, message, btn_info, input_info);
            }
        }

        public  void ShowCustomPopup(GameObject parent, string prefab_name, string title, string message, ChangeProjPopup.PopupButtonInfo[] btn_info, string[] input_info)
        {
            ChangeProjPopup inputPopup = ShowPopup(parent, prefab_name).GetComponent<ChangeProjPopup>();

            if (inputPopup != null)
            {
                inputPopup.SetPopup(title, message, btn_info);
            }
        }

        public void ShowCustomPopup(GameObject parent, string prefab_name, string title, string message, TranslatePopup.PopupButtonInfo[] btn_info, Translation[] translation_info)
        {
            TranslatePopup translatePopup = ShowPopup(parent, prefab_name).GetComponent<TranslatePopup>();

            if (translatePopup != null)
            {
                translatePopup.SetPopup(title, message, btn_info, translation_info);
            }
        }

        public void ShowCustomPopup(GameObject parent, string prefab_name, string title, string message, TranslateSettingPopup.PopupButtonInfo[] btn_info)
        {
            TranslateSettingPopup translatePopup = ShowPopup(parent, prefab_name).GetComponent<TranslateSettingPopup>();

            if (translatePopup != null)
            {
                translatePopup.SetPopup(title, message, btn_info);
            }
        }

    }
}