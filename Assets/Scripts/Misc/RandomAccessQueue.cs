namespace Misc
{
    public class RandomAccessQueue<T> where T : new()
    {
        private readonly T[] _storage;
        private readonly int _capacity;

        public RandomAccessQueue(int size)
        {
            if (size <= 0) return;
            _storage = new T[size];
            _capacity = size;
        }

        private int _head;
        private int _tail;
        public int count { get; private set; }

        public bool Enqueue(T element)
        {
            if (count == _capacity) return false;
            count++;
            if (_tail < _capacity - 1) _tail++;
            else _tail = 0;
            _storage[_tail] = element;
            return true;
        }

        public T Dequeue()
        {
            if (count == 0) return new T();
            count--;
            var result = _storage[_head];
            if (_head < _capacity - 1) _head++;
            else _head = 0;
            return result;
        }

        public T Get(int index)
        {
            if (index > count - 1) return new T();
            var targetIndex = _head + index;
            if (targetIndex > _capacity - 1) targetIndex -= _capacity;
            return _storage[targetIndex];
        }

        public void Clear()
        {
            _head = 0;
            _tail = 0;
            count = 0;
        }
    }
}