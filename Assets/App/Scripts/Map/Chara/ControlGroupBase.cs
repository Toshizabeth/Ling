﻿//
// ModelGroupBase.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.07.10
//

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using Zenject;

namespace Ling.Chara
{
	/// <summary>
	/// 複数のCharaControlを管理する
	/// </summary>
	public abstract class ControlGroupBase<TControl, TModel, TView> :
		MonoBehaviour,
		IEnumerable<TControl>
		where TControl : CharaControl<TModel, TView>
		where TModel : CharaModel
		where TView : Chara.ViewBase
	{
		#region 定数, class, enum

		public struct Enumerator : IEnumerator<TControl>
		{
			private readonly List<TControl> _list;
			private int _index;

			public Enumerator(List<TControl> list)
			{
				_list = list;
				_index = -1;
			}

			public TControl Current => _list[_index];
			object IEnumerator.Current => _list[_index];

			public void Dispose() { }
			public bool MoveNext() => ++_index < _list.Count();
			public void Reset() => _index = 0;

		}

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		[Inject] protected Utility.SaveData.ISaveDataHelper _saveDataHelper;

		private Tilemap _tilemap;

		#endregion


		#region プロパティ

		/// <summary>
		/// このグループが管理しているCharaControl
		/// </summary>
		public List<TControl> Controls { get; } = new List<TControl>();

		public List<TModel> Models { get; } = new List<TModel>();

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public IEnumerator<TControl> GetEnumerator() => new Enumerator(Controls);

		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(Controls);

		public void SetTilemap(Tilemap tilemap)
		{
			_tilemap = tilemap;
		}

		public async virtual UniTask SetupAsync()
		{
		}

		/// <summary>
		/// 復帰時の処理
		/// </summary>
		public async virtual UniTask ResumeAsync()
		{
		}

		/// <summary>
		/// 内部データをすべて初期値に戻す
		/// </summary>
		public void Reset()
		{
			ResetInternal();

			Controls.Clear();
			Models.Clear();
		}

		/// <summary>
		/// 移動Processをすべて実行する
		/// </summary>
		public void ExecuteMoveProcesses()
		{
			foreach (var control in Controls)
			{
				control.ExecuteMoveProcess();
			}
		}

		/// <summary>
		/// 移動プロセスがすべて終わるまで待機する
		/// </summary>
		public async UniTask WaitForMoveProcessAsync()
		{
#if false
			// 条件に達したら終了
			// 条件は移動プロセスがすべて終わっているとき
			bool isWait = false;
			do
			{
				isWait = false;

				foreach (var control in Controls)
				{
					await control.WaitAllProcessEnded();
				}

			} while (isWait);
#endif

			foreach (var control in Controls)
			{
				await control.WaitMoveProcess();
			}
		}

		/// <summary>
		/// 攻撃Processを順番に実行していく
		/// </summary>
		public void ExecuteAttackProcesses()
		{
			foreach (var control in Controls)
			{
				control.ExecuteAttackProcess();
			}
		}

		public TControl FindInPos(Vector2Int cellPosition)
		{
			return Controls.Find(control_ => control_.Model.CellPosition.Value == cellPosition);
		}

		#endregion


		#region private 関数
		
		protected virtual void ResetInternal() { }

		#endregion
	}
}
