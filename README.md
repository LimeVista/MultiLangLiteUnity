# Multi-language(I18N) for Unity
一个用于小型 Unity 项目的多语言轻量级工具

## 实现方式
将多语言 `JSON` 生成 `C#` 静态代码，利用 `R.Strings.id` 进行绑定获取，`GetText` 无 `IO` ,效率高。不适合文本量很大的项目。

## 使用方式
* 将符合 `SystemLanguage` 枚举名称的 `JSON` 文件置入 `Assets/Editor/MultiLang/Vaules` 文件夹， `JSON` 文件命名规则如 `SystemLanguage.English -> English.json`。 `English.json` 为默认语言，不可缺少。 `JSON` 格式参照 [文件格式](https://github.com/LimeVista/MultiLangLiteUnity/blob/master/Assets/Editor/MultiLang/Vaules/English.json)

* 使用 `Unity` 编辑器菜单 `Tools/MultiLanguage/Debug` 或 `Tools/MultiLanguage/Release` 生成 `R.cs` 文件

* 获取 `I18n` 文本方法:
```C#
I18n.GetText(R.Strings.Lime);   // 方式一
I18n.Instance[R.Strings.Name];  // 方式二

// 附加参数方式
I18n.GetText(R.Strings.Note, "This is a note");
```

## 高级使用
> 详见： [I18n](https://github.com/LimeVista/MultiLangLiteUnity/blob/master/Assets/Scripts/UnityI18n/I18n.cs)