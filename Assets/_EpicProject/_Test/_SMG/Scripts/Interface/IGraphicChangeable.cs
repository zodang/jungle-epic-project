using Define;

public interface IGraphicChangeable
{
    GraphicType GetCurrentValue();
    void SetValue(GraphicType type);
}
