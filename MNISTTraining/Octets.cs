
namespace MNSITTraining;

internal static class Octets
{

    //Octet-swaps inspired by: https://stackoverflow.com/questions/217980/c-sharp-little-endian-or-big-endian

    public static short SwapOctets(this short value)
    {
        return (short) (value >> 8 | ((value & 0xff) << 8));
    }

    public static int SwapOctets(this int value)
    {
        return value >> 24 | ((value & 0xff0000) >> 8) | ((value & 0xff00) << 8) | ((value & 0xff) << 24);
    }

    public static int FromBigEndian(this int value)
    {
        if(!BitConverter.IsLittleEndian) return value;
        return value.SwapOctets();
    }

    public static int FromLittleEndian(this int value)
    {
        if(BitConverter.IsLittleEndian) return value;
        return value.SwapOctets();
    }
}
