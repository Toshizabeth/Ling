﻿// 
// CharaControl.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2020.08.10
// 

using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Zenject;
using Ling.Map.TileDataMapExtensions;
using Ling.Const;
using UnityEngine.Tilemaps;
using Utility.Extensions;
using System;

namespace Ling.Chara
{
	/// <summary>
	/// 簡易Controller参照用インターフェース
	/// </summary>
	public interface ICharaController
	{
		CharaModel Model { get; }

		ViewBase View { get; }

		CharaStatus Status { get; }

		ICharaMoveController MoveController { get; }
		Exp.ICharaExpController ExpController { get; }

		CharaEquipControl EquipControl { get; }
		
		/// <summary>
		/// キャラ名
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Tilemap情報を設定する
		/// </summary>
		void SetTilemap(Tilemap tilemap, int mapLevel);

		/// <summary>
		/// ダメージを受けた時
		/// </summary>
		void Damage(long value);

		TProcess AddMoveProcess<TProcess>() where TProcess : Utility.ProcessBase;
		TProcess AddAttackProcess<TProcess>() where TProcess : Utility.ProcessBase;
	}

	/// <summary>
	/// キャラのModelとViewをつなげる役目と操作を行う
	/// </summary>
	public abstract partial class CharaControl<TModel, TView> : MonoBehaviour, ICharaController, ICharaMoveController, ICharaActionController
		where TModel : CharaModel
		where TView : ViewBase
	{
		#region 定数, class, enum

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[SerializeField] protected TModel _model = default;
		[SerializeField] protected TView _view = default;
		[SerializeField] private CharaMover _charaMover = default;

		[Inject] protected DiContainer _diContainer = default;
		[Inject] private Utility.ProcessManager _processManager = default;
		[Inject] private Utility.IEventManager _eventManager = default;
		[Inject] private Utility.SaveData.ISaveDataHelper _saveDataHelper = default;

		private List<Utility.ProcessBase> _moveProcesses = new List<Utility.ProcessBase>();
		private List<Utility.ProcessBase> _attackProcess = new List<Utility.ProcessBase>();
		private Subject<CharaControl<TModel, TView>> _onSetuped = new Subject<CharaControl<TModel, TView>>();
		private CharaEquipControl _equipControl = new CharaEquipControl();

		#endregion


		#region プロパティ


		public TModel Model => _model;

		public TView View => _view;

		public CharaStatus Status => _model.Status;

		
		/// <summary>
		/// キャラ名
		/// </summary>
		string ICharaController.Name => _model.Name;

		/// <summary>
		/// 動きの制御を行うメソッドにアクセスするためのInterface
		/// </summary>
		/// <value></value>
		public ICharaMoveController MoveController => this;

		/// <summary>
		/// キャラクタを動かすヘルパクラス
		/// </summary>
		public CharaMover CharaMover => _charaMover;

		/// <summary>
		/// 設定済みの場合true
		/// </summary>
		public bool IsSetuped { get; private set; }


		/// <summary>
		/// 準備が整った時に通知が呼び出される
		/// </summary>
		public IObservable<CharaControl<TModel, TView>> OnSetuped => _onSetuped;

		
		/// <summary>
		/// 経験値管理者
		/// </summary>
		public abstract Exp.ICharaExpController ExpController { get; }

		/// <summary>
		/// 装備関連の操作
		/// </summary>
		public CharaEquipControl EquipControl => _equipControl;


		// ICharaController
		CharaModel ICharaController.Model => _model;
		ViewBase ICharaController.View => _view;

		#endregion


		#region public, protected 関数

		public void Setup()
		{
			_equipControl.Setup(_model.Status);

			// 死亡時
			Status.IsDead.Where(isDead_ => isDead_)
				.SelectMany(_ =>
				{
					// キャラが死亡した時
					_eventManager.Trigger(new EventDead { chara = this });

					return View.PlayDeadAnimation();
				})
				.Subscribe(_ =>
				{
					// 死亡処理
					DestroyProcess();
				},
				() =>
				{
					Utility.Log.Print("死にアニメーション終わり");
				}).AddTo(gameObject);

			// 向きが変わったとき
			_model.Dir.Subscribe(dir_ =>
				{
					_view.SetDirection(dir_);
				});

			// セルの座標が変更されたとき
			_model.CellPosition
				.Where(_ => _model.IsReactiveCellPosition)
				.Subscribe(cellPosition_ =>
				{
					_view.SetCellPos(cellPosition_);
				});

			SetupInternal();

			IsSetuped = true;

			_onSetuped.OnNext(this);
			_onSetuped.OnCompleted();
		}

		/// <summary>
		/// Tilemap情報を設定する
		/// </summary>
		public void SetTilemap(Tilemap tilemap, int mapLevel)
		{
			_view.SetTilemap(tilemap, mapLevel);

			CharaMover.SetTilemap(tilemap);
		}

		/// <summary>
		/// どういう行動をするか攻撃、移動AIクラスから思考し、決定する。
		/// </summary>
		public async UniTask ThinkAIProcess(Utility.Async.WorkTimeAwaiter timeAwaiter)
		{
			// 自分が状態異常で行動できない場合はスキップ

			// 第一優先として、自分が「特技」「攻撃」ができるか。

			// 攻撃
			await _model.AttackAI.ExecuteAsync(this, timeAwaiter);

			if (_model.AttackAI.CanActable)
			{
				return;
			}

			// それができない場合、「移動」をする。
			await _model.MoveAI.ExecuteAsync(this, timeAwaiter);
		}

		/// <summary>
		/// AIを設定する
		/// </summary>
		public TMoveAI AttachMoveAI<TMoveAI>(bool isResume) where TMoveAI : AI.Move.AIBase
		{
			var moveAI = default(TMoveAI);
			if (isResume)
			{
				moveAI = GetComponent<TMoveAI>();
			}
			else
			{
				moveAI = _diContainer.InstantiateComponent<TMoveAI>(gameObject);
			}
			
			_model.SetMoveAI(moveAI);

			return moveAI;
		}

		public TAttackAI AttachAttackAI<TAttackAI>(bool isResume) where TAttackAI : AI.Attack.AIBase
		{
			var attackAI = default(TAttackAI);
			if (isResume)
			{
				attackAI = GetComponent<TAttackAI>();
			}
			else
			{
				attackAI = _diContainer.InstantiateComponent<TAttackAI>(gameObject);
			}
			_model.SetAttackAI(attackAI);

			return attackAI;
		}

		/// <summary>
		/// 移動プロセスの追加
		/// 実行は待機する
		/// </summary>
		public TProcess AddMoveProcess<TProcess>() where TProcess : Utility.ProcessBase
		{
			var process = _diContainer.Instantiate<TProcess>();
			_processManager.Attach(process, transform, waitForStart: true);
			_moveProcesses.Add(process);

			return process;
		}

		/// <summary>
		/// 攻撃プロセスの追加
		/// 実行は待機する
		/// </summary>
		public TProcess AddAttackProcess<TProcess>() where TProcess : Utility.ProcessBase
		{
			var process = _diContainer.Instantiate<TProcess>();
			_processManager.Attach(process, transform, waitForStart: true);
			_attackProcess.Add(process);

			return process;
		}

		/// <summary>
		/// 移動プロセスの実行
		/// </summary>
		public void ExecuteMoveProcess()
		{
			var tmp = _moveProcesses.ToArray();
			foreach (var process in tmp)
			{
				// 終了時、移動プロセスリストから削除する
				process.AddAllFinishAction(action_ =>
					{
						_moveProcesses.Remove(process);
					});

				process.SetEnable(true);
			}
		}

		/// <summary>
		/// 攻撃プロセスの実行
		/// </summary>
		public void ExecuteAttackProcess()
		{
			var tmp = _attackProcess.ToArray();
			foreach (var process in tmp)
			{
				// 終了時、攻撃プロセスリストから削除する
				process.AddAllFinishAction(action_ =>
					{
						_attackProcess.Remove(process);
					});

				process.SetEnable(true);
			}
		}

		public async UniTask WaitMoveProcess()
		{
			if (!_moveProcesses.IsNullOrEmpty())
			{
				await UniTask.WaitUntil(() => _moveProcesses.IsNullOrEmpty());
			}

			await WaitPostProess();
		}

		public async UniTask WaitAttackProcess()
		{
			if (!_attackProcess.IsNullOrEmpty())
			{
				await UniTask.WaitUntil(() => _attackProcess.IsNullOrEmpty());
			}

			await WaitPostProess();
		}

		/// <summary>
		/// 指定した座標に移動できるか
		/// </summary>
		public bool CanMove(Map.TileDataMap tileDataMap, in Vector2Int addMoveDir)
		{
			// 目的地
			var destPos = _model.CellPosition.Value + addMoveDir;

			// 範囲外なら移動できない
			if (!tileDataMap.InRange(destPos.x, destPos.y))
			{
				return false;
			}

			var tileFlag = tileDataMap.GetTileFlag(destPos.x, destPos.y);
			if (tileFlag.HasAny(_model.UnmovableTileFlag))
			{
				return false;
			}
			

			return true;
		}

		/// <summary>
		/// 指定した座標に攻撃できるか
		/// </summary>
		public bool CanAttack(Map.TileDataMap tileDataMap, in Vector2Int addMoveDir)
		{
			return false;
		}

		protected  virtual void SetupInternal() {}

		protected virtual void DestroyProcessInternal()
		{

		}

		/// <summary>
		/// ダメージを受けた時
		/// </summary>
		void ICharaController.Damage(long value)
		{
			// ダメージを受けたイベントを送る
			_eventManager.Trigger(new EventDamage { chara = this, value = value });

			Status.HP.SubCurrent(value);
		}

		#endregion


		#region private 関数

		/// <summary>
		/// 削除処理
		/// </summary>
		private void DestroyProcess()
		{
			// 削除イベントを投げる
			var eventRemove = _model.EventRemove;
			eventRemove.chara = this;
			Utility.EventManager.SafeTrigger(eventRemove);

			DestroyProcessInternal();
		}

		/// <summary>
		/// 行動後、追加処理が終わるまで待機する
		/// </summary>
		private async UniTask WaitPostProess()
		{
			foreach (var postProcess in _model.PostProcessers)
			{
				if (!postProcess.ShouldExecute) continue;

				await postProcess.ExecuteAsync();
			}
		}


		#endregion


		#region MonoBegaviour

		private void Awake()
		{
			if (_charaMover == null)
			{
				_charaMover = _view.GetComponent<CharaMover>();
			}

			_charaMover.SetModel(this);
		}

		#endregion
	}
}