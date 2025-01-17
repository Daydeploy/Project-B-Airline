public interface IAccess<T>
{
    List<T> LoadAll();
    void WriteAll(List<T> data);
}