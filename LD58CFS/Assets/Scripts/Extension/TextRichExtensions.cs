#region

//文件创建者：Egg
//创建时间：10-04 08:02

#endregion

using UnityEngine;

namespace LD58.Extension
{
    public static class LegacyTextRichExtensions
    {
        // ===== 颜色 =====
        public static string ToRed(this string text) => $"<color=red>{text}</color>";
        public static string ToGreen(this string text) => $"<color=green>{text}</color>";
        public static string ToBlue(this string text) => $"<color=blue>{text}</color>";
        public static string ToYellow(this string text) => $"<color=yellow>{text}</color>";
        public static string ToCyan(this string text) => $"<color=cyan>{text}</color>";
        public static string ToMagenta(this string text) => $"<color=magenta>{text}</color>";
        public static string ToWhite(this string text) => $"<color=white>{text}</color>";
        public static string ToBlack(this string text) => $"<color=black>{text}</color>";

        // 自定义颜色（十六进制）
        public static string ToColor(this string text, string hexColor)
        {
            // 自动补 #（如果没写）
            if (!hexColor.StartsWith("#")) hexColor = "#" + hexColor;
            return $"<color={hexColor}>{text}</color>";
        }

        // 自定义颜色（Color 类型，自动转十六进制）
        public static string ToColor(this string text, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{hex}>{text}</color>";
        }

        // ===== 字体大小 =====
        public static string ToSize(this string text, int size) => $"<size={size}>{text}</size>";

        // ===== 样式 =====
        public static string ToBold(this string text) => $"<b>{text}</b>";
        public static string ToItalic(this string text) => $"<i>{text}</i>";
        public static string ToBoldItalic(this string text) => $"<b><i>{text}</i></b>";

        // ===== 其他 =====
        public static string ToAlpha(this string text, float alpha)
        {
            alpha = Mathf.Clamp01(alpha);
            return $"<alpha={Mathf.RoundToInt(alpha * 255)}>{text}</alpha>";
        }

        // 组合示例：病毒终端常用
        public static string ToVirusGreen(this string text) => text.ToColor("#00ff41").ToBold();
        public static string ToWarningRed(this string text) => text.ToColor("#ff3333").ToBold();
        public static string ToInfoCyan(this string text) => text.ToCyan().ToItalic();
    }
}