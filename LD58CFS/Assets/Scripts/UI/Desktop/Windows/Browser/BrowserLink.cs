#region

//文件创建者：Egg
//创建时间：10-06 11:31

#endregion

namespace LD58.UI
{
    public sealed class BrowserLink : BlackBoardItemAcceptor
    {
        protected override void Accept(BlackBoardItem blackBoardItem)
        {
            FindFirstObjectByType<BrowserWindow>()
                .AddBrowserLinkHistoryItem(Selection.SelectedBlackBoardItem.Value, true);
        }
    }
}