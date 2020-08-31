# cocoa-ios-date-identification
iOSの接触日が特定できない不具合に対応するためHASH値より日にちを特定するプログラムです。

処理のメインは
covidTest/Program.cs
にて行われています。

# 作成者について
えじもじゃ（Twitterアカウント：[@edisonMJsss](https://twitter.com/edisonMJsss)/GitHubアカウント：[@edisonmjsss](https://github.com/edisonmjsss)）です。

# 日にちの特定
アプリはサーバーから陽性者が提供したキー情報を  
ダウンロードしています。

ダウンロードしたデータの中には  
接触した可能性がある日も記載されています。  

iPhoneの接触のログ記録の中にあるHashは  
ダウンロードデータから計算されています。  

そのHashを再計算する事にでダウンロードデータと接触ログの記録を  
紐付ける事が出来る様になりました。  

# Hash
ある情報Aを特定の計算方法で計算した結果です。  
この計算結果は情報Aが同じ物であれば必ず同じ結果になります。  

逆に1文字でも違う情報で計算すると全く違う結果になります。  
(同じ結果になる事はほぼ100%有り得ません)  

今まではiPhoneの接触のログ記録で見れる計算結果(答え)しか分かりませんでしたが  
計算方法と情報Aを特定し同じ計算結果(答え)を再計算しました。  

Googleに公開されている情報から計算方法と情報Aを特定しました。  
[https://github.com/google/exposure-notifications-internals](https://github.com/google/exposure-notifications-internals)

# iPhoneの接触ログ記録確認方法
[このツイート](https://twitter.com/teriha8t8/status/1299202626001666054?s=19)を参考に接触のログ記録を確認して  
MatchCountが1以上の記録を探してHashの値を確認して下さい。  

iPhoneで確認したHashを[このサイト](https://cacaotest.sakura.ne.jp/)で入力してください。  
※Hashの先頭５文字程度で１件に絞り込めます。  

## License

These codes are licensed under CC0.

[![CC0](http://i.creativecommons.org/p/zero/1.0/88x31.png "CC0")](http://creativecommons.org/publicdomain/zero/1.0/deed.ja)