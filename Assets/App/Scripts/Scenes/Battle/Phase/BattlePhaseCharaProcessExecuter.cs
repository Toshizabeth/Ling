﻿//
// BattlePhaseCharaProcess.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.08.23
//

using Cysharp.Threading.Tasks;

namespace Ling.Scenes.Battle.Phase
{
	/// <summary>
	/// キャラクタの行動プロセスをすべて実行する
	/// </summary>
	public class BattlePhaseCharaProcessExecuter : BattlePhaseBase
    {
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		private Chara.CharaManager _charaManager;

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public override void Awake() 
		{ 
			_charaManager = Resolve<Chara.CharaManager>();
		}

		public override void Init()
		{
			ExecuteAsync().Forget();
		}

		public override void Proc() 
		{
		}


		#endregion


		#region private 関数

		public async UniTask ExecuteAsync()
		{
			// まずは移動Processをすべて叩く
			_charaManager.ExecuteMoveProcesses();

			// 移動が終わるまで待機
			await _charaManager.WaitForMoveProcessAsync();

			// 終わったら順番に攻撃・特技Processを叩く

			// すべて終わったらターン終了
			Change(BattleScene.Phase.PlayerAction);
		}

		#endregion
	}
}