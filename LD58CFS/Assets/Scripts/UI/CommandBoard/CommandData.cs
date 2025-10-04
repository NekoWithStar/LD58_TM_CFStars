#region

//文件创建者：Egg
//创建时间：10-04 08:37

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace LD58.UI.CommandBoard
{
    [CreateAssetMenu(fileName = "CommandData", menuName = "LD58/CommandData", order = 0)]
    public sealed class CommandData : ScriptableObject
    {
        public List<Command> Commands = new();
    }
}