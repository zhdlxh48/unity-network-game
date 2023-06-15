using System.Text;

namespace ServerLib;

public static class StringUtility
{
    public static Encoding DefaultEncoding = Encoding.UTF8;
    
    public static byte[] ToBuffer(this string str, Encoding? encoding = null)
    {
        if (encoding == null)
        {
            return DefaultEncoding.GetBytes(str);
        }

        return encoding.GetBytes(str);
    }
        
    public static Memory<byte> ToMemory(this string str, Encoding? encoding = null)
    {
        return ToBuffer(str, encoding).AsMemory();
    }
        
    public static string ToMessage(this Memory<byte> mem, Encoding? encoding = null)
    {
        return ToMessage(mem.ToArray(), encoding);
    }
        
    public static string ToMessage(this byte[] buf, Encoding? encoding = null)
    {
        if (encoding == null)
        {
            return DefaultEncoding.GetString(buf);
        }

        return encoding.GetString(buf);
    }
}