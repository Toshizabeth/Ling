﻿//
// Events.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.05.03
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Ling.Scenes.Battle
{
	/// <summary>
	/// Playerの移動距離
	/// </summary>
	public class EventPlayerMove
	{
		public Vector3Int moveDistance; // 移動距離
	}

	public class EventMessageText
	{
		public string text;
	}

	/// <summary>
	/// 選択肢を出す
	/// </summary>
	public class EventMessageTextSelect
	{
		public string text;
		public string[] selectTexts;
		public System.Action<int> onSelected;
	}

	/// <summary>
	/// Phaseを変更させる
	/// </summary>
	public class EventChangePhase
	{
		public Phase phase;
	}

	/// <summary>
	/// 次の階層に移動した
	/// </summary>
	public class EventChangeNextStage
	{
		// 移動後の情報
		public int level;
		public UnityEngine.Tilemaps.Tilemap tilemap;
	}

	/// <summary>
	/// ProecssContainerに処理を追加する
	/// </summary>
	public class EventAddProcessContainer
	{
		public ProcessType Type;
		public Utility.ProcessBase Process;
	}
}
