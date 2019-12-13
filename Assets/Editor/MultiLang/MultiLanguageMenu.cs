//
// Copyright (C) 2017 LimeVista
// Author: LimeVista(https://github.com/LimeVista/MultiLangLiteUnity)
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the The MIT License (MIT).
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
// See The MIT License (MIT) for more details.
//
// You should have received a copy of The MIT License (MIT)
// along with this library.
//
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{

    #region 支持语言列表
    /**
     *  支持语言
     *  Afrikaans	        南非荷兰语
     *  Arabic	            阿拉伯语
     *  Basque	            巴斯克
     *  Belarusian	        白俄罗斯
     *  Bulgarian	        保加利亚
     *  Catalan	            加泰罗尼亚
     *  Chinese	            中文（请使用中文简体和中文繁体）
     *  Czech	            捷克
     *  Danish	            丹麦
     *  Dutch	            荷兰
     *  English	            英语
     *  Estonian	        爱沙尼亚
     *  Faroese	            法罗群岛
     *  Finnish	            芬兰
     *  French	            法语
     *  German	            德语
     *  Greek	            希腊文
     *  Hebrew  	        希伯来语
     *  Hugarian   	        匈牙利
     *  Hungarian	        匈牙利
     *  Icelandic	        冰岛语
     *  Indonesian	        印尼语
     *  Italian	            意大利语
     *  Japanese	        日语
     *  Korean	            韩语
     *  Latvian	            拉脱维亚
     *  Lithuanian	        立陶宛语
     *  Norwegian	        挪威语
     *  Polish	            波兰
     *  Portuguese	        葡萄牙语
     *  Romanian	        罗马尼亚
     *  Russian	            俄语
     *  SerboCroatian	    塞尔博克罗地亚
     *  Slovak	            斯洛伐克
     *  Slovenian	        斯洛维尼亚
     *  Spanish	            西班牙语
     *  Swedish	            瑞典语
     *  Thai	            泰国
     *  Turkish	            土耳其语
     *  Ukrainian	        乌克兰
     *  Vietnamese	        越南语
     *  ChineseSimplified	中国简体
     *  ChineseTraditional	中国繁体
     */
    #endregion

    public static class MultiLanguageMenu
    {
        [MenuItem("Tools/MultiLanguage/Debug")]
        public static void MultiLanguageDebug()
        {
            MultiLanguageImpl(true);
        }

        [MenuItem("Tools/MultiLanguage/Release")]
        public static void MultiLanguageRelease()
        {
            MultiLanguageImpl(false);
        }

        private static void MultiLanguageImpl(bool debug)
        {
            var grs = new GenerateRStrings(debug);
            if (!grs.Reader()) return;
            grs.Generate();
            EditorUtility.DisplayDialog("成功", "操作成功", "确定");
        }
    }

    class GenerateRStrings
    {
        /// <summary>
        /// Unity 所有支持语言列表
        /// </summary>
        private readonly string[] LangNames = Enum.GetNames(typeof(SystemLanguage));

        /// <summary>
        /// I18n
        /// </summary>
        private readonly Dictionary<string, StringItem> Items =
            new Dictionary<string, StringItem>();

        /// <summary>
        /// 是否为 Debug 模式
        /// </summary>
        private readonly bool IsDebug = false;

        /// <summary>
        /// 翻译语言列表
        /// </summary>
        private readonly List<string> TransLangs = new List<string>();

        public GenerateRStrings(bool debug)
        {
            IsDebug = debug;
        }

        /// <summary>
        /// 读取分析多语言 Json
        /// </summary>
        /// <returns>是否操作成功，并且不存在异常</returns>
        public bool Reader()
        {
            Items.Clear();
            TransLangs.Clear();
            var defLangFile = MultiLangUtils.LangFullPath + "English.json";
            var fs = Directory.GetFiles(MultiLangUtils.LangFullPath, "*.json");
            if (!File.Exists(defLangFile))
            {
                "不存在默认语言(English.json)，操作失败".TintError();
                return false;
            }

            // 默认语言
            TransLangs.Add("English");
            var json = JsonMapper.ToObject<JsonData>(File.ReadAllText(defLangFile));
            foreach (string key in (json as IDictionary).Keys)
            {
                var item = new StringItem(key);
                item.Value.Add(SystemLanguage.English, json[key].ToString());
                Items.Add(item.Key, item);
            }

            // Debug.Log(LangNames.Connect());
            foreach (var f in fs)
            {
                if (f.EndsWith("English.json"))
                    continue;
                var name = Path.GetFileNameWithoutExtension(f);
                foreach (var lang in LangNames)
                {
                    if (lang != name) continue;
                    TransLangs.Add(lang);
                    // Debug.Log(lang);
                    if (!AddValues(f, (SystemLanguage)Enum.Parse(typeof(SystemLanguage), lang)))
                        return false;
                }
            }
            return Check();
        }

        /// <summary>
        /// 生成并写入代码
        /// </summary>
        public void Generate()
        {
            var rs = MultiLangUtils.RSTemplate;
            var idStr = new StringBuilder();
            var cacheStr = new StringBuilder();
            var implStr = new StringBuilder();
            var caseStr = new StringBuilder();
            int i = 20190000;
            foreach (var item in Items)
            {
                idStr.AppendLine(item.Value.GenerateResourceID(i++));
                cacheStr.AppendLine(item.Value.GenerateResourceString());
                implStr.AppendLine(item.Value.GenerateMethod());
                caseStr.AppendLine(item.Value.GenerateCase());
            }
            var code = rs.ReplaceSign("id", idStr.ToString())
                .ReplaceSign("cache", cacheStr.ToString())
                .ReplaceSign("impl", implStr.ToString())
                .ReplaceSign("case", caseStr.ToString())
                .ToString();
            File.WriteAllText(MultiLangUtils.CodeFilePath, code);
        }

        /// <summary>
        /// 添加翻译值
        /// </summary>
        /// <param name="path">json 语言文件路径</param>
        /// <param name="lang">语言</param>
        /// <returns>是否添加成功</returns>
        private bool AddValues(string path, SystemLanguage lang)
        {
            var json = JsonMapper.ToObject<JsonData>(File.ReadAllText(path));
            foreach (string key in (json as IDictionary).Keys)
            {
                if (!Items.ContainsKey(key))
                {
                    if (IsDebug)
                    {
                        Debug.LogWarning($"{lang.Name()}.json 存在无效字段 => {key}");
                        continue;
                    }
                    $"{lang.Name()}.json 存在无效字段 => {key}，操作失败".TintError();
                    return false;
                }
                var item = Items[key];
                item.Value.Add(lang, json[key].ToString());
            }
            return true;
        }

        /// <summary>
        /// 检查多国语言是否符合规范
        /// </summary>
        /// <returns>是否通过检查</returns>
        private bool Check()
        {
            if (IsDebug) return true;
            foreach (var item in Items)
            {
                if (item.Value.Value.Count == TransLangs.Count)
                    continue;
                var nonExist = new List<string>(TransLangs);
                foreach (var l in item.Value.Value.Keys)
                    nonExist.Remove(l.Name());

                $"[{item.Value.Key}] 缺少 {nonExist.Connect()} 语言，操作失败".TintError();
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 多国语言字段
    /// </summary>
    class StringItem
    {
        /// <summary>
        /// Key
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// 多语言
        /// </summary>
        public readonly Dictionary<SystemLanguage, string> Value
            = new Dictionary<SystemLanguage, string>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">Key</param>
        public StringItem(string key) { Key = key; }

        /// <summary>
        /// 生成如下资源 id
        /// public const int Name = id;
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>代码</returns>
        public string GenerateResourceID(int id)
        {
            return $"{3.Tabs()}public const int {Key.FirstCharToUpper()} = {id};";
        }

        /// <summary>
        /// 生成如下资源 id
        /// CachedMap.Add("Name", Name);
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>代码</returns>
        public string GenerateResourceString()
        {
            var key = Key.FirstCharToUpper();
            return $"{4.Tabs()}CachedMap.Add(\"{key}\", {key});";
        }

        /// <summary>
        /// 生成如下方法代码
        /// 
        /// private string AImpl()
        /// {
        ///     switch (I18n.Inst().CurrentLanguage)
        ///     {
        ///         case SystemLanguage.ChineseSimplified:
        ///             return "zhCN";
        ///         ...
        ///         default:
        ///             return "en";
        ///     }
        /// }
        /// </summary>
        /// <returns>代码</returns>
        public string GenerateMethod()
        {
            var sb = new StringBuilder();
            foreach (var e in Value)
            {
                if (e.Key == SystemLanguage.English)
                    continue;
                GenerateLanguageCase(sb, e.Key.Name(), e.Value);
            }

            // 对中文进行友好适配
            if (Value.TryGetValue(SystemLanguage.ChineseTraditional,
                out string zh))
            {
                GenerateLanguageCase(sb, "Chinese", zh);
            }
            else if (Value.TryGetValue(SystemLanguage.ChineseSimplified,
             out string zhCN))
            {
                GenerateLanguageCase(sb, "Chinese", zhCN);
            }

            // 默认语言适配
            sb.Append(5.Tabs())
                .AppendLine("case SystemLanguage.English:")
                .Append(5.Tabs())
                .AppendLine("default:")
                .Append(6.Tabs())
                .Append("return \"")
                .Append(Value[SystemLanguage.English])
                .AppendLine("\";");

            return MultiLangUtils.MethodTemplate
                .ReplaceSign("name", Key.FirstCharToUpper())
                .ReplaceSign("case", sb.ToString());
        }

        /// <summary>
        /// 生成分支语句
        /// </summary>
        /// <returns>分支语句</returns>
        public string GenerateCase()
        {
            var name = Key.FirstCharToUpper();
            return $"{5.Tabs()}case {name}:\r\n{6.Tabs()} return {name}Impl();";
        }

        private static void GenerateLanguageCase(StringBuilder sb, string lang, string value)
        {
            sb.Append(5.Tabs())
                .Append("case SystemLanguage.")
                .Append(lang)
                .AppendLine(":")
                .Append(6.Tabs())
                .Append("return \"")
                .Append(value)
                .AppendLine("\";");
        }
    }

    static class MultiLangUtils
    {
        public const string LangFullPath = "Assets/Editor/MultiLang/Vaules/";
        public const string CodeFilePath = "Assets/Scripts/UnityI18n/R.cs";

        /// <summary>
        /// 方法模板
        /// </summary>
        public static string MethodTemplate { get; } =
           File.ReadAllText($"{LangFullPath}/RSM.template");

        /// <summary>
        /// R.cs 代码模板
        /// </summary>
        public static string RSTemplate
        {
            get
            {
                return File.ReadAllText($"{LangFullPath}/RS.template");
            }
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>输出</returns>
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// 替换模板符号
        /// </summary>
        /// <param name="oldText">被替换字符串</param>
        /// <param name="sign">替换符</param>
        /// <param name="newText">替换字符串</param>
        /// <returns>新字符串</returns>
        public static string ReplaceSign(this string oldText, string sign, string newText)
        {
            return oldText.Replace($"${{{sign}}}", newText);
        }

        /// <summary>
        /// 获取枚举类型名字
        /// </summary>
        /// <param name="lang">SystemLanguage</param>
        /// <returns>Name</returns>
        public static string Name(this SystemLanguage lang)
        {
            return Enum.GetName(typeof(SystemLanguage), lang);
        }

        /// <summary>
        /// 将字符串列表连接为字符串
        /// </summary>
        /// <param name="list">串列表</param>
        /// <returns>字符串</returns>
        public static string Connect(this IList<string> list)
        {
            var sb = new StringBuilder("[");
            foreach (var e in list) sb.Append(' ').Append(e).Append(",");
            sb.Remove(sb.Length - 1, 1);
            return sb.Append(" ]").ToString();
        }

        /// <summary>
        /// 提示异常
        /// </summary>
        /// <param name="tint">提示语句</param>
        public static void TintError(this string tint)
        {
            EditorUtility.DisplayDialog("错误", tint, "确定");
            Debug.LogError(tint);
        }

        /// <summary>
        /// 将 Tab 转换为空格
        /// </summary>
        /// <param name="n">Tab 数量</param>
        /// <returns></returns>
        public static string Tabs(this int n)
        {
            if (n < 0) throw new ArgumentException("n => [0, int.MaxValue] ");
            return new string(' ', n << 2);
        }
    }
}
