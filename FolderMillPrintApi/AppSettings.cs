using FolderMillPrintApi.Models;

namespace FolderMillPrintApi
{
    public class AppSettings : IAppSettings
    {
        private readonly AppConfig _appConfig;
        public AppSettings(AppConfig appConfig) => _appConfig = appConfig;

        public PrintConfig PrintConfig { get => _appConfig.PrintConfig; }
    }
}
