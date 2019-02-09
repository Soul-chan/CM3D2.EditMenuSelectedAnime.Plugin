・CM3D2.EditMenuSelectedAnime.Plugin 1.0.0.0

・概要
　エディットメニューの選択中アイテムを見つけやすいようにアイコンを拡縮させます。
　メニュー変更時には選択中アイテムが先頭行に来るように自動スクロールさせます。

・使用方法
　「UnityInjector」フォルダに「CM3D2.EditMenuSelectedAnime.Plugin.dll」を入れてください。

・カスタマイズ
　拡縮が邪魔、とか自動スクロールが邪魔、とか片方の機能しか必要がない場合は、
　ソースの20行目と21行目で設定出来ますので、書き換えて「AutoCompile」に入れ
　ビルドしてください。
　下記のように「true」を「false」に書き換えると無効にできます。
　両方「false」にすると何もしなくなります。
　
　自動スクロールを無効にする場合
　	public const bool AutoScrollEnable = false;
　
　拡縮を無効にする場合
　	public const bool ScalingEnable = false;

・ソースについて
　改変、転載、ご自由にどうぞ


※MODはKISSサポート対象外です。
※MODを利用するに当たり、問題が発生してもKISSは一切の責任を負いかねます。
※カスタムメイド3D2を購入されている方のみが利用できます。
※カスタムメイド3D2上で表示する目的以外の利用は禁止します。
※これらの事項は http://kisskiss.tv/kiss/diary.php?no=558 を優先します。
