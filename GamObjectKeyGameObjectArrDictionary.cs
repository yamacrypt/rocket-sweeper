using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using VRC.SDKBase;
using VRC.Udon;

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class GamObjectKeyGameObjectArrDictionary : UdonSharpBehaviour
    {
        private GameObject[] _keys;
        private GameObject[][] _values;
        private int _size;

        public GameObject[] GetValue(GameObject key)
        {
            int index = IndexOf(key);

            if (index >= 0)
            {
                return _values[index];
            }
            Debug.LogError($"{nameof(key)} is not found.");
#if UNITY_EDITOR
#pragma warning disable CS0251 // Indexing an array with a negative index
            return _values[-1]; // Throw OutOfRangeException
#pragma warning restore CS0251 // Indexing an array with a negative index
#else
            return null;
#endif
        }

        public GameObject[] GetValueOrDefault(GameObject key, GameObject[] defaultValue)
        {
            int index = IndexOf(key);

            if (index >= 0)
            {
                return _values[index];
            }
            return defaultValue;
        }

        private void LogKeysAndValues()
        {
            if (_size == 0)
            {
                Debug.Log($"_keys = [], _values = []");
            }
            else
            {
                GameObject[] values = new GameObject[_size];
                Array.Copy(_values, values, _size);
                //Debug.Log($"_keys = [{int.Join(", ", _keys, 0, _size)}], _values = [{int.Join(", ", values)}]");
            }
        }

        public bool HasItem(GameObject key) => IndexOf(key) >= 0;

        public int Count => _size;
        public int Capacity
        {
            get => _keys == null ? 0 : _keys.Length;
        }

        [Obsolete("Use Count Property.")]
        public int GetCount() => _size;
        [Obsolete("Use Capacity Property.")]
        public int GetCapacity() => _keys == null ? 0 : _keys.Length;

        public int[] GenerateKeysArray()
        {
            int[] keys = new int[_size];
            if (_size > 0)
            {
                Array.Copy(keys, 0, _keys, 0, _size);
            }
            return keys;
        }
        public GameObject[] GenerateValuesArray()
        {
            GameObject[] values = new GameObject[_size];
            if (_size > 0)
            {
                Array.Copy(values, 0, _values, 0, _size);
            }
            return values;
        }

        public void Add(GameObject key, GameObject[] value) => SetValue(key, value);

        public void SetValue(GameObject key, GameObject[] value)
        {
            var index = BinarySearch(key);

            if (index >= 0)
            {
                _values[index] = value;
                _keys[index] = key;
            }
            else
            {
                index = ~index;
                InsertValue(index, key, value);
            }
        }

        public int IndexOf(GameObject key)
        {
            int index = BinarySearch(key);
            return index >= 0 ? index : -1;
        }

        public bool Remove(GameObject key)
        {
            int index = BinarySearch(key);
            if (index < 0)
            {
                return false;
            }
            return RemoveByIndex(index);
        }

        public bool RemoveByIndex(int index)
        {
            if (index < 0 || index >= _size)
            {
                return false;
            }

            _size--;
            if (index < _size)
            {
                Array.Copy(_keys, index + 1, _keys, index, _size - index);
                Array.Copy(_values, index + 1, _values, index, _size - index);
            }
            _keys[_size] = null;  
            _values[_size] = null;  // want to be target of GC

            return true;
        }

        private int BinarySearch(GameObject key)
        {
            return _keys != null ? Array.BinarySearch(_keys, 0, _size, key) : -1;
        }

        #region Private Methods

        private void InsertValue(int index, GameObject key, GameObject[] value)
        {
            if (index > _size)
            {
                Debug.LogError($"{nameof(index)} is out of range.");
                return;
            }

            ExpandList(_size + 1);

            if (index < _size)
            {
                Array.Copy(_keys, index, _keys, index + 1, _size - index);
                Array.Copy(_values, index, _values, index + 1, _size - index);
            }
            _keys[index] = key;
            _values[index] = value;
            _size++;
        }

        private void ExpandList(int min)
        {
            if (min <= 0)
            {
                return;
            }

            if (_keys == null)
            {
                _keys = new GameObject[min];
                _values = new GameObject[min][];
            }
            else if (_keys.Length < min)
            {
                int newCapacity = Mathf.Max(min, _keys.Length + 256);
                var newKey = new GameObject[newCapacity];
                var newValue = new GameObject[newCapacity][];

                for (int i = 0; i < _size; i++)
                {
                    newKey[i] = _keys[i];
                    newValue[i] = _values[i];
                }
                _keys = newKey;
                _values = newValue;
            }
        }

        private bool HasAllocation(int min)
        {
            return min <= GetCapacity();
        }


        #endregion
    }