//
// Launcher.cs
// ProductName Ling
//
// Created by toshiki sakamoto on 2021.01.07
//

using Zenject;
using MessagePipe;
using UniRx;

namespace Ling
{
	/// <summary>
	/// プロジェクト側のLauncher
	/// Common.Launcherを継承して使用する
	/// </summary>
	public class Launcher : Common.Launcher
	{
		#region 定数, class, enum

		#endregion


		#region public, protected 変数

		#endregion


		#region private 変数

#if DEBUG
		[Inject] private Utility.DebugConfig.DebugRootMenuData _debugRoot = default;
#endif

		#endregion


		#region プロパティ

		#endregion


		#region コンストラクタ, デストラクタ

		#endregion


		#region public, protected 関数

		#endregion


		#region private 関数

		protected override void Awake()
		{
			base.Awake();

#if DEBUG
			_debugManager.Setup(_debugRoot);
#endif

			var subscriber = GlobalMessagePipe.GetSubscriber<Common.NoticeEvent>();
			subscriber.Subscribe(ev => 
				{
					UnityEngine.Debug.Log(ev.Message);
				}).AddTo(this);
		}

		#endregion
	}
}
