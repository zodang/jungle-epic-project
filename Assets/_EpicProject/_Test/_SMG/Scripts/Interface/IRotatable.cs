public interface IRotatable
{
    float GetMinValue(); // 다이얼의 최소값
    float GetMaxValue(); // 다이얼의 최대값
    float GetCurrentValue(); // 다이얼의 시작값
    void SetValue(float value); // 다이얼의 값 변경 시마다 호출
}
