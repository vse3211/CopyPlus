namespace CopyPlus
{
    public interface IFolderPicker
    {
        Task<string> PickFolder();
    }
}
