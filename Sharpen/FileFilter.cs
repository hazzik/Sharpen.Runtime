namespace Sharpen
{
    public interface FileFilter
    {
        bool Accept(FilePath filePath);
    }
}