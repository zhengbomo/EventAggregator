using System;

namespace Shagu.EventAggregator
{
    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Subscribes an instance with message type
        /// </summary>
        void Subscribe<T>(object subscriber, Action<T> handler);

        /// <summary>
        /// Subscribes an instance with key and message type
        /// </summary>
        void Subscribe<T>(object subscriber, string key, Action<T> handler);

        /// <summary>
        /// publishes a message.
        /// </summary>
        void Publish<T>(T data);

        /// <summary>
        /// publishes a message with key.
        /// </summary>
        void Publish<T>(string key, T data);

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        void Unsubscribe(object subscriber);

        /// <summary>
        /// Unsubscribes the instance for message type.
        /// </summary>
        void Unsubscribe<T>(object subscriber);

        /// <summary>
        /// clear all subscriber
        /// </summary>
        void Clear();
    }
}
