using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if CHECK_APPLY
using System.Reflection;
#endif

namespace HiPerf
{
    /// <summary>
    /// Enables state changes to be queued and enables checking for unauthorized writes.
    /// </summary>
    public abstract class Applyable
    {
        /// <summary>
        /// The queue that holds state changes to be applied
        /// </summary>
        [LocallyManaged]
        private List<Action> _applyQueue = new List<Action>();

#if CHECK_APPLY
        /// <summary>
        /// The hash this object is supposed to have right now
        /// </summary>
        [LocallyManaged]
        private int _checkHash = 0;
        /// <summary>
        /// Wether this is the first check (first apply is ignored)
        /// </summary>
        [LocallyManaged]
        private bool _isFirstCheckRun = true;
#endif

        /// <summary>
        /// Queue a state change to be applied to this object
        /// </summary>
        /// <remarks>This function is thread safe</remarks>
        /// <param name="action">The action to be applied</param>
        public void Queue(Action action)
        {
            lock(_applyQueue)
                _applyQueue.Add(action);
        }

        /// <summary>
        /// Apply all pending state changes synchronously.
        /// </summary>
        /// <remarks>This function is thread safe</remarks>
        public void Apply()
        {

#if CHECK_APPLY
            if(!_isFirstCheckRun)
                if (_checkHash != CalculateHash())
                    throw new UnauthorizedAccessException("Non-mutable state changed between applies. Please prefix locally managed members with [LocallyManaged]");
#endif

            lock(_applyQueue)
            {
                foreach (Action a in _applyQueue)
                    a();
                _applyQueue.Clear();
            }

#if CHECK_APPLY
            _isFirstCheckRun = false;
            _checkHash = CalculateHash();
#endif
        }

#if CHECK_APPLY
        /// <summary>
        /// Creates a unique hashcode for the data in this instance
        /// </summary>
        /// <returns>A hash</returns>
        private int CalculateHash()
        {
            int hash = 0;
            for (Type t = this.GetType(); t != null; t = t.BaseType)
            {
                var fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                var skipped = fields.Where(m => m.GetCustomAttribute(typeof(LocallyManaged)) == null);
                var values = skipped.Select(m => m.GetValue(this)).Where(a => a != null);
                var hashes = values.Select(m => m.GetHashCode());

                if (!hashes.Any())
                    continue;

                hash ^= hashes.Aggregate((prev, item) => prev ^ item);
            }

            return hash;
        }
#endif
    }
}

