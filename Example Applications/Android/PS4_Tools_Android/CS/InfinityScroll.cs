using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace DesignLibrary_Tutorial
{
    public abstract class InfiniteScrollRecyclerViewAdapter<T> : RecyclerView.Adapter
    {
        protected const int VIEW_TYPE_PROGRESSBAR = -2147483648;

        private readonly LinearLayoutManager _layoutManager;

        private readonly bool _isVerticalLayoutManager;

        private readonly BackgroundWorker _worker;

        private List<T> _items;

        private int _currentItems;

        private bool _hasMoreItems;

        private bool _isLoading;

        public override int ItemCount
        {
            get
            {
                if (this._isLoading)
                {
                    return this._currentItems + 1;
                }
                return this._currentItems;
            }
        }

        //protected InfiniteScrollRecyclerViewAdapter(RecyclerView recyclerView)
        //{
        //    if (recyclerView == null)
        //    {
        //        throw new Exception("RecyclerView is null");
        //    }
        //    RecyclerView.LayoutManager layoutManager = recyclerView.GetLayoutManager();
        //    if (layoutManager == null)
        //    {
        //        throw new Exception("RecyclerView LayoutManager must be set before initializing the Adapter");
        //    }
        //    this._layoutManager = (layoutManager as LinearLayoutManager);
        //    if (this._layoutManager == null)
        //    {
        //        throw new Exception("RecyclerView must be using LineraLayoutManager");
        //    }
        //    this._isVerticalLayoutManager = (this._layoutManager.Orientation == 1);
        //    EndlessScrollListener scrollListener = new EndlessScrollListener();
        //    scrollListener.Scrolled += new Action(this.OnScrolled);
        //    recyclerView.AddOnScrollListener(scrollListener);
        //    BackgroundWorker expr_81 = new BackgroundWorker();
        //    this._worker = expr_81;
        //    this._worker.DoWork += (new DoWorkEventHandler(this.WorkerLoadPage));
        //    this._worker.RunWorkerCompleted += (new RunWorkerCompletedEventHandler(this.WorkerOnPageLoad));
        //}

        protected InfiniteScrollRecyclerViewAdapter(RecyclerView recyclerView,IEnumerable<T> mvalues)
        {
            if (recyclerView == null)
            {
                throw new Exception("RecyclerView is null");
            }
            RecyclerView.LayoutManager layoutManager = recyclerView.GetLayoutManager();
            if (layoutManager == null)
            {
                throw new Exception("RecyclerView LayoutManager must be set before initializing the Adapter");
            }
            this._layoutManager = (layoutManager as LinearLayoutManager);
            if (this._layoutManager == null)
            {
                throw new Exception("RecyclerView must be using LineraLayoutManager");
            }
            SetItems(mvalues);
            this._isVerticalLayoutManager = (this._layoutManager.Orientation == 1);
            EndlessScrollListener scrollListener = new EndlessScrollListener();
            scrollListener.Scrolled += new Action(this.OnScrolled);
            recyclerView.AddOnScrollListener(scrollListener);
            BackgroundWorker expr_81 = new BackgroundWorker();
            this._worker = expr_81;
            this._worker.DoWork += (new DoWorkEventHandler(this.WorkerLoadPage));
            this._worker.RunWorkerCompleted += (new RunWorkerCompletedEventHandler(this.WorkerOnPageLoad));
        }

        public void SetItems(IEnumerable<T> items)
        {
            this._items = ((items != null) ? Enumerable.ToList<T>(items) : new List<T>());
            int prevItemsNumber = this._currentItems;
            this._currentItems = Enumerable.Count<T>(this._items);
            this._hasMoreItems = true;
            base.NotifyItemRangeRemoved(0, prevItemsNumber);
            base.NotifyItemRangeInserted(0, this._currentItems);
        }

        public override int GetItemViewType(int position)
        {
            if (position == this._currentItems && this._isLoading)
            {
                return -2147483648;
            }
            return this.ItemViewType(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == -2147483648)
            {
                LinearLayout expr_13 = new LinearLayout(parent.Context);
                expr_13.LayoutParameters = (new LinearLayout.LayoutParams(this._isVerticalLayoutManager ? -1 : -2, this._isVerticalLayoutManager ? -2 : -1));
                expr_13.SetGravity(GravityFlags.Center);
                expr_13.AddView(new ProgressBar(parent.Context));
                return new DefaultViewHolder(expr_13);
            }
            return this.GetViewHolder(parent, viewType);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!(holder is DefaultViewHolder))
            {
                this.DoBindViewHolder(holder, position);
            }
        }

        protected T GetItem(int position)
        {
            if (position >= this._items.Count)
            {
                return default(T);
            }
            return this._items[position];
        }

        private void OnScrolled()
        {
            if (!this._isLoading && this._hasMoreItems && this._layoutManager.FindLastVisibleItemPosition() == this._currentItems - 1)
            {
                this.LoadNextPage();
            }
        }

        private void LoadNextPage()
        {
            this._isLoading = true;
            base.NotifyItemInserted(this._currentItems);
            this._worker.RunWorkerAsync();
        }

        private void WorkerLoadPage(object sender, DoWorkEventArgs args)
        {
            IEnumerable<T> newItems = this.LoadMoreItems(this._currentItems);
            if (newItems != null && Enumerable.Any<T>(newItems))
            {
                args.Result = (newItems);
            }
        }

        private void WorkerOnPageLoad(object sender, RunWorkerCompletedEventArgs args)
        {
            if (args.Result != null)
            {
                List<T> newitems = Enumerable.ToList<T>((IEnumerable<T>)args.Result);
                this._currentItems += Enumerable.Count<T>(newitems);
                this._hasMoreItems = true;
                this._items.AddRange(newitems);
            }
            else
            {
                this._hasMoreItems = false;
            }
            this._isLoading = false;
            base.NotifyDataSetChanged();
        }

        protected abstract int ItemViewType(int position);

        public abstract RecyclerView.ViewHolder GetViewHolder(ViewGroup parent, int viewType);

        public abstract void DoBindViewHolder(RecyclerView.ViewHolder holder, int position);

        protected abstract IEnumerable<T> LoadMoreItems(int numberOfExistingItems);
    }

    internal class EndlessScrollListener : RecyclerView.OnScrollListener
    {
        [method: CompilerGenerated]
        [CompilerGenerated]
        public event Action Scrolled;

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            if (this.Scrolled != null)
            {
                this.Scrolled.Invoke();
            }
        }
    }

    internal class DefaultViewHolder : RecyclerView.ViewHolder
    {
        internal DefaultViewHolder(View view) : base(view)
        {
        }
    }
}