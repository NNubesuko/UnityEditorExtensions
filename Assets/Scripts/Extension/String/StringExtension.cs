using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringExtension {

    /*
     * 秒の文字列を TimeSpan に変換する
     */
    public static TimeSpan SecondsToTimeSpan(this string self) {
        return TimeSpan.FromSeconds(
            double.Parse(self)
        );
    }

    /*
     * ミリ秒の文字列を TimeSpan に変換する
     */
    public static TimeSpan MillisecondsToTimeSpan(this string self) {
        return TimeSpan.FromMilliseconds(
            double.Parse(self)
        );
    }

    /*
     * 文字列に日本語か全角スペースが含まれているか判定するメソッド
     */
    public static bool IsJapanese(this string self) {
        bool isJapanese = Regex.IsMatch(
            self,
            @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]+|[　]+"
        );
        return isJapanese;
    }

    /*
     * 絶対パスから Assets/ のパスへ変換
     */
    public static string AbsoluteToAssetsPath(this string self) {
        return self.Replace("\\", "/").Replace(Application.dataPath, "Assets");
    }

    /*
     * Assets/ のパスから絶対パスへ変更
     */
    public static string AssetsToAbsolutePath(this string self) {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return self.Replace("Assets", Application.dataPath).Replace("/", "\\");
        #else
                return self.Replace("Assets", Application.dataPath);
        #endif
    }

}