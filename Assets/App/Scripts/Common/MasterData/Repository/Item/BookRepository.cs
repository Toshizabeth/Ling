﻿//
// BoolRepository.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2020.11.09
//

using Ling.MasterData.Item;

namespace Ling.MasterData.Repository.Item
{
	/// <summary>
	/// BookMaster Repository
	/// </summary>
	public class BookRepository : Utility.MasterData.InheritanceMasterRepository<ItemMaster, BookMaster>
	{
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		public BookMaster Find(Const.Item.Book type) =>
			(BookMaster)Entities.Find(entity => ((BookMaster)entity).Type == type);

		#endregion


		#region private 関数

		#endregion
	}
}
