using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Overkill.PubSub
{
    /// <summary>
    /// The PubSub service is used for different Core systems, plugins, and vehicle drivers to communicate with one another in a decoupled fashion.
    /// 
    /// Overkill comes with some Core topics that handle generic scenarios that every Overkill-enabled vehicle may be interested in. Plugins and vehicle drivers
    /// may also include their own Topics, and all of these can be subscribed to and transformed.
    /// </summary>
    public class PubSubService : IPubSubService
    {
        private Dictionary<string, List<Func<IPubSubTopic, IPubSubTopic>>> middlewares;
        private Dictionary<string, List<Func<IPubSubTopic, IPubSubTopic>>> transformers;
        private Dictionary<string, List<Action<IPubSubTopic>>> subscribers;
        private Dictionary<Type, string> topics;

        public PubSubService()
        {
            middlewares = new Dictionary<string, List<Func<IPubSubTopic, IPubSubTopic>>>();
            transformers = new Dictionary<string, List<Func<IPubSubTopic, IPubSubTopic>>>();
            subscribers = new Dictionary<string, List<Action<IPubSubTopic>>>();
            topics = new Dictionary<Type, string>();
        }

        /// <summary>
        /// Searches all loaded assemblies for classes that inherit PubSub Topic
        /// These will be used for communication between Core systems as well as plugins and vehicle drivers
        /// </summary>
        public void DiscoverTopics()
        {
            var topicTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && typeof(IPubSubTopic).IsAssignableFrom(x))
                .ToList();

            topicTypes.ForEach(topicType =>
            {
                Console.WriteLine($"Discovered PubSub topic: {topicType.Name}");
                topics.Add(topicType, topicType.Name);
            });
        }

        /// <summary>
        /// Used to dispatch a Topic to any Core systems, plugins, or vehicle drivers that are interested and subscribed to it
        /// </summary>
        /// <param name="topic">The Topic object</param>
        public void Dispatch(IPubSubTopic topic)
        {
            var topicName = topic.GetType().Name;

            //Send the topic through any registered middleware
            if(middlewares.ContainsKey(topicName))
            {
                for(var i=0;i<middlewares[topicName].Count;i++)
                {
                    var newTopic = middlewares[topicName][i](topic);

                    if(newTopic == null) //if it's null, take that as the topic being thrown away and return
                    {
                        return;
                    } else if(newTopic.GetType() == topic.GetType()) //Verify the middleware is simply modifying the topic and not returning some other topic (misuse)
                    {
                        topic = newTopic;
                    }
                }
            }

            //Check to see if there are any topic transformers registered for this topic
            if(transformers.ContainsKey(topicName))
            {
                //Create a list of variants. We will still send our current topic as is, too.
                var variants = transformers[topicName]
                    .Select(transformer => transformer(topic))
                    .ToList();

                //Dispatch the transformed topics
                variants.ForEach(topic => Dispatch(topic));
            }
                
            //Send the topic to its subscribers
            if (subscribers.ContainsKey(topicName))
            {
                subscribers[topicName].ForEach(listener => listener(topic));
            }
        }

        /// <summary>
        /// Subscribes to a specific Topic
        /// </summary>
        /// <typeparam name="T">The type of topic to subscribe to</typeparam>
        /// <param name="listener">A callback that is invoked when the Topic is dispatched through PubSub</param>
        public void Subscribe<T>(Action<T> listener)
        {
            var topicName = typeof(T).Name;

            if(!subscribers.ContainsKey(topicName))
            {
                subscribers.Add(topicName, new List<Action<IPubSubTopic>>());
            }

            subscribers[topicName].Add(new Action<IPubSubTopic>(i => listener((T)i)));
        }

        /// <summary>
        /// Registers a middleware class to a specific Topic. When this Topic is dispatched, it will call this (and any other middleware) before
        /// reaching subscribers
        /// </summary>
        /// <typeparam name="T">The type of Topic to register the middleware for</typeparam>
        /// <param name="function">The function to use to mutate the specified topic type</param>
        public void Middleware<T>(Func<T, T> function)
        {
            var topicName = typeof(T).Name;

            if(!middlewares.ContainsKey(topicName))
            {
                middlewares.Add(topicName, new List<Func<IPubSubTopic, IPubSubTopic>>());
            }

            middlewares[topicName].Add((Func<IPubSubTopic, IPubSubTopic>)((object)function));
        }

        /// <summary>
        /// Registers a Topic Transformer to a specific Topic. This is similar to middleware, however it simply transforms a Topic into another and all
        /// variants (and the original Topic) will be dispatched to each of their subscribers. Middleware is run before Transformers.
        /// </summary>
        /// <typeparam name="T">The topic type to transform</typeparam>
        /// <param name="function">The function to use to return a different Topic type</param>
        public void Transform<T>(Func<T, IPubSubTopic> function)
        {
            var topicName = typeof(T).Name;

            if (!transformers.ContainsKey(topicName))
            {
                transformers.Add(topicName, new List<Func<IPubSubTopic, IPubSubTopic>>());
            }

            transformers[topicName].Add((Func<IPubSubTopic, IPubSubTopic>)((object)function));
        }
    }
}
