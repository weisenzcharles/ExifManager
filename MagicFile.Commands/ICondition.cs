namespace MagicFile
{
    public interface ICondition
    {
        bool IsSatisfyThisCondition(FileInfo file);
    }
}
