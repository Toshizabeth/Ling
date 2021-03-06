﻿// 
// BattleMesageView.cs  
// ProductName Ling
//  
// Created by toshiki sakamoto on 2020.05.06
// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Utility;
using System.Linq;
using ModestTree;

namespace Ling.Scenes.Battle.Message
{
	/// <summary>
	/// 
	/// </summary>
	public class MessageView : MonoBehaviour
	{
		#region 定数, class, enum

		class TextData
		{
			public string text;
			public System.Action onFinish;
		}

		#endregion


		#region public 変数

		#endregion


		#region private 変数

		[SerializeField] private Transform _windowRoot = null;
		[SerializeField] private Transform _contentsRoot = null;
		[SerializeField] private MessageItemView _textItem = null;
		[SerializeField] private int _textNum = 5;
		[SerializeField] private int _textItemHeight = 50;
		[SerializeField] private float _topPadding = 0.0f;
		[SerializeField] private float _itemSpace = 0.0f;
		[SerializeField] private float _itemUpperAnimationTime = 0.2f;
		[SerializeField] private float _messageDisplaySpeed = 0.05f;

		private Queue<MessageItemView> _textItemQueue = new Queue<MessageItemView>();
		private Queue<MessageItemView> _activeTextItemQueue = new Queue<MessageItemView>();
		private Queue<TextData> _textQueue = new Queue<TextData>();
		private BoolReactiveProperty _canNextTextShow = new BoolReactiveProperty();
		private SerialDisposable _textAnimDisposable = new SerialDisposable();

		#endregion


		#region プロパティ

		public ReadOnlyReactiveProperty<bool> CanNextTextShow => _canNextTextShow.ToReadOnlyReactiveProperty();

		#endregion


		#region public, protected 関数

		public void Setup()
		{
			_textItem.gameObject.SetActive(false);

			// Textを作成
			for (int i = 0; i < _textNum; ++i)
			{
				var instance = Instantiate<MessageItemView>(_textItem, _contentsRoot);
				instance.ChangeTextDisplaySpeed(_messageDisplaySpeed);

				_textItemQueue.Enqueue(instance);
			}

			_canNextTextShow.Value = true;
		}

		public void SetText(string text, System.Action finish = null)
		{
			Utility.Log.Print($"Message : {text}");

			_textQueue.Enqueue(new TextData { text = text, onFinish = finish });

			ShowTextIfNeeded();
		}

		public void Clear()
		{
			// 動きを止める
			if (!_textAnimDisposable.IsDisposed)
			{
				_textAnimDisposable.Dispose();
				_textAnimDisposable = new SerialDisposable();
			}

			while (!_activeTextItemQueue.IsEmpty())
			{
				var item = _activeTextItemQueue.Dequeue();
				item.gameObject.SetActive(false);

				_textItemQueue.Enqueue(item);
			}

			_textQueue.Clear();
			_canNextTextShow.Value = true;
		}

		#endregion


		#region private 関数


		/// <summary>
		/// 可能であればテキストを出す
		/// </summary>
		private void ShowTextIfNeeded()
		{
			// 表示することができるか
			if (!_canNextTextShow.Value) return;
			if (_textQueue.Count <= 0) return;

			_canNextTextShow.Value = false;

			// 最大数出ている場合上にずらす
			_textAnimDisposable.Disposable = Observable
				.FromCoroutine(() => PlayAnimationActiveTextItem())
				.Subscribe(_ =>
				{
					var textData = _textQueue.Dequeue();

					// 使用キューに追加
					var activeTextItem = _textItemQueue.Dequeue();
					_activeTextItemQueue.Enqueue(activeTextItem);

					// テキストを設定する
					activeTextItem.SetText(textData.text);
					activeTextItem.OnTextShowEnd =
						() =>
						{
							textData.onFinish?.Invoke();

							// テキスト表示終了時
							// 次に進める
							_canNextTextShow.Value = true;

							ShowTextIfNeeded();
						};

					AdjustItemsPosition();
				});
		}


		private IEnumerator PlayAnimationActiveTextItem()
		{
			AdjustItemsPosition();

			// 最大数表示されていない場合はアニメーションしない
			if (_activeTextItemQueue.Count < _textNum)
			{
				yield break;
			}

			foreach (var item in _activeTextItemQueue)
			{
				item.UpperAnimation(_textItemHeight + _itemSpace, _itemUpperAnimationTime);
			}

			while (true)
			{
				// 全て終わってないと先に進ませない
				if (_activeTextItemQueue.All(item_ => !item_.IsPlayAnimation))
				{
					break;
				}

				yield return null;
			}

			// 一番上のアイテムを未使用キューに戻す
			var dequeueItem = _activeTextItemQueue.Dequeue();
			dequeueItem.gameObject.SetActive(false);

			_textItemQueue.Enqueue(dequeueItem);

			AdjustItemsPosition();
		}

		/// <summary>
		/// アイテムの座標を調整する
		/// </summary>
		private void AdjustItemsPosition()
		{
			float posY = _topPadding;

			foreach (var item in _activeTextItemQueue)
			{
				item.SetPositionY(posY);

				// 差分込みで足し合わせる
				posY -= _textItemHeight + _itemSpace;
			}
		}

		#endregion


		#region MonoBegaviour

		#endregion
	}
}