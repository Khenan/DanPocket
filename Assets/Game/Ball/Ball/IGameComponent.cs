public interface IGameElementComponent
{
    void InitGameElementManager(IGameElementManager _gameElementManager);
    void ComponentAwake();
    void ComponentStart();
    void ComponentOnEnable();
    void ComponentOnDisable();
    void ComponentUpdate();
    void ComponentFixedUpdate();
}
