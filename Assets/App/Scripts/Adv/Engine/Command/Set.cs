﻿//
// SetCommand.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2019.04.21
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace Ling.Adv.Engine.Command
{
    /// <summary>
    /// 
    /// </summary>
    public class Set : Base
    {
        #region 定数, class, enum

        #endregion


        #region public, protected 変数

        #endregion


        #region private 変数

        private ScriptType _scriptType = ScriptType.NONE;

        #endregion


        #region プロパティ

        /// <summary>
        /// コマンドタイプ
        /// </summary>
        /// <value>The type.</value>
        public override ScriptType Type { get { return _scriptType; } }

        /// <summary>
        /// 足し合わされる値
        /// </summary>
        /// <value>The value data.</value>
        public Value.ValueInt ValueData { get; protected set; }

        /// <summary>
        /// そのまま代入される値
        /// </summary>
        /// <value>The set value.</value>
        public Value.Data SetValue { get; protected set; }

        /// <summary>
        /// たされる値
        /// </summary>
        /// <value>The add value.</value>
        public Value.Data AddValue { get; protected set; }

        #endregion


        #region コンストラクタ, デストラクタ

        #endregion


        #region public, protected 関数

        /// <summary>
        /// コマンド作成
        /// </summary>
        /// <returns>The create.</returns>
        public static Base Create(Creator creator, Lexer lexer)
        {
            string str1 = lexer.GetString();
            string str2 = lexer.GetString();

            var value = new Value.ValueInt();

            bool isSuccess = lexer.GetValue(value);

            if (string.IsNullOrEmpty(str1) || 
                string.IsNullOrEmpty(str2) || 
                !isSuccess ||
                !string.IsNullOrEmpty(lexer.GetString()))
            {
                Log.Error("syntax error");
                return null; 
            }

            var valueManager = creator.ValueManager;

            switch (str2)
            {
                case "=":
                    {
                        var instance = new Set();
                        instance._scriptType = ScriptType.SET_VALUE_CMD;
                        instance.ValueData = valueManager.FindValue<Value.ValueInt>(str1);
                        instance.SetValue = value;

                        creator.AddCommand(instance);

                        return instance;
                    }

                case "+":
                    {
                        var instance = new Set();
                        instance._scriptType = ScriptType.CALC_VALUE_CMD;
                        instance.ValueData = valueManager.FindValue<Value.ValueInt>(str1);
                        instance.AddValue = value;

                        creator.AddCommand(instance);

                        return instance;
                    }

                case "-":
                    {
                        var instance = new Set();
                        instance._scriptType = ScriptType.CALC_VALUE_CMD;
                        instance.ValueData = valueManager.FindValue<Value.ValueInt>(str1);

                        // 符号を逆にする
                        value.Change();

                        instance.AddValue = value;

                        creator.AddCommand(instance);

                        return instance;
                    }

                default:
                    Log.Error("syntax error");
                    return null;
            }
        }

        #endregion


        #region private 関数

        #endregion
    }
}