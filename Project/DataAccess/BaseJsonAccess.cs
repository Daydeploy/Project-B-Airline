public abstract class BaseJsonAccess<T> : IAccess<T>
{
    protected readonly GenericJsonAccess<T> JsonAccess;

    protected BaseJsonAccess(string filePath) => JsonAccess = new GenericJsonAccess<T>(filePath);

    public virtual List<T> LoadAll() => JsonAccess.LoadAll();

    public virtual void WriteAll(List<T> model) => JsonAccess.WriteAll(model);
}