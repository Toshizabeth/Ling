﻿//
// Data.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2019.09.08
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace Ling.Adv.Chara
{
	/// <summary>
	/// 
	/// </summary>
    public class Data
    {
        #region 定数, class, enum

        #endregion


        #region public, protected 変数

        #endregion


        #region private 変数

        /// <summary>
        /// 表情
        /// </summary>
        private Dictionary<string, string> _dictFace = new Dictionary<string, string>();

        #endregion


        #region プロパティ

        /// <summary>
        /// キャラの名前
        /// </summary>
        public string Name { get; private set; }

        #endregion


		#region コンストラクタ, デストラクタ

        public Data(string name)
        {
            Name = name;
        }

		#endregion


        #region public, protected 関数

        public void LoadFace(string key, string filename)
        {
            _dictFace[key] = filename;
        }


        /// <summary>
        /// 一気に表情も渡す
        /// </summary>
        /// <param name="name"></param>
        /// <param name="face"></param>
        public void LoadFace(string name, Dictionary<string, string> face)
        {
            _dictFace = face;
        }

        /// <summary>
        /// 表情名からファイル名を取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFilename(string key)
        {
            string result;
            _dictFace.TryGetValue(key, out result);

            return result;
        }

        #endregion


        #region private 関数

        #endregion
    }
}