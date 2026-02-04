namespace ODBP.Features
{
    public class StorageConfig
    {
        public string ImagesPath { get; }

        public StorageConfig(IHostEnvironment environment)
        {
            var startPath = Directory.GetParent(environment.ContentRootPath)?.FullName ?? environment.ContentRootPath;
            ImagesPath = Path.Combine(startPath, "data", "images");
        }

        public void EnsureDirectoryExists()
        {
            if (!Directory.Exists(ImagesPath))
            {
                Directory.CreateDirectory(ImagesPath);
            }
        }
    }
}
