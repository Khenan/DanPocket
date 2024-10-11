using UnityEngine;

/// <summary>
/// Provides extension methods for Vector2, Vector3 and Transform types.
/// </summary>
public static class UfVector
{
    /// <summary>
    /// Converts a Vector2 to a Vector3.
    /// </summary>
    /// <param name="_value">The Vector2 to convert.</param>
    /// <returns>The converted Vector3.</returns>
    public static Vector3 ToVector3(this Vector2 _value) => _value;

    /// <summary>
    /// Converts a Vector3 to a Vector2.
    /// </summary>
    /// <param name="_value">The Vector3 to convert.</param>
    /// <returns>The converted Vector2.</returns>
    public static Vector2 ToVector2(this Vector3 _value) => _value;

    public static bool IsApproximately(this Vector3 _vector, Vector3 _other, float _tolerance = 0.01f) =>
        Mathf.Abs(_vector.x - _other.x) < _tolerance && Mathf.Abs(_vector.y - _other.y) < _tolerance && Mathf.Abs(_vector.z - _other.z) < _tolerance;

    /// <summary>
    /// Creates a new Vector3 with the specified x, y, and z values. If any value is not provided, the corresponding value from the original Vector3 is used.
    /// </summary>
    /// <param name="_vector">The original Vector3.</param>
    /// <param name="_x">The x value for the new Vector3. If not provided, the x value from the original Vector3 is used.</param>
    /// <param name="_y">The y value for the new Vector3. If not provided, the y value from the original Vector3 is used.</param>
    /// <param name="_z">The z value for the new Vector3. If not provided, the z value from the original Vector3 is used.</param>
    /// <returns>A new Vector3 with the specified x, y, and z values.</returns>
    public static Vector3 With(this Vector3 _vector, float _x = float.NaN, float _y = float.NaN, float _z = float.NaN)
    {
        if (float.IsNaN(_x)) _x = _vector.x;
        if (float.IsNaN(_y)) _y = _vector.y;
        if (float.IsNaN(_z)) _z = _vector.z;
        return new Vector3(_x, _y, _z);
    }

    public static Vector2 With(this Vector2 _vector, float _x = float.NaN, float _y = float.NaN, float _z = float.NaN) => _vector.ToVector3().With(_x, _y, _z).ToVector2();

    /// <summary>
    /// Creates a new Vector3 by adding the specified x, y, and z values to the original Vector3. If any value is not provided, zero is used.
    /// </summary>
    /// <param name="_vector">The original Vector3.</param>
    /// <param name="_x">The x value to add. If not provided, zero is used.</param>
    /// <param name="_y">The y value to add. If not provided, zero is used.</param>
    /// <param name="_z">The z value to add. If not provided, zero is used.</param>
    /// <returns>A new Vector3 with the added x, y, and z values.</returns>
    public static Vector3 WithAdded(this Vector3 _vector, float _x = 0, float _y = 0, float _z = 0) =>
        new(_vector.x + _x, _vector.y + _y, _vector.z + _z);

    /// <summary>
    /// Sets the position of a Transform with the specified x, y, and z values. If any value is not provided, the corresponding value from the original position is used.
    /// </summary>
    /// <param name="_transform">The Transform to set the position of.</param>
    /// <param name="_x">The x value for the new position. If not provided, the x value from the original position is used.</param>
    /// <param name="_y">The y value for the new position. If not provided, the y value from the original position is used.</param>
    /// <param name="_z">The z value for the new position. If not provided, the z value from the original position is used.</param>
    public static void SetPosWith(this Transform _transform, float _x = float.NaN, float _y = float.NaN, float _z = float.NaN) =>
        _transform.position = _transform.position.With(_x, _y, _z);

    /// <summary>
    /// Adds the specified x, y, and z values to the position of the Transform. If any value is not provided, zero is used.
    /// </summary>
    /// <param name="_transform">The Transform to add the position to.</param>
    /// <param name="_x">The x value to add. If not provided, zero is used.</param>
    /// <param name="_y">The y value to add. If not provided, zero is used.</param>
    /// <param name="_z">The z value to add. If not provided, zero is used.</param>
    public static void AddToPos(this Transform _transform, float _x = 0, float _y = 0, float _z = 0) =>
        _transform.position = _transform.position.WithAdded(_x, _y, _z);
}
