using UnityEngine;
using Random = System.Random;

/// <summary>
/// Mathematics helpers for faster and best results.
/// </summary>
public static class Mathematics 
{
    /// <summary>
    /// Single variable for all requests.
    /// </summary>
    public static readonly Random random = new Random();

    /// <summary>
    /// Random Range between the specified a and b.
    /// </summary>
    /// <returns>The random Value.</returns>
    /// <param name="a">The Min value.</param>
    /// <param name="b">The Max value.</param>
    public static int Random(int a, int b)
    {
        return random.Next(a, b);
    }

    public static int Random(int max)
    {
        return random.Next(max);
    }

    public static string Number5()
    {
        return Random(11111, 99999).ToString("00000");
    }

    public static string Number6()
    {
        return Random(111111, 999999).ToString("000000");
    }

    public static Vector3 HorizontalCirclePoint(float angle)
    {
        return new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    /// <summary>
    /// min < value < max
    /// MIN < result > MAX
    /// </summary>
    public static float Interpolate(float value, float min, float max, float MIN, float MAX)
    {
        return Mathf.Lerp(MIN, MAX, Mathf.InverseLerp(min, max, value));
    }

    /// <summary>
    /// min < value < max
    /// MIN < result > MAX
    /// </summary>
    public static Vector2 Interpolate(Vector2 value, Vector2 min, Vector2 max, Vector2 MIN, Vector2 MAX)
    {
        value.x = Interpolate(value.x, min.x, max.x, MIN.x, MAX.x);
        value.y = Interpolate(value.y, min.y, max.y, MIN.y, MAX.y);
        return value;
    }

    public static float Distance(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    /// <summary>
    /// To find closest Texture size for compression
    /// </summary>
    public static int ClosestMultipleOf4(int value)
    {
        return (value + 3) & ~0x03;
    }

    public static float PingPong01()
    {
        return Mathf.PingPong(Time.time, 1.0f);
    }

    public static float PercentToCoef(float percent)
    {
        return 1.0f + percent / 100;
    }
}