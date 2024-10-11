using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Provides extension methods for Color.
/// </summary>
public static class UfColor
{
    #region Colors
    public static Color Red { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, 0, 0f, 1f); } }
    public static Color Orange { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, .5f, 0f, 1f); } }
    public static Color Yellow { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, 1f, 0f, 1f); } }
    public static Color Green { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 1f, 0f, 1f); } }
    public static Color Cyan { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 1f, 1f, 1f); } }
    public static Color Blue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 0f, 1f, 1f); } }
    public static Color Purple { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, 0f, 1f, 1f); } }
    public static Color Magenta { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, 0f, 1f, 1f); } }
    public static Color White { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, 1f, 1f, 1f); } }
    public static Color Black { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 0f, 0f, 1f); } }
    public static Color Grey { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, .5f, .5f, 1f); } }
    public static Color Clear { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 0f, 0f, 0f); } }
    public static Color Pink { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, 0, .5f, 1f); } }
    public static Color Brown { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, .25f, 0f, 1f); } }
    public static Color Turquoise { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 1f, .5f, 1f); } }
    public static Color Lime { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, 1f, 0f, 1f); } }
    public static Color Teal { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, .5f, .5f, 1f); } }
    public static Color Maroon { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, 0f, 0f, 1f); } }
    public static Color Olive { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, .5f, 0f, 1f); } }
    public static Color Navy { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, 0f, .5f, 1f); } }
    public static Color Indigo { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.25f, 0f, .5f, 1f); } }
    public static Color Violet { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, 0f, .5f, 1f); } }
    public static Color Peach { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, .5f, .5f, 1f); } }
    public static Color Coral { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, .5f, .25f, 1f); } }
    public static Color Mint { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, 1f, .5f, 1f); } }
    public static Color Lavender { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, .5f, 1f, 1f); } }
    public static Color Beige { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, 1f, .5f, 1f); } }
    public static Color Tan { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, .5f, 0f, 1f); } }
    public static Color Khaki { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.5f, .5f, .25f, 1f); } }
    public static Color Salmon { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, .5f, .25f, 1f); } }
    public static Color CoralRed { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, .25f, .25f, 1f); } }
    public static Color SkyBlue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.25f, .5f, 1f, 1f); } }
    public static Color SeaGreen { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, .5f, .25f, 1f); } }
    public static Color ForestGreen { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(0f, .25f, 0f, 1f); } }
    public static Color OliveGreen { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.25f, .5f, 0f, 1f); } }
    public static Color Gold { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(1f, .84f, 0f, 1f); } }
    public static Color Silver { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return new Color(.75f, .75f, .75f, 1f); } }
    #endregion


    public static Color With(this Color _color, float _r = float.NaN, float _g = float.NaN, float _b = float.NaN, float _a = float.NaN)
    {
        if (float.IsNaN(_r)) _r = _color.r;
        if (float.IsNaN(_g)) _g = _color.g;
        if (float.IsNaN(_b)) _b = _color.b;
        if (float.IsNaN(_a)) _a = _color.a;
        return new Color(_r, _g, _b, _a);
    }

    public static Color32 With(this Color32 _color, byte? _r = null, byte? _g = null, byte? _b = null, byte? _a = null)
    {
        _r ??= _color.r;
        _g ??= _color.g;
        _b ??= _color.b;
        _a ??= _color.a;
        return new Color32(_r.Value, _g.Value, _b.Value, _a.Value);
    }

    public static Color IntToColor(this int _colorInt)
    {
        System.Random _randomInt = new(_colorInt);
        int _randomizedInt = _randomInt.Next();

        byte _r = (byte)((_randomizedInt >> 16) & 0xFF);
        byte _g = (byte)((_randomizedInt >> 8) & 0xFF);
        byte _b = (byte)(_randomizedInt & 0xFF);

        byte _a = 255;

        return new Color32(_r, _g, _b, _a);
    }

    #region Log
    private const float LOG_EXCLUDE_HUE_START = 0.55f;
    private const float LOG_EXCLUDE_HUE_END = 0.75f;
    private const float LOG_EXCLUDE_HUE_RANGE = LOG_EXCLUDE_HUE_END - LOG_EXCLUDE_HUE_START;
    private const float LOG_AVAILABLE_HUE = 1f - LOG_EXCLUDE_HUE_RANGE;
    private const float LOG_MIN_SATURATION = 0.8f;
    private const float LOG_MIN_BRIGHTNESS = 0.8f;

    public static Color RandomLogColor() => Random.ColorHSV().SetColorLogFriendly();
    public static Color SetColorLogFriendly(this Color _color)
    {
        Color.RGBToHSV(_color, out float _h, out float _s, out float _v);
        float _convertedHue = _h * LOG_AVAILABLE_HUE;
        if (_convertedHue > LOG_EXCLUDE_HUE_START) _convertedHue += LOG_EXCLUDE_HUE_RANGE;
        float _convertedSaturation = _s * (1 - LOG_MIN_SATURATION) + LOG_MIN_SATURATION;
        float _convertedBrightness = _v * (1 - LOG_MIN_BRIGHTNESS) + LOG_MIN_BRIGHTNESS;
        return Color.HSVToRGB(_convertedHue, _convertedSaturation, _convertedBrightness);
    }

    public static Color IntToLogColor(this int _colorInt) => IntToColor(_colorInt).SetColorLogFriendly();

    internal static string ColorToHex(Color _color) => ColorUtility.ToHtmlStringRGB(_color);
    internal static Color HexToColor(string _value) => ColorUtility.TryParseHtmlString(_value, out Color _color) ? _color : Color.white;
    public static Color GreyAt(float _lerpToWhite) => Color.Lerp(Color.black, Color.white, _lerpToWhite);


    #endregion
}
