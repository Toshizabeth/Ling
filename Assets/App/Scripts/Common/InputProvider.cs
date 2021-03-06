﻿//
// IInputProvider.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.12.30
//

using UnityEngine;
using UnityEngine.InputSystem;

#if false
namespace Ling.Common
{
	/// <summary>
	/// インプット処理を担う
	/// </summary>
	public interface IInputProvider
    {
		/// <summary>
		/// キーが押されている間
		/// </summary>
		bool GetKey(KeyCode keyCode);

		/// <summary>
		/// キーが押されたとき一回だけ
		/// </summary>
		bool GetKeyDown(KeyCode keyCode);

		/// <summary>
		/// キーが離されたとき一回だけ
		/// </summary>
		bool GetKeyUp(KeyCode keyCode);
	}

#if UNITY_EDITOR
	/// <summary>
	/// キーボード入力
	/// </summary>
	public class KeyInputProvider : IInputProvider
	{
		/// <summary>
		/// キーが押されている間
		/// </summary>
		public bool GetKey(KeyCode keyCode) =>
			UnityEngine.Input.GetKey(keyCode);

		/// <summary>
		/// キーが押されたとき一回だけ
		/// </summary>
		public bool GetKeyDown(KeyCode keyCode) =>
			UnityEngine.Input.GetKeyDown(keyCode);

		/// <summary>
		/// キーが離されたとき一回だけ
		/// </summary>
		public bool GetKeyUp(KeyCode keyCode) =>
			UnityEngine.Input.GetKeyUp(keyCode);
	}
#endif
}
#endif