// Abstract Command Interface
public interface ICommand<T> where T : struct
{
    void Execute(T args);
}
