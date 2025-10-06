#region

//文件创建者：Egg
//创建时间：10-06 12:39

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.Data
{
    [CreateAssetMenu(fileName = "LevelDatas", menuName = "Data/LevelData")]
    public sealed class LevelDatas : ScriptableObject
    {
        [LabelText("所有关卡数据")] public List<LevelData> Datas = new();
    }
}