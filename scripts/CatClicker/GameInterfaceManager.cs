using GDF.Util;
using Godot;
using System.Numerics;
using System.Text;

namespace CatClicker;

public partial class GameInterfaceManager : SingletonNode<GameInterfaceManager>
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    private int _bulkPurchaseAmount = 1;
    
    public int GetBulkPurchaseAmount()
    {
        return _bulkPurchaseAmount;
    }
    public void SetBulkPurchaseAmount(int amount)
    {
        _bulkPurchaseAmount = amount;
        EmitSignalUpdated();
    }

    private void Update()
    {
        EmitSignalUpdated();
    }

    private StringBuilder _sb = new();

    public string FormatNumber(BigInteger n)
    {
        _sb.Clear();
        if (n < 0)
        {
            _sb.Append('-');
            n = BigInteger.Abs(n);
        }

        FormatIntegerLog10(n, _sb, ShortScaleNumberSuffixes);
        return _sb.ToString();
    }
    public string FormatTime(double sec)
    {
        _sb.Clear();
        if (sec < 0)
        {
            _sb.Append('-');
            sec *= -1;
        }

        FormatInteger((int)sec, _sb, TimeSuffixes);
        return _sb.ToString();
    }


    private static readonly (int Magnitude, string Suffix)[] ShortScaleNumberSuffixes = new[]
    {
        (3*1+3, "million"),
        (3*2+3, "billion"),
        (3*3+3, "trillion"),
        (3*4+3, "quadrillion"),
        (3*5+3, "quintillion"),
        (3*6+3, "sextillion"),
        (3*7+3, "septillion"),
        (3*8+3, "octillion"),
        (3*9+3, "nonillion"),
        (3*10+3, "decillion"),
        (3*11+3, "undecillion"),
        (3*12+3, "duodecillion"),
    };
    private static readonly (int Magnitude, string Suffix)[] LongScaleNumberSuffixes = new[]
    {
        (6*1, "million"),
        (6*2, "billion"),
        (6*3, "trillion"),
        (6*4, "quadrillion"),
        (6*5, "quintillion"),
        (6*6, "sextillion"),
        (6*7, "septillion"),
        (6*8, "octillion"),
        (6*9, "nonillion"),
        (6*10, "decillion"),
        (6*11, "undecillion"),
        (6*12, "duodecillion"),
    };
    private static readonly (int Magnitude, string Suffix)[] TimeSuffixes = new[]
    {
        (1, "seconds"),
        (60, "minutes"),
        (60*60, "hours"),
        (60*60*24, "days"),
        (60*60*24*30, "months"),
        (60*60*24*365, "years")
    };

    private static readonly BigInteger Ten = new BigInteger(10);
    private static void FormatIntegerLog10(BigInteger n, StringBuilder sb, (int Magnitude, string Suffix)[] suffixes)
    {
        if (n.IsZero)
        {
            sb.Append('0');
            return;
        }
        int log10 = Mathf.FloorToInt(BigInteger.Log10(n));

        for (int i = suffixes.Length - 1; i >= 0; i--)
        {
            var entry = suffixes[i];
            if (log10 >= entry.Magnitude)
            {
                if (entry.Suffix == null)
                {
                    sb.Append(n.ToString("N0"));
                }
                else
                {
                    sb.Append(((double)(n * Ten / BigInteger.Pow(10, entry.Magnitude)) / 10).ToString("N1"));
                    sb.Append(' ');
                    sb.Append(entry.Suffix);
                }

                return;
            }
        }

        sb.Append(n.ToString("N0"));
        return;
    }
    private static void FormatInteger(BigInteger n, StringBuilder sb, (int Magnitude, string Suffix)[] suffixes)
    {
        if (n.IsZero)
        {
            sb.Append('0');
            return;
        }
        for (int i = suffixes.Length - 1; i >= 0; i--)
        {
            var entry = suffixes[i];
            if (n >= entry.Magnitude)
            {
                if (entry.Suffix == null)
                {
                    sb.Append(n.ToString("N0"));
                }
                else
                {
                    sb.Append(((double)(n * Ten / entry.Magnitude) / 10).ToString("N1"));
                    sb.Append(' ');
                    sb.Append(entry.Suffix);
                }

                return;
            }
        }

        sb.Append(n.ToString("N0"));
        return;
    }
}