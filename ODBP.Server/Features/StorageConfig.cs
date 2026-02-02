namespace ODBP.Features
{
    public static class StorageConfig
    {
        public const string ImagesPath = "/app/data/images";

        public static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(ImagesPath))
            {
                Directory.CreateDirectory(ImagesPath);
            }
        }
    }
}
