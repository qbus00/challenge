using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Challenge.IncrementalLoading
{
    public interface ISupportIncrementalLoading
    {
        int PageSize { get; set; }

        bool HasMoreItems { get; set; }

        MvxNotifyTask LoadMoreTask { get; }

        IMvxCommand LoadMoreItemsCommand { get; }
    }
}