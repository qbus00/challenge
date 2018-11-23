using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Challenge.IncrementalLoading
{
    public class IncrementalListView : ListView
    {
        int _lastPosition;
        IList _itemsSource;
        ISupportIncrementalLoading _incrementalLoading;

        public IncrementalListView()
        {
            ItemAppearing += OnItemAppearing;
        }

        public IncrementalListView(ListViewCachingStrategy cachingStrategy)
            : base(cachingStrategy)
        {
            ItemAppearing += OnItemAppearing;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                _itemsSource = ItemsSource as IList;

                if (_itemsSource == null)
                {
                    throw new Exception($"{nameof(IncrementalListView)} requires that {nameof(ItemsSource)} be of type IList");
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
            {
                _incrementalLoading = BindingContext as ISupportIncrementalLoading;

                if (_incrementalLoading == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"{nameof(IncrementalListView)} BindingContext does not implement {nameof(ISupportIncrementalLoading)}. This is required for incremental loading to work.");
                }
            }
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            int position = _itemsSource?.IndexOf(e.Item) ?? 0;

            if (_itemsSource != null)
            {
                if (PreloadCount <= 0)
                    PreloadCount = 1;

                int preloadIndex = Math.Max(_itemsSource.Count - PreloadCount, 0);

                if ((position > _lastPosition || (position == _itemsSource.Count - 1)) && (position >= preloadIndex))
                {
                    _lastPosition = position;

                    if (
                        (_incrementalLoading.LoadMoreTask == null || _incrementalLoading.LoadMoreTask.IsCompleted)
                         && !IsRefreshing && _incrementalLoading.HasMoreItems)
                    {
                        LoadMoreItems();
                    }
                }
            }
        }

        void LoadMoreItems()
        {
            var command = _incrementalLoading.LoadMoreItemsCommand;
            if (command != null && command.CanExecute(null))
                command.Execute(null);
        }

        public static readonly BindableProperty PreloadCountProperty =
            BindableProperty.Create(nameof(PreloadCount), typeof(int), typeof(IncrementalListView), 0);

        public int PreloadCount
        {
            get => (int) GetValue(PreloadCountProperty);
            set => SetValue(PreloadCountProperty, value);
        }
    }
}