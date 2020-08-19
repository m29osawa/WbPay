
keytool実行時のパラメーター

PassWord（キーストアのパスワード）=Wellba!1122


First and Last Name（姓名）=wba.jp
Organizational Unit（組織単位）=MockServer
Organization（組織）=Wellba
City/Locality（都市／地方）=Azabu
State/Province（都道府県／州）=Tokyo
Country Code（国コード）=JP


鍵の生成
keytool -genkey -keystore .keystore -alias tomcat -keyalg RSA

Current dir に.keystoreができる。"%USERPROFILE%"\.keystoreかも
生成された鍵は90日間しか有効ではないらしい：
90日間有効な2,048ビットのRSAのキー・ペアと自己署名型証明書(SHA256withRSA)を生成しています

CSRファイルの作成
keytool -certreq -keyalg RSA -alias tomcat -file tomcat.csr

証明書を入れる場合は、このcsrファイルを認証局に送る。

その後、入手した証明書をkeystoreにimportする。

証明書のエクスポート

keytool -keystore .keystore -exportcert -alias tomcat -file tomcat.cer -rfc

Windows の証明書の管理

ローカルコンピューターに保存されている、サーバー証明書の管理

certlm.msc

ユーザーアカウントを証明するユーザー証明書の管理

certmgr.msc


