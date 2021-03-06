﻿// 
// InputScene.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2021.03.22
// 

using UnityEngine;
using Zenject;

namespace Ling.Scenes.Input
{
	/// <summary>
	/// 入力UI
	/// </summary>
	public class InputScene : Common.Scene.Base
	{
		#region 定数, class, enum

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[SerializeField] private Common.Input.MoveInputProvider _moveProvider = default;
		[SerializeField] private Common.Input.ActionInputProvider _actionProvider = default;

		[Inject] private Common.Input.IInputManager _inputManager = default;

		#endregion


		#region プロパティ

		public override Common.Scene.SceneID SceneID => Common.Scene.SceneID.Input;

		#endregion


		#region public, protected 関数

		#endregion


		#region private 関数

		#endregion


		#region MonoBegaviour

		private void Awake()
		{
			_inputManager?.Bind(_moveProvider);
			_inputManager?.Bind(_actionProvider);
		}

		#endregion

	}
}
