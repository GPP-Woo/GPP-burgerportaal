namespace ODBP.Features
{
    public class StorageConfig
    {
        public string ImagesPath { get; }

        public StorageConfig(IConfiguration configuration)
        {
            var path = configuration["IMAGES_PATH"];
            ImagesPath = !string.IsNullOrWhiteSpace(path) ? path : "/app/data/images";
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
