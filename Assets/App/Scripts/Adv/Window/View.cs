﻿// 
// View.cs  
// ProductName Ling
//  
// Create by toshiki sakamoto on 2019.04.30.
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Ling.Adv.Window
{
    /// <summary>
    /// 
    /// </summary>
    public class View : Utility.ObjCreator<View> 
    {
        #region 定数, class, enum

        #endregion


        #region public 変数

        #endregion


        #region private 変数

        [SerializeField] private Window _window = null;

        #endregion


        #region プロパティ

        public Window Win { get { return _window; } }

        #endregion


        #region public, protected 関数

        public static string PrefabName()
        {
            return Common.GetResourcePath("AdvMain");
        }

        public static bool IsAwakeActive()
        {
            return false; 
        }


        public override void Setup()
        {
            _window.Setup();
        }

        #endregion


        #region private 関数

        #endregion


        #region MonoBegaviour

        /// <summary>
        /// 初期処理
        /// </summary>
        void Awake()
        {
        }

        /// <summary>
        /// 更新前処理
        /// </summary>
        void Start()
        {
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        void OnDestoroy()
        {
        }

        #endregion
    }
}