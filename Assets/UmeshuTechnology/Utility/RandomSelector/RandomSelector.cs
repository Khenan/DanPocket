using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelector<T>
{
    private List<RandomSelectorItem> items = new();
    private bool IsEmpty => items.Count == 0;
    public int Count => items.Count;

    // Parameters
    private const float MIN_WEIGHT_VALUE = 0.0001f;
    private const int NB_TURN_MIN_TO_HAVE_ITEM_AGAIN = 1;
    private float BaseWeightLoss => Count > 0 ? 1 / (float)Count : 0;
    private float WeightLossAtSelection => .2f * BaseWeightLoss;
    public string LastLog { get; private set; } = string.Empty;

    public bool TryGetRandomItem(out T _randomItem, params Func<T, bool>[] _filterMethods)
    {
        _randomItem = default;

        if (TryFilterPossibleItems(_filterMethods))
        {
            NormalizeWeights();
            float _random = UnityEngine.Random.value;
            foreach (RandomSelectorItem _item in items)
            {
                if (!_item.CanBeSelected) continue;
                if (_random <= _item.weightForThisSelection)
                {
                    ItemHasBeenSelected(_item);
                    _randomItem = _item.value;
                    break;
                }
                else _random -= _item.weightForThisSelection;
            }
        }
        else
        {
            Debug.LogError("All possibilities got filtered, rerolling with no filter");
            return TryGetRandomItem(out _randomItem);
        }

        return _randomItem != null && !_randomItem.Equals(default(T));
    }

    public T GetRandomItem(params Func<T, bool>[] _filterMethods)
    {
        _ = TryGetRandomItem(out T _returned, _filterMethods);
        return _returned;
    }

    private bool TryFilterPossibleItems(Func<T, bool>[] _filterMethods)
    {
        bool _oneIsValid = false;
        foreach (RandomSelectorItem _item in items)
        {
            _item.isFilterValid = true;
            foreach (Func<T, bool> _filterMethod in _filterMethods)
            {
                _item.isFilterValid = _filterMethod.Invoke(_item.value);
                if (!_item.isFilterValid) break; ;
            }
            if (_item.isFilterValid) _oneIsValid = true;
        }
        return _oneIsValid;
    }

    public void AddPossibleItems(float _weightAllocated, params T[] _addedValues)
    {
        if (_addedValues.Length == 0) return;
        _weightAllocated = Mathf.Max(_weightAllocated, MIN_WEIGHT_VALUE * _addedValues.Length);

        float _newValueWeight = _weightAllocated / (float)_addedValues.Length;
        foreach (T _value in _addedValues)
        {
            RandomSelectorItem _existingItem = items.Find(_x => _x.value.Equals(_value));
            if (_existingItem != null) continue;
            items.Add(new(_newValueWeight, _value));
        }
    }

    public void RemovePossibleItems(params T[] _removedValues)
    {
        foreach (T _value in _removedValues)
        {
            RandomSelectorItem _existingItem = items.Find(_x => _x.value.Equals(_value));
            if (_existingItem != null) items.Remove(_existingItem);
        }
    }

    public void UpdatePossibleItems(float _weightAllocatedForNewItems, params T[] _possibleValues)
    {
        List<RandomSelectorItem> _filteredItems = new();
        List<T> _notExistingValues = new();
        foreach (T _value in _possibleValues)
        {
            RandomSelectorItem _existingItem = items.Find(_x => _x.value.Equals(_value));
            if (_existingItem != null) _filteredItems.Add(_existingItem);
            else _notExistingValues.Add(_value);
        }
        items = _filteredItems;
        AddPossibleItems(_weightAllocatedForNewItems, _notExistingValues.ToArray());
    }

    private void NormalizeWeights()
    {
        if (IsEmpty) return;

        float _totalWeight = 0;
        foreach (RandomSelectorItem _item in items)
        {
            if (!_item.CanBeSelected) continue;
            _totalWeight += _item.weight;
        }

        foreach (RandomSelectorItem _item in items)
        {
            if (!_item.CanBeSelected) continue;
            float _ratio = 1 / _totalWeight;
            _item.weightForThisSelection = _item.weight * _ratio;
        }
    }

    private void ItemHasBeenSelected(RandomSelectorItem _selectedItem)
    {
        LastLog = GetDebugItemSelection(_selectedItem);
        foreach (RandomSelectorItem _item in items)
        {
            _item.turnRemainingBeforeCanBeSelected = Mathf.Max(0, _item.turnRemainingBeforeCanBeSelected - 1);
        }
        _selectedItem.turnRemainingBeforeCanBeSelected = NB_TURN_MIN_TO_HAVE_ITEM_AGAIN;
        _selectedItem.weight = Mathf.Max(MIN_WEIGHT_VALUE, _selectedItem.weight - WeightLossAtSelection);
    }

    private string GetDebugItemSelection(RandomSelectorItem _selectedItem)
    {
        string _debugLog = "RANDOM SELECTION\n";
        foreach (RandomSelectorItem _item in items)
        {
            bool _isSelected = _selectedItem.Equals(_item);
            if (!_item.isFilterValid)
            {
                _debugLog += "<color=red>";
                _debugLog += "Filtered";
            }
            else if (_item.turnRemainingBeforeCanBeSelected > 0)
            {
                _debugLog += "<color=orange>";
                _debugLog += "CD : " + _item.turnRemainingBeforeCanBeSelected;
            }
            else
            {
                if (_isSelected) _debugLog += "<color=green>";
                else _debugLog += "<color=white>";
                _debugLog += (_item.weightForThisSelection * 100).ToString("F1") + "%";
            }
            _debugLog += " - " + _item.value + "</color>" + "\n";
        }
        return _debugLog;
    }

    private class RandomSelectorItem
    {
        public float weightForThisSelection = 1.0f;
        public float weight = 1.0f;
        public int turnRemainingBeforeCanBeSelected = 0;
        public bool isFilterValid = false;
        public T value;

        public bool CanBeSelected => turnRemainingBeforeCanBeSelected == 0 && isFilterValid;

        public RandomSelectorItem(float _weight, T _value)
        {
            weight = _weight;
            value = _value;
        }
    }
}
