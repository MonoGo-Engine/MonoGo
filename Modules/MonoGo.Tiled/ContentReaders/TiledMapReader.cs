﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGo.Tiled.MapStructure;
using MonoGo.Tiled.MapStructure.Objects;
using System.Collections.Generic;
using System.IO;

namespace MonoGo.Tiled.ContentReaders
{
    /// <summary>
    /// Reads Tiled map data from a content pipeline.
    /// </summary>
    public class TiledMapReader : ContentTypeReader<TiledMap>
	{
        /// <summary>
        /// Reads a Tiled map from the content pipeline.
        /// </summary>
        /// <param name="input">The content reader used to read the map data.</param>
        /// <param name="existingInstance">An existing instance of <see cref="TiledMap"/>, if available.</param>
        /// <returns>A new instance of <see cref="TiledMap"/> populate
        protected override TiledMap Read(ContentReader input, TiledMap existingInstance)
		{
			var tiledMap = new TiledMap
			{
				Name = input.AssetName,
				BackgroundColor = input.ReadObject<Color?>(),
				Width = input.ReadInt32(),
				Height = input.ReadInt32(),
				TileWidth = input.ReadInt32(),
				TileHeight = input.ReadInt32(),
				RenderOrder = (RenderOrder)input.ReadByte(),
				Orientation = (Orientation)input.ReadByte(),
				StaggerAxis = (StaggerAxis)input.ReadByte(),
				StaggerIndex = (StaggerIndex)input.ReadByte(),
				HexSideLength = input.ReadInt32()
			};

			ReadTilesets(input, tiledMap);
			ReadTileLayers(input, tiledMap);
			ReadObjectLayers(input, tiledMap);
			ReadImageLayers(input, tiledMap);

			tiledMap.Properties = input.ReadObject<Dictionary<string, string>>();
			return tiledMap;
		}

        #region Tilesets

        /// <summary>
        /// Reads the tilesets from the content reader and populates the Tiled map.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to populate.</param>
        private void ReadTilesets(ContentReader input, TiledMap map)
		{
			var tilesetsCount = input.ReadInt32();
			var tilesets = new TiledMapTileset[tilesetsCount];

			for (var i = 0; i < tilesetsCount; i += 1)
			{
				tilesets[i] = new TiledMapTileset();

				tilesets[i].Name = input.ReadString();
				tilesets[i].TexturePaths = input.ReadObject<string[]>();

				if (input.ReadBoolean())
				{
					var texturesCount = input.ReadInt32();
					tilesets[i].Textures = new Texture2D[texturesCount];
					for (var k = 0; k < texturesCount; k += 1)
					{
						var path = Path.Combine(Path.GetDirectoryName(input.AssetName), input.ReadString());
						tilesets[i].Textures[k] = input.ContentManager.Load<Texture2D>(path);
					}
				}

				tilesets[i].FirstGID = input.ReadInt32();
				tilesets[i].TileWidth = input.ReadInt32();
				tilesets[i].TileHeight = input.ReadInt32();
				tilesets[i].Spacing = input.ReadInt32();
				tilesets[i].Margin = input.ReadInt32();
				tilesets[i].TileCount = input.ReadInt32();
				tilesets[i].Columns = input.ReadInt32();
				tilesets[i].Offset = input.ReadVector2();
				
				var tiles = new TiledMapTilesetTile[tilesets[i].TileCount];
				for (var k = 0; k < tiles.Length; k += 1)
				{
					tiles[k] = ReadTilesetTile(input, map);
					tiles[k].Tileset = tilesets[i];
				}
				tilesets[i].Tiles = tiles;
				tilesets[i].BackgroundColor = input.ReadObject<Color?>();
				tilesets[i].Properties = input.ReadObject<Dictionary<string, string>>();
			}
			map.Tilesets = tilesets;
		}


        /// <summary>
        /// Reads a single tileset tile from the content reader.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to which the tile belongs.</param>
        /// <returns>A populated <see cref="TiledMapTilesetTile"/> instance.</returns>
        private TiledMapTilesetTile ReadTilesetTile(ContentReader input, TiledMap map)
		{
			var tile = new TiledMapTilesetTile();
			tile.GID = input.ReadInt32();
			tile.TextureID = input.ReadInt32();
			tile.TexturePosition = input.ReadObject<Rectangle>();
			tile.ObjectsDrawingOrder = (TiledMapObjectDrawingOrder)input.ReadByte();
			
			var objectsCount = input.ReadInt32();
			var objects = new TiledObject[objectsCount];

			for (var k = 0; k < objectsCount; k += 1)
			{
				objects[k] = ReadObject(input, map);
			}
			tile.Objects = objects;
			tile.Properties = input.ReadObject<Dictionary<string, string>>();
			
			return tile;
		}

        #endregion Tilesets

        /// <summary>
        /// Reads a generic Tiled map layer from the content reader.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="layer">The layer to populate.</param>
        private void ReadLayer(ContentReader input, TiledMapLayer layer)
		{
			layer.Name = input.ReadString();
			layer.ID = input.ReadInt32();
			layer.Visible = input.ReadBoolean();
			layer.Opacity = input.ReadSingle();
			layer.Offset = input.ReadVector2();
			layer.Properties = input.ReadObject<Dictionary<string, string>>();
		}

        #region Tiles

        /// <summary>
        /// Reads the tile layers from the content reader and populates the Tiled map.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to populate.</param>
        private void ReadTileLayers(ContentReader input, TiledMap map)
		{
			var layersCount = input.ReadInt32();
			var layers = new TiledMapTileLayer[layersCount];

			for(var i = 0; i < layersCount; i += 1)
			{
				var layer = new TiledMapTileLayer();
				ReadLayer(input, layer);
				layer.Width = input.ReadInt32();
				layer.Height = input.ReadInt32();
				layer.TileWidth = map.TileWidth;
				layer.TileHeight = map.TileHeight;
			
				var tiles = new TiledMapTile[layer.Width][];
				for(var x = 0; x < layer.Width; x += 1)
				{
					tiles[x] = new TiledMapTile[layer.Height];
				}
				for(var y = 0; y < layer.Height; y += 1)
				{
					for(var x = 0; x < layer.Width; x += 1)
					{
						tiles[x][y] = ReadTile(input);
					}
				}
				layer.Tiles = tiles;

				layers[i] = layer;
			}
			map.TileLayers = layers;
		}

        /// <summary>
        /// Reads a single tile from the content reader.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <returns>A populated <see cref="TiledMapTile"/> instance.</returns>
        private TiledMapTile ReadTile(ContentReader input)
		{
			var tile = new TiledMapTile();
			tile.GID = input.ReadInt32();
			tile.FlipHor = input.ReadBoolean();
			tile.FlipVer = input.ReadBoolean();
			tile.FlipDiag = input.ReadBoolean();

			return tile;
		}

        #endregion Tiles

        #region Objects

        /// <summary>
        /// Reads the object layers from the content reader and populates the Tiled map.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to populate.</param>
        private void ReadObjectLayers(ContentReader input, TiledMap map)
		{
			var layersCount = input.ReadInt32();
			var layers = new TiledMapObjectLayer[layersCount];
			
			for(var i = 0; i < layersCount; i += 1)
			{
				var layer = new TiledMapObjectLayer();
				ReadLayer(input, layer);

				layer.DrawingOrder = (TiledMapObjectDrawingOrder)input.ReadByte();
				layer.Color = input.ReadColor();

				var objectsCount = input.ReadInt32();
				var objects = new TiledObject[objectsCount];

				for(var k = 0; k < objectsCount; k += 1)
				{
					objects[k] = ReadObject(input, map);
				}

				layer.Objects = objects;

				layers[i] = layer;
			}
			map.ObjectLayers = layers;
		}

        /// <summary>
        /// Reads a collection of Tiled objects from the content reader.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to which the objects belong.</param>
        /// <returns>An array of <see cref="TiledObject"/> instances.</returns>
        private TiledObject ReadObject(ContentReader input, TiledMap map)
		{
			var obj = ReadBaseObject(input);
			var objType = (TiledObjectType)input.ReadByte();

			if (objType == TiledObjectType.Tile)
			{
				return ReadTileObject(input, obj, map);
			}

			if (objType == TiledObjectType.Point)
			{
				return ReadPointObject(obj);
			}

			if (objType == TiledObjectType.Polygon)
			{
				return ReadPolygonObject(input, obj);
			}

			if (objType == TiledObjectType.Ellipse)
			{
				return ReadEllipseObject(obj);
			}

			if (objType == TiledObjectType.Text)
			{
				return ReadTextObject(input, obj);
			}

			if (objType == TiledObjectType.Rectangle)
			{
				return ReadRectangleObject(obj);
			}
			
			return obj;
		}

        /// <summary>
        /// Reads a single Tiled object from the content reader.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to which the object belongs.</param>
        /// <returns>A populated <see cref="TiledObject"/> instance.</returns>
        private TiledObject ReadBaseObject(ContentReader input)
		{
			var obj = new TiledObject();
			obj.Name = input.ReadString();
			obj.Type = input.ReadString();
			obj.ID = input.ReadInt32();
			obj.Position = input.ReadVector2();
			obj.Size = input.ReadVector2();
			obj.Rotation = input.ReadSingle();
			obj.Visible = input.ReadBoolean();
			obj.Properties = input.ReadObject<Dictionary<string, string>>();

			return obj;
		}

		TiledObject ReadTileObject(ContentReader input, TiledObject baseObj, TiledMap map)
		{
			var obj = new TiledTileObject(baseObj);

			obj.GID = input.ReadInt32();
			obj.FlipHor = input.ReadBoolean();
			obj.FlipVer = input.ReadBoolean();

			obj.Tileset = map.GetTileset(obj.GID);
		
			return obj;
		}
		
		TiledObject ReadPointObject(TiledObject baseObj) =>
			new TiledPointObject(baseObj);
		
		TiledObject ReadPolygonObject(ContentReader input, TiledObject baseObj)
		{
			var obj = new TiledPolygonObject(baseObj);

			obj.Closed = input.ReadBoolean();
			obj.Points = input.ReadObject<Vector2[]>();

			return obj;
		}

		TiledObject ReadEllipseObject(TiledObject baseObj) =>
			new TiledEllipseObject(baseObj);
		
		TiledObject ReadTextObject(ContentReader input, TiledObject baseObj)
		{
			var obj = new TiledTextObject(baseObj);

			obj.Text = input.ReadString();
			obj.Color = input.ReadColor();
			obj.WordWrap = input.ReadBoolean();
			obj.HorAlign = (TiledTextAlign)input.ReadByte();
			obj.VerAlign = (TiledTextAlign)input.ReadByte();
			obj.Font = input.ReadString();
			obj.FontSize = input.ReadInt32();
			obj.Underlined = input.ReadBoolean();
			obj.StrikedOut = input.ReadBoolean();

			return obj;
		}

		TiledObject ReadRectangleObject(TiledObject baseObj) =>
			new TiledRectangleObject(baseObj);

        #endregion Objects		

        #region Images

        /// <summary>
        /// Reads the image layers from the content reader and populates the Tiled map.
        /// </summary>
        /// <param name="input">The content reader.</param>
        /// <param name="map">The Tiled map to populate.</param>
        private void ReadImageLayers(ContentReader input, TiledMap map)
		{
			var layersCount = input.ReadInt32();
			var layers = new TiledMapImageLayer[layersCount];
			
			for(var i = 0; i < layersCount; i += 1)
			{
				var layer = new TiledMapImageLayer();
				ReadLayer(input, layer);

				layer.TexturePath = Path.Combine(Path.GetDirectoryName(input.AssetName), input.ReadString());
				layer.Texture = input.ContentManager.Load<Texture2D>(input.ReadString());
				layer.TransparentColor = input.ReadColor();

				layers[i] = layer;
			}
			map.ImageLayers = layers;
		}
		
		#endregion Images
	}
}
