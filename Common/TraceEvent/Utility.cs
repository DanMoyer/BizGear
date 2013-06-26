//     Copyright (c) Microsoft Corporation.  All rights reserved.
using System.Collections.Generic;
using System.Diagnostics;
using Address = System.UInt64;

namespace Utilities
{
    // Utilities for TraceEventParsers
    /// <summary>
    /// A HistoryDictionary is designed to look up 'handles' (pointer sized quantities), that might get reused
    /// over time (eg Process IDs, thread IDs).  Thus it takes a handle AND A TIME, and finds the value
    /// associated with that handle at that time.   
    /// </summary>
    public class HistoryDictionary<T>
    {
        public HistoryDictionary(int initialSize)
        {
            entries = new Dictionary<long, HistoryValue>(initialSize);
        }
        /// <summary>
        /// Adds the association taht 'id' has the value 'value' from 'startTime100ns' ONWARD until
        /// it is superseed by the same id being added with a time that is after this.   Thus if
        /// I did Add(58, 1000, MyValue1), and add(58, 500, MyValue2) 'TryGetValue(58, 750, out val) will return
        /// MyValue2 (since that value is 'in force' between time 500 and 1000.   
        /// </summary>
        public void Add(Address id, long startTime100ns, T value, bool isEndRundown = false)
        {
            HistoryValue entry;
            if (!entries.TryGetValue((long)id, out entry))
            {
                // rundown events are 'last chance' events that we only add if we don't already have an entry for it.  
                if (isEndRundown)
                    startTime100ns = 0;
                entries.Add((long)id, new HistoryValue(startTime100ns, id, value));
            }
            else
            {
                for (; ; )
                {
                    if (entry.next == null)
                    {
                        entry.next = new HistoryValue(startTime100ns, id, value);
                        break;
                    }

                    // We sort the entries from smallest to largest time. 
                    if (startTime100ns < entry.startTime100ns)
                    {
                        // This entry belongs in front of this entry.  
                        // Insert it before the current entry by moving the current entry after it.  
                        HistoryValue newEntry = new HistoryValue(entry);
                        entry.startTime100ns = startTime100ns;
                        entry.value = value;
                        entry.next = newEntry;
                        Debug.Assert(entry.startTime100ns <= entry.next.startTime100ns);
                        break;
                    }
                    entry = entry.next;
                }
            }
            count++;
        }
        // TryGetValue will return the value associated with an id that was placed in the stream 
        // at time100ns OR BEFORE.  
        public bool TryGetValue(Address id, long time100ns, out T value)
        {
            HistoryValue entry;
            if (entries.TryGetValue((long)id, out entry))
            {
                // The entries are shorted smallest to largest.  
                // We want the last entry that is smaller (or equal) to the target time) 
                HistoryValue last = null;
                for (; ; )
                {
                    if (time100ns < entry.startTime100ns)
                        break;
                    last = entry;
                    entry = entry.next;
                    if (entry == null)
                        break;
                }
                if (last != null)
                {
                    value = last.value;
                    return true;
                }
            }
            value = default(T);
            return false;
        }
        public IEnumerable<HistoryValue> Entries
        {
            get
            {
#if DEBUG
            int ctr = 0;
#endif
                foreach (HistoryValue entry in entries.Values)
                {
                    HistoryValue list = entry;
                    while (list != null)
                    {
#if DEBUG
                    ctr++;
#endif
                        yield return list;
                        list = list.next;
                    }
                }
#if DEBUG
            Debug.Assert(ctr == count);
#endif
            }
        }
        public int Count { get { return count; } }

        public class HistoryValue
        {
            public Address Key { get { return key; } }
            public long StartTime100ns { get { return startTime100ns; } }
            public T Value { get { return value; } }
            #region private
            internal HistoryValue(HistoryValue entry)
            {
                this.key = entry.key;
                this.startTime100ns = entry.startTime100ns;
                this.value = entry.value;
                this.next = entry.next;
            }
            internal HistoryValue(long startTime100ns, Address key, T value)
            {
                this.key = key;
                this.startTime100ns = startTime100ns;
                this.value = value;
            }

            internal Address key;
            internal long startTime100ns;
            internal T value;
            internal HistoryValue next;
            #endregion
        }
        #region private
        Dictionary<long, HistoryValue> entries;
        int count;
        #endregion
    }
}