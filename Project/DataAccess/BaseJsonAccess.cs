public abstract class BaseJsonAccess<T> : IDataAccess<T>
{
    protected readonly GenericJsonAccess<T> JsonAccess;
    public string FileName { get; set; }

    public BaseJsonAccess(string fileName)
    {
        FileName = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, fileName));
        JsonAccess = new GenericJsonAccess<T>(fileName);
    }

    public virtual List<T> LoadAll() => JsonAccess.LoadAll();
    public virtual void WriteAll(List<T> items) => JsonAccess.WriteAll(items);
}