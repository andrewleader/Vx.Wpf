using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vx.Wpf
{
    public abstract class VxElement
    {
        protected abstract Type UIType { get; }

        internal UIElement ToUIElement()
        {
            if (this is VxComponent c)
            {
                return c.ToUI();
            }

            var el = Activator.CreateInstance(UIType) as UIElement;

            ApplyProperties(el, Activator.CreateInstance(this.GetType()) as VxElement);

            return el;
        }

        public VxElement()
        {
            if (!(this is VxComponent))
            {
                InitializeDefaultValues();
            }
        }

        private static Dictionary<Type, UIElement> _defaultUIValues = new Dictionary<Type, UIElement>();
        private void InitializeDefaultValues()
        {
            UIElement? defaultUI;
            if (!_defaultUIValues.TryGetValue(UIType, out defaultUI))
            {
                defaultUI = Activator.CreateInstance(UIType) as UIElement;
                _defaultUIValues.Add(UIType, defaultUI!);
            }

            foreach (var p in this.GetType().GetProperties().Where(i => i.CanRead && i.CanWrite))
            {
                var currVal = p.GetValue(this);
                var uiProp = UIType.GetProperty(p.Name);
                if (uiProp != null)
                {
                    var desiredDefaultVal = uiProp.GetValue(defaultUI);
                    if (!object.Equals(currVal, desiredDefaultVal))
                    {
                        p.SetValue(this, desiredDefaultVal);
                    }
                }
            }
        }

        private void VxElement_Click(object sender, RoutedEventArgs e)
        {
            (this.GetType().GetProperty("Click").GetValue(this) as Action)();
        }

        private static Type _elementCollectionType = typeof(List<VxElement>);

        internal void ApplyProperties(UIElement el, VxElement prevEl)
        {
            if (this is VxComponent)
            {
                // TODO: Support updating sub-components?
                return;
            }

            try
            {
                var props = this.GetType().GetProperties().Where(i => i.CanRead).ToArray();

                foreach (var prop in props)
                {
                    var propType = prop.PropertyType;
                    var uiProp = UIType.GetProperty(prop.Name);
                    var prevVal = prop.GetValue(prevEl);
                    var newVal = prop.GetValue(this);

                    if (!prop.CanWrite)
                    {
                        // Children...
                        if (_elementCollectionType.IsAssignableFrom(propType))
                        {
                            var uiCollection = uiProp.GetValue(el) as UIElementCollection;
                            ReconcileList(prevVal as List<VxElement>, newVal as List<VxElement>, uiCollection);
                        }
                    }
                    else
                    {
                        // Handle nested singleton child views
                        if (newVal is VxElement newVxElement)
                        {
                            if (prevVal is VxElement oldVxElement && oldVxElement.GetType() == newVxElement.GetType())
                            {
                                var existingUIVisual = uiProp!.GetValue(el) as UIElement;
                                newVxElement.ApplyProperties(existingUIVisual, oldVxElement);
                            }
                            else
                            {
                                uiProp!.SetValue(el, newVxElement.ToUIElement());
                            }
                        }

                        else if (object.Equals(prevVal, newVal))
                        {
                            continue;
                        }

                        else if (typeof(Delegate).IsAssignableFrom(propType))
                        {
                            EventInfo eventInfo = UIType.GetEvent(prop.Name)!;

                            var existing = prevEl.GetExistingDelegate(prop.Name);
                            if (existing != null)
                            {
                                eventInfo.RemoveEventHandler(el, existing);
                            }

                            var newHandler = GetOrCreateDelegate(prop.Name, eventInfo.EventHandlerType!, el);
                            eventInfo.AddEventHandler(el, newHandler);
                        }

                        else
                        {
                            uiProp.SetValue(el, newVal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dictionary<string, Delegate> _delegates = new Dictionary<string, Delegate>();

        private Delegate? GetExistingDelegate(string propName)
        {
            _delegates.TryGetValue(propName, out Delegate? d);
            return d;
        }

        private Delegate GetOrCreateDelegate(string propName, Type eventHandlerType, UIElement el)
        {
            if (_delegates.TryGetValue(propName, out Delegate? d))
            {
                return d;
            }

            var parameters = eventHandlerType
                .GetMethod("Invoke")!
                .GetParameters()
                .Select(i => System.Linq.Expressions.Expression.Parameter(i.ParameterType))
                .ToArray();

            Action action = delegate
            {
                var subscriber = this.GetType().GetProperty(propName)!.GetValue(this) as Delegate;

                if (subscriber != null)
                {
                    subscriber.DynamicInvoke(el);
                }
            };

            var handler = System.Linq.Expressions.Expression.Lambda(
                eventHandlerType,
                System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(action), "Invoke", Type.EmptyTypes),
                parameters)
                .Compile();

            _delegates[propName] = handler;
            return handler;
        }

        private void VxElement_TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            (this.GetType().GetProperty(e.RoutedEvent.Name)!.GetValue(this) as TextChangedEventHandler)?.Invoke(sender, e);
        }

        private void VxElement_RoutedEventHandler(object sender, RoutedEventArgs e)
        {
            (this.GetType().GetProperty(e.RoutedEvent.Name)!.GetValue(this) as RoutedEventHandler)?.Invoke(sender, e);
        }

        private static void ReconcileList(List<VxElement> oldList, List<VxElement> newList, UIElementCollection actualCollection)
        {
#if DEBUG
            try
            {
#endif
                // Exclude rendering null items
                newList = newList.Where(v => v != null).ToList();

                if (oldList.Count == 0)
                {
                    foreach (var val in newList)
                    {
                        actualCollection.Add(val.ToUIElement());
                    }

                    return;
                }

                if (newList.Count == 0)
                {
                    actualCollection.Clear();
                    return;
                }

                // Exclude rendering null items
                oldList = oldList.Where(v => v != null).ToList();

                int i = 0;

                for (; i < oldList.Count; i++)
                {
                    var oldItem = oldList[i];
                    var newItem = newList.ElementAtOrDefault(i);

                    if (newItem == null)
                    {
                        oldList.RemoveAt(i);
                        actualCollection.RemoveAt(i);
                    }
                    else if (oldItem.GetType() == newItem.GetType())
                    {
                        newItem.ApplyProperties(actualCollection[i], oldItem);
                    }
                    else if (oldList.Count < newList.Count)
                    {
                        oldList.Insert(i, newItem);
                        actualCollection.Insert(i, newItem.ToUIElement());
                    }
                    else if (oldList.Count > newList.Count)
                    {
                        oldList.RemoveAt(i);
                        actualCollection.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        oldList[i] = newItem;
                        actualCollection.RemoveAt(i);
                        actualCollection.Insert(i, newItem.ToUIElement());
                    }
                }

                if (oldList.Count < newList.Count)
                {
                    for (; i < newList.Count; i++)
                    {
                        actualCollection.Add(newList[i].ToUIElement());
                    }
                }
#if DEBUG
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                System.Diagnostics.Debugger.Break();
            }
#endif
        }
    }
}
