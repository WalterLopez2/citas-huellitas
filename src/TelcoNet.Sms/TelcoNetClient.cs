namespace TelcoNet.Sms;

public static class TelcoNetClient
{
    public const int Ok = 0;
    public const int InvalidNumber = 21;

    public static int SendMessage(string msisdn, string body)
    {
        if (msisdn.Length != 11 || !msisdn.StartsWith("504") || !msisdn.All(char.IsDigit))
            return InvalidNumber;

        Console.WriteLine($"[TelcoNet] SMS a +{msisdn}: {body}");
        return Ok;
    }
}
