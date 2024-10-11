using Umeshu.Utility;
using UnityEngine;

namespace Umeshu.Uf
{
    public static partial class UfMath // Noise Methods
    {
        public static float[,] GenerateNoise(NoiseSettings _noiseSettings) => GenerateNoise
        (
            _seed: _noiseSettings.Seed,
            _width: _noiseSettings.GenerateSize(),
            _height: _noiseSettings.GenerateSize(),
            _scale: _noiseSettings.GenerateNoiseScale(),
            _octave: _noiseSettings.GenerateNoiseOctave(),
            _persistance: _noiseSettings.GenerateNoisePersistance(),
            _lacunarity: _noiseSettings.GenerateNoiseLacunarity()
        );

        public static float[,] GenerateNoise(int _seed, int _width, int _height, float _scale, int _octave, float _persistance, float _lacunarity)
        {
            System.Random _rng = new(_seed);
            Vector2[] _octaveOffset = new Vector2[_octave];
            for (int _i = 0; _i < _octave; _i++)
            {
                float _offsetY = _rng.Next(-100000, 100000);
                float _offsetX = _rng.Next(-100000, 100000);
                _octaveOffset[_i] = new Vector2(_offsetX, _offsetY);
            }
            float[,] _noiseMap = new float[_width, _height];
            if (_scale <= 0) _scale = 0.0001f;
            float _minNoiseHeight = float.MaxValue;
            float _maxNoiseHeight = float.MinValue;
            float _halfWidth = _width / 2f;
            float _halfHeight = _height / 2f;

            for (int _y = 0; _y < _height; _y++)
            {
                for (int _x = 0; _x < _width; _x++)
                {
                    float _amplitude = 1;
                    float _frequency = 1;
                    float _noiseHeight = 0;
                    for (int _i = 0; _i < _octave; _i++)
                    {
                        float _sampleX = (_x - _halfWidth + _octaveOffset[_i].x) / _scale * _frequency;
                        float _sampleY = (_y - _halfHeight + _octaveOffset[_i].y) / _scale * _frequency;
                        float _perlinValue = Mathf.PerlinNoise(_sampleX, _sampleY) * 2 - 1;
                        _noiseHeight += _perlinValue * _amplitude;
                        _amplitude *= _persistance;
                        _frequency *= _lacunarity;
                    }
                    if (_noiseHeight > _maxNoiseHeight) _maxNoiseHeight = _noiseHeight;
                    else if (_noiseHeight < _minNoiseHeight) _minNoiseHeight = _noiseHeight;
                    _noiseMap[_x, _y] = _noiseHeight;
                }
            }
            for (int _y = 0; _y < _height; _y++)
            {
                for (int _x = 0; _x < _width; _x++)
                {
                    _noiseMap[_x, _y] = Mathf.InverseLerp(_minNoiseHeight, _maxNoiseHeight, _noiseMap[_x, _y]);
                }
            }

            return _noiseMap;
        }

        [System.Serializable]
        public class NoiseSettings
        {
            [field: SerializeField]
            public int Seed { get; private set; }
            private System.Random random;
            private float NextRandomValue => random.Next() / (float)int.MaxValue;
            [SerializeField, MinMaxRange(1, 1000)]
            IntRange sizeRange;
            [SerializeField, MinMaxRange(.01f, 1000)]
            FloatRange noiseScale;
            [SerializeField, MinMaxRange(1, 8)]
            IntRange noiseOctaveRange;
            [SerializeField, MinMaxRange(0, 1)]
            FloatRange noisePersistance;
            [SerializeField, MinMaxRange(1, 100)]
            FloatRange noiseLacunarity;

            public void Start(int _seed)
            {
                this.Seed = _seed;
                random = new System.Random(_seed);
            }
            public void RegenerateSeed() => Start(Seed);
            public void GenerateSeed() => Start((int)(Random.value * int.MaxValue));
            public int GenerateSize() => sizeRange.GetIntValueAt(NextRandomValue);
            public float GenerateNoiseScale() => noiseScale.GetValueAt(NextRandomValue);
            public int GenerateNoiseOctave() => noiseOctaveRange.GetIntValueAt(NextRandomValue);
            public float GenerateNoisePersistance() => noisePersistance.GetValueAt(NextRandomValue);
            public float GenerateNoiseLacunarity() => noiseLacunarity.GetValueAt(NextRandomValue);

        }
    }
}
