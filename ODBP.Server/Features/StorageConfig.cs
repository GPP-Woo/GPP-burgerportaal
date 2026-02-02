namespace ODBP.Features
{
    public class StorageConfig
    {
        public const string ImagesPath = "/app/data/images";

        public void EnsureDirectoryExists()
        {
            if (!Directory.Exists(ImagesPath))
            {
                Directory.CreateDirectory(ImagesPath);
            }
        }
    }
}
