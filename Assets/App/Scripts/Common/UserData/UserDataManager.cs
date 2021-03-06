﻿//
// UserDataManager.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.07.10
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

using Ling.UserData.Item;
using Ling.UserData.Equipment;
using Ling.UserData.Repository;

using Zenject;

namespace Ling.UserData
{
	/// <summary>
	/// 各種ユーザーデータのRepositoryを返す
	/// </summary>
	public interface IUserDataHolder
	{
		ItemUserDataRepository ItemRepository { get; }
		EquipmentUserDataRepository EquipmentRepository { get; }
	}

	

	/// <summary>
	/// ユーザーごとに保持されるデータ
	/// </summary>
	public class UserDataManager : Utility.UserData.UserDataManager, IUserDataHolder
	{
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		#endregion


		#region プロパティ

		ItemUserDataRepository IUserDataHolder.ItemRepository => GetRepository<ItemUserDataRepository>();
		EquipmentUserDataRepository IUserDataHolder.EquipmentRepository => GetRepository<EquipmentUserDataRepository>();

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public async override UniTask LoadAll()
		{
			// Todo:読み込むか作成するか
#if UNITY_EDITOR
			var isForceResume = Common._Debug.Editor.CommonDebugMenu.IsForceResumeChecked;
			if (isForceResume)
			{
				LoadSaveDataRepositoryTask<ItemUserData, ItemUserDataRepository>("ItemUserData");
				LoadSaveDataRepositoryTask<EquipmentUserData, EquipmentUserDataRepository>("EquipmentUserData");
			}
			else
			{
				CreateRepository<ItemUserData, ItemUserDataRepository>("ItemUserData");
				CreateRepository<EquipmentUserData, EquipmentUserDataRepository>("EquipmentUserData");
			}
#else
#endif

			// 非同期でTaskを実行し、すべての処理が終わるまで待機
			await UniTask.WhenAll(_loadTasks);

			LoadFinished();
		}

		/// <summary>
		/// すべてのデータを保存する
		/// </summary>
		public async UniTask SaveAll()
		{

		}


		#endregion


		#region private 関数

		#endregion
	}
}
