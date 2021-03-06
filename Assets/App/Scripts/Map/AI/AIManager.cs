﻿// 
// AIUtility.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2020.08.13
// 

using UnityEngine;
using Zenject;

namespace Ling.AI
{
	/// <summary>
	/// AI管理クラス
	/// </summary>
	public class AIManager : MonoBehaviour
	{
		#region 定数, class, enum

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[Inject] private AIUtility.CharaFinder _charaFinder = default;

		#endregion


		#region プロパティ

		#endregion


		#region public, protected 関数

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
		}

		#endregion
	}
}