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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityI18n;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log(I18n.GetText(R.Strings.Lime));    // 使用方式 1
        Debug.Log(I18n.Instance[R.Strings.Name]);   // 使用方式 2

        // 附加参数方式
        Debug.Log(I18n.GetText(R.Strings.Note, "This is a note"));
    }
}
