using System.Security.Cryptography;
using System.Text;

public static class Util
{
    public static string GenerateRamdomKey(int length = 5)
    {
        var pattern = @"qazwsxedcrfvtgbyhnujmiklopQAZWSXEDCRFVTGBYHNUJMIKLOP0123456789!";
        var sb = new StringBuilder();
        var rd = new Random();
        for (int i = 0; i < length; i++)
        {
            sb.Append(pattern[rd.Next(0, pattern.Length)]);
        }

        return sb.ToString();
    }
}
