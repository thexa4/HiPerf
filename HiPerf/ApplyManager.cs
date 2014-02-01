using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiPerf
{
    /// <summary>
    /// Holds references to Applyable objects and applies changes concurrently
    /// </summary>
    /// <typeparam name="T">The type of applyable objects to use</typeparam>
    public class ApplyManager<T> : IEnumerable<T> where T : Applyable
    {
        /// <summary>
        /// The list of Applyable instances
        /// </summary>
        List<T> _members = new List<T>();

        /// <summary>
        /// The number of Applyable instances
        /// </summary>
        public int Count { get { return _members.Count; } }

        /// <summary>
        /// Returns an enumerator over the Applyable Instances
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _members.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator over the Applyable Instances
        /// </summary>
        /// <returns>An enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Integrates all changes concurrently for all instances
        /// </summary>
        public void Apply()
        {
            ForAll(a => a.Apply());
        }
        
        /// <summary>
        /// Runs a function concurrently for all instances
        /// </summary>
        /// <param name="action">The function to run</param>
        public void ForAll(Action<T> action)
        {
            
#if NO_MULTITHREAD || CHECK_APPLY
            lock (_members)
                foreach (var m in _members)
                    action(m);
#else
            lock (_members)
                Parallel.ForEach(_members, action);
#endif
        }

        /// <summary>
        /// Adds a new instance to the manager
        /// </summary>
        /// <param name="member">The instance to add</param>
        public void Add(T member)
        {
            lock(_members)
                _members.Add(member);
        }

        /// <summary>
        /// Removes an instance from the manager
        /// </summary>
        /// <param name="member">The instance to remove</param>
        public void Remove(T member)
        {
            lock (_members)
                _members.Remove(member);
        }
    }

    public class ApplyManager : ApplyManager<Applyable> { }
}
