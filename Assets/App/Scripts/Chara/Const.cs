﻿//
// Const.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.06.28
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Ling.Chara
{
	/// <summary>
	/// キャラ周りの定数
	/// </summary>
	public class Const
    {

	}


	/// <summary>
	/// キャラの種類
	/// </summary>
	public enum CharaType
	{
		None,
		Player,
		Enemy,
	}

	/// <summary>
	/// 敵の種類 (プールシステム等に使われる)
	/// </summary>
	public enum EnemyType
	{
		Normal, // 通常の敵
		Boss,   // ボス
	}

	public static class ConstExtensions
	{
		public static Map.TileFlag ToTileFlag(this CharaType self)
		{
			switch (self)
			{
				case CharaType.Player: return Map.TileFlag.Player;
				case CharaType.Enemy: return Map.TileFlag.Enemy;

				default:
					Utility.Log.Error("CharaTypeからTileFlagへの変換ができません " + self);
					return Map.TileFlag.None;
			}
		}


		/// <summary>
		/// 通常の敵か
		/// </summary>
		public static bool IsEnemyNormal(this EnemyType enemyType) =>
			enemyType == EnemyType.Normal;

		/// <summary>
		/// ボス敵か
		/// </summary>
		public static bool IsEnemyBoss(this EnemyType enemyType) =>
			enemyType == EnemyType.Boss;
	}
}
