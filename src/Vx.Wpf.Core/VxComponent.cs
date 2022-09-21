using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Vx.Wpf
{
    public abstract class VxComponent : VxElement
    {
        protected override Type UIType => throw new NotImplementedException();

        private bool _isRootComponent;
        /// <summary>
        /// When adding a VxComponent to normal view, you must set this so that it displays
        /// </summary>
        public bool IsRootComponent
        {
            get => _isRootComponent;
            set
            {
                if (_isRootComponent && value == false)
                {
                    throw new NotSupportedException("Cannot change from root element to not root");
                }

                _isRootComponent = value;

                if (value)
                {
                    InitializeRootComponent();
                }
            }
        }

        public UIElement ToUI()
        {
            IsRootComponent = true;

            _uiElement = new ContentControl();

            InitializeForDisplay();

            return _uiElement;
        }

        private ContentControl _uiElement;

        //protected virtual bool IsDependentOnBindingContext => false;

        //private object _oldBindingContext;
        //protected override void OnBindingContextChanged()
        //{
        //    base.OnBindingContextChanged();

        //    if (_oldBindingContext is INotifyPropertyChanged oldValPropChanged)
        //    {
        //        UnsubscribeToPropertyValue(oldValPropChanged);
        //    }

        //    _oldBindingContext = BindingContext;

        //    if (BindingContext is INotifyPropertyChanged newValPropChanged)
        //    {
        //        if (_propertyValuePropertyChangedHandler == null)
        //        {
        //            _propertyValuePropertyChangedHandler = new WeakEventHandler<PropertyChangedEventArgs>(PropertyValue_PropertyChanged).Handler;
        //        }

        //        SubscribeToPropertyValue(newValPropChanged);
        //    }

        //    if (IsDependentOnBindingContext && BindingContext != null)
        //    {
        //        InitializeForDisplay();
        //    }
        //}

        private void InitializeRootComponent()
        {
            if (_hasInitializedForDisplay)
            {
                throw new NotSupportedException("You must set IsRootComponent before placing the component in your views.");
            }

            //base.DescendantAdded += VxComponent_DescendantAdded;
        }

        private void VxComponent_Loaded(object sender, RoutedEventArgs e)
        {
            // Only the root component renders at this time
            // When a root component is rendering views that contain another nested component, the final parent will only be the immediate parent, not the root component
            //if (!IsRootComponent)
            //{
            //    return;
            //}

            InitializeForDisplay();
        }

        private bool _hasInitializedForDisplay;
        private void InitializeForDisplay()
        {
            //if (IsDependentOnBindingContext && BindingContext == null)
            //{
            //    return;
            //}

            if (_hasInitializedForDisplay)
            {
                return;
            }

            _hasInitializedForDisplay = true;

            Initialize();

            //if (IsRootComponent && _additionalComponentsToInitialize != null)
            //{
            //    foreach (var c in _additionalComponentsToInitialize)
            //    {
            //        c.InitializeForDisplay();
            //    }

            //    _additionalComponentsToInitialize = null;
            //}

            //var renderedContentContainer = PrepRenderedContentContainer();
            //if (renderedContentContainer != null)
            //{
            //    base.Content = renderedContentContainer;
            //}

            SubscribeToStates();
            SubscribeToProperties();
            SubscribeToHotReload();

            RenderActual();
        }

        //protected virtual View PrepRenderedContentContainer()
        //{
        //    // Nothing in this case since we just set content directly
        //    return null;
        //}

        //private class VxCommand : ICommand
        //{
        //    private Action _action;
        //    public VxCommand(Action action)
        //    {
        //        _action = action;
        //    }

        //    public event EventHandler CanExecuteChanged;

        //    public bool CanExecute(object parameter)
        //    {
        //        return true;
        //    }

        //    public void Execute(object parameter)
        //    {
        //        _action();
        //    }
        //}

        //protected ICommand CreateCommand(Action action)
        //{
        //    return new VxCommand(action);
        //}

        //private Dictionary<string, Binding> _itemDisplayBindings;
        //protected Binding CreateItemDisplayBinding(string propertyPath)
        //{
        //    if (_itemDisplayBindings == null)
        //    {
        //        _itemDisplayBindings = new Dictionary<string, Binding>();
        //    }

        //    if (_itemDisplayBindings.TryGetValue(propertyPath, out Binding existing))
        //    {
        //        return existing;
        //    }

        //    var binding = new Binding { Path = propertyPath };
        //    _itemDisplayBindings[propertyPath] = binding;
        //    return binding;
        //}

        //private Dictionary<string, DataTemplate> _dataTemplates;
        //protected DataTemplate CreateViewCellItemTemplate<T>(string templateName, Func<T, View> render)
        //{
        //    if (_dataTemplates == null)
        //    {
        //        _dataTemplates = new Dictionary<string, DataTemplate>();
        //    }

        //    if (_dataTemplates.TryGetValue(templateName, out DataTemplate existing))
        //    {
        //        return existing;
        //    }

        //    var newTemplate = new VxViewCellItemTemplate<T>(render);
        //    _dataTemplates[templateName] = newTemplate;
        //    return newTemplate;
        //}

        //protected DataTemplate CreateViewCellItemTemplate<T, V>() where V : VxBindingComponent<T>
        //{
        //    if (_dataTemplates == null)
        //    {
        //        _dataTemplates = new Dictionary<string, DataTemplate>();
        //    }

        //    var templateName = typeof(V).FullName;

        //    if (_dataTemplates.TryGetValue(templateName, out DataTemplate existing))
        //    {
        //        return existing;
        //    }

        //    var newTemplate = new VxViewCellItemTemplateComponent<T, V>();
        //    _dataTemplates[templateName] = newTemplate;
        //    return newTemplate;
        //}

        //protected DataTemplate CreateItemTemplate<T>(string templateName, Func<T, View> render)
        //{
        //    if (_dataTemplates == null)
        //    {
        //        _dataTemplates = new Dictionary<string, DataTemplate>();
        //    }

        //    if (_dataTemplates.TryGetValue(templateName, out DataTemplate existing))
        //    {
        //        return existing;
        //    }

        //    var newTemplate = new VxItemTemplate<T>(render);
        //    _dataTemplates[templateName] = newTemplate;
        //    return newTemplate;
        //}

        private bool _subscribedToHotReload;
        private void SubscribeToHotReload()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                VxHotReload.ReRender += VxHotReload_ReRender;
                _subscribedToHotReload = true;
            }
        }

        private void VxHotReload_ReRender(object? sender, EventArgs e)
        {
            MarkDirty();
        }

        private void SubscribeToStates()
        {
            var stateType = typeof(VxState);
            foreach (var prop in this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.CanRead && stateType.IsAssignableFrom(i.PropertyType)))
            {
                var state = prop.GetValue(this) as VxState;
                state.ValueChanged += State_ValueChanged;
            }
            foreach (var prop in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => stateType.IsAssignableFrom(i.FieldType)))
            {
                var state = prop.GetValue(this) as VxState;
                state.ValueChanged += State_ValueChanged;
            }
        }

        private PropertyChangedEventHandler _propertyValuePropertyChangedHandler;
        private static Type _iNotifyPropertyChangedType = typeof(INotifyPropertyChanged);

        private void SubscribeToProperties()
        {
            //_propertyValuePropertyChangedHandler = PropertyValue_PropertyChanged;

            //var seenProps = new HashSet<string>();

            //foreach (var prop in this.GetType().GetProperties().Where(i => i.CanWrite && i.CanRead && _iNotifyPropertyChangedType.IsAssignableFrom(i.PropertyType) && i.GetCustomAttribute<VxStateAttribute>() != null))
            //{
            //    // Avoid subscribing to properties overridden by "new" keyword
            //    if (seenProps.Add(prop.Name))
            //    {
            //        var propVal = prop.GetValue(this) as INotifyPropertyChanged;
            //        if (propVal != null)
            //        {
            //            SubscribeToPropertyValue(propVal);
            //        }
            //    }
            //}
        }

        private void SubscribeToPropertyValue(INotifyPropertyChanged propertyValue)
        {
            propertyValue.PropertyChanged += _propertyValuePropertyChangedHandler;
        }

        private void UnsubscribeToPropertyValue(INotifyPropertyChanged propertyValue)
        {
            propertyValue.PropertyChanged -= _propertyValuePropertyChangedHandler;
        }

        private void PropertyValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MarkDirty();
        }

        protected virtual VxElement Render()
        {
            return null;
        }

        private void State_ValueChanged(object sender, EventArgs e)
        {
            MarkDirty();
        }

        private bool _dirty = true;

        /// <summary>
        /// Marks this component for re-render on next UI cycle
        /// </summary>
        protected void MarkDirty()
        {
            lock (this)
            {
                if (_dirty)
                {
                    return;
                }

                _dirty = true;
            }

            _uiElement.Dispatcher.BeginInvoke(RenderActual);
        }

        public static int LastMillisecondsToRender { get; private set; }

        private static Type _uiElementCollectionType = typeof(UIElementCollection);
        private static Type _elementType = typeof(UIElement);
        //private static Type _visualElementType = typeof(VisualElement);
        //private static Type _iVisualType = typeof(IVisual);
        private static Type _textBoxType = typeof(TextBox);
        private static Type _listViewType = typeof(ListView);
        //private static Type _pickerType = typeof(Picker);
        private static Type _resourceDictionaryType = typeof(ResourceDictionary);
        private static Type _vxComponentType = typeof(VxComponent);
        //private static Type _imageSourceType = typeof(ImageSource);
        //private static Type _carouselViewType = typeof(CarouselView);

        /// <summary>
        ///  Properties that shouldn't be set (for internal renderer use only)
        /// </summary>
        //private static PropertyInfo[] _propertiesToIgnore = new PropertyInfo[]
        //{
        //    typeof(VisualElement).GetProperty(nameof(Batched)),
        //    typeof(VisualElement).GetProperty(nameof(IsInNativeLayout)),
        //    typeof(VisualElement).GetProperty(nameof(Visual)),
        //    typeof(VisualElement).GetProperty(nameof(IsNativeStateConsistent)),
        //    typeof(VisualElement).GetProperty(nameof(DisableLayout)),
        //    typeof(VisualElement).GetProperty(nameof(IsPlatformEnabled))
        //};

        private void RenderActual()
        {
            lock (this)
            {
                _dirty = false;
            }

            var now = DateTime.Now;

            VxElement newView = Render();
            VxElement oldView = _currentElement;

            if (oldView == null || oldView.GetType() != newView.GetType())
            {
                _currentElement = newView;
                _uiElement.Content = newView.ToUIElement();
            }

            else
            {
                // Transfer over the properties
                newView.ApplyProperties(_uiElement.Content as UIElement, _currentElement);
                _currentElement = newView;
            }

            LastMillisecondsToRender = (DateTime.Now - now).Milliseconds;
        }

        private VxElement _currentElement;

        /// <summary>
        /// Do NOT get or set this
        /// </summary>
        public new object Content
        {
            get => throw new InvalidOperationException("Don't call VxComponent.Content");
        }

        /// <summary>
        /// This is called only once, before Render is called, and only called when the component is actually going to be displayed (not called for the virtual components that will be discarded). No need to call base.Initialize().
        /// </summary>
        protected virtual void Initialize()
        {
            // Nothing
        }

        private static readonly Type[] _typesToIgnore = new Type[]
        {
            typeof(Transform), // From RenderTransform
            typeof(Point), // From RenderTransformOrigin
            typeof(Transform3D),
            typeof(DataTemplate),
            typeof(ControlTemplate),
            typeof(Style)
        };

        private static void ReconcileViewOfSameType(UIElement oldView, UIElement newView)
        {
            // When properties are overridden with new keyword, need to skip evaluating those, as reflection returns all and doesn't allow differentiating, so we track which are seen so far and skip dupes (the highest level props are returned frist)
            HashSet<string> seenProps = new HashSet<string>();
            var props = newView.GetType().GetProperties().Where(i => i.CanRead).ToArray();

            // First have to set ItemsSource before setting 
            //var itemsSourceProp = newView.GetType().GetProperty("ItemsSource");
            //if (itemsSourceProp != null)
            //{
            //    seenProps.Add(itemsSourceProp.Name);
            //}

            // Transfer over the properties (we only look at properties at VisualElement (contains backgroundcolor, etc) and above, since if we transfer properties from lower like Element, things go bad)
            foreach (var prop in newView.GetType().GetProperties().Where(i => i.CanRead))
            {
#if DEBUG
                try
                {
#endif
                    // We already handled the property (the upper class declared the property using "new" modifier, don't want to assign lower-level ones)
                    if (!seenProps.Add(prop.Name))
                    {
                        continue;
                    }

                    var propType = prop.PropertyType;

                    if (_typesToIgnore.Contains(propType))
                    {
                        continue;
                    }

                    if (!prop.CanWrite)
                    {
                        // Children and other similar properties
                        if (_uiElementCollectionType.IsAssignableFrom(propType))
                        {
                            var newVal = prop.GetValue(newView);

                            ReconcileList(prop.GetValue(oldView) as UIElementCollection, newVal as UIElementCollection);
                        }
                    }
                    else
                    {
                        // Skip assigning Resources properties
                        if (_resourceDictionaryType.IsAssignableFrom(prop.PropertyType))
                        {
                            continue;
                        }

                        // Don't assign internal renderer properties
                        //if (prop.DeclaringType == _visualElementType)
                        //{
                        //    switch (prop.Name)
                        //    {
                        //        case nameof(Batched):
                        //        case nameof(IsInNativeLayout):
                        //        case nameof(Visual):
                        //        case nameof(IsNativeStateConsistent):
                        //        case nameof(DisableLayout):
                        //        case nameof(IsPlatformEnabled):
                        //            continue;
                        //    }
                        //}

                        // Don't assign text-related fields that are based on current editing values
                        if (prop.DeclaringType == _textBoxType)
                        {
                            switch (prop.Name)
                            {
                                case nameof(TextBox.SelectionLength):
                                case nameof(TextBox.SelectionStart):
                                    continue;
                            }
                        }

                        // Don't assign ListView SelectedItem
                        if (prop.DeclaringType == _listViewType)
                        {
                            switch (prop.Name)
                            {
                                case nameof(ListView.SelectedItem):
                                    continue;
                            }
                        }

                        // Don't assign Picker SelectedItem or SelectedIndex
//                        if (prop.DeclaringType == _pickerType)
//                        {
//                            switch (prop.Name)
//                            {
//                                case nameof(Picker.SelectedItem):
//                                case nameof(Picker.SelectedIndex):
//                                    continue;

//                                case nameof(Picker.ItemDisplayBinding):
//                                    if (!object.ReferenceEquals(prop.GetValue(oldView), prop.GetValue(newView)))
//                                    {
//#if DEBUG
//                                        System.Diagnostics.Debugger.Break();
//#endif
//                                        throw new InvalidOperationException("Changing ItemDisplayBinding isn't supported. You should define this as a field and re-use the same value in each Render.");
//                                    }
//                                    break;
//                            }
//                        }

                        // Don't assign CarouselView Position or CurrentItem
                        //if (prop.DeclaringType == _carouselViewType)
                        //{
                        //    switch (prop.Name)
                        //    {
                        //        case nameof(CarouselView.Position):
                        //        case nameof(CarouselView.CurrentItem):
                        //            continue;
                        //    }
                        //}

                        // Don't re-assign ImageSources
                        //if (prop.PropertyType == _imageSourceType)
                        //{
                        //    continue;
                        //}

                        // For ListView, if ItemsSource was set, and is changing to a different list and had a currently selected item
                        //if (newView is ListView newListView && oldView is ListView oldListView && oldListView.SelectedItem != null && newListView.ItemsSource != null && oldListView.ItemsSource != null && !object.Equals(oldListView.ItemsSource, newListView.ItemsSource) && VxListViewExtensions.GetBindSelectedItem(oldListView) != null)
                        //{
                        //    // If the new listview specified a selected item
                        //    var selected = VxListViewExtensions.GetBindSelectedItem(newListView);
                        //    if (selected != null && selected.Value != null)
                        //    {
                        //        // And if the new list contains that...
                        //        bool contains = false;
                        //        foreach (var el in newListView.ItemsSource)
                        //        {
                        //            if (object.Equals(el, selected.Value))
                        //            {
                        //                contains = true;
                        //                break;
                        //            }
                        //        }

                        //        if (contains)
                        //        {
                        //            // Then we have to ignore the next event which will set the selected item to null, since it'll subsequently get set to the new selected value
                        //            VxListViewExtensions.ItemSelectedToIgnore.Add(oldListView);
                        //        }
                        //    }
                        //}

                        // If a View property
                        // If the view is a VxComponent, we want to apply its properties like margin or background color, but NOT reconcile its Content... since that'll be updated by the VxComponent itself... Note that this is already handled by VxComponent declaring a new Content property that's get-only (and so the underneath properties get caught above earlier)
                        // Also note that we DO want to transfer over view properties on VxComponent if they're properties of the component, like a custom component can declare a view as a property that parent components can set... that DOES need to be transferred over (which will later be reconciled in the render)... we'll let the else transfer it over since that also calls mark dirty
                        if (_elementType.IsAssignableFrom(propType) && !(oldView is VxComponent))
                        {
                            var newVal = prop.GetValue(newView);

                            var oldVal = prop.GetValue(oldView);
                            if (oldVal == null || oldVal.GetType() != newVal.GetType())
                            {
                                prop.SetValue(oldView, newVal);
                            }
                            else
                            {
                                ReconcileViewOfSameType(oldVal as UIElement, newVal as UIElement);
                            }
                        }
                        else
                        {
                            var newVal = prop.GetValue(newView);

                            // For updating properties... if this was a component's property...
                            if ((oldView as object) is VxComponent existingComponent && _vxComponentType.IsAssignableFrom(prop.DeclaringType))
                            {
                                // First get the existing value
                                var oldVal = prop.GetValue(oldView);

                                // If it's new
                                if (!object.Equals(oldVal, newVal))
                                {
                                    // Transfer the value and mark dirty
                                    prop.SetValue(oldView, newVal);
                                    existingComponent.MarkDirty();

                                    //if (prop.GetCustomAttribute<VxSubscribeAttribute>() != null)
                                    //{
                                    //    // Unsubscribe from old property
                                    //    if (oldVal is INotifyPropertyChanged oldValPropChanged)
                                    //    {
                                    //        existingComponent.UnsubscribeToPropertyValue(oldValPropChanged);
                                    //    }

                                    //    // And subscribe to that property
                                    //    if (newVal is INotifyPropertyChanged newValPropChanged)
                                    //    {
                                    //        existingComponent.SubscribeToPropertyValue(newValPropChanged);
                                    //    }
                                    //}
                                }
                            }
                            else
                            {
                                // Just set the value like normal
                                prop.SetValue(oldView, newVal);
                            }

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

            // Transfer bindings
            //VxBindings.TransferBindings(newView, oldView);

            // Handle ListView ItemSelected
            //if (newView is ListView newListView)
            //{
            //    if (oldView == null)
            //    {
            //        // Assign SelectedItem
            //        var selectedItemState = VxListViewExtensions.GetBindSelectedItem(newListView);
            //        if (selectedItemState != null)
            //        {
            //            newListView.SelectedItem = selectedItemState.Value;
            //        }

            //        // Subscribe to ItemSelected
            //        newListView.ItemSelected += ListView_ItemSelected;
            //    }

            //    else if (oldView is ListView oldListView)
            //    {
            //        // Transfer the binding
            //        var selectedItemState = VxListViewExtensions.GetBindSelectedItem(newListView);
            //        VxListViewExtensions.SetBindSelectedItem(oldListView, selectedItemState);

            //        // Assign SelectedItem
            //        if (selectedItemState != null)
            //        {
            //            oldListView.SelectedItem = selectedItemState.Value;
            //        }
            //    }
            //}

            // Transfer attached properties
            if (oldView is FrameworkElement oldViewFE && newView is FrameworkElement newViewFE)
            {
                Grid.SetRow(oldViewFE, Grid.GetRow(newViewFE));
                Grid.SetColumn(oldViewFE, Grid.GetColumn(newViewFE));
                Grid.SetRowSpan(oldViewFE, Grid.GetRowSpan(newViewFE));
                Grid.SetColumnSpan(oldViewFE, Grid.GetColumnSpan(newViewFE));
            }

            //if (newView is ListView)
            //{
            //    VxListViewExtensions.SetBindSelectedItem(oldView, VxListViewExtensions.GetBindSelectedItem(newView));
            //}
        }

        //private static void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    var listView = sender as ListView;

        //    VxState itemSelectedState = VxListViewExtensions.GetBindSelectedItem(listView);

        //    if (itemSelectedState != null)
        //    {
        //        itemSelectedState.Value = e.SelectedItem;
        //    }
        //}

        private static void ReconcileList(UIElementCollection oldList, UIElementCollection newList)
        {
#if DEBUG
            try
            {
#endif
                if (oldList.Count == 0)
                {
                    foreach (var val in newList)
                    {
                        oldList.Add(val as UIElement);
                    }

                    return;
                }

                if (newList.Count == 0)
                {
                    oldList.Clear();
                    return;
                }

                // Need to copy list since if I add any views from this list, it auto-removes the view from the other view
                var newViews = new List<UIElement>(newList.OfType<UIElement>());

                int i = 0;

                for (; i < oldList.Count; i++)
                {
                    var oldItem = oldList[i];
                    var newItem = newViews.ElementAtOrDefault(i);

                    if (newItem == null)
                    {
                        oldList.RemoveAt(i);
                    }
                    else if (oldItem.GetType() == newItem.GetType())
                    {
                        ReconcileViewOfSameType(oldItem, newItem);
                    }
                    else if (oldList.Count < newViews.Count)
                    {
                        oldList.Insert(i, newItem);
                    }
                    else if (oldList.Count > newViews.Count)
                    {
                        oldList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        oldList[i] = newItem;
                    }
                }

                if (oldList.Count < newViews.Count)
                {
                    for (; i < newViews.Count; i++)
                    {
                        oldList.Add(newViews[i]);
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

        //protected T FindAncestorOrSelf<T>() where T : VxComponent
        //{
        //    if (this is T expected)
        //    {
        //        return expected;
        //    }

        //    return FindAncestor<T>();
        //}

        //protected T FindAncestor<T>() where T : VxComponent
        //{
        //    VxComponent comp = GetParentComponent();
        //    while (comp != null)
        //    {
        //        if (comp is T compT)
        //        {
        //            return compT;
        //        }

        //        comp = comp.GetParentComponent();
        //    }

        //    throw new InvalidOperationException("Couldn't find a component ancestor of the specified type.");
        //}

        //protected VxComponent GetParentComponent()
        //{
        //    Element el = Parent;
        //    while (el != null)
        //    {
        //        if (el is VxComponent comp)
        //        {
        //            return comp;
        //        }

        //        el = el.Parent;
        //    }

        //    throw new InvalidOperationException("Component didn't have a parent component, you might have called a method in an incorrect manner.");
        //}

        //private VxComponent GetRootComponent()
        //{
        //    if (IsRootComponent)
        //    {
        //        return this;
        //    }

        //    Element el = Parent;
        //    while (el != null)
        //    {
        //        if (el is VxComponent comp && comp.IsRootComponent)
        //        {
        //            return comp;
        //        }

        //        el = el.Parent;
        //    }

        //    throw new InvalidOperationException("Component didn't have a parent, you might have called ShowPopup in an incorrect manner.");
        //}
    }
}
