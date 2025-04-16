using Microsoft.Xna.Framework;
using MonoGo.Engine;
using MonoGo.Engine.Drawing;
using MonoGo.Engine.SceneSystem;
using MonoGo.Engine.Utils.Tilemaps;
using MonoGo.Tiled.MapStructure;
using System;
using System.Collections.Generic;

namespace MonoGo.Tiled
{
    /// <summary>
    /// Basic map builder class. Creates a map from Tiled data structures.
    /// Can be extended to customize map building behavior.
    /// </summary>
    public class MapBuilder
    {
        /// <summary>
        /// Gets the Tiled map data structure used for building the map.
        /// </summary>
        public readonly TiledMap TiledMap;

        /// <summary>
        /// Gets or sets the scene representing the built map.
        /// </summary>
        public Scene MapScene { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the map has been successfully loaded.
        /// </summary>
        public bool Loaded { get; protected set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapBuilder"/> class.
        /// </summary>
        /// <param name="tiledMap">The Tiled map data structure to use for building the map.</param>
        public MapBuilder(TiledMap tiledMap) =>
            TiledMap = tiledMap;

        /// <summary>
        /// Builds the map scene from the Tiled map template.
        /// </summary>
        /// <remarks>
        /// Building occurs in four stages:
        /// - Building tilesets.
        /// - Building tile layers.
        /// - Building object layers.
        /// - Building image layers.
        /// 
        /// Each stage can be overridden for customization. Override this method for full control over map loading.
        /// </remarks>
        public virtual void Build()
        {
            if (!Loaded)
            {
                MapScene = SceneMgr.CreateScene(TiledMap.Name);

                var tilesets = BuildTilesets(TiledMap.Tilesets);

                BuildTileLayers(tilesets);
                BuildObjectLayers();
                BuildImageLayers();

                Loaded = true;
            }
        }

        /// <summary>
        /// Unloads the map scene and releases associated resources.
        /// </summary>
        public virtual void Destroy()
        {
            SceneMgr.DestroyScene(MapScene);
            MapScene = null;

            Loaded = false;
        }

        #region Map building.

        /// <summary>
        /// An array that maps tile indices to their corresponding tilesets.
        /// </summary>
        private Tileset[] _tilesetLookupMap;

        /// <summary>
        /// Builds tilesets from Tiled templates.
        /// </summary>
        /// <param name="tilesets">The array of Tiled tilesets to convert.</param>
        /// <returns>A list of converted tilesets.</returns>
        protected virtual List<Tileset> BuildTilesets(TiledMapTileset[] tilesets)
        {
            var convertedTilesets = new List<Tileset>();

            foreach (var tileset in tilesets)
            {
                var tilesetTilesList = new List<ITilesetTile>();

                if (tileset.Textures != null)
                {
                    for (var y = 0; y < tileset.Height; y += 1)
                    {
                        for (var x = 0; x < tileset.Width; x += 1)
                        {
                            var tile = tileset.Tiles[y * tileset.Width + x];
                            var tileTexture = tileset.Textures[tile.TextureID];
                            var frame = new Frame(tileTexture, tile.TexturePosition.ToRectangleF(), Vector2.Zero);
                            var tilesetTile = new BasicTilesetTile(frame);
                            tilesetTilesList.Add(tilesetTile);
                        }
                        // Note: Tileset origins in Tiled are in the left bottom corner.
                    }
                }

                var finalTileset = new Tileset(tilesetTilesList.ToArray(), tileset.Offset, tileset.FirstGID);
                convertedTilesets.Add(finalTileset);
            }

            return convertedTilesets;
        }

        /// <summary>
        /// Creates a lookup table to optimize tile matching for tilemaps.
        /// </summary>
        /// <param name="tilesets">The list of tilesets to include in the lookup table.</param>
        protected virtual void CreateTileLookupTable(List<Tileset> tilesets)
        {
            var tilesetIndex = 0;
            foreach (var tileset in tilesets)
            {
                var tilesetMaxIndex = tileset.StartingIndex + tileset.Count;
                if (tilesetIndex < tilesetMaxIndex)
                {
                    tilesetIndex = tilesetMaxIndex;
                }
            }
            _tilesetLookupMap = new Tileset[tilesetIndex + 1];

            foreach (var tileset in tilesets)
            {
                for (var i = 0; i < tileset.Count; i += 1)
                {
                    _tilesetLookupMap[tileset.StartingIndex + i] = tileset;
                }
            }
        }

        /// <summary>
        /// Retrieves the tileset associated with a given tile index.
        /// </summary>
        /// <param name="index">The tile index.</param>
        /// <returns>The corresponding tileset, or <c>null</c> if not found.</returns>
        protected Tileset GetTilesetFromTileIndex(int index)
        {
            try
            {
                return _tilesetLookupMap[index];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// Builds tile layers from Tiled templates.
        /// </summary>
        /// <param name="tilesets">The list of tilesets to use for building tile layers.</param>
        /// <returns>A list of built tile layers.</returns>
        protected virtual List<Layer> BuildTileLayers(List<Tileset> tilesets)
        {
            CreateTileLookupTable(tilesets);

            var layers = new List<Layer>();

            foreach (var tileLayer in TiledMap.TileLayers)
            {
                var layer = MapScene.CreateLayer(tileLayer.Name);
                layer.Priority = GetLayerPriority(tileLayer);

                var tilemap = new BasicTilemap(layer, tileLayer.Width, tileLayer.Height, tileLayer.TileWidth, tileLayer.TileHeight)
                {
                    Offset = tileLayer.Offset
                };

                for (var y = 0; y < tilemap.Height; y += 1)
                {
                    for (var x = 0; x < tilemap.Width; x += 1)
                    {
                        var tileIndex = tileLayer.Tiles[x][y].GID;

                        tilemap.SetTileUnsafe(
                            x, y,
                            new BasicTile(
                                tileIndex,
                                GetTilesetFromTileIndex(tileIndex),
                                tileLayer.Tiles[x][y].FlipHor,
                                tileLayer.Tiles[x][y].FlipVer,
                                tileLayer.Tiles[x][y].FlipDiag
                            )
                        );
                    }
                }

                layers.Add(layer);
            }

            return layers;
        }

        /// <summary>
        /// Builds object layers from Tiled templates.
        /// </summary>
        /// <returns>A list of built object layers.</returns>
        protected virtual List<Layer> BuildObjectLayers()
        {
            var layers = new List<Layer>();

            foreach (var objectLayer in TiledMap.ObjectLayers)
            {
                var layer = MapScene.CreateLayer(objectLayer.Name);
                layer.Priority = GetLayerPriority(objectLayer);

                foreach (var obj in objectLayer.Objects)
                {
                    TiledEntityFactoryPool.MakeEntity(obj, layer, this);
                }
                layers.Add(layer);
            }
            return layers;
        }

        /// <summary>
        /// Builds image layers from Tiled templates.
        /// </summary>
        /// <returns>A list of built image layers.</returns>
        protected virtual List<Layer> BuildImageLayers()
        {
            var layers = new List<Layer>();

            foreach (var imageLayer in TiledMap.ImageLayers)
            {
                var layer = MapScene.CreateLayer(imageLayer.Name);
                layer.Priority = GetLayerPriority(imageLayer);

                var frame = new Frame(
                    imageLayer.Texture,
                    imageLayer.Texture.Bounds.ToRectangleF(),
                    Vector2.Zero
                );
                new ImageLayerRenderer(layer, imageLayer.Offset, frame);

                layers.Add(layer);
            }

            return layers;
        }

        /// <summary>
        /// Retrieves the priority of a Tiled layer from its properties.
        /// </summary>
        /// <param name="layer">The Tiled layer.</param>
        /// <returns>The priority value, or 0 if not specified.</returns>
        protected int GetLayerPriority(TiledMapLayer layer)
        {
            try
            {
                return int.Parse(layer.Properties["priority"]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion Map building.
    }
}
