#region

//文件创建者：Egg
//创建时间：10-06 11:00

#endregion

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.UI.DataBase
{
    [Serializable]
    public class MailMessage
    {
        public string MailId;
        public string Sender;
        public string Recipient;
        public string Subject;
        public string Body;
        public string Timestamp;
    }

    public sealed class DataBase : MonoBehaviour
    {
        [SerializeField] private List<MailMessage> MailMessages = new();
        [SerializeField] private float             FlowSpeed;
        [SerializeField] private Vector2           Range;

        [SerializeField] private List<DataBaseRow> DataBaseRows = new();
        
        private void Awake()
        {
            for (var i = 0; i < MailMessages.Count; i++)
            {
                DataBaseRows[i].SetText(new List<string>
                {
                    MailMessages[i].MailId,
                    MailMessages[i].Sender,
                    MailMessages[i].Recipient,
                    MailMessages[i].Subject,
                    MailMessages[i].Body,
                    MailMessages[i].Timestamp,
                });
            }
        }

        private void Update()
        {
            foreach (var dataBaseRow in DataBaseRows)
            {
                var rect = dataBaseRow.GetComponent<RectTransform>();
                rect.anchoredPosition -= new Vector2(0, FlowSpeed * Time.deltaTime);
                if (rect.anchoredPosition.y < Range.y)
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, Range.x);
            }
        }
    }
}