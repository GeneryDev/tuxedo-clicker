using GDF.Resources;

namespace TuxedoClicker;

[LibraryAccessibleInEditor]
public partial class Effects : ResourceLibrary<Effect, Effect>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new LibraryConfig()
        {
            Roots = new []{new LibraryConfig.LibraryRoot("res://resources/effects")},
            CacheResources = true,
            PreloadAll = true
        };
    }
}