using UnityEngine;

public class TMPText_AnimationData
{
    public Vector2 offset = Vector2.zero;
    public float scale = 1;
    public float rotation = 0;

    public float timeAtCharacterCreation;
    public float time;

    public Color32 color;
    public string word;
    public char character;
    public int characterIndex;

    public float GetPercentageOfApparition(float _time, float _characterApparitionOffset)
    {
        float _timeOffsetByCharacter = (timeAtCharacterCreation + characterIndex * _characterApparitionOffset);

        return Mathf.Clamp01((time - _timeOffsetByCharacter) / _time);
    }
}
