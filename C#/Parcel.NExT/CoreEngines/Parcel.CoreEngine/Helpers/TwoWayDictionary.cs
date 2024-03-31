using System.Collections;

namespace Parcel.CoreEngine.Helpers
{
    public class TwoWayDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>> 
        where T1: notnull
        where T2: notnull
    {
        #region Members
        private readonly Dictionary<T1, T2> _forward = [];
        private readonly Dictionary<T2, T1> _reverse = [];
        #endregion

        #region Construction
        public TwoWayDictionary()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }
        #endregion

        #region Methods
        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Remove(T1 t1)
        {
            T2 revKey = Forward[t1];
            _forward.Remove(t1);
            _reverse.Remove(revKey);
        }

        public void Remove(T2 t2)
        {
            T1 forwardKey = Reverse[t2];
            _reverse.Remove(t2);
            _forward.Remove(forwardKey);
        }
        #endregion

        #region Interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }
        #endregion

        #region Subtype
        public class Indexer<T3, T4>
            where T3 : notnull
            where T4 : notnull
        {
            private readonly Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }

            public bool Contains(T3 key)
            {
                return _dictionary.ContainsKey(key);
            }
        }
        #endregion
    }
}
