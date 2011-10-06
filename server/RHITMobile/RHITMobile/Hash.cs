using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RHITMobile
{
    public class Hash<T>
        where T : class
    {
        private T[] _array;
        private int _size;
        private int _items;
        private int _index;

        public Hash(int initialSize)
        {
            _array = new T[initialSize];
            _size = initialSize;
        }

        public void WriteStatus()
        {
            Console.WriteLine("{0}/{1}", _items, _size);
        }

        public IEnumerable<ThreadInfo> LookForNewIndex(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            while (true)
            {
                yield return TM.Sleep(currentThread, 1);
                if (_array[_index] != null)
                {
                    _index = (_index + 1) % _size;
                }
            }
        }

        public IEnumerable<ThreadInfo> CheckForIncreaseSize(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            while (true)
            {
                yield return TM.Sleep(currentThread, 1);
                if (_items > _size / 2)
                {
                    IncreaseSize();
                }
            }
        }

        public int Insert(T item)
        {
            if (_items >= _size - 4)
            {
                IncreaseSize();
            }
            while (_array[_index] != null)
            {
                _index = (_index + 1) % _size;
            }
            int result = _index;
            _array[_index] = item;
            _items++;
            _index = (_index + 1) % _size;
            return result;
        }

        private void IncreaseSize()
        {
            var newArray = new T[_size * 2];
            for (int i = 0; i < _size; i++)
            {
                newArray[i] = _array[i];
            }
            _array = newArray;
            _size *= 2;
        }

        public void Remove(int index)
        {
            if (_array[index] != null)
            {
                _array[index] = null;
                _items--;
            }
        }

        public T this[int id]
        {
            get
            {
                return _array[id];
            }
        }
    }
}
