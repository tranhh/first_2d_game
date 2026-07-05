using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private float finalValue;
    private bool isModified = true;

    public float GetValue()
    {
        if (isModified)
        {
            finalValue = GetFinalValue(); // move this outside if statement when testing stats so it gets recalculated everytime
            isModified = false;
        }
        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        isModified = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        isModified = true;
        //which is similar to:
        // foreach(modifier in modifiers)
        //      if(modifier.source == source)
        //          Remove(modifier.source);
        // shorter: remove all modifier that returned true
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach (var modifier in modifiers)
        {
            finalValue += modifier.value;
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}