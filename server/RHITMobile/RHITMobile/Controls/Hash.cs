using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RHITMobile
{
    /// <summary>
    /// Maintains a hash of items with a limitless size
    /// </summary>
    /// <typeparam name="T">Type of items in the hash</typeparam>
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

        /// <summary>
        /// Writes the number of items and current array size
        /// </summary>
        public void WriteStatus()
        {
            Console.WriteLine("{0}/{1}", _items, _size);
        }

        /// <summary>
        /// Moves the index pointer to an empty slot
        /// </summary>
        /// <param name="TM">ThreadManager</param>
        /// <returns>No return</returns>
        public IEnumerable<ThreadInfo> LookForNewIndex(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            while (true)
            {
                yield return TM.Sleep(currentThread, 1000);
                lock (_array) {
                    if (_array[_index] != null) {
                        _index = (_index + 1) % _size;
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if the array needs to increase in size
        /// </summary>
        /// <param name="TM">ThreadManager</param>
        /// <returns>No return</returns>
        public IEnumerable<ThreadInfo> CheckForIncreaseSize(ThreadManager TM)
        {
            var currentThread = TM.CurrentThread;
            while (true)
            {
                yield return TM.Sleep(currentThread, 60000);
                lock (_array) {
                    if (_items > _size / 2) {
                        IncreaseSize();
                    }
                }
            }
        }

        /// <summary>
        /// Inserts an item into the hash
        /// </summary>
        /// <param name="item">Item to insert</param>
        /// <returns>Index of the item</returns>
        public int Insert(T item)
        {
            lock (_array) {
                // If the number of items is within four of the limit, increase the size
                if (_items >= _size - 4) {
                    IncreaseSize();
                }

                // Find the next empty slot
                while (_array[_index] != null) {
                    _index = (_index + 1) % _size;
                }
                int result = _index;

                // Insert the item
                _array[_index] = item;
                _items++;
                _index = (_index + 1) % _size;
                return result;
            }
        }

        /// <summary>
        /// Increases the size of the array
        /// </summary>
        private void IncreaseSize()
        {
            var newArray = new T[_size * 2];
            for (int i = 0; i < _size; i++) {
                newArray[i] = _array[i];
            }
            _array = newArray;
            _size *= 2;
        }

        /// <summary>
        /// Removes an item from the hash
        /// </summary>
        /// <param name="index">Index of item to remove</param>
        public void Remove(int index)
        {
            lock (_array) {
                if (_array[index] != null) {
                    _array[index] = null;
                    _items--;
                }
            }
        }

        /// <summary>
        /// Gets the item with an index
        /// </summary>
        /// <param name="id">Index of the item to get</param>
        /// <returns>Item in the hash</returns>
        public T this[int id]
        {
            get
            {
                lock (_array) {
                    return _array[id];
                }
            }
        }
    }
}
