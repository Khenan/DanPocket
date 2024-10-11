using System;
using System.Collections;
using System.Collections.Generic;
using Umeshu.Common;
using Umeshu.Uf;
using Umeshu.USystem;
using Umeshu.Utility;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class EnumBasedKey
{
    public EnumBasedKey() { }
    public void SetName(string _name) => Name = _name;
    public override string ToString() => Name;
    public string Name { get; private set; }
}
