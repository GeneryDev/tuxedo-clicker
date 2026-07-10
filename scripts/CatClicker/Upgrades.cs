using GDF.Resources;

namespace CatClicker;

[LibraryAccessibleInEditor]
public partial class Upgrades : ResourceLibrary<Upgrade, Upgrade>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new LibraryConfig()
        {
            Roots = new []{new LibraryConfig.LibraryRoot("res://resources/upgrades")},
            CacheResources = true,
            PreloadAll = true
        };
    }
}