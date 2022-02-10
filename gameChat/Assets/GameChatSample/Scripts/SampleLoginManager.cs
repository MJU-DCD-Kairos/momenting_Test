using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameChatUnity;



namespace GameChatSample
{
    public class SampleLoginManager : MonoBehaviour
    {
        [SerializeField]
        GameObject PopupRoot;

        [SerializeField]
        GameObject LoadingPanel;

        [SerializeField]
        string initProjectID;

        [SerializeField]
        Text ProjectID;


        PopupManager p_manager;

        #region LifeCycle

        void Awake()
        {
            p_manager = new PopupManager();

            string init_project = "";
            init_project = initProjectID.Trim();
            ProjectID.text = init_project;

            GameChat.initialize(init_project);
        }

        private void OnEnable()
        {
            Debug.Log("[OnEnable] Login Scene");
            GameChat.dispatcher.onConnected += onConnected;
            GameChat.dispatcher.onErrorReceived += onErrorReceived;
        }

        private void OnDisable()
        {
            Debug.Log("[OnDisable] Login Scene");
            GameChat.dispatcher.onConnected -= onConnected;
            GameChat.dispatcher.onErrorReceived -= onErrorReceived;
        }
        #endregion


        #region EventListener

        void onConnected(string messge)
        {
            SampleGlobalData.G_isSocketConnected = true;

            CustomizedPopup.PopupButtonInfo[] btn_info = new CustomizedPopup.PopupButtonInfo[1];
            btn_info[0].callback = () =>
            {
                LoadingPanel.SetActive(false);
                SceneManager.LoadSceneAsync("SampleScene_Main");
            };
            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Connect Success !", "로그인이 완료되었습니다.", btn_info);

        }

        void onErrorReceived(string message, GameChatException exception)
        {
            CustomizedPopup.PopupButtonInfo[] b_info = new CustomizedPopup.PopupButtonInfo[1];
            b_info[0].callback = () =>
            {
                LoadingPanel.SetActive(false);
            };
            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Login Failed!", "Error - " + exception.ToJson(), b_info);
        }

        #endregion



        #region UI

        public void ClickLoginButton(Text user_id)
        {
            string opt_user_id = user_id.text.Trim();

            if (string.IsNullOrEmpty(opt_user_id))
            {
                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Login Failed", "로그인 아이디를 입력해주세요", new CustomizedPopup.PopupButtonInfo[0]);
                return;
            }

            LoadingPanel.SetActive(true);

            GameChat.connect(opt_user_id, (Member user, GameChatException exception) =>
            {
                if (exception != null)
                {
                    CustomizedPopup.PopupButtonInfo[] b_info = new CustomizedPopup.PopupButtonInfo[1];
                    b_info[0].callback = () =>
                    {
                        LoadingPanel.SetActive(false);
                    };
                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Login Failed!", "Error - " + exception.ToJson(), b_info);
                    return;
                }
                SampleGlobalData.G_isConnected = true;
                SampleGlobalData.G_User = user;
            });
        }

        #endregion

        public void ClickBtnChangeProject()
        {
            InputPopup.PopupButtonInfo[] btn_info = new InputPopup.PopupButtonInfo[1];

            btn_info[0].callback = (string[] inputs) =>
            {
                ProjectID.text = inputs[0];
                GameChat.initialize(inputs[0]);
            };

            string _profile = string.IsNullOrEmpty(ProjectID.text) ? "Enter Profile Image Url" : ProjectID.text.Trim();
            string[] input_info = new string[1] { ProjectID.text };

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_Input",
    "Update ProjectID", "변경할 프로젝트 아이디를 입력해주세요.", btn_info, input_info);
        }
    }
}
