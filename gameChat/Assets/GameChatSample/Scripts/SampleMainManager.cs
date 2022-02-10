using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameChatUnity;
using GameChatUnity.SimpleJSON;
using GameChatUnity.SocketIO;
// using GameChatUnity.Extension;

namespace GameChatSample
{
    public partial class SampleMainManager : MonoBehaviour
    {
        [SerializeField]
        GameObject PopupRoot;

        PopupManager p_manager;

        [SerializeField]
        GameObject LoadingPanel;


        [Header("User Info")]
        [SerializeField]
        RawImage ProfileImage;

        [SerializeField]
        Texture DefaultProfileImage;

        [SerializeField]
        Text UserId;

        [SerializeField]
        Text Nickname;

        [Header("Layout Chat UI")]

        [SerializeField]
        InputField InputSendMsg;

        [SerializeField]
        Text StatusSubscribe;


        [SerializeField]
        Dropdown DropdownSubscribeChannel;
        HashSet<string> poolSubscription;

        [Header("Normal Text Chat")]

        public bool m_showDebug = true;
        public Text m_ChattingText;
        private System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();


        [Header("Extension Chat")]

        private bool _tmpToggle = false;

        [SerializeField]
        GameObject msgRoot;


        #region  LifeCycle

        void Awake()
        {
            p_manager = new PopupManager();
        }

        private void OnEnable()
        {
            Debug.Log("[OnEnable] MainScene");
            GameChat.dispatcher.onConnected += onConnected;
            GameChat.dispatcher.onDisconnected += onDisconnected;
            GameChat.dispatcher.onErrorReceived += onErrorReceived;
            GameChat.dispatcher.onMessageReceived += onMessageReceived;
        }

        private void OnDisable()
        {
            Debug.Log("[OnDisable] MainScene");
            GameChat.dispatcher.onConnected -= onConnected;
            GameChat.dispatcher.onDisconnected -= onDisconnected;
            GameChat.dispatcher.onErrorReceived -= onErrorReceived;
            GameChat.dispatcher.onMessageReceived -= onMessageReceived;
        }

        private void Start()
        {
            stringBuilder.Clear();

            Member user = SampleGlobalData.G_User;
            UserId.text = SampleGlobalData.G_User.id;

            if (!string.IsNullOrEmpty(user.nickname))
                Nickname.text = user.nickname;

            GetProfileImage();
            poolSubscription = new HashSet<string>();

            RefreshChannelList(() =>
            {
                RefreshChannelListUI();
                RefreshSubscribeStateUI();
                RefreshAPITargetChannelUI();
            });

            if (SampleGlobalData.G_isSocketConnected)
            {
                PrintChatMessage(Color.blue, "[ 소켓서버와 연결되었습니다. ]");
            }
        }


        void GetProfileImage()
        {
            string profile_url = SampleGlobalData.G_User.profile_url.Trim();

            //프로필 이미지 로드
            if (string.IsNullOrEmpty(profile_url))
            {
                ProfileImage.texture = DefaultProfileImage;
            }
            else
            {
                //소셜 프로파일 이미지 로드를 성공
                StartCoroutine(GetTexture(profile_url, (Texture tex) =>
                {
                    if (tex != null && !ReferenceEquals(tex, null))
                    {
                        //curProfileUrl = GameChat.setting.ProfileUrl;
                        ProfileImage.texture = tex;
                    }
                    else
                    {
                        ProfileImage.texture = DefaultProfileImage;
                    }
                }));
            }
        }



        IEnumerator GetTexture(string tex_url, System.Action<Texture> callback)
        {
            if (!string.IsNullOrEmpty(tex_url))
            {
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(tex_url);
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    if (callback != null)
                        callback(null);
                }
                else
                {
                    Texture myTexture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;

                    if (callback != null)
                        callback(myTexture);
                }
            }
            else
            {
                if (callback != null)
                    callback(null);
            }
        }

        #endregion

        #region Chatting EventListener

        private void onConnected(string message)
        {
            PrintChatMessage(Color.blue, "[ 소켓서버와 연결되었습니다. ]");
        }

