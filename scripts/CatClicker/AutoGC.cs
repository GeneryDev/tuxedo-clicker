using System;
using GDF.Util;

namespace CatClicker;

public partial class AutoGC : SingletonNode<AutoGC>
{
    public static void CollectAndWait()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
    
    public override void _Notification(int what)
    {
        base._Notification(what);
        // For some godforsaken reason, some stale uncollected array references are causing crashes on exit.
        // Never had this happen before on other projects prior to 4.6.
        if (what == NotificationWMCloseRequest)
        {
            CollectAndWait();
        }
    }
}