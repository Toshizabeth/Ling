﻿// 
// View.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2020.04.20
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace Ling._Debug.Builder
{
	/// <summary>
	/// 
	/// </summary>
	public class View : MonoBehaviour
	{
		#region 定数, class, enum

		[System.Serializable]
		public class Setting
		{
			[SerializeField] private InputField _widthInputField = null;
			[SerializeField] private InputField _heightInpuField = null;
			[SerializeField] private Dropdown _builderTypeDropdown = null;
			[SerializeField] private Button _executeButton = null;
			[SerializeField] private InputField _randomSeed = null;

			public int Width => int.Parse(_widthInputField.text);
			public int Height => int.Parse(_heightInpuField.text);
			public int RandomSeed => int.Parse(_randomSeed.text);
			public Dropdown BuilderTypeDropdown => _builderTypeDropdown;
			public System.Action OnExecute { get; set; }

			public void Setup()
			{
				_executeButton.OnClickAsObservable().Subscribe(_ => OnExecute?.Invoke());
			}
		}

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[SerializeField] private Setting _setting = null;
		[SerializeField] private MapDrawView _drawView = null;

		[Zenject.Inject] private Utility.IEventManager _eventManager = null;

		#endregion


		#region プロパティ

		public System.Action<Setting> OnExecute { get; set; }

		public MapDrawView MapDrawView => _drawView;

		#endregion


		#region public, protected 関数

		public void Setup()
		{
			_setting.Setup();

			_setting.OnExecute = () => OnExecute?.Invoke(_setting);

			_eventManager.Add<Utility.EventTouchPoint>(this,
				ev_ =>
				{
					if (ev_.gameObject == null) return;

					var debugTile = ev_.gameObject.GetComponent<DebugTile>();
					if (debugTile == null) return;

					_drawView.SetTileText(debugTile.TileFlag.ToString());
				});
		}

		#endregion


		#region private 関数

		#endregion


		#region MonoBegaviour

		/// <summary>
		/// 初期処理
		/// </summary>
		void Awake()
		{
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
			_eventManager.RemoveAll(this);
		}

		#endregion
	}
}