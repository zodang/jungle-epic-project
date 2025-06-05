public interface IScalable
{
    float GetMinValue(); // 슬라이더의 최소값
    float GetMaxValue(); // 슬라이더의 최대값
    float GetCurrentValue(); // 슬라이더의 시작값
    void SetValue(float value); // 슬라이더 값 변경 시마다 호출
}

