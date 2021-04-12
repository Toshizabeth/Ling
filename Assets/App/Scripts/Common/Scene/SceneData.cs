﻿//
// SceneData.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.04.17
//

using System.Collections.Generic;
using Zenject;

namespace Ling.Common.Scene
{
	/// <summary>
	/// シーン情報
	/// SceneManagerで管理される
	/// </summary>
	public class SceneData
	{
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		#endregion


		#region プロパティ

		/// <summary>
		/// シーンID
		/// </summary>
		public SceneID SceneID { get; set; }

		/// <summary>
		/// シーン遷移時のArgument
		/// </summary>
		public Argument Argument { get; set; }

		public System.Action<DiContainer> BindAction { get; set; }

		/// <summary>
		/// このシーンでAddSceneされているもの
		/// </summary>
		public List<SceneData> ActiveAddSceneData { get; } = new List<SceneData>();

		/// <summary>
		/// ChangeSceneされる前に自分でAddSceneして作成していたデータ
		/// </summary>
		public List<SceneData> PrevAddSceneData { get; } = new List<SceneData>();

		/// <summary>
		/// 自分がAddSceneで生成された場合の生成者(親)
		/// </summary>
		public SceneData ParentData { get; private set; }

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public void SetParent(SceneData parentData) =>
			ParentData = parentData;

		/// <summary>
		/// 自分が親としてAddSceneされる時に情報を保持する
		/// </summary>
		public void PushAddSceneData(SceneData addSceneData)
		{
			ActiveAddSceneData.Add(addSceneData);

			// 親を自分とする
			addSceneData.ParentData = this;
		}

		public void RemoveAddSceneData(SceneData addSceneData)
		{
			addSceneData.ParentData = null
			ActiveAddSceneData.Remove(addSceneData);
		}

		/// <summary>
		/// AddSceneDataをキャッシュリストに情報を移動させる
		/// その後AddSceneDataは削除
		/// </summary>
		public void MoveToCacheByAddSceceData()
		{
			PrevAddSceneData.Clear();
			PrevAddSceneData.AddRange(ActiveAddSceneData);

			ActiveAddSceneData.Clear();
		}

		#endregion


		#region private 関数

		#endregion
	}
}