        private void onMessageReceived(Message message)
        {
            if (message.sender.id == UserId.text)
                PrintChatMessage(Color.green, message);
            else
                PrintChatMessage(Color.black, message);
        }

        private void onDisconnected(string message)
        {
            poolSubscription.Clear();
            RefreshSubscribeStateUI();

            PrintChatMessage(Color.red, "[ 소켓서버와의 연결이 끊어졌습니다. ]");
        }

        private void onErrorReceived(string message, GameChatException exception)
        {
            CustomizedPopup.PopupButtonInfo[] b_info = new CustomizedPopup.PopupButtonInfo[1];
            b_info[0].callback = () =>
            {
                LoadingPanel.SetActive(false);
            };
            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Error Received", exception.ToJson(), b_info);
        }

        public static void RefreshChannelList(System.Action callback)
        {
            if (!SampleGlobalData.G_isConnected)
            {
                Debug.LogError("RefreshChannelList - GameChat is not Connected now!");
                return;
            }

            GameChat.getChannels(0, 30, (List<Channel> channels, GameChatException exception) =>
            {
                if (exception != null)
                {
                    Debug.Log("getChannels Exception Log => " + exception.ToJson());
                    return;
                }
                SampleGlobalData.G_ChannelList = channels;
                callback();
            });
        }

        #endregion



        #region UI_Interface

        //(현재 선택한 채널) 클릭 - toggle
        public void ClickSubscribeToggle()
        {
            string target_ch_id = SampleGlobalData.G_ChannelList[DropdownSubscribeChannel.value].id;

            if (poolSubscription.Contains(target_ch_id))
            {
                Debug.Log("Subscribing Now - Channel => " + target_ch_id);
                GameChat.unsubscribe(target_ch_id);
                poolSubscription.Remove(target_ch_id);
            }
            else
            {
                Debug.Log("Unsubscribing Now Channel => " + target_ch_id);
                GameChat.subscribe(target_ch_id);
                poolSubscription.Add(target_ch_id);
            }

            RefreshSubscribeStateUI();
        }


        //(현재 선택한) 채널에 대해, Subscribe UI 상태 전환
        public void RefreshSubscribeStateUI()
        {
            string target_ch_id = SampleGlobalData.G_ChannelList[DropdownSubscribeChannel.value].id;

            if (poolSubscription.Contains(target_ch_id))
            {
                Debug.Log("Subscribing Now - Channel => " + target_ch_id);
                StatusSubscribe.text = "Subscribing";
                StatusSubscribe.color = Color.red;
            }
            else
            {
                Debug.Log("Unsubscribing Now Channel => " + target_ch_id);
                StatusSubscribe.text = "Unsubscribing";
                StatusSubscribe.color = Color.blue;
            }
        }


        public void ClickBtnSendMessage()
        {
            ClearTranslateMessage();

            string msg = InputSendMsg.text;

            if (string.IsNullOrEmpty(msg.Trim()))
                return;

            GameChat.sendMessage(SampleGlobalData.G_ChannelList[DropdownSubscribeChannel.value].id, msg);
            InputSendMsg.text.Remove(0, InputSendMsg.text.Length);
            InputSendMsg.text = "";

        }

        public void ClickBtnLogout()
        {
            LoadingPanel.SetActive(true);

            CustomizedPopup.PopupButtonInfo[] b_info = new CustomizedPopup.PopupButtonInfo[1];
            b_info[0].callback = () =>
            {
                GameChat.disconnect();
                SampleGlobalData.G_isConnected = false;
                LoadingPanel.SetActive(false);
                SceneManager.LoadSceneAsync("SampleScene_Login");
            };
            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Logout", "연결 해제. 로그인 화면으로 이동", b_info);
        }

