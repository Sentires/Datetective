using System;
using System.Collections.Generic;
using TheDates.Runtime.Dialogue;
using TheDates.Runtime.General;
using TheDates.Runtime.Input;
using TheDates.Runtime.Quests;
using UnityEngine;

namespace TheDates.Runtime
{
    public class GameEventsManager : PersistentSingleton<GameEventsManager>
    {
        // Hard-coded events
        public InputEvents InputEvents;
        public QuestEvents QuestEvents;
        public DialogueEvents DialogueEvents;
        
        private Dictionary<Type, EventHandler> _eventHandlers;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LazyInstance() {
            if (!TryCreateInstance()) {
                Debug.LogWarning("Creating new GameEventsManager");
            }
        }
        
        protected override void Awake() {
            base.Awake();
            
            // Eventually I want to rely on the dictionary, as it could become more reusable without hard-coding the handlers in this class
            CreateDictionary(new QuestEvents(), new DialogueEvents(), new InputEvents());
            QuestEvents = GetHandler<QuestEvents>();
            InputEvents = GetHandler<InputEvents>();
            DialogueEvents = GetHandler<DialogueEvents>();
            
            InputEvents.ChangeInputEventContext(InputEvents.Context.World);
        }
        
        private void CreateDictionary(params EventHandler[] handlers) {
            _eventHandlers = new Dictionary<Type, EventHandler>();
            foreach (var handler in handlers) {
                _eventHandlers.TryAdd(handler.GetType(), handler);
            }
        }
        
        public bool TryGetHandler<T>(out T handler) where T : EventHandler
        {
            var type = typeof(T);
            if (!_eventHandlers.TryGetValue(type, out var item)) {
                Debug.LogError($"No handler registered for {type.Name}");
                handler = null;
                return false;
            }
            
            handler = item as T;
            return handler != null;
        }
        
        public T GetHandler<T>() where T : EventHandler
        {
            var type = typeof(T);
            if (!_eventHandlers.TryGetValue(type, out var item)) {
                Debug.LogError($"No handler registered for {type.Name}");
            }
            
            return item as T;
        }
    }


    public abstract class EventHandler
    {
        // Actions are the main target, they inherit from MulticastDelegate apparently
        // These are here in case I need them later

        protected EventHandler()
        { }
        
        /*
        public static T Subscribe<T>(T existingDelegate, T newHandler) where T : MulticastDelegate
        {
            return (T)Delegate.Combine(existingDelegate, newHandler);
        }
        public static T Unsubscribe<T>(T existingDelegate, T handlerToRemove) where T : MulticastDelegate
        {
            return (T)Delegate.Remove(existingDelegate, handlerToRemove);
        }
        */
    }
}
