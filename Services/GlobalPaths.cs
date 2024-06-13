namespace summeringsMakker.Services;

public static class GlobalPaths
{
    private static string _projectRootPath;

    public static string ProjectRootPath
    {
        get
        {
            if (string.IsNullOrEmpty(_projectRootPath))
            {
                string binPath = AppDomain.CurrentDomain.BaseDirectory;
                _projectRootPath = Directory.GetParent(binPath).Parent.Parent.Parent.FullName;
            }
            return _projectRootPath;
        }
    }
}