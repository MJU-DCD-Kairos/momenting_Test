using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameChatUnity;


namespace GameChatSample
{
    public partial class SampleMainManager : MonoBehaviour
    {
        [Header("API Target Channel List")]

        [SerializeField]
        Dropdown DropdownTargetChannel;

        [SerializeField]
        Text NameTargetChannel;

        [SerializeField]
        Text UIDTargetChannel;

        [SerializeField]
        GameObject rootTranslationList;

        private string SrcLang;
        private string DstLang;



        //채널 드롭다운 UI 갱신
        public void RefreshChannelListUI()
        {
            DropdownTargetChannel.ClearOptions();
            DropdownSubscribeChannel.ClearOptions();

            List<Dropdown.OptionData> ch_list = new List<Dropdown.OptionData>();

            foreach (Channel elem in SampleGlobalData.G_ChannelList)
            {
                ch_list.Add(new Dropdown.OptionData(elem.id));
            }

            DropdownTargetChannel.AddOptions(ch_list);
            DropdownSubscribeChannel.AddOptions(ch_list);
        }

        public void RefreshAPITargetChannelUI()
        {
            if (SampleGlobalData.G_ChannelList.Count <= 0)
            {
                NameTargetChannel.text = "";
                UIDTargetChannel.text = "";
                return;
            }

            Channel curChannel = SampleGlobalData.G_ChannelList[DropdownTargetChannel.value];
            NameTargetChannel.text = curChannel.name;
            UIDTargetChannel.text = string.IsNullOrEmpty(curChannel.unique_id.Trim()) ? "<not defined>" : curChannel.unique_id;
        }



        public void ClickBtnTranslateMessage()
        {
            GameChat.translateMessage(SampleGlobalData.G_ChannelList[DropdownSubscribeChannel.value].id, SrcLang, DstLang, InputSendMsg.text.Trim(), (List<Translation> msg_list, GameChatException exception) =>
            {
                if (exception != null)
                {
                    Debug.LogError(exception.ToJson());
                    return;
                }

                TranslatePopup.PopupButtonInfo[] btn_info = new TranslatePopup.PopupButtonInfo[1];
                btn_info[0].callback = (string contents) =>
                {
                    if (!string.IsNullOrEmpty(contents))
                        InputSendMsg.text = contents;
                };

                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_Translation", "translations", "", btn_info, msg_list.ToArray());
            });

        }


        public void ClickBtnTranslateSetting()
        {
            TranslateSettingPopup.PopupButtonInfo[] btn_info = new TranslateSettingPopup.PopupButtonInfo[1];
            btn_info[0].callback = (string _srcLang, string _dstLang) =>
            {
                Debug.Log("Save Translation Setting - Source : " + _srcLang);
                SrcLang = _srcLang;
                Debug.Log("Save Translation Setting - Destination : " + _dstLang);
                DstLang = _dstLang;
            };

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_TranslateSetting", "Translate Setting", "", btn_info);
        }



        void ClearTranslateMessage()
        {
            foreach (Transform elem in rootTranslationList.transform)
            {
                Destroy(elem.gameObject);
            }
        }


        public void ClickGetChannel()
        {
            string channel_id = SampleGlobalData.G_ChannelList[DropdownTargetChannel.value].id;

            GameChat.getChannel(channel_id, null, (Channel channel, GameChatException exception) =>
            {

                if (exception != null)
                {
                    Debug.Log("Exception Log => " + exception.ToJson());
                    return;
                }

                string result = "";
                int idx = 0;

                string sub_result = "";
                sub_result += "\n\n id : " + channel.id;
                sub_result += "\n project_id : " + channel.project_id;
                sub_result += "\n name : " + channel.name;
                sub_result += "\n uniqueId : " + channel.unique_id;
                //sub_result += "\n join_count : " + channel.join_count;
                sub_result += "\n created_at : " + channel.created_at;
                sub_result += "\n updated_at : " + channel.updated_at;
                sub_result += "\n\n";
                result += sub_result;
                idx++;
                //Debug.Log(sub_result);
                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "getChannels", result, new CustomizedPopup.PopupButtonInfo[0]);
            });
        }

        public void ClickRefreshChannelList()
        {
            //갱신 전, 채널 아이디 저장
            string cur_id = "";
            if (SampleGlobalData.G_ChannelList.Count > 0)
                cur_id = SampleGlobalData.G_ChannelList[DropdownTargetChannel.value].id;

            RefreshChannelList(() =>
            {
                RefreshChannelListUI();

                //커서 인덱스 재지정
                int idx = 0;
                for (int i = 0; i < SampleGlobalData.G_ChannelList.Count; i++)
                {
                    if (cur_id.Equals(SampleGlobalData.G_ChannelList[i].id))
                    {
                        idx = i;
                        break;
                    }
                }
                DropdownTargetChannel.value = idx;
                DropdownSubscribeChannel.value = idx;

                RefreshSubscribeStateUI();
                RefreshAPITargetChannelUI();
            });
        }

        public void ClickGetMessages()
        {

            string channel_id = SampleGlobalData.G_ChannelList[DropdownTargetChannel.value].id;

            GameChat.getMessages(channel_id, 0, 100, "", "", "desc",
                (List<Message> messages, GameChatException exception) =>
                {
                    if (exception != null)
                    {
                        Debug.Log("Exception Log => " + exception.ToJson());
                        return;
                    }

                    string result = "";

                    int idx = 0;
                    foreach (Message elem in messages)
                    {
                        string sub_result = "[ Message " + idx + " ]";
                        sub_result += "\n\n id : " + elem.message_id;
                        //sub_result += "\n type : " + elem.content.type;
                        //sub_result += "\n lang : " + elem.content.lang;
                        //sub_result += "\n translated : " + elem.content.translated;

                        sub_result += "\n sender : " + elem.sender.id;
                        sub_result += "\n created_at : " + elem.created_at;
                        sub_result += "\n content : " + elem.content;
                        sub_result += "\n\n";
                        result += sub_result;
                        idx++;
                    }

                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "getMessages", result, new CustomizedPopup.PopupButtonInfo[0]);
                });
        }

        public void ClickGetSubscriptions()
        {

            string channel_id = SampleGlobalData.G_ChannelList[DropdownTargetChannel.value].id;

            GameChat.getSubscriptions(channel_id, 0, 100,
                (List<Subscription> subscriptions, GameChatException exception) =>
                {
                    if (exception != null)
                    {
                        Debug.LogError("get Except Exception Log => " + exception.ToJson());
                        return;
                    }

                    string result = "";
                    int idx = 0;
                    foreach (Subscription elem in subscriptions)
                    {
                        string sub_result = "[ " + idx + " ]";
                        sub_result += "\n id : " + elem.id;
                        sub_result += "\n channel_id : " + elem.channel_id;
                        sub_result += "\n user_id : " + elem.user_id;
                        sub_result += "\n created_at : " + elem.created_at;
                        sub_result += "\n\n";
                        result += sub_result;
                        idx++;
                    }

                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "getSubscriptions", result, new CustomizedPopup.PopupButtonInfo[0]);
                });
        }




        //public void ClickUpdateChannel()
        //{
        //    if (SampleGlobalData.G_ChannelList.Count <= 0)
        //    {
        //        Debug.LogError("There is no Channel Selected");
        //        return;
        //    }
        //    string target_ch_id = SampleGlobalData.G_ChannelList[ChannelOptions.value].id;
        //    GameChat.updateChannel(target_ch_id, curChannelName.text,
        //        (JSONNode result, GameChatException exception)
        //        =>
        //        {
        //            if (exception != null)
        //            {
        //                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Update Failed", "Update Channel Failed : " + exception.ToJson(), null);
        //                return;
        //            }

        //            string ch_id = result["updateChannel"]["channel"]["id"];

        //            if (ch_id.Equals(target_ch_id))
        //            {
        //                SampleMainManager.RefreshChannelList(() =>
        //                {
        //                    RefreshChannelDropdown_UI();
        //                    RefreshTargetChanel_UI();
        //                });

        //                p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Update Success", "Update Channel Succeeded : " + ch_id, null);
        //            }
        //            else
        //            {
        //                Debug.LogError("Update Channel Error. Result is different with Target Channel");
        //                Debug.LogErrorFormat("target CH :{0} result CH : {1}", target_ch_id, ch_id);
        //            }
        //        });
        //}

        public void ClickCreateChannel()
        {
            InputPopup.PopupButtonInfo[] btn_info = new InputPopup.PopupButtonInfo[1];
            btn_info[0].callback = (string[] inputs) =>
            {
                string channel_name = inputs[0];
                string unique_id = string.IsNullOrEmpty(inputs[1]) ? null : inputs[1];

                GameChat.createChannel(channel_name, unique_id, (Channel channel, GameChatException exception) =>
                {
                    if (exception != null)
                    {
                        Debug.Log("Exception Log => " + exception.ToJson());
                        return;
                    }
                    string result = "\n\n id : " + channel.id;
                    result += "\n project_id : " + channel.project_id;
                    result += "\n name : " + channel.name;
                    //result += "\n join_count : " + channel.join_count;
                    result += "\n created_at : " + channel.created_at;
                    result += "\n updated_at : " + channel.updated_at;
                    result += "\n\n";

                    CustomizedPopup.PopupButtonInfo[] info = new CustomizedPopup.PopupButtonInfo[0];

                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Create Channel", result, info);


                    //Subscription 리스트 추가
                    //Channel cursor = SampleGlobalData.G_ChannelList[ChannelOptions.value];
                    //GameObject prefab_subscription = Resources.Load<GameObject>("UnitSubscription");
                    //prefab_subscription.name = channel.id;

                    //GameObject instance = Instantiate(prefab_subscription) as GameObject;
                    //instance.transform.Find("ID").gameObject.GetComponent<Text>().text = channel.id;
                    //string text_subscribe = "UnSubscribe";
                    //Color color_subscribe = Color.blue;

                    //Text check_mark = instance.transform.Find("Status").gameObject.GetComponent<Text>();
                    //check_mark.text = text_subscribe;
                    //check_mark.color = color_subscribe;

                    //instance.transform.SetParent(rootSubscriptionList.transform);
                    //instance.transform.localScale = new Vector3(1, 1, 1);

                    ClickRefreshChannelList();

                });
            };

            string[] input_info = new string[2] { "Name", "UniqueId" };

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_Input_2",
                "CreateChannel", "생성할 Channel의 Name/UniqueId를 차례로 입력해주세요. (필수)Name (선택)UniqueId", btn_info, input_info);
        }





        public void ClickCreateChannel_dup()
        {
            InputPopup.PopupButtonInfo[] btn_info = new InputPopup.PopupButtonInfo[1];
            btn_info[0].callback = (string[] inputs) =>
            {
                string channel_name = inputs[0];
                string unique_id = string.IsNullOrEmpty(inputs[1]) ? null : inputs[1];



                //Debug.Log("첫번째 생성 시도");
                //GameChat.createChannel(channel_name, unique_id, (Channel channel, GameChatException exception) =>
                //{
                //    string result = "";

                //    if (exception != null)
                //    {
                //        Debug.Log("Exception Log => " + exception.ToJson());
                //        result += "Exception Log => " + exception.ToJson();
                //        //return;
                //    }
                //    else
                //    {
                //        result = "\n\n id : " + channel.id;
                //        result += "\n project_id : " + channel.project_id;
                //        result += "\n name : " + channel.name;
                //        //result += "\n join_count : " + channel.join_count;
                //        result += "\n created_at : " + channel.created_at;
                //        result += "\n updated_at : " + channel.updated_at;
                //        result += "\n\n";
                //    }



                //    CustomizedPopup.PopupButtonInfo[] info = new CustomizedPopup.PopupButtonInfo[0];
                //    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Create Channel", result, info);


                //    //Subscription 리스트 추가
                //    //Channel cursor = SampleGlobalData.G_ChannelList[ChannelOptions.value];
                //    //GameObject prefab_subscription = Resources.Load<GameObject>("UnitSubscription");
                //    //prefab_subscription.name = channel.id;

                //    //GameObject instance = Instantiate(prefab_subscription) as GameObject;
                //    //instance.transform.Find("ID").gameObject.GetComponent<Text>().text = channel.id;
                //    //string text_subscribe = "UnSubscribe";
                //    //Color color_subscribe = Color.blue;

                //    //Text check_mark = instance.transform.Find("Status").gameObject.GetComponent<Text>();
                //    //check_mark.text = text_subscribe;
                //    //check_mark.color = color_subscribe;

                //    //instance.transform.SetParent(rootSubscriptionList.transform);
                //    //instance.transform.localScale = new Vector3(1, 1, 1);

                //    ClickRefreshChannelList();

                //});





                Debug.Log("첫번째 생성 시도");
                GameChat.createChannel(channel_name, unique_id, (Channel channel, GameChatException exception) =>
                {
                    string result = "";

                    if (exception != null)
                    {
                        Debug.Log("Exception Log => " + exception.ToJson());
                        result += "Exception Log => " + exception.ToJson();
                        //return;
                    }
                    else
                    {
                        result = "\n\n id : " + channel.id;
                        result += "\n project_id : " + channel.project_id;
                        result += "\n name : " + channel.name;
                        //result += "\n join_count : " + channel.join_count;
                        result += "\n created_at : " + channel.created_at;
                        result += "\n updated_at : " + channel.updated_at;
                        result += "\n\n";
                    }



                    CustomizedPopup.PopupButtonInfo[] info = new CustomizedPopup.PopupButtonInfo[0];
                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Create Channel", result, info);


                    //Subscription 리스트 추가
                    //Channel cursor = SampleGlobalData.G_ChannelList[ChannelOptions.value];
                    //GameObject prefab_subscription = Resources.Load<GameObject>("UnitSubscription");
                    //prefab_subscription.name = channel.id;

                    //GameObject instance = Instantiate(prefab_subscription) as GameObject;
                    //instance.transform.Find("ID").gameObject.GetComponent<Text>().text = channel.id;
                    //string text_subscribe = "UnSubscribe";
                    //Color color_subscribe = Color.blue;

                    //Text check_mark = instance.transform.Find("Status").gameObject.GetComponent<Text>();
                    //check_mark.text = text_subscribe;
                    //check_mark.color = color_subscribe;

                    //instance.transform.SetParent(rootSubscriptionList.transform);
                    //instance.transform.localScale = new Vector3(1, 1, 1);

                    ClickRefreshChannelList();

                });
                Debug.Log("두번째 생성 시도");

                GameChat.createChannel(channel_name, unique_id, (Channel channel, GameChatException exception) =>
                {
                    string result = "";

                    if (exception != null)
                    {
                        Debug.Log("Exception Log => " + exception.ToJson());
                        result += "Exception Log => " + exception.ToJson();
                        //return;
                    }
                    else
                    {
                        result = "\n\n id : " + channel.id;
                        result += "\n project_id : " + channel.project_id;
                        result += "\n name : " + channel.name;
                        //result += "\n join_count : " + channel.join_count;
                        result += "\n created_at : " + channel.created_at;
                        result += "\n updated_at : " + channel.updated_at;
                        result += "\n\n";
                    }



                    CustomizedPopup.PopupButtonInfo[] info = new CustomizedPopup.PopupButtonInfo[0];
                    p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Create Channel", result, info);


                    //Subscription 리스트 추가
                    //Channel cursor = SampleGlobalData.G_ChannelList[ChannelOptions.value];
                    //GameObject prefab_subscription = Resources.Load<GameObject>("UnitSubscription");
                    //prefab_subscription.name = channel.id;

                    //GameObject instance = Instantiate(prefab_subscription) as GameObject;
                    //instance.transform.Find("ID").gameObject.GetComponent<Text>().text = channel.id;
                    //string text_subscribe = "UnSubscribe";
                    //Color color_subscribe = Color.blue;

                    //Text check_mark = instance.transform.Find("Status").gameObject.GetComponent<Text>();
                    //check_mark.text = text_subscribe;
                    //check_mark.color = color_subscribe;

                    //instance.transform.SetParent(rootSubscriptionList.transform);
                    //instance.transform.localScale = new Vector3(1, 1, 1);

                    ClickRefreshChannelList();

                });

            };

            string[] input_info = new string[2] { "Name", "UniqueId" };

            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_Input_2",
                "CreateChannel(2개연속생성)", "생성할 Channel의 Name/UniqueId를 차례로 입력해주세요. (필수)Name (선택)UniqueId", btn_info, input_info);
        }





        //public void ClickDeleteChannel()
        //{
        //    if (SampleGlobalData.G_ChannelList.Count <= 0)
        //    {
        //        Debug.Log("Channel list is empty!");
        //        return;
        //    }

        //    string target_ch_id = SampleGlobalData.G_ChannelList[ChannelOptions.value].id;

        //    GameChat.deleteChannel(target_ch_id, (JSONNode result, GameChatException exception) =>
        //    {
        //        if (exception != null)
        //        {
        //            Debug.Log("Exception Log => " + exception.ToJson());
        //            return;
        //        }

        //        string ch_id = result["deleteChannel"]["channel"]["id"];

        //        if (ch_id.Equals(target_ch_id))
        //        {
        //            p_manager.ShowCustomPopup(PopupRoot, "GameChatPopup_BtnOne", "Delete Success", "Delete Channel Succeeded : " + ch_id, null);
        //        }
        //        else
        //        {
        //            Debug.LogError("Delete Channel Error. [ Target - " + target_ch_id + " | Deleted - " + ch_id + " ]");
        //            return;
        //        }


        //        SampleMainManager.RefreshChannelList(
        //            () =>
        //            {
        //                RefreshSubscriptions_UI();
        //                RefreshChannelDropdown_UI();

        //                //커서 인덱스 재지정
        //                if (ChannelOptions.value > 0)
        //                    ChannelOptions.value--;

        //                RefreshTargetChanel_UI();

        //            });


        //    });
        //}


    }
}
