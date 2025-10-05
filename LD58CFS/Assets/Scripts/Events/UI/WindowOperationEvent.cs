#region

//文件创建者：Egg
//创建时间：10-05 11:45

#endregion

namespace LD58.UI
{
    public enum EWindowOperation
    {
        Open,
        Close,
        Minimize,
        Focus,
        ToggleActive
    }
    
    public struct WindowOperationEvent
    {
        public EWindowOperation Operation;
        public string WindowName;
    }
}