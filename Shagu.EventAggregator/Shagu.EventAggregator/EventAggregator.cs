using System;
using System.Collections.Generic;
using System.Linq;

namespace Shagu.EventAggregator
{
    /// <summary>
    /// an implementation of IEventAggregator
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        #region Private Field
        
        private readonly object locker = new object();
        private readonly List<Handler> handlers = new List<Handler>();

        #endregion

        /// <summary>
        /// publish message
        /// </summary>
        /// <typeparam name="T">message type</typeparam>
        public void Publish<T>(string key, T data)
        {
            
            lock (locker)
            {
                Cleanup();

                //matching type
                foreach (var l in handlers.Where(a => a.Type == typeof (T) && a.Key.Equals(key)))
                {
                    var action = l.Action as Action<T>;
                    if (action != null) action(data);
                }
            }
        }

        public void Publish<T>(T data)
        {
            lock (locker)
            {
                Cleanup();

                //matching type
                foreach (var l in handlers.Where(a => a.Type == typeof(T)))
                {
                    var action = l.Action as Action<T>;
                    if (action != null) action(data);
                }
            }
        }

        public void Subscribe<T>(object subscriber, Action<T> handler)
        {
            Subscribe(subscriber, null, handler);
        }

        public void Subscribe<T>(object subscriber, string key, Action<T> handler)
        {
            lock (locker)
            {
                handlers.Add(new Handler
                {
                    Action = handler,
                    Sender = new WeakReference(subscriber),
                    Key = key,
                    Type = typeof(T)
                });
            }
        }

        /// <summary>
        /// unregiste subscriber
        /// </summary>
        public void Unsubscribe(object subscriber)
        {
            lock (locker)
            {
                Cleanup();

                var query = handlers.Where(a => a.Sender.Target.Equals(subscriber));

                foreach (var h in query.ToList())
                {
                    handlers.Remove(h);
                }
            }
        }

        /// <summary>
        /// unregiste message type of subscriber
        /// </summary>
        public void Unsubscribe<T>(object subscriber)
        {
            lock (locker)
            {
                Cleanup();

                var query = handlers.Where(a => a.Sender.Target.Equals(subscriber) && a.Type == typeof(T));

                foreach (var h in query.ToList())
                {
                    handlers.Remove(h);
                }
            }
        }

        /// <summary>
        /// clear subscribers
        /// </summary>
        public void Clear()
        {
            handlers.Clear();
        }

        #region Private Method
        
        /// <summary>
        /// clean the subscribers which had been garbage collected.
        /// </summary>
        private void Cleanup()
        {
            foreach (var l in handlers.Where(a => !a.Sender.IsAlive).ToList())
            {
                handlers.Remove(l);
            }
        } 

        #endregion

        #region Handler

        /// <summary>
        /// subscribers info of WeakReference
        /// </summary>
        private class Handler
        {
            public object Action { get; set; }

            /// <summary>
            /// message subscriber
            /// </summary>
            public WeakReference Sender { get; set; }

            /// <summary>
            /// message type
            /// </summary>
            public Type Type { get; set; }

            /// <summary>
            /// message key
            /// </summary>
            public string Key { get; set; } 
        }

        #endregion

       
    }
}
