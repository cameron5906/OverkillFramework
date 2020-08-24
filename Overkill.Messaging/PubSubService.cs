using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
        private IServiceProvider serviceProvider;
        private Dictionary<string, List<IPubSubMiddleware>> middlewares;
        private Dictionary<string, List<IPubSubTopicTransformer>> transformers;
        private Dictionary<string, List<Action<IPubSubTopic>>> subscribers;
        private Dictionary<Type, string> topics;

        public PubSubService(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
            middlewares = new Dictionary<string, List<IPubSubMiddleware>>();
            transformers = new Dictionary<string, List<IPubSubTopicTransformer>>();
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
        public async Task Dispatch(IPubSubTopic topic)
        {
            var topicName = topic.GetType().Name;

            //Send the topic through any registered middleware
            if(middlewares.ContainsKey(topicName))
            {
                for(var i=0;i<middlewares[topicName].Count;i++)
                {
                    var newTopic = await middlewares[topicName][i].Process(topic);

                    //Verify the middleware is simply modifying the topic and not returning some other topic (misuse)
                    if(newTopic.GetType() == topic.GetType())
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
                    .Select(async(x) => await x.Process(topic))
                    .Select(x => x.Result)
                    .ToList();

                //Send to any subscribers who are interested in these transformed topics
                variants.ForEach(variant =>
                {
                    var variantTopicName = variant.GetType().Name;
                    if(subscribers.ContainsKey(variantTopicName))
                    {
                        subscribers[variantTopicName].ForEach(subscriber => subscriber(variant));
                    }
                });
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
                Console.WriteLine($"Subscriber assigned for topic: {topicName}");
                subscribers.Add(topicName, new List<Action<IPubSubTopic>>());
            }

            subscribers[topicName].Add(new Action<IPubSubTopic>(i => listener((T)i)));
        }

        /// <summary>
        /// Registers a middleware class to a specific Topic. When this Topic is dispatched, it will call this (and any other middleware) before
        /// reaching subscribers
        /// </summary>
        /// <typeparam name="T">The type of Topic to register the middleware for</typeparam>
        /// <param name="middlewareType">The Type of the middleware class to register</param>
        public void Middleware<T>(Type middlewareType)
        {
            var topicName = typeof(T).Name;

            if(!middlewares.ContainsKey(topicName))
            {
                Console.WriteLine($"Middleware \"{ middlewareType.Name}\" registered for: {topicName}");
                middlewares.Add(topicName, new List<IPubSubMiddleware>());
            }

            middlewares[topicName].Add((IPubSubMiddleware)Activator.CreateInstance(middlewareType, new[] { serviceProvider }));
        }

        /// <summary>
        /// Registers a Topic Transformer to a specific Topic. This is similar to middleware, however it simply transforms a Topic into another and all
        /// variants (and the original Topic) will be dispatched to each of their subscribers. Middleware is run before Transformers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transformerType"></param>
        public void Transform<T>(Type transformerType)
        {
            var topicName = typeof(T).Name;

            if (!transformers.ContainsKey(topicName))
            {
                Console.WriteLine($"Topic transformer \"{ transformerType.Name}\" registered for: {topicName}");
                transformers.Add(topicName, new List<IPubSubTopicTransformer>());
            }

            transformers[topicName].Add((IPubSubTopicTransformer)Activator.CreateInstance(transformerType, new[] { serviceProvider }));
        }
    }
}