        public void ClickBtnShowUserStatus()
        {
            string result = "";

            result += "\n\n id : " + SampleGlobalData.G_User.id;
            result += "\n nickname : " + SampleGlobalData.G_User.nickname;
            result += "\n project_id : " + SampleGlobalData.G_User.project_id;
            result += "\n profile_url : " + SampleGlobalData.G_User.profile_url;
            result += "\n country : " + SampleGlobalData.G_User.country;
            result += "\n remoteip : " + SampleGlobalData.G_User.remoteip;
            result += "\n adid : " + SampleGlobalData.G_User.adid;
            result += "\n network : " + SampleGlobalData.G_User.network;

            result += "\n logined_at : " + SampleGlobalData.G_User.logined_at;
            result += "\n created_at : " + SampleGlobalData.G_User.created_at;
            result += "\n updated_at : " + SampleGlobalData.G_User.updated_at;
            result += "\n\n";

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "showStatus", result, new CustomizedPopup.PopupButtonInfo[0]);
        }

        public void ClickBtnUpdateNickname()
        {
            InputPopup.PopupButtonInfo[] btn_info = new InputPopup.PopupButtonInfo[1];

            btn_info[0].callback = (string[] inputs) =>
            {
                string nickname = inputs[0];

                GameChat.setNickname(SampleGlobalData.G_User.id, nickname, (member, exception) =>
                {
                    if (exception != null)
                    {
                        p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setNickname Failed", exception.ToJson(), new CustomizedPopup.PopupButtonInfo[0]);
                        return;
                    }

                    SampleGlobalData.G_User.nickname = member.nickname;
                    Nickname.text = member.nickname;
                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setNickname Completed!", "닉네임 변경 완료 -> " + member.nickname, new CustomizedPopup.PopupButtonInfo[0]);
                });
            };

            string[] input_info = new string[1] { "Enter NickName..." };

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_Input",
                "Update Nickname", "변경할 유저 정보를 입력해주세요.", btn_info, input_info);
        }

        public void ClickBtnUserProfile()
        {
            InputPopup.PopupButtonInfo[] btn_info = new InputPopup.PopupButtonInfo[1];

            btn_info[0].callback = (string[] inputs) =>
            {
                GameChat.setProfileUrl(SampleGlobalData.G_User.id, inputs[0], (member, exception) =>
                {
                    if (exception != null)
                    {
                        p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setNickname Failed", exception.ToJson(), new CustomizedPopup.PopupButtonInfo[0]);
                        return;
                    }

                    SampleGlobalData.G_User.profile_url = member.profile_url;
                    GetProfileImage();
                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Update ProfileUrl Completed!", "프로필 변경 완료 -> " + member.profile_url, new CustomizedPopup.PopupButtonInfo[0]);
                });
            };

            string _profile = string.IsNullOrEmpty(SampleGlobalData.G_User.profile_url) ? "Enter Profile Image Url" : SampleGlobalData.G_User.profile_url.Trim();
            string[] input_info = new string[1] { _profile };

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_Input",
    "Update ProfileUrl", "변경할 유저 프로필 url을 입력해주세요.", btn_info, input_info);
        }


        //public void ClickBtnUpdateStatus()
        //{
        //    InputPopup.PopupButtonInfo[] btn_info = new InputPopup.PopupButtonInfo[2];
        //    btn_info[0].callback = (string[] inputs) =>
        //    {
        //        string nickname = inputs[0];
        //        GameChat.setNickname(SampleGlobalData.G_User.id, nickname, (member, exception) =>
        //        {
        //            if (exception != null)
        //            {
        //                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setNickname Failed", exception.ToJson(), null);
        //                return;
        //            }
        //            SampleGlobalData.G_User.nickname = member.nickname;
        //            Nickname.text = member.nickname;
        //            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setNickname Completed!", "닉네임 변경 완료 -> " + member.nickname, null);
        //            //Debug.Log("Nick Updated User -> " + SampleGlobalData.G_User.nickname);
        //        });
        //    };

        //    btn_info[1].callback = (string[] inputs) =>
        //    {
        //        string profile_url = inputs[1];
        //        GameChat.setProfileUrl(SampleGlobalData.G_User.id, profile_url, (member, exception) =>
        //        {
        //            if (exception != null)
        //            {
        //                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setProfileUrl Failed", exception.ToJson(), null);
        //                return;
        //            }
        //            SampleGlobalData.G_User.profile_url = member.profile_url;
        //            GetProfileImage(member.profile_url);
        //            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "setProfileUrl Completed!", "프로필이미지 변경 완료 -> " + member.profile_url, null);
        //            //Debug.Log("Profile Updated User -> " + SampleGlobalData.G_User.profile_url);
        //        });
        //    };

        //    string[] input_info = new string[2] { "Enter NickName...", "Enter ProfileUrl..." };
        //    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_UpdateUserInfo",
        //        "Update UserStatus", "변경할 유저 정보를 입력해주세요.", btn_info, input_info);
        //}

        #endregion




        #region Public_Interface

        public void ClearDebugText()
        {
            m_ChattingText.text = "";
        }

        private void PrintChatMessage(Color color, Message message)
        {
            Color _color;
            if (message.sender.id == UserId.text)
                _color = Color.green;
            else
                _color = Color.black;


            //** [Start] If NOT use GameChat Extension
            GameObject msgObject = Resources.Load<GameObject>("Normal_Text");
            if (msgObject == null)
            {
                Debug.LogError("GamePot Sample - Could not found " + "Normal_Text" + " in Resources folder");
            }

            string _msg = "";
            if (!string.IsNullOrEmpty(message.channel_id))
                _msg += (string)("[ " + message.channel_id + " ]");

            if (!string.IsNullOrEmpty(message.sender.id))
                _msg += (string)(" [ id : " + message.sender.id + " ] ");
            if (!string.IsNullOrEmpty(message.content))
                _msg += message.content;

            Text _target = msgObject.GetComponent<Text>();
            _target.color = _color;
            _target.text = _msg;
            //** [End] If NOT use GameChat Extension



            ////** [Start] If use GameChat Extension

            //GameObject msgObject = Resources.Load<GameObject>("TMP_Text");
            //if (msgObject == null)
            //{
            //    Debug.LogError("GamePot Sample - Could not found " + "TMP_Text" + " in Resources folder");
            //}

            //TMP_GameChatTextUGUI _target = msgObject.GetComponent<TMP_GameChatTextUGUI>();
            //_target.color = _color;

            //string _msg = "";
            //if (!string.IsNullOrEmpty(message.channel_id))
            //    _msg += (string)("[ " + message.channel_id + " ]");

            //if (!string.IsNullOrEmpty(message.sender.id))
            //    _msg += (string)(" [ id : " + message.sender.id + " ] ");
            //if (!string.IsNullOrEmpty(message.content))
            //    _msg += message.content;

            //_target.SetMessage(_msg);
            ////_tmpToggle = !_tmpToggle;
            //_target.isHyperLinked = true;

            ////** [End] If use GameChat Extension


            msgObject = Instantiate(msgObject) as GameObject;
            msgObject.transform.parent = msgRoot.transform;
            msgObject.transform.localScale = new Vector3(1, 1, 1);
            msgObject.transform.localPosition = Vector3.zero;
            msgObject.transform.SetAsLastSibling();
        }




        private void PrintChatMessage(Color color, string message)
        {
            //** [Start] If NOT use GameChat Extension
            GameObject msgObject = Resources.Load<GameObject>("Normal_Text");
            if (msgObject == null)
            {
                Debug.LogError("GamePot Sample - Could not found " + "Normal_Text" + " in Resources folder");
            }

            Text _target = msgObject.GetComponent<Text>();
            _target.color = color;
            _target.text = message;
            //** [Ent] If NOT use GameChat Extension


            ////** [Start] If use GameChat Extension

            //GameObject msgObject = Resources.Load<GameObject>("TMP_Text");
            //if (msgObject == null)
            //{
            //    Debug.LogError("GamePot Sample - Could not found " + "TMP_Text" + " in Resources folder");
            //}

            //TMP_GameChatTextUGUI _target = msgObject.GetComponent<TMP_GameChatTextUGUI>();
            //_target.color = color;
            //_target.SetMessage(message);
            //_tmpToggle = !_tmpToggle;
            //_target.isHyperLinked = _tmpToggle;

            ////** [End] If use GameChat Extension

            msgObject = Instantiate(msgObject) as GameObject;
            msgObject.transform.parent = msgRoot.transform;
            msgObject.transform.localScale = new Vector3(1, 1, 1);
            msgObject.transform.localPosition = Vector3.zero;
            msgObject.transform.SetAsLastSibling();
        }

        #endregion










    }

}
