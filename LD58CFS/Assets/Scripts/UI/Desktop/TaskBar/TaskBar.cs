#region

//文件创建者：Egg
//创建时间：10-05 10:40

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.UI
{
    public sealed class TaskBar : MonoBehaviour
    {
        [SerializeField] private Transform _taskItemContainer;
        [SerializeField] private GameObject  _taskItemPrefab;
        
        private readonly Dictionary<string, TaskItem> _taskItems = new();

        private void Awake()
        {
            foreach (Transform child in _taskItemContainer)
            {
                Destroy(child.gameObject);
            }
            _taskItems.Clear();
        }
        
        [Button]
        public TaskItem AddTaskItem(string taskName, Sprite icon)
        {
            var go = Instantiate(_taskItemPrefab, _taskItemContainer);
            var item = go.GetComponent<TaskItem>();
            item.SetText(taskName);
            item.SetIcon(icon);
            _taskItems[taskName] = item;
            return item;
        }
        
        [Button]
        public void RemoveTaskItem(string taskName)
        {
            if (_taskItems.TryGetValue(taskName, out var item))
            {
                Destroy(item.gameObject);
                _taskItems.Remove(taskName);
            }
        }
    }
}