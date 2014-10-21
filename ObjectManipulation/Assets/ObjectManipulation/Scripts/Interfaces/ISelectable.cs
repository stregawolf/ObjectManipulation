
/// <summary>
/// ISelectable provides a standard interface for selecting something
/// </summary>
/// <typeparam name="T">The class that will need to be passed in when calling the Select method</typeparam>
public interface ISelectable<T> {
    bool IsSelected();
    void Select(T selector);
    void Deselect();
}
