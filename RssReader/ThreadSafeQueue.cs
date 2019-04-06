using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RssReader
{
    /// <summary>
    /// Need a queue that is threadsafe. This should be that collection.
    /// </summary>
    public class ThreadSafeQueue
    {
        // I had modified the original ThreadSafeQueue class, but I am 
        // reimplementing with the ConcurrentQueue class. I've left the
        // old modifications commented out below.

        private ConcurrentQueue<int> concurrentQueue;

        public ThreadSafeQueue()
        {
            concurrentQueue = new ConcurrentQueue<int>();
        }

        /// <summary>
        /// Add a value to the queue.
        /// </summary>
        /// <param name="value">integer to be added</param>
        public void Enqueue(int value)
        {
            concurrentQueue.Enqueue(value);
        }

        /// <summary>
        /// Returns first element from queue, or defaultValue if empty
        /// </summary>
        /// <param name="defaultValue">Default value to return if queue is empty</param>
        /// <returns></returns>
        public int Pop(int defaultValue)
        {
            int returnValue;

            if (concurrentQueue.TryDequeue(out returnValue))
            {
                return returnValue;
            }
            else
            {
                Console.Error.WriteLine("Error: Failed to pop queue.");
                return defaultValue;
            }
        }


        /// <summary>
        /// Returns true if list is not empty
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return !concurrentQueue.IsEmpty;
        }

        /*
     
        // Mutex 
        private readonly object _lock = new object();

        /// <summary>
        /// Backing data store is constant.
        /// </summary>
        private List<int> BACKING_LIST;

        public ThreadSafeQueue() {
            BACKING_LIST = new List<int>();
        }

        /// <summary>
        /// Add value to the queue.
        /// </summary>
        /// <param name="value">Float value to add</param>
        public void Enqueue(int value)
        {
            // We should lock here too if we're trying to be thread-safe
            lock (_lock)
            {
                BACKING_LIST.Add(value);
            }

            // Lock shouldn't be reset each enqueue
            // _lock = new object();
        }

        /// <summary>
        /// Return and remove the first value from the queue.
        /// </summary>
        /// <param name="defaultValue">Default to return if queue is empty.</param>
        /// <returns>Number.</returns>
        public int Pop(int defaultValue) // default value was missing but was commented
        {
            int value; // Store an int

            lock(_lock) { // Thread safety for the win
                
                //value = BACKING_LIST.Take(BACKING_LIST.Count).Last(); // Take the leading value in the queue
                try
                {
                    value = BACKING_LIST.First();
                    BACKING_LIST.RemoveAt(0);
                }
                catch (Exception e) // List is empty
                {
                    Console.Error.WriteLine("Error: Attempted to pop empty list.");
                    Console.Error.WriteLine("Error: " + e);
                    return defaultValue;
                }
                
            }

            // Removing value from within the lock instead of from here.
            //BACKING_LIST.Remove(value); // Make sure to remove that value.

            return value; // Return the value
        }

        public bool Any()
        {
            return BACKING_LIST.Any();
        }
        */
    }
}
