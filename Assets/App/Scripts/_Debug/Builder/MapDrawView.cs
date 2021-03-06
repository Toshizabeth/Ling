﻿// 
// MapDrawView.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2020.04.20
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.Attribute;
using System.Linq;
using Ling.Const;

namespace Ling._Debug.Builder
{
	/// <summary>
	/// 
	/// </summary>
	public class MapDrawView : MonoBehaviour
	{
		#region 定数, class, enum

		[System.Serializable]
		public class ColorData
		{
			[SerializeField, EnumFlags] private Const.TileFlag _tileFlag;
			[SerializeField] private Color _color = Color.white;

			public bool HasFlag(Const.TileFlag tileFlag) =>
				_tileFlag.HasFlag(tileFlag);

			public Color Color => _color;
		}

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[SerializeField] private Transform _root = null;
		[SerializeField] private SpriteRenderer _mapSprite = null;  // 
		[SerializeField] private ColorData[] _colorData = null;
		[SerializeField] private Color[] _otherColor;
		[SerializeField] private Text _tileFlagText = null;

		private int _width, _height;
		private SpriteRenderer[] _drawSprites;
		private Map.Builder.BuilderConst.BuilderType _builderType;
		private Coroutine _coroutine;

		#endregion


		#region プロパティ

		#endregion


		#region public, protected 関数

		public void Setup(int width, int height, Map.Builder.BuilderConst.BuilderType builderType)
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}

			if (_drawSprites != null)
			{
				foreach (var obj in _drawSprites)
				{
					GameObject.Destroy(obj.gameObject);
				}
			}

			_width = width;
			_height = height;
			_builderType = builderType;
			_drawSprites = new SpriteRenderer[width * height];

			switch (_builderType)
			{
				case Map.Builder.BuilderConst.BuilderType.Split:
					SetUp_Split();
					break;

				default:
					break;
			}
		}

		public void DrawUpdate(Map.Builder.IBuilder builder)
		{
			switch (_builderType)
			{
				case Map.Builder.BuilderConst.BuilderType.Split:
					_coroutine = StartCoroutine(DrawUpdate_Split(builder));
					break;

				default:
					break;
			}
		}

		public void SetTileText(string text)
		{
			_tileFlagText.text = text;
		}

		public bool TryGetSpriteRenderer(int index, out SpriteRenderer renderer)
		{
			renderer = null;

			if (index < 0 || index >= _drawSprites.Length) return false;

			renderer = _drawSprites[index];
			return true;
		}

		#endregion


		#region private 関数

		private bool TryGetColor(Const.TileFlag tileFlag, out Color color)
		{
			color = Color.white;

			var colorData = System.Array.Find(_colorData, elm_ => elm_.HasFlag(tileFlag));
			if (colorData == null) return false;

			color = colorData.Color;
			return true;
		}

		private void SetUp_Split()
		{
			var width = _mapSprite.bounds.size.x;
			var height = _mapSprite.bounds.size.y;

			Utility.Log.Print($"Width {width}, Height{height}");

			var startVector = new Vector2(width * (_width * -0.5f) + width * 0.5f,
										  height * (_height * 0.5f) + height * 0.5f);

			// 全てのオブジェクトをSpriteにする
			for (int i = 0; i < _width * _height; ++i)
			{
				var x = i % _width;
				var y = i / _height;

				var drawSprite = _drawSprites[i] = GameObject.Instantiate(_mapSprite, _root);
				drawSprite.transform.localPosition = new Vector3(startVector.x + x * width, startVector.y - y * height, 0.0f);
				drawSprite.gameObject.SetActive(true);
			}
		}

		private IEnumerator DrawUpdate_Split(Map.Builder.IBuilder builder)
		{
			var splitBuilder = builder as Map.Builder.Split.Builder;
			if (splitBuilder == null) yield break;

			void Draw(RectInt rect, int colorIndex)
			{
				for (int y = rect.yMin; y < rect.yMax; ++y)
				{
					for (int x = rect.xMin; x < rect.xMax; ++x)
					{
						//DrawByIndex(y * _width + x, tileFlag, colorIndex);
						var draw = _drawSprites[y * _width + x];
						var color = _otherColor[Mathf.Min(colorIndex, _otherColor.Length - 1)];

						draw.color = color;
					}
				}
			}
			void DrawByTileData(Map.TileData tileData, Const.TileFlag tileFlag)
			{
				var draw = _drawSprites[tileData.Index];

				if (TryGetColor(tileFlag, out Color color))
				{
					draw.color = color;

					var debugTile = draw.GetComponent<DebugTile>();
					if (debugTile == null) return;

					debugTile.TileFlag = tileFlag;
					debugTile.SetTileData(tileData);
				}
			}

			// 一つ先にすすめる
			var enumerator = builder.ExecuteDebug(null);
			while (enumerator.MoveNext())
			{
				// 区画情報
				var mapRect = splitBuilder.MapRect;
				for (int i = 0; i < mapRect.RectCount; ++i)
				{
					var data = mapRect[i];
					Draw(data.rect, i);
				}

				var tileDataMap = splitBuilder.TileDataMap;
				foreach (var tileData in tileDataMap)
				{
					DrawByTileData(tileData, tileData.Flag);
				}

				yield return new WaitForSeconds(enumerator.Current);
			}
		}

		#endregion


		#region MonoBegaviour

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		#endregion
	}
}