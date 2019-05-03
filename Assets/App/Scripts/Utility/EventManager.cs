﻿//
// Event.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2019.04.30
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace Ling.Utility
{
    [DefaultExecutionOrder(-50)]    // 実行速度を早くする
    public class EventManager : MonoBehaviour
    {
        private void Awake()
        {
            Event.Create();

            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            Event.Destroy();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Event : Singleton<Event>
    {
        #region 定数, class, enum

        public class Base
        {
        }


        public interface IEventListener
        {
            void Remove(System.Object listener);

            void SafeDestroy(bool isForce = false);
        }

        /// <summary>
        /// ひとつのイベントに対するリスナークラス
        /// </summary>
        public class Listener<T> : IEventListener where T : Base
        {
            private static Listener<T> _instance;

            private Dictionary<System.Object, Action<T>> _listeners = 
                new Dictionary<System.Object, Action<T>>();


            public static Listener<T> Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new Listener<T>();
                    }

                    return _instance;
                }
            }

            public static bool IsNull
            {
                get
                {
                    return _instance == null;
                }
            }

            public static void Destroy()
            {
                _instance = null;
            }


            /// <summary>
            /// リスナーを追加する
            /// </summary>
            /// <param name="listener">Listener.</param>
            /// <param name="act">Act.</param>
            public bool Add(System.Object listener, Action<T> act)
            {
                // 重複チェック
                if (_listeners.ContainsKey(listener))
                {
                    return false;
                }

                _listeners[listener] = act;
                return true;
            }

            /// <summary>
            /// リスナーを削除
            /// </summary>
            /// <param name="listener">Listener.</param>
            public void Remove(System.Object listener)
            {
                _listeners.Remove(listener);
            }

            /// <summary>
            /// イベント発行
            /// </summary>
            /// <param name="ev">Ev.</param>
            public void Trigger(T ev)
            { 
                foreach(var elm in _listeners)
                {
                    elm.Value(ev); 
                }
            }


            /// <summary>
            /// なかったら削除する
            /// </summary>
            public void SafeDestroy(bool isForce = false)
            { 
                if (_instance == null)
                {
                    return; 
                }

                if (isForce || _instance._listeners.Count == 0)
                {
                    Destroy();
                }
            }
        };

        #endregion


        #region public, protected 変数

        #endregion


        #region private 変数

        private Dictionary<System.Object, List<IEventListener>> _dictListeners = 
            new Dictionary<System.Object, List<IEventListener>>();

        private bool _isTrigger = false;
        private List<System.Object> _removeList = new List<System.Object>();

        #endregion


        #region プロパティ


        #endregion


        #region コンストラクタ, デストラクタ

        #endregion


        #region public, protected 関数

        /// <summary>
        /// イベント追加する
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="act">Act.</param>
        /// <typeparam name="TEvent">The 1st type parameter.</typeparam>
        public void Add<TEvent>(System.Object listener, System.Action<TEvent> act) where TEvent : Event.Base
        {
            var instance = Listener<TEvent>.Instance;
            if (!instance.Add(listener, act))
            {
                // 追加できなかった
                instance.SafeDestroy();
                return;
            }

            List<IEventListener> list = null;
            if (!_dictListeners.TryGetValue(listener, out list))
            {
                list = new List<IEventListener>();

                _dictListeners.Add(listener, list);
            }

            list.Add(instance);
        }

        /// <summary>
        /// イベントを発行する
        /// </summary>
        /// <param name="ev">Ev.</param>
        /// <typeparam name="TEvent">The 1st type parameter.</typeparam>
        public void Trigger<TEvent>(TEvent ev) where TEvent : Event.Base
        {
            if (Listener<TEvent>.IsNull)
            {
                return; 
            }

            _isTrigger = true;

            var instance = Listener<TEvent>.Instance;
            instance.Trigger(ev);

            _isTrigger = false;


            // 削除がずらされているとき
            foreach(var elm in _removeList)
            {
                DoAllRemove(elm);
            }

            _removeList.Clear();
        }

        /// <summary>
        /// すべて削除
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <typeparam name="TListener">The 1st type parameter.</typeparam>
        public void AllRemove(System.Object listener)
        {
            // 追加中ならずらす
            if (_isTrigger)
            {
                _removeList.Add(listener);
                return;
            }

            DoAllRemove(listener);
        }



        public static void SafeAdd<TEvent>(System.Object listener, System.Action<TEvent> act) where TEvent : Event.Base
        { 
            if (IsNull)
            {
                return; 
            }

            Instance.Add(listener, act);
        }

        public static void SafeTrigger<TEvent>(TEvent ev) where TEvent : Event.Base
        {
            if (IsNull)
            {
                return;
            }

            Instance.Trigger(ev);
        }

        public static void SafeAllRemove(System.Object listener)
        {
            if (IsNull)
            {
                return;
            }

            Instance.AllRemove(listener);
        }

        #endregion


        #region private 関数



        /// <summary>
        /// 実際の削除処理
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <typeparam name="TListener">The 1st type parameter.</typeparam>
        private void DoAllRemove(System.Object listener)
        {
            List<IEventListener> list = null;
            if (!_dictListeners.TryGetValue(listener, out list))
            {
                return;
            }

            foreach (var elm in list)
            {
                elm.Remove(listener);

                elm.SafeDestroy();
            }

            _dictListeners.Remove(listener);
        }

        /*
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance); 
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }

        private void Update()
        {

        }*/

        public void OnDestory()
        {
//            if (Instance == this)
            {
                foreach (var elm in _dictListeners)
                {
                    var list = elm.Value;

                    foreach (var elm2 in list)
                    {
                        elm2.SafeDestroy(isForce: true);
                    }
                }
            }
        }

        #endregion
    }
}