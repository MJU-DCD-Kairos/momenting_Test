using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//** [Start] If use GameChat Extension
// using TMPro;
// using GameChatUnity.Extension;

// public class Sample_TMP_Selector : MonoBehaviour, IPointerClickHandler
// {
//     // Get link and open page
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         TMP_GameChatTextUGUI _target = GetComponent<TMP_GameChatTextUGUI>();

//         int linkIndex = TMP_TextUtilities.FindIntersectingLink(_target, eventData.position, eventData.pressEventCamera);
//         Debug.Log("OnPointerClick() - " + linkIndex);
//         if (linkIndex == -1)
//             return;
//         TMP_LinkInfo linkInfo = _target.textInfo.linkInfo[linkIndex];
//         string selectedLink = linkInfo.GetLinkID();
//         if (selectedLink != "")
//         {
//             Debug.LogFormat("Open link {0}", selectedLink);
//             Application.OpenURL(selectedLink);
//         }
//     }
// }
//** [End] If use GameChat Extension
