using FolderMillPrintApi.Models;

namespace FolderMillPrintApi
{
    public interface IAppSettings
    {
        PrintConfig PrintConfig { get; }
    }
}
