# VRChatLogEventOSC

VRChatのoutput-logを監視して特定のイベント発生時にOSCを送信するツール。
一部のイベントは詳細情報でフィルタリングもできます。
取得可能なイベントの一覧は[こちら](#events)

## 目次

- [VRChatLogEventOSC](#vrchatlogeventosc)
  - [目次](#目次)
  - [既知の問題](#既知の問題)
  - [ダウンロード](#ダウンロード)
  - [実行](#実行)
  - [動作環境](#動作環境)
  - [使い方](#使い方)
    - [コントロールウィンドウ](#コントロールウィンドウ)
      - [Control](#control)
      - [Config](#config)
    - [設定ウィンドウ](#設定ウィンドウ)
      - [Events](#events)
      - [Button](#button)
      - [Other](#other)
    - [エディターウィンドウ](#エディターウィンドウ)
      - [共通項目](#共通項目)
      - [フィルタリング項目](#フィルタリング項目)
  - [ThirdParty](#thirdparty)
  - [License](#license)
  - [その他](#その他)

## 既知の問題

- インスタンスからの退出時の`On Player Left`は、VRChatに送信する場合アバターのExpressionParameterに反映されません。代わりに`Left Room`を使用してください。
- `Accept Friend Request`および`Accept Request Invite`は、実環境での動作確認が行えていません(自力でテストできないため)。過去のログの読み込みでは正常動作を確認しています。

## ダウンロード

[Release](https://github.com/Shiokai/VRChatLogEventOSC/releases)からダウンロードできます。
また、[Booth](https://shiokai.booth.pm/items/4282528)でも配布しています。

実行には.NET6のDesktop Runtimeが必要です。
Runtimeがインストールされていない場合、[ダウンロードページ](https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0)からダウンロードしてインストールするか、コマンドプロンプトから

```cmd
winget install Microsoft.DotNet.DesktopRuntime.6
```

を実行してインストールしてください。

何らかの要因でRuntimeのインストールができない場合、Runtime統合版であるVRChatLogEventOSC_SelfContainedを代わりにダウンロードしてください。
ただし実行ファイルのファイルサイズが大きく(約30倍)なります。

## 実行

ダウンロードしたzipファイルを展開し、`VRChatLogEventOSC.exe`を実行してください。
実行中はタスクトレイにアイコンが表示されます。
設定の編集のみを行う場合、`VRChatLogEventOSC-setting.bat`を実行するか、コマンドライン引数`--setting`または`-s`を与えて実行してください。

## 動作環境

64bit Windowsで動作します。

なお、`Program Files`等アクセス制限のあるフォルダでは正常に実行できません。

## 使い方

実行すると、タスクトレイに常駐し、最新のVRChatのログを監視し特定のイベントが発生した際にOSCを送信します。

初めて実行した際に、実行ファイルと同じ階層に`setting.json`および`config.json`が生成されます。
これらのファイルに設定およびコンフィグが保存されています。
不用意に削除、移動、リネーム等を行った場合、設定やコンフィグが読み込めない場合があります。

なお、複数同時起動し、設定やコンフィグの編集を行った場合、設定やコンフィグの保存・適用が行われるのはそのウィンドウを呼び出したものだけです。自身以外で編集した設定やコンフィグを読み込みたい場合、アプリケーションを一度終了する必要があります。

タスクトレイのアイコンを右クリックすると、以下の項目が表示されます。

- `Open Control`: コントロールウィンドウを開きます
- `Open Setting`: 設定ウィンドウを開きます
- `Pause [✓]/[]`: クリックする毎に、ログの読み取りの一時停止/再開を切り替えます。再開方法はコントロールウィンドウの`Restart`と同様です。
- `Quit`: アプリケーションを終了します

コントロールウィンドウはアイコンのダブルクリックでも開けます。
また、アイコンにマウスをかざすことでログの読み取りが停止されているかを確認できます。

### コントロールウィンドウ

アプリケーションの動作コントロール、およびコンフィグの変更を行うウィンドウです。

#### Control

- `Pause`: ログの読み取りを一時停止します
- `Restart`: 現在の位置からログの読み取りを再開します (停止中に発生したイベントは読み込まれません)
- `Continue`: 最後の読み取り位置からログの読み取りを再開します
- `Rescan`: ログの先頭から読み取りを再開します
- `Quit`: アプリケーションを終了します

#### Config

- `LogDirectory`: VRChatのoutput_logが出力されるフォルダを指定します (Default: `C:\Users\<UserName>\AppData\LocalLow\VRChat\VRChat`)
- `IP Address`: OSC送信先のIP Addressを指定します (Default: `127.0.0.1`)
- `Port`: OSC送信先のポート番号を指定します (Default: 9000)
- `Tuned`: 一部のイベントの発行タイミングを調整します。発行タイミングでVRChatのアバターのExpressionParameterが値を受け取れないイベントについて、その発行タイミングを遅らせて対応します。 (Default: true)
  - `JoiningRoomURL`: `EnteredWorld`のタイミングで発行されます
  - `JoiningRoomName`: `EnteredWorld`のタイミングで発行されます

- `Apply`: コンフィグを保存、適用します

### 設定ウィンドウ

OSCを送信するイベントを設定します。

#### Events

設定可能なイベントは以下の通りです。

- `Joining Room URL`: ワールドにJoinする際に発行されるイベント。ワールドのURLに関連した情報が取得できます。
- `Joining Room Name`: ワールドにJoinする際に発行されるイベント。ワールドの名前が取得できます。
- `Accept Friend Request`: 自身に対して送られたFrieand RequestをAcceptした際に発行されるイベント
- `Played Video`: 動画プレイヤーで動画を再生した際に発行されるイベント
- `Accept Invite`: 自身に対して送られたInviteをAcceptした際に発行されるイベント
- `Accept Request Invite`: 自身に対して送られたRequest InviteをAcceptした際に発行されるイベント
- `On Player Joined`: UserがJoinした際に発行されるイベント
- `On Player Left`: UserがLeaveした際に発行されるイベント
- `Took Screenshot`: 写真を撮影した際に発行されるイベント
- `Left Room`: 自身がインスタンスを離れる際に発行されるイベント
- `Entered World`: 自身がインスタンスに入った際に発行されるイベント
- `Rejoining`: Rejoinをしたタイミングで発行されるイベント
- `GoHome`: GoHomeをしたタイミングで発行されるイベント

各イベントのボタンをクリックすると、そのイベントに対する設定の一覧が表示されます。
設定の一覧では、以下の項目がプレビューできます。

- `Name`
- `OSCAddress`
- `OSCValue`
- `OSCType`

項目の詳細は[エディターウィンドウ](#エディターウィンドウ)の項を参照してください。

#### Button

設定ウィンドウには以下のボタンが存在します。

- `Up`: 選択された設定を一覧上で一つ上に移動します
- `Down`: 選択された設定を一覧上で一つ下に移動します
- `Add`: 新規設定追加用にエディターウィンドウを開きます
- `Edit`: 選択された設定の編集用にエディターウィンドウを開きます
- `Delete`: 選択された設定を削除します
- `Apply`: 現在の設定を保存、適用します

#### Other

設定のダブルクリックでその設定の編集用にエディターウィンドウを開きます。
同じイベントのOSCの送信は、設定一覧で上にあるものから順に行われます。
エディターウィンドウを開いている間、設定ウィンドウの操作はできません。

### エディターウィンドウ

設定の内容を編集します。

以下の項目が編集可能です。

#### 共通項目

- `Setting Name`: 任意。設定に割り当てることのできる名前です。設定ウィンドウでのプレビューでの設定識別用。
- `OSC Address`: OSC送信先のアドレスです (Default: `/avatar/parameters/<EventName>`)
- `OSC Value`: 送信するOSCの値です。空欄の場合、その設定は無視されます
  - `Bool`: true/false
  - `Int`: 0\~255
  - `Float`: -1.0\~1.0
  - `String`
- `OSC Type`: OSCの送信方法です
  - `Button`: 設定された値を送信し、その0.3秒後に既定の値(false/0/0.0/空文字)を送信します
  - `Toggle`: 設定された値を送信します

なお、`OSC Address`が`/chatbox/input`であり、かつ`OSC Type`が`Toggle`の場合に限り、`OSC Value`に設定した値と同時にbool値`true`を送信します。
これは、VRChatのChatboxが受け付けるOSCが`/chatbox/input s b`の形式なためです。

#### フィルタリング項目

以下の項目を設定することで、イベントの内容に応じてOSCを送信するかしないかを振り分けることができます。
空欄またはプルダウンの`None`に設定されている項目は評価されません。
すべての項目はANDの完全一致です。

各項目の説明は以下の通りです。

- `UserName`: イベントに関連するユーザーのDisplay Name
- `UserID`: イベントに関連するユーザーのID(`usr_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)
- `WorldName`: イベントに関連するワールドの名前
- `WorldURL`: イベントに関連するインスタンスのURL。`wrld`からはじまり、nonceまで含めて完全に一致する必要があります。
- `WorldID`: イベントに関連するワールドのID(`wrld_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)
- `InstanceID`: イベントに関連するインスタンスのID
- `InstanceType`: イベントに関連するインスタンスのType
  - `None`: 指定なし
  - `Public`
  - `Friends+`
  - `Friends`
  - `Invite+`
  - `Invite`
- `WorldUserID`: イベントに関連するインスタンスの作成者のID(`usr_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)
- `Region`: イベントに関連するインスタンスのRegion
  - `None`: 指定なし
  - `US`
  - `USE`
  - `EU`
  - `JP`
- `Message`: イベントに関連するMessage
- `URL`: イベントに関連するURL

イベントによって設定できるものと設定できないものがあります。
各イベントで設定できる項目は以下の通りです。

- `JoinedRoomURL`
  - `WorldURL`: JoinするインスタンスのURL
  - `WorldID`: JoinするワールドのID
  - `InstanceID`: JoinするインスタンスのID
  - `InstanceType`: JoinするインスタンスのType
  - `WorldUserID`: Joinするインスタンスの作成者のID
  - `Region`: JoinするインスタンスのRegion
- `JoinedRoomName`
  - `WorldName`: Joinするワールドの名前
- `AcceptFriendRequest`
  - `UserName`: Friend Requestを送信したUserのDisplay Name
  - `UserID`: Friend Requestを送信したUserのID
- `PlayedVideo`
  - `URL`: 再生される動画のURL
- `AcceptInvite`
  - `UserName`: Inviteを送信したUserのDisplay Name
  - `UserID`: Inviteを送信したUserのID
  - `WorldURL`: InviteによってJoinする先のインスタンスURL
  - `WorldID`: InviteによってJoinする先のワールドID
  - `InstanceID`: InviteによってJoinする先のインスタンスID
  - `InstanceType`: InviteによってJoinする先のインスタンスType
  - `WorldUserID`: InviteによってJoinする先のインスタンスの作成者のID
  - `Region`: InviteによってJoinする先のインスタンスのRegion
  - `WorldName`: InviteによってJoinする先のワールドの名前
  - `Message`: Inviteに付与されたMessage
- `AcceptRequestInvite`
  - `UserName`: Request Inviteを送信したUserのDisplay Name
  - `UserID`: Request Inviteを送信したUserのID
  - `Message`: Request Inviteに付与されたMessage
- `OnPlayerJoined`
  - `DisplayName`: JoinしたUserのDisplay Name
- `OnPlayerLeft`
  - `DisplayName`: LeaveしたUserのDisplay Name
- `TookScreenshot`
  - (無し)
- `LeftRoom`
  - (無し)
- `EnteredWorld`
  - (無し)
- `Rejoining`
  - `WorldURL`: RejoinするインスタンスのURL
  - `WorldID`: RejoinするワールドのID
  - `InstanceID`: RejoinするインスタンスのID
  - `InstanceType`: RejoinするインスタンスのType
  - `WorldUserID`: Rejoinするインスタンスの作成者のID
  - `Region`: RejoinするインスタンスのRegion
- `GoHome`
  - (無し)

## ThirdParty

- VRChatActivityTools (License: "MIT")
- Reactive Extensions (License: "MIT")
- ReactiveProperty(License  : "MIT")
- Rug.Osc (License: "MIT")
- SystemTrayApp.WPF (License: "0BSD")
- Microsoft.Xaml.Behaviors.Wpf (License: "MIT")

詳細は[ThirdPartyNotice](ThirdPartyNotices.md)を参照してください。

## License

MIT License
詳細は[LICENSE](LICENSE)を参照してください。

## その他

VRChatのログの出力形式の変更等により、予告なく正常に動作しなくなる可能性があります。
ご理解の上でご利用ください。
