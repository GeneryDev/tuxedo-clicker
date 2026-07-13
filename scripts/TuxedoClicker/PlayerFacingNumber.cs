using System.Numerics;

namespace TuxedoClicker;

public struct PlayerFacingNumber
{
    public const int BigThreshold = 1_000;
    
    public decimal DecimalValue;
    public bool IsBig;
    public BigInteger BigIntegerValue;

    public PlayerFacingNumber(decimal decimalValue)
    {
        if (decimalValue >= BigThreshold)
        {
            BigIntegerValue = new BigInteger(decimalValue);
            IsBig = true;
        }
        else
        {
            DecimalValue = decimalValue;
            IsBig = false;
        }
    }

    public PlayerFacingNumber(BigInteger bigIntegerValue)
    {
        if (bigIntegerValue >= BigThreshold)
        {
            BigIntegerValue = bigIntegerValue;
            IsBig = true;
        }
        else
        {
            DecimalValue = (decimal)bigIntegerValue;
            IsBig = false;
        }
    }

    public override string ToString()
    {
        if (IsBig)
        {
            return GameInterfaceManager.Instance.FormatNumber(BigIntegerValue);
        }
        else
        {
            return DecimalValue.ToString("N1");
        }
    }
}