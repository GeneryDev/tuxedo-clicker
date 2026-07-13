using GDF.Resources;

namespace TuxedoClicker;

[LibraryAccessibleInEditor]
public partial class PointGenerators : ResourceLibrary<PointGenerator, PointGenerator>
{
    public override LibraryConfig GetLibraryConfig()
    {
        return new LibraryConfig()
        {
            Roots = new []{new LibraryConfig.LibraryRoot("res://resources/point_generators")},
            CacheResources = true,
            PreloadAll = true
        };
    }
}