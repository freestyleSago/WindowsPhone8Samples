using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Telerik.Core.Data
{
    internal class BatchLoadingProvider<T> : IDisposable
    {
        private ISupportIncrementalLoading incrementalLoadingSource;
        private ICollection<T> loadedItemsCollection;
        private IAsyncOperation<LoadMoreItemsResult> loadItemsOperation;

        public BatchLoadingProvider(ISupportIncrementalLoading incrementalLoadingSource, ICollection<T> loadedItemsCollection)
        {
            if (incrementalLoadingSource == null)
                throw new ArgumentException("The batch loading provider requires ISupportIncrementalLoading to operate!");

            this.incrementalLoadingSource = incrementalLoadingSource;
            this.loadedItemsCollection = loadedItemsCollection;

            var incrementalBatchLoadingSource = incrementalLoadingSource as IIncrementalBatchLoading;
            if (incrementalBatchLoadingSource != null)
            {
                this.BatchSize = incrementalBatchLoadingSource.BatchSize;
            }
        }

        public event EventHandler<BatchLoadingEventArgs> StatusChanged;

        public uint? BatchSize { get; private set; }

        public virtual bool ShouldRequestItems(int lastRequestedIndex)
        {
            return lastRequestedIndex >= this.loadedItemsCollection.Count &&
                (this.loadItemsOperation == null || this.loadItemsOperation.Status != AsyncStatus.Started) &&
                this.incrementalLoadingSource.HasMoreItems;
        }

        public virtual void RequestItems(int lastRequestedIndex)
        {
            if (this.ShouldRequestItems(lastRequestedIndex))
            {
                this.OnStatusChanged(BatchLoadingStatus.ItemsRequested);

                this.loadItemsOperation = this.incrementalLoadingSource.LoadMoreItemsAsync(this.BatchSize ?? (uint)(lastRequestedIndex - this.loadedItemsCollection.Count + 1));

                if (this.loadItemsOperation != null)
                {
                    this.loadItemsOperation.Completed += (s, e) =>
                    {
                        if (this.loadItemsOperation.Status == AsyncStatus.Completed)
                        {
                            this.OnStatusChanged(BatchLoadingStatus.ItemsLoaded);
                        }
                        else
                        {
                            this.OnStatusChanged(BatchLoadingStatus.ItemsLoadFailed);
                        }
                    };
                }
            }
        }

        public void Dispose()
        {
            if (this.loadItemsOperation != null)
            {
                this.loadItemsOperation.Cancel();
            }
        }

        protected virtual void OnStatusChanged(BatchLoadingStatus status)
        {
            var handler = this.StatusChanged;
            if (handler != null)
            {
                handler(this, new BatchLoadingEventArgs(status));
            }
        }
    }
}
