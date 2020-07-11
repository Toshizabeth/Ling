﻿//
// BattlePhaseCharaCreate.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.04.30
//

using Ling.Chara;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Ling.Scenes.Battle.Phase
{
	/// <summary>
	/// 
	/// </summary>
	public class BattlePhaseCharaSetup : BattlePhaseBase
	{
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		private Map.Builder.IManager _builderManager = null;
		private MapManager _mapManager;
		private Chara.CharaManager _charaManager;

		private Chara.Player _player;

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数


		public override void Init()
		{
			_builderManager = Resolve<Map.Builder.IManager>();
			_mapManager = Resolve<MapManager>();
			_charaManager = Resolve<CharaManager>();

			var builder = _mapManager.CurrentMapData.Builder;
			var playerPos = builder.GetPlayerInitPosition();

			// プレイヤーにMap情報を初期座標を設定
			_player = _charaManager.Player;
			Scene.MapControl.SetPlayerModelInCurrentMap(_player);

			_player.SetCellPos(playerPos);

			Change(BattleScene.Phase.PlayerAction);
		}

		public override void Proc() 
		{
		}

		public override void Term() 
		{ 
		}

		#endregion


		#region private 関数

		#endregion
	}
}
