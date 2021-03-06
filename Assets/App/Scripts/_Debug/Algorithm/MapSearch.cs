﻿// 
// MapSearchSettings.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2020.08.24
// 

using UnityEngine;
using UnityEngine.UI;
using Ling.Map.TileDataMapExtensions;
using Ling.Const;
using Cysharp.Threading.Tasks;

namespace Ling._Debug.Algorithm
{
	/// <summary>
	/// Map走査デバッグの設定
	/// </summary>
	[System.Serializable]
	public class MapSearch : MonoBehaviour
	{
		#region 定数, class, enum

		public enum SearchType
		{
			AStar,
		}

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[SerializeField] private SearchType _searchType = SearchType.AStar;
		[SerializeField] private Toggle _toggleSearch = default;
		[SerializeField] private Text _firstSelected = default;
		[SerializeField] private Text _secondSelected = default;
		[SerializeField] private Transform _tileRoot = default;
		[SerializeField] private AStarScoreTileView _astarScoreTileView = default;
		[SerializeField] private Text _textNodeCost = default;
		[SerializeField] private Text _textNodeEstimatedCost = default;
		[SerializeField] private Text _textNodeScore = default;

		[Zenject.Inject] private Utility.IEventManager _eventManager = null;
		[Zenject.Inject] private Utility.Algorithm.Search _search = default;

		private bool _isSearchMode;
		private Builder.DebugTile _firstTileData;
		private Builder.DebugTile _secondTileData;
		private Map.TileDataMap _searchMapTarget;   // サーチ対象
		private AStarScoreTileView[] _scoreTileView;

		#endregion


		#region プロパティ

		#endregion


		#region public, protected 関数

		public void Setup(int width, int height)
		{
			if (_scoreTileView != null)
			{
				foreach (var obj in _scoreTileView)
				{
					GameObject.Destroy(obj.gameObject);
				}
			}

			_scoreTileView = new AStarScoreTileView[width * height];

			for (var y = 0; y < height; ++y)
			{
				for (var x = 0; x < width; ++x)
				{
					var index = y * width + x;
					var view = GameObject.Instantiate(_astarScoreTileView, _tileRoot);
					view.gameObject.SetActive(false);

					_scoreTileView[index] = view;
				}
			}
		}


		public void SetTileInfo(Text text, Builder.DebugTile tileData)
		{
			text.text = $"{tileData.Pos}";
		}

		public void SetSearchParam(Map.TileDataMap tileDataMap) =>
			_searchMapTarget = tileDataMap;

		public void SetFollowTargets(int index, Transform target)
		{
			if (index < 0 || index >= _scoreTileView.Length) return;

			_scoreTileView[index].SetFollowTarget(target);
		}

		#endregion


		#region private 関数

		private async UniTask ExectuteAsync()
		{
			var param = new Utility.Algorithm.Astar.Param();
			param.start = _firstTileData.Pos;
			param.end = _secondTileData.Pos;
			param.width = _searchMapTarget.Width;

			param.onCanMove = (pos_) =>
				{
					var tileFlag = _searchMapTarget.GetTileFlag(pos_);
					if (tileFlag.HasAny(TileFlag.Obstacle))
					{
						// 障害物は移動できないとする
						return false;
					}

					return true;
				};

			param.onCanDiagonalMove = pos_ =>
				{
					var tileFlag = _searchMapTarget.GetTileFlag(pos_);
					if (tileFlag.HasAny(TileFlag.Wall))
					{
						return false;
					}

					return true;
				};

			param.onTileCostGetter = (pos_) =>
				{
					// コストは常に１
					return 1;
				};

			param.onCreatedNode = (node_) =>
				{
					// ノード生成ごとにタイルに情報を書き込む
					var tileView = _scoreTileView[node_.index];
					tileView.gameObject.SetActive(true);
					tileView.Node = node_;
					tileView.SetScore(node_.score);
				};

			await _search.Astar.DebugExecuteAsync(param);

			// 探索が終わったらルートを表示させる
			var nodes = _search.Astar.GetRouteNodes();
			foreach (var node in nodes)
			{
				var tileView = _scoreTileView[node.index];
				tileView.SetTextColor(Color.red);
			}
		}

		private void SetNodeInfo(Utility.Algorithm.Astar.Node node)
		{
			if (node == null) return;

			_textNodeCost.text = node.cost.ToString();
			_textNodeEstimatedCost.text = node.estimatedCost.ToString();
			_textNodeScore.text = node.score.ToString();
		}

		#endregion


		#region MonoBegaviour

		/// <summary>
		/// 初期処理
		/// </summary>
		void Awake()
		{
			_toggleSearch.onValueChanged.AddListener(isOn_ =>
				{
					_isSearchMode = isOn_;
				});


			_eventManager.Add<Utility.EventTouchPoint>(this,
				ev_ =>
				{
					if (ev_.gameObject == null) return;

					if (ev_.intParam >= 0 && ev_.intParam < _scoreTileView.Length)
					{
						var scoreTileView = _scoreTileView[ev_.intParam];
						SetNodeInfo(scoreTileView.Node);
					}

					var debugTile = ev_.gameObject.GetComponent<Builder.DebugTile>();
					if (debugTile == null) return;

					// マップ走査モード
					if (!_isSearchMode) return;

					if (_firstTileData == null)
					{
						_firstTileData = debugTile;
						SetTileInfo(_firstSelected, _firstTileData);
						return;
					}

					if (_secondTileData == null)
					{
						_secondTileData = debugTile;
						SetTileInfo(_secondSelected, _secondTileData);

						ExectuteAsync().Forget();
					}
				});
		}


		/// <summary>
		/// 更新前処理
		/// </summary>
		void Start()
		{
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		void Update()
		{
		}

		/// <summary>
		/// 終了処理
		/// </summary>
		void OnDestroy()
		{
		}

		#endregion
	}
}