using MonoGame.AssetService.Builder;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGo.Pipeline.SpriteGroup;
using System.Reflection;

/*#if DEBUG
using System.Diagnostics;
Debugger.Launch();
#endif*/

var contentCollectionArgs = new ContentBuilderParams()
{
    Mode = ContentBuilderMode.Builder,
    SourceDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../Assets")),
    WorkingDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../")),
    OutputDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../bin")),
    IntermediateDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../obj"))
};

Assembly.Load("MonoGo.Pipeline");
Assembly.Load("MonoGame.AssetService.Builder");

var builder = new Builder();

if (args is not null && args.Length > 0)
{
    builder.Run(args);
}
else
{
    builder.Run(contentCollectionArgs);
}

if (builder.FailedToBuild == 0)
{
    builder.WriteAssetManifest();
}

return builder.FailedToBuild > 0 ? -1 : 0;

public class Builder : ContentBuilder
{
    public override IContentCollection GetContentCollection()
    {
        var contentCollection = new ContentCollection();

        contentCollection.Include<WildcardRule>("Engine/Effects/*.fx");
        contentCollection.Include<WildcardRule>("Engine/Fonts/*.spritefont");
        contentCollection.IncludeCopy<WildcardRule>("Game/GUI/*.json");
        contentCollection.Include<WildcardRule>("Game/GUI/*.png");       
        contentCollection.Include<WildcardRule>("*.spritegroup", new SpriteGroupImporter(), new SpriteGroupProcessor());

        return contentCollection;
    }

    public void WriteAssetManifest()
    {
        var writer = new AssetManifestWriter(
        [
            new SpriteGroupManifestContributor(),
        ]);

        writer.Write(Parameters, GetContentCollection(), Logger);
    }
}
