/*
MIT License

Copyright (c) 2020 Unary Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace Unary.Common.Collections
{
    public class BiDictionary<K, V>
    {
        private Dictionary<K, V> KeyToValue;
        private Dictionary<V, K> ValueToKey;

        public BiDictionary()
        {
            KeyToValue = new Dictionary<K, V>();
            ValueToKey = new Dictionary<V, K>();
        }

        public void Clear()
        {
            KeyToValue.Clear();
            ValueToKey.Clear();
        }

        public void Set(K Key, V Value)
        {
            KeyToValue[Key] = Value;
            ValueToKey[Value] = Key;
        }

        public void Remove(K Key)
        {
            if(KeyToValue.ContainsKey(Key))
            {
                ValueToKey.Remove(KeyToValue[Key]);
                KeyToValue.Remove(Key);
            }
        }

        public void Remove(V Value)
        {
            if (ValueToKey.ContainsKey(Value))
            {
                KeyToValue.Remove(ValueToKey[Value]);
                ValueToKey.Remove(Value);
            }
        }

        public K GetKey(V Value)
        {
            if(ValueToKey.ContainsKey(Value))
            {
                return ValueToKey[Value];
            }
            else
            {
                return default;
            }
        }

        public V GetValue(K Key)
        {
            if(KeyToValue.ContainsKey(Key))
            {
                return KeyToValue[Key];
            }
            else
            {
                return default;
            }
        }

        public K this[V Value]
        {
            get 
            { 
                return GetKey(Value);
            }
        }

        public V this[K Key]
        {
            get 
            {
                return GetValue(Key);
            }
        }

        public List<K> GetAllKeys()
        {
            List<K> Result = new List<K>();

            foreach(var Entry in KeyToValue)
            {
                Result.Add(Entry.Key);
            }

            return Result;
        }

        public List<V> GetAllValues()
        {
            List<V> Result = new List<V>();

            foreach (var Entry in ValueToKey)
            {
                Result.Add(Entry.Key);
            }

            return Result;
        }

        public List<Tuple<K, V>> GetAll()
        {
            List<Tuple<K, V>> Result = new List<Tuple<K, V>>();

            List<K> Keys = GetAllKeys();

            foreach(var Key in Keys)
            {
                Tuple<K, V> NewTuple = new Tuple<K, V>(Key, GetValue(Key));
                Result.Add(NewTuple);
            }

            return Result;
        }

        public bool KeyExists(K Key)
        {
            return KeyToValue.ContainsKey(Key);
        }

        public bool ValueExists(V Value)
        {
            return ValueToKey.ContainsKey(Value);
        }
    }
}
