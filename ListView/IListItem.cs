namespace SalesDemo_DevGr_A.ListView
{
    public interface IListItem
    {
        ListItemType GetListItemType();

        string Text { get; set; }
    }
}