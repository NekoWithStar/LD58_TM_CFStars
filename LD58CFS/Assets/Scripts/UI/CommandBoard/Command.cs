#region

//文件创建者：Egg
//创建时间：10-04 08:33

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.UI.CommandBoard
{
    [Serializable]
    public sealed class Command
    {
        public string                        CommandName;
        public List<CommandParameterWrapper> Parameters = new();
    }

    [Serializable]
    public class CommandParameterWrapper
    {
        [SerializeReference, HideReferenceObjectPicker]
        public CommandParameter Parameter;
    }

    [Serializable]
    public class CommandParameter
    {
        public                 string Name;
        [NonSerialized] public string Value;

        public List<string> AcceptedBlackBoardKey = new();

        public virtual string GetValue()
        {
            return Value;
        }
    }
}