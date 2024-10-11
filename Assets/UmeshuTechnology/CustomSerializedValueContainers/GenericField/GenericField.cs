using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class GenericField : PropertyAttribute
{
    public GenericField(bool _showAsList = false)
    {
        this.showAsList = _showAsList;
    }
    public readonly bool showAsList;
}

[AttributeUsage(AttributeTargets.Class)]
public class GenericComponent : PropertyAttribute
{

}