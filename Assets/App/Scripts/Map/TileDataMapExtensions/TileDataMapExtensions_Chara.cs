﻿//
// TileDataMapExtensions_Chara.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.08.18
//

using UnityEngine;
using Ling;
using Ling.Const;
using Ling.Chara;

namespace Ling.Map.TileDataMapExtensionss.Chara
{
	/// <summary>
	/// Chara関連
	/// </summary>
	public static class TileDataMapExtensions_Chara
    {
		public static RoomData FindRoomData(this TileDataMap self, ICharaController chara)
		{
			var pos = chara.Model.CellPosition.Value;
			if (self.TryGetRoomData(pos.x, pos.y, out var roomData))
			{
				return roomData;
			}

			return null;
		}
	}
}
