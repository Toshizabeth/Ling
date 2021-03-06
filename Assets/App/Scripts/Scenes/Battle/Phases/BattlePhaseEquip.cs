﻿//
// BattlePhaseEquip.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2021.05.08
//

using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Zenject;
using Utility.Extensions;
using Ling.UserData;

namespace Ling.Scenes.Battle.Phases
{
	/// <summary>
	/// アイテム装備
	/// </summary>
	public class BattlePhaseEquip : BattlePhaseBase
	{
		#region 定数, class, enum

		public class Arg : Utility.PhaseArgument
		{
			public UserData.Equipment.EquipmentUserData Entity;
		}

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		[Inject] private Chara.CharaManager _charaManager = default;
		[Inject] private IUserDataHolder _userDataHolder = default;

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public override void PhaseStart()
		{
			// アイテムによって処理を変更する
			var arg = Argument as Arg;

/*
			// スキル
			var skill = arg.Item.Skill;
			var player = _charaManager.Player;
			var skillProcess = player.AddAttackProcess<Skill.SkillProcess>();
			skillProcess.Setup(player, skill);

			// キャラアクションに移動する
			var process = new Process.ProcessCharaAction(player);
			Scene.ProcessContainer.Add(ProcessType.PlayerSkill, process);

			Change(Phase.PlayerSkill);*/
		}

		public override async UniTask PhaseStartAsync(CancellationToken token)
		{
			var arg = Argument as Arg;
			var battleManager = BattleManager.Instance;
			var player = _charaManager.Player;

			// 装備処理を呼び出す
			var equipRepository = _userDataHolder.EquipmentRepository;
			var equipResult = equipRepository.Equip(arg.Entity);

			// 外す処理から
			var detachEquipment = equipResult.detach;
			if (detachEquipment != null)
			{
				if (detachEquipment.Category == Const.Equipment.Category.Weapon)
				{
					player.EquipControl.DetachWeapon();
				}
				else
				{
					player.EquipControl.DetachShield();
				}

				// todo: 仮
				battleManager.ShowMessage($"{equipResult.detach.Name} を 外した");
			}

			// 装着
			var attachEquipment = equipResult.attach;
			if (attachEquipment != null)
			{
				if (attachEquipment.Category == Const.Equipment.Category.Weapon)
				{
					player.EquipControl.AttachWeapon(attachEquipment.WeaponMaster);
				}
				else
				{
					player.EquipControl.AttachShield(attachEquipment.ShieldMaster);
				}

				// todo: 仮
				battleManager.ShowMessage($"{equipResult.attach.Name} を 装着した");
			}

			// メッセージ終了まで待機
			await battleManager.WaitMessageSending();

			// 終わったら敵の思考に移動する
			Change(Phase.EnemyTink);
		}

		public override void PhaseUpdate()
		{
		}

		public override void PhaseStop()
		{
		}

		#endregion


		#region private 関数


		#endregion
	}
}
