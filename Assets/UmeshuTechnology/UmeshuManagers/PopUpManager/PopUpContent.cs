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

namespace Umeshu.USystem.PopUp
{
    public abstract class PopUpContent : MonoBehaviour
    {
        public event Action onCloseRequest;

        public void Close() => onCloseRequest?.Invoke();
    }
}