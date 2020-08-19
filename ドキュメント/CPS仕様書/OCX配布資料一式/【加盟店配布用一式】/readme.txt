2019.10.11 ソリマチ技研

１．インストーラーについて

	【利用方法】
	インストーラー「OCX_Installer.bat」を"管理者権限"で実行してください。
	下記の処理が行われOCXが利用可能になります。

	※上記方法で管理者権限で実行できない場合は、下記の方法で実施ください。
	　１．コマンドプロンプトを管理者権限で実行
	　　　「Win＋s」で検索バーが出るので、"cmd"と入力
　　　　　　　コマンドプロンプトが出てくると思うので右クリック→管理者として実行

	　２．コマンドプロンプトでインストーラのディレクトリに移動
	   　　"cd (インストーラーのパス)"　と入力すると移動できます。

	　３．コマンドプロンプトで"OCX_Installer.bat"と入力してEnter

	
		・「C:\OPOS\CPS\EVRW\CPS_PaymentModule」ディレクトリの作成
		・OCXを上記ディレクトリにコピー
		・CCOのインストール
		・VisualStudio2008 runtimeの登録
		・レジストリの登録
		・WindowsへのOCXの登録
		
	【事前設定】
		①鍵ファイルの配置
		　下記のディレクトリに鍵ファイルを配置してください。
		　「C:\OPOS\CPS\EVRW\CPS_PaymentModule」
			rsa_private_key.pem
			rsa_public_key.pem
			serverPublicKey.pem

		②Settings.jsonに事前設定情報を記載してください。
		　設定内容は「EVRW_APG_PAYTREECPMSS.docx」の"2.3	設定ファイル構成"を参照ください。
		　スタブやISS環境及び本番環境への接続先は本ファイルに記載します。
		　本ファイルはOpenメソッド実行時に読み込まれるため、記載内容を変更した場合は再度Close→Openをして読み込みなおしてください。

	【注意事項】
		・「C:\OPOS\CPS\EVRW\CPS_PaymentModule」以下のファイルは移動させないでください。

２．テストツールについて
	OCXの動作を確認するテストツール「EVRWTest.exe」を同梱しています。
	OCXがインストールされていない場合起動しないためご注意下さい。
	
	【使用方法】
	・「EVRWTest.exe」を"管理者権限"で起動します。
	・実行したいボタンをクリックすることで任意のOCXのメソッドを実行可能です。
	・プロパティも同様にボタンをクリックすることで値を設定可能です。
	・AsyncModeプロパティがTRUEの場合ポップアップでOCXのイベントが通知されます。
	
	【操作例】
	・支払の実行
		１．Open("CPS_PaymentModule");
		２．ClaimDevice(5000);
		３．DeviceEnabledプロパティをTRUEに設定
		４．AsyncModeプロパティをTRUEに設定
		５．SetParameterInfomation()メソッドを利用して決済に必要な情報をタグ設定
			「PAYTREE対応OPOSドライバ設定項目一覧.xlsm」を参照
			支払の場合は下記のタグを設定ください。
			
				端末識別番号	terminalUniqueCode
				シリアルNo.	serialNo
				支払チャンネル	payType
				正札金額	amount
				レシート番号	receiptNo
				バーコード	oneTimeCode

				スタブ　　　　　stubUrl：stubPay/version=1.0/B01/0/0/

		６．"備考",及び"拡張情報"を入力する場合はAdditionalSecurityInfomationプロパティにJSON形式で入力
		７．SubtractValue()により支払APIを実行
		８．実行結果はOutpuCompleteEvent/ErrorEventのどちらかで通知される
			８－１．成功時　OutpuCompleteEventが通知される
				サーバーからのレスポンスはAdditionalSecurityInfomationプロパティ及びタグに設定される。
			
			８－２．失敗時　ErrorEventが通知される
				サーバーからのレスポンスはAdditionalSecurityInfomationに設定される。


３．制限事項
　・現在ログ機能に不備があり、日付が変わるとインクリメントされる機能が有効になっておらず。
　　ログが上書きされる動作となっております。こちらに関しては修正いたしますのでご了承ください。
