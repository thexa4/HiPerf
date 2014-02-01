using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPerf
{
    public class ApplyManager<T> : IEnumerable<T> where T : Applyable
    {
        List<T> _members = new List<T>();

        public int Count { get { return _members.Count; } }

        public IEnumerator<T> GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Apply()
        {
#if CHECK_APPLY
            lock(_members)
                foreach (var m in _members)
                    m.Apply();
#else
            lock (_members)
                Parallel.ForEach(_members, m => m.Apply());
#endif
        }

        public void Add(T member)
        {
            lock(_members)
                _members.Add(member);
        }

        public void Remove(T member)
        {
            lock (_members)
                _members.Remove(member);
        }
    }

    public class ApplyManager : ApplyManager<Applyable> { }
}
