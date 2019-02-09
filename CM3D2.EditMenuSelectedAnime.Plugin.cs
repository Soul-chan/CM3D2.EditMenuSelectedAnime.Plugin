using System;
using System.Reflection;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;
using System.Collections.Generic;

namespace CM3D2.EditMenuSelectedAnime.Plugin
{
	[PluginFilter( "CM3D2x64" )]
	[PluginFilter( "CM3D2x86" )]
	[PluginFilter( "CM3D2OHx64" )]
	[PluginFilter( "CM3D2OHx86" )]
	[PluginName( "EditMenuSelectedAnime" )]
	[PluginVersion( "1.0.0.0" )]

	// 定数
	public static class Define
	{
		public const bool AutoScrollEnable = true;	// 選択中のボタンまで自動スクロールするかどうか
		public const bool ScalingEnable = true;		// ボタンの拡縮を使用するかどうか

		public const double ScalingSpeed = 2.0;		// ボタンの拡縮スピード
		public const int ScalingSize = 12;			// ボタンの拡縮サイズ
	}

	public class EditMenuSelectedAnime : UnityInjector.PluginBase
	{
		private bool m_isSceneEdit = false;
		private bool m_isInstallMenu = false;
		private bool m_isInstallGrp = false;
		private double m_angle = 0.0;   // 拡縮で使うSinの角度

		public static EditMenuSelectedAnime Instance { get; private set; }
		private void Awake()
		{
		}
		private void OnLevelWasLoaded( int level )
		{
			Instance = this;
			m_isSceneEdit = false;
			m_isInstallMenu = false;
			m_isInstallGrp = false;

			// エディットならインストールフラグを立てる
			if ( Application.loadedLevelName == "SceneEdit" )
			{
				m_isSceneEdit = true;
				m_isInstallMenu = true;
				m_isInstallGrp = true;
				ResetScaleSize();
			}
		}

		private void Update()
		{
			if ( m_isSceneEdit )
			{
				// インストール開始
				if ( m_isInstallMenu )
				{
					InstallMenu();
				}
				if ( m_isInstallGrp )
				{
					InstallGrp();
				}

				// 拡縮用の角度を更新
				m_angle += (Time.deltaTime * Math.PI * Define.ScalingSpeed);
			}
		}
		// エディットメニューのUIGridにコンポーネントをつける
		private void InstallMenu()
		{
			Transform uiRoot = GameObject.Find( "UI Root" ).transform;

			// エディットメニューのUIGridを探してSelectedAnimeCtrlコンポーネントをつける
			GameObject uiGrid = uiRoot.Find( "ScrollPanel-MenuItem/Scroll View/UIGrid" ).gameObject;
			if ( uiGrid &&
				 uiGrid.GetComponent<SelectedAnimeCtrl>() == null )
			{
				uiGrid.AddComponent<SelectedAnimeCtrl>();
				m_isInstallMenu = false;
			}
		}
		// グループ選択オブジェクトを探す
		private void InstallGrp()
		{
			Transform uiRoot = GameObject.Find( "UI Root" ).transform;

			// グループ選択のUIGridを探してSelectedAnimeCtrlコンポーネントをつける
			GameObject uiGrid = uiRoot.Find( "ScrollPanel-GroupSet/Scroll View/UIGrid" ).gameObject;
			if ( uiGrid &&
				 uiGrid.GetComponent<SelectedAnimeCtrl>() == null )
			{
				uiGrid.AddComponent<SelectedAnimeCtrl>();
				m_isInstallMenu = false;
			}
		}

		// 拡縮するための加算サイズを返す
		// メニューアイテムとグループ選択のボタン拡縮を同期させる為に
		// 拡縮サイズはメインスクリプト側で管理する
		public int GetScaleSize()
		{
			if ( Define.ScalingEnable )
			{
				return (int)(Math.Sin( m_angle ) * Define.ScalingSize);
			}
			return 0;
		}
		// 拡縮するための加算サイズをリセット
		public void ResetScaleSize()
		{
			m_angle = 0.0;
		}
	}

	//////////////////////////////////////////////////////
	//////////////////////////////////////////////////////
	//////////////////////////////////////////////////////
	/// メニューアイテムリストのUIGridに付けるコンポーネント
	public class SelectedAnimeCtrl : MonoBehaviour
	{
		private bool m_isInstall = false;
		private void Start()
		{
		}

		private void Update()
		{
			if ( !m_isInstall )
			{
				// 子供が作られ終わったら
				if ( transform.childCount > 0 )
				{
					UIGrid grid = transform.GetComponent<UIGrid>();

					if ( grid )
					{
						// 子供(ボタン)を取得
						List<Transform> items = grid.GetChildList();
						float topY = float.MinValue;
						Transform topButton = null;
						Transform selectedButton = null;

						// フレームに拡縮アニメーション用のコンポーネントを付けていく
						foreach ( var item in items )
						{
							Transform frame = item.Find( "Frame" );

							if ( frame )
							{
								// コンポーネントを付ける
								frame.gameObject.AddComponent<SelectedAnime>();

								// フレームがアクティブなら選択されたボタン
								if ( frame.gameObject.activeSelf )
								{
									selectedButton = frame.parent;
								}
								// 一番上の列にあるボタンを探しておく
								// 上のほうがローカルYが大きい
								if ( frame.parent.localPosition.y > topY )
								{
									topY = frame.parent.localPosition.y;
									topButton = frame.parent;
								}
							}
						}

						// 選択されたボタンをまでスクロールするかどうか
						if ( Define.AutoScrollEnable )
						{
							if ( selectedButton && topButton )
							{
								UICenterOnChild coc = GetComponent<UICenterOnChild>();
								UIScrollView sv = transform.parent.GetComponent<UIScrollView>();
								UIScrollBar sb = transform.parent.parent.GetComponentInChildren<UIScrollBar>();

								if ( coc && sv && sb )
								{
									// スクロールバーが有効なら
									if ( sv.shouldMoveVertically )
									{
										// ローカル位置にボタンの座標が入っている
										float buttonPos = Math.Abs(selectedButton.localPosition.y - topButton.localPosition.y);
										// 一番下まで下げた時のビューの位置
										float bottomPos = sv.bounds.size.y - sv.panel.height + 20;
										// スクロールバーの位置
										float scrollVal = 0.0f;

										// バーの位置を算出する
										scrollVal = buttonPos / bottomPos;
										scrollVal = Math.Max( Math.Min( scrollVal, 1.0f ), 0.0f );

										sb.value = scrollVal;
										sb.ForceUpdate();
									}
								}
							}
						}

						m_isInstall = true;
					}
				}
			}
		}
		private void OnEnable()
		{
			// ほかのメニューに変える際にもOnEnableに来るので
			// 再インストールされる様にフラグをクリアする
			m_isInstall = false;
		}
	}


	//////////////////////////////////////////////////////
	//////////////////////////////////////////////////////
	//////////////////////////////////////////////////////
	/// ボタンのFrameに付けるコンポーネント
	public class SelectedAnime : MonoBehaviour
	{
		UISprite m_frameSpr;
		UI2DSprite m_buttonSpr;
		int m_frameWidth = 0;
		int m_frameHeight = 0;
		int m_buttonWidth = 0;
		int m_buttonHeight = 0;
		private void Start()
		{
			// フレームのUISpriteを探して幅と高さを覚えておく
			m_frameSpr = transform.GetComponent<UISprite>();
			if ( m_frameSpr )
			{
				m_frameWidth = m_frameSpr.width;
				m_frameHeight = m_frameSpr.height;
			}

			// ボタンのUI2DSpriteも探して幅と高さを覚えておく
			Transform button = transform.parent.Find( "Button" );
			if ( button )
			{
				m_buttonSpr = button.GetComponent<UI2DSprite>();
				if ( m_buttonSpr )
				{
					m_buttonWidth = m_buttonSpr.width;
					m_buttonHeight = m_buttonSpr.height;
				}
			}
		}

		private void Update()
		{
			// フレームとボタンを拡縮して見つけやすくする
			if ( m_frameSpr && m_buttonSpr )
			{
				int addSize = EditMenuSelectedAnime.Instance.GetScaleSize();

				m_frameSpr.width = m_frameWidth + addSize;
				m_frameSpr.height = m_frameHeight + addSize;

				m_buttonSpr.width = m_buttonWidth + addSize;
				m_buttonSpr.height = m_buttonHeight + addSize;
			}
		}

		private void OnDisable()
		{
			EditMenuSelectedAnime.Instance.ResetScaleSize();

			// 変更したサイズを戻しておく
			if ( m_frameSpr )
			{
				m_frameSpr.width = m_frameWidth;
				m_frameSpr.height = m_frameHeight;
			}
			if ( m_buttonSpr )
			{
				m_buttonSpr.width = m_buttonWidth;
				m_buttonSpr.height = m_buttonHeight;
			}
		}
	}
}
