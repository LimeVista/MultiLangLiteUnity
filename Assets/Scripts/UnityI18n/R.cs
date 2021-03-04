using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 本文件由模板自动生成，请勿手动更改
/// </summary>
namespace UnityI18n
{
    /// <summary>
    /// 资源类
    /// </summary>
    public static class R
    {
        public class Strings : ITextHolder
        {
            public readonly static Strings Instance = new Strings();

            private readonly Dictionary<string, int> CachedMap = new Dictionary<string, int>();

            private Strings()
            {
                InitCache();
            }
            
            #region /* 字符串资源编号 */

            public const int Lime = 20190000;
            public const int Name = 20190001;
            public const int Note = 20190002;

            #endregion

            #region /* string cache*/

            private void InitCache()
            {
                CachedMap.Add("Lime", Lime);
                CachedMap.Add("Name", Name);
                CachedMap.Add("Note", Note);

            }

            #endregion

            #region /* Impl */

            private static string LimeImpl()
            {
                switch (I18n.Instance.CurrentLanguage)
                {
                    case SystemLanguage.ChineseSimplified:
                        return "青柠";
                    case SystemLanguage.Japanese:
                        return "ライム";
                    case SystemLanguage.Korean:
                        return "레몬";
                    case SystemLanguage.Chinese:
                        return "青柠";
                    case SystemLanguage.English:
                    default:
                        return "Lime";

                }
            }
            private static string NameImpl()
            {
                switch (I18n.Instance.CurrentLanguage)
                {
                    case SystemLanguage.ChineseSimplified:
                        return "姓名";
                    case SystemLanguage.Japanese:
                        return "名前";
                    case SystemLanguage.Korean:
                        return "이름";
                    case SystemLanguage.Chinese:
                        return "姓名";
                    case SystemLanguage.English:
                    default:
                        return "Name";

                }
            }
            private static string NoteImpl()
            {
                switch (I18n.Instance.CurrentLanguage)
                {
                    case SystemLanguage.ChineseSimplified:
                        return "笔记:\"{0}.\"";
                    case SystemLanguage.Japanese:
                        return "ノート:{0}.";
                    case SystemLanguage.Korean:
                        return "주:{0}.";
                    case SystemLanguage.Chinese:
                        return "笔记:\"{0}.\"";
                    case SystemLanguage.English:
                    default:
                        return "note:{0}.";

                }
            }

            #endregion

            public string FindText(int stringId)
            {
                switch (stringId)
                {
                    case Lime:
                         return LimeImpl();
                    case Name:
                         return NameImpl();
                    case Note:
                         return NoteImpl();

                }

                throw new ResourcesNotFoundException(stringId + "not found");
            }

            public string FindText(string stringId)
            {
                if (CachedMap.TryGetValue(stringId, out int value))
                {
                    return FindText(value);
                }
                return null;
            }
        }
    }

    /// <summary>
    /// 通过编译完成对初始化的注入
    /// </summary>
    public partial class I18n
    {
        partial void InitHook()
        {
            lock (Locker)
            {
                if (Holder == null)
                {
                    CurrentLanguage = Application.systemLanguage;
                    Holder = R.Strings.Instance;
                }
            }
        }
    }
}