using UnityEngine;
using UnityEngine.UI;

public class ArcSlider : Slider
{
    [Header("Arc Settings")]
    [SerializeField] private float startAngle = -60f;
    [SerializeField] private float endAngle = 60f;
    [SerializeField] private float radius = 100f;

    [Header("Curve Scaling")]
    [SerializeField]
    private AnimationCurve vertexCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 30, 0, 0.01f), new Keyframe(0.5f, 0.25f), new Keyframe(1, 0, -30, 0, 0.01f, 0));
    // Cho phép bạn vẽ curve trong Inspector để điều chỉnh

    protected override void Set(float input, bool sendCallback)
    {
        base.Set(input, sendCallback);

        if (handleRect != null)
        {
            float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);

            // Lerp góc
            float angle = Mathf.Lerp(startAngle, endAngle, normalizedValue);

            // Xác định toạ độ tròn cơ bản
            float rad = angle * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            // Áp dụng curve scale theo giá trị (từ 0 → 1)
            float yScale = vertexCurve.Evaluate(normalizedValue);

            // Scale X, Y khác nhau nếu cần (ở đây scale Y thôi)
            pos = new Vector2(pos.x, pos.y * yScale) * radius;

            handleRect.anchoredPosition = pos;
            handleRect.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}
