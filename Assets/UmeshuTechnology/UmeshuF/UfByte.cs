namespace Umeshu.Uf
{
    public static class UfByte
    {
        public static bool GetBit(this byte _byte, int _bitIndex) => (_byte & (1 << _bitIndex)) != 0;

        public static byte SetBit(this byte _byte, int _bitIndex, bool _value) => _value ? (byte)(_byte | (1 << _bitIndex)) : (byte)(_byte & ~(1 << _bitIndex));

        public static byte SetBits(this byte _byte, int _startBitIndex, int _bitCount, byte _value)
        {
            int _mask = _bitCount.GetMask();
            return (byte)((_byte & ~(_mask << _startBitIndex)) | ((_value & _mask) << _startBitIndex));
        }

        public static byte GetBits(this byte _byte, int _startBitIndex, int _bitCount)
        {
            int _mask = _bitCount.GetMask();
            return (byte)((_byte >> _startBitIndex) & _mask);
        }

        public static int GetMask(this int _bitCount) => (1 << _bitCount) - 1;
    }
}
