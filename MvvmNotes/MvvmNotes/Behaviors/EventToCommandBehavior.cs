﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MvvmNotes.Behaviors
{
    public class EventToCommandBehavior : BaseBehavior<View>
    {
        private Delegate eventHandler;

        public static readonly BindableProperty EventNameProperty = BindableProperty.Create(nameof(EventName),typeof(string),typeof(EventToCommandBehavior),null, propertyChanged : OnEventNameChanged);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),typeof(ICommand),typeof(EventToCommandBehavior));
        public static readonly BindableProperty InputConverterProperty = BindableProperty.Create(nameof(Converter), typeof(IValueConverter), typeof(EventToCommandBehavior), null);

        public string EventName
        {
            get { return (string) GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty,value);}
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty);}
            set { SetValue(CommandProperty,value);}
        }

        public IValueConverter Converter
        {
            get
            {
                return (IValueConverter)GetValue(InputConverterProperty);
            }
            set { SetValue(InputConverterProperty, value); }
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            RegisterEvent(EventName);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            DeregisterEvent(EventName);
            base.OnDetachingFrom(bindable);
        }

        private void RegisterEvent(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return;

            EventInfo eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException($"EventToCommandBehavior: Can't register the '{name}' event.");
            }

            MethodInfo methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(AssociatedObject,eventHandler);
        }

        private void DeregisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (eventHandler == null)
            {
                return;
            }

            EventInfo eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException($"EventToCommandBehavior: Can't de-register the '{name}' event.");
            }

            eventInfo.RemoveEventHandler(AssociatedObject, eventHandler);
        }

        void OnEvent(object sender, object eventArgs)
        {
            object resolvedParameter = new object();

            if (Command == null)
                return;

            else if (Converter != null)
            {
                resolvedParameter = Converter.Convert(eventArgs, typeof(object), null, null);
            }
            else
            {
                var arg = eventArgs as ItemTappedEventArgs;
                if (arg == null)
                {
                    resolvedParameter = eventArgs;
                }
                else
                {
                    resolvedParameter = arg.Item;
                }

                //resolvedParameter = eventArgs;
            }

            if (Command.CanExecute(resolvedParameter))
            {
                Command.Execute(resolvedParameter);
            }
        }


        static void OnEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EventToCommandBehavior)bindable;
            if (behavior.AssociatedObject == null)
            {
                return;
            }

            string oldEventName = (string)oldValue;
            string newEventName = (string)newValue;

            behavior.DeregisterEvent(oldEventName);
            behavior.RegisterEvent(newEventName);
        }
    }
}
