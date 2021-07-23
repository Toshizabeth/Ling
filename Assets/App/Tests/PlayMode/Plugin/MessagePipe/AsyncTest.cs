//
// AsyncTest.cs
// ProductName Ling
//
// Created by  on 2021.07.13
//

using NUnit.Framework;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine.TestTools;
using Zenject;
using MessagePipe;
using System.Threading;


namespace Ling.Tests.EditMode.Plugin.MessagePipeTest
{
	/// <summary>
	/// Asyncテスト
	/// </summary>
	public class AsyncTest
	{
		#region 定数, class, enum

		// イベント送る方
		public class Publisher
		{
			[Inject] private IAsyncPublisher<MyEvent> _asyncPublisher;

			public async UniTask SendAsync(MyEvent ev)
			{
				// Subscriberの購読処理が終わるまで待つ
				await _asyncPublisher.PublishAsync(ev);

				Debug.Log("イベント送信完了");
			}
		}

		// イベント受け取る方
		public class Subscriber
		{
			[Inject] private IAsyncSubscriber<MyEvent> _asyncSubscriber;
			[Inject] private ISubscriber<MyEvent> _subscriber;

			private CancellationTokenSource _cts = new CancellationTokenSource();
			private IDisposable _disposable;

			public void Subscribe()
			{
				var bag = DisposableBag.CreateBuilder();

                // イベントが来たら反応する
                _subscriber
					.Subscribe(ev =>
					{
						Debug.Log($"イベント受信完了: {ev.Message}");

					}).AddTo(bag);

				_disposable = bag.Build();
			}

			/// <summary>
            /// 最初のイベントが来るまで待機
            /// </summary>
			public async UniTask FirstAsync()
			{
				var ev = await _asyncSubscriber.FirstAsync(_cts.Token);

				Debug.Log($"イベント受信完了: {ev.Message}");
			}

			/// <summary>
			/// Subscribe内で待機
			/// </summary>
			public void SubscribeAsync(System.Action onFinished)
			{
				var bag = DisposableBag.CreateBuilder();

				_asyncSubscriber
					.Subscribe(async (x, ctr) =>
					{
						var time = Time.realtimeSinceStartup;

						// ここで非同期処理が可能
						// 大体1秒待機
						await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ctr);

						time = Time.realtimeSinceStartup - time;
						Debug.Log($"イベント受信完了: {x.Message} 待機秒数: {time}秒");

						// 終わったらよびだし
						onFinished?.Invoke();
					}).AddTo(bag);
				
				_disposable = bag.Build();
			}

			public void Close()
            {
				_disposable?.Dispose();
            }
		}

		// 送るイベント
		public class MyEvent
		{
			public string Message;
		}

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

		private DiContainer _container;

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		[SetUp]
		public void Setup()
		{
			_container = new DiContainer();
			_container.BindMessageBroker<MyEvent>(_container.BindMessagePipe());
		}

		[TearDown]
		public void TearDown()
        {
			_container.UnbindAll();
		}

		[UnityTest]
		public IEnumerator 非同期送信テスト()
		{
			Debug.Log("イベント受信");

			// イベントを受ける方
			var subscriber = _container.Instantiate<Subscriber>();
			subscriber.Subscribe();

			var task2 = UniTask.Create(async () =>
				{
					Debug.Log("イベント送信");

					// イベントを投げる方
					var publisher = _container.Instantiate<Publisher>();
					await publisher.SendAsync(new MyEvent { Message = "イベント" });
				});

			return UniTask.ToCoroutine(async () => await UniTask.WhenAll(task2));
		}

		[UnityTest]
		public IEnumerator 非同期送受信テスト()
		{
			var task1 = UniTask.Create(async () =>
				{
					bool isFinished = false;
					Debug.Log("イベント受信");

					// イベントを受ける方
					var subscriber = _container.Instantiate<Subscriber>();
					subscriber
						.SubscribeAsync(() =>
						{
							// 終わったらフラグをtrueにして終了させる
							isFinished = true;
						});

					return UniTask.WaitUntil(() => isFinished);
				});

			var task2 = UniTask.Create(async () =>
				{
					Debug.Log("イベント送信");

					// イベントを投げる方
					var publisher = _container.Instantiate<Publisher>();
					await publisher.SendAsync(new MyEvent { Message = "イベント" });
				});

			return UniTask.ToCoroutine(async () => await UniTask.WhenAll(task1, task2));
		}


		[UnityTest]
		public IEnumerator 非同期送受信テスト_FirstAsync()
		{
			var task1 = UniTask.Create(async () =>
				{
					Debug.Log("イベント受信");

					// イベントを受ける方
					var subscriber = _container.Instantiate<Subscriber>();
					await subscriber.FirstAsync();
				});

			var task2 = UniTask.Create(async () =>
				{
					Debug.Log("イベント送信");

					// イベントを投げる方
					var publisher = _container.Instantiate<Publisher>();
					await publisher.SendAsync(new MyEvent { Message = "イベント" });
				});

			return UniTask.ToCoroutine(async () => await UniTask.WhenAll(task1, task2));
		}


		[UnityTest]
		public IEnumerator 非同期送受信テスト_GlobalMessagePipe()
		{
			// GlobalMessagePipeを使用する前にSetProviderに設定する必要がある
			GlobalMessagePipe.SetProvider(_container.AsServiceProvider());

			// 非同期用のPublisher/Subscriberを生成する
			var asyncSubscriber = GlobalMessagePipe.GetAsyncSubscriber<MyEvent>();
			var asyncPublisher = GlobalMessagePipe.GetAsyncPublisher<MyEvent>();

			var cts = new CancellationTokenSource();

			var task1 = UniTask.Create(async () =>
				{
					Debug.Log("イベント受信");

					// イベントを受ける方
					var ev = await asyncSubscriber.FirstAsync(cts.Token);

					Debug.Log($"イベント受信完了 : {ev.Message}");
				});

			var task2 = UniTask.Create(async () =>
				{
					Debug.Log("イベント送信");

					// イベントを投げる方
					await asyncPublisher.PublishAsync(new MyEvent { Message = "イベント" }, cts.Token);

					Debug.Log("イベント送信完了");
				});

			return UniTask.ToCoroutine(async () => await UniTask.WhenAll(task1, task2));
		}

		[UnityTest]
		public IEnumerator あえてエラー出す() =>
			UniTask.ToCoroutine(async () =>
				{
					Assert.False(true);
				});

		#endregion


		#region private 関数

		#endregion
	}
}
