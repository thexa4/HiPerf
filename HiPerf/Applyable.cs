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
    public class Applyable
    {
        /// <summary>
        /// The queue that holds state changes to be applied
        /// </summary>
        private List<Action> _applyQueue = new List<Action>();

#if CHECK_APPLY
        /// <summary>
        /// The hash this object is supposed to have right now
        /// </summary>
        private int _checkHash = 0;
        /// <summary>
        /// Wether this is the first check (first apply is ignored)
        /// </summary>
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
            var members = this.GetType().GetMembers();

            foreach(var m in members)
            {
                if (m.CustomAttributes.Any(a => a.AttributeType == typeof(LocallyManaged)))
                    continue;
                
                var propInfo = (m as PropertyInfo);
                var fieldInfo = (m as FieldInfo);
                if (propInfo == null && fieldInfo == null)
                    continue;

                object val = null;
                if (propInfo != null)
                    val = propInfo.GetValue(this);
                if (fieldInfo != null)
                    val = fieldInfo.GetValue(this);

                if (val != null)
                    hash ^= val.GetHashCode();
            }

            return hash;
        }
#endif
    }
}
