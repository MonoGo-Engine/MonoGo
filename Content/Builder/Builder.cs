using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MonoGame.Framework.Content.Pipeline.Builder;
using MonoGo.Pipeline.SpriteGroup;
using MonoGo.Pipeline.Tiled;
using System.Reflection;

/*#if DEBUG
using System.Diagnostics;
#endif*/

var contentCollectionArgs = new ContentBuilderParams()
{
    Mode = ContentBuilderMode.Builder,
    SourceDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../Assets")),
    WorkingDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../")),
    OutputDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../bin")),
    IntermediateDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../obj")),
    Platform = TargetPlatform.DesktopGL
};

Assembly.Load("MonoGo.Pipeline");

var builder = new Builder(contentCollectionArgs);

/*#if DEBUG
Debugger.Launch();
#endif*/

if (args is not null && args.Length > 0)
{
    builder.Run(args);
}
else
{
    builder.Run(contentCollectionArgs);
}

return builder.FailedToBuild > 0 ? -1 : 0;

public class Builder : ContentBuilder
{
    public ContentBuilderParams ContentCollectionArgs;

    public Builder(ContentBuilderParams contentCollectionArgs) : base()
    {
        ContentCollectionArgs = contentCollectionArgs;
    }

    public override IContentCollection GetContentCollection()
    {
        var contentCollection = new ContentCollection();

        contentCollection.Include<WildcardRule>("Engine/Effects/*.fx");
        contentCollection.Include<WildcardRule>("Engine/Fonts/*.spritefont");

        contentCollection.IncludeCopy<WildcardRule>("Engine/GUI/*.json");
        contentCollection.Include<WildcardRule>("Engine/GUI/*.png");

        contentCollection.Include<WildcardRule>("Demo/Maps/*.png");
        contentCollection.Include<WildcardRule>("*.tmx", new TiledMapImporter(), new PassThroughProcessor());        
        contentCollection.Include<WildcardRule>("*.spritegroup", new SpriteGroupImporter(), new SpriteGroupProcessor());

        return contentCollection;
    }
}
