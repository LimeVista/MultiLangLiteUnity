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
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UnityI18n
{
    /// <summary>
    /// 多国语言类
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class I18n
    {
        private static readonly object Locker = new object();   // 同步锁
        private static I18n Inst;                               // I18n 单例
        private ITextHolder Holder;                             // 文本实现持有者

        partial void InitHook();                                // 默认初始化注入

        /// <summary>
        /// 获取、设置语言
        /// </summary>
        public SystemLanguage CurrentLanguage { get; set; }

        private I18n() { }

        /// <summary>
        /// 获得单例
        /// </summary>
        /// <returns>单例</returns>
        public static I18n Instance
        {
            get
            {
                if (Inst == null)
                {
                    lock (Locker)
                    {
                        if (Inst == null)
                            Inst = new I18n();
                    }
                }
                return Inst;
            }
        }

        /// <summary>
        /// 初始化，非必须，如果自动生成 R.cs 能自动注入
        /// </summary>
        /// <param name="holder">自动生成的ITextHolder</param>
        public static void Init(ITextHolder holder)
        {
            Init(holder, Application.systemLanguage);
        }

        /// <summary>
        ///  初始化，非必须，如果自动生成 R.cs 能自动注入
        /// </summary>
        /// <param name="holder">自动生成的ITextHolder</param>
        /// <param name="lang">语言</param>
        public static void Init(ITextHolder holder, SystemLanguage lang)
        {
            Instance.Holder = holder ?? throw new NullReferenceException("holder must be not null");
            Instance.CurrentLanguage = lang;
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="stringId">字符串编号</param>
        /// <returns>字符串</returns>
        public string this[int stringId]
        {
            get
            {
                if (Holder == null)
                {
                    InitHook(); // 分部方法注入实现
                    if (Holder == null)
                        throw new NullReferenceException("MultiLanguage.Init(ITextHolder, uint) is not called!");
                }
                return Holder.FindText(stringId);
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="stringId">通过字符串查询字符串编号</param>
        /// <returns>字符串</returns>
        public string this[string stringId]
        {
            get
            {
                if (Holder == null)
                {
                    InitHook(); // 分部方法注入实现
                    if (Holder == null)
                        throw new NullReferenceException("MultiLanguage.Init(ITextHolder, uint) is not called!");
                }
                return Holder.FindText(stringId);
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="stringId">字符串编号</param>
        /// <param name="args">额外参数</param>
        /// <returns>字符串</returns>
        public static string GetText(int stringId, params object[] args)
        {
            return string.Format(Instance[stringId], args);
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="stringId">字符串编号</param>
        /// <param name="args">额外参数</param>
        /// <returns>字符串</returns>
        public static string GetText(string stringId, params object[] args)
        {
            return string.Format(Instance[stringId], args);
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="stringId">字符串编号</param>
        /// <returns>字符串</returns>
        public static string GetText(int stringId)
        {
            return Instance[stringId];
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="stringId">字符串编号</param>
        /// <returns>字符串</returns>
        public static string GetText(string stringId)
        {
            return Instance[stringId];
        }
    }

    /// <summary>
    /// 多国语言持有者
    /// </summary>
    public interface ITextHolder
    {
        string FindText(int stringId);

        string FindText(string stringId);
    }

    /// <summary>
    /// 未找到资源异常，资源 ID 错误
    /// </summary>
    public class ResourcesNotFoundException : ArgumentException
    {
        public ResourcesNotFoundException() : base() { }

        public ResourcesNotFoundException(string message) : base(message) { }
    }
}
