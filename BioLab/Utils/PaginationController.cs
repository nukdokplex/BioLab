using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BioLab.Utils
{
    class PaginationController
    {

        private PaginationControls Controls;

        protected long _currentPage;
        private long CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                Controls.SetEnabledState(false);
                if (value <= LastPage && value >= 0)
                {
                    _currentPage = value;
                    Controls.CurrentPageTextBox.Text = (value + 1).ToString();
                    OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, Controls));
                }
                
            }
        }

        private long LastPage
        {
            get
            {
                return ((EntriesCount - 1) / EntriesPerPage + 1)-1;
            }
        }

        private long EntriesCount = 0;
        private int EntriesPerPage = 20;
        private string CurrentSearchQuery = "";
        public long CurrentOffset 
        { 
            get
            {
                return CurrentPage * EntriesPerPage;
            } 
        }

        public PaginationController(int entriesPerPage, long entriesCount, PaginationControls controls, EventHandler<PaginationOnRefreshEventArgs> onRefresh)
        {
            EntriesPerPage = entriesPerPage;
            Controls = controls;
            EntriesCount = entriesCount;
            Init();

        }

        public PaginationController(long entriesCount, PaginationControls controls)
        {
            Controls = controls;
            EntriesCount = entriesCount;
            Init();
        }

        protected void Init()
        {
            CurrentPage = 0;
            InitControls();
        }

        protected void InitControls()
        {
            Controls.FirstPageButton.Click += SetFirstPage;
            Controls.PreviousPageButton.Click += DecrementPage;
            Controls.NextPageButton.Click += IncrementPage;
            Controls.LastPageButton.Click += SetLastPage;
            Controls.CurrentPageTextBox.KeyDown += SpecifyPage;
        }

        private void IncrementPage(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
        }

        private void DecrementPage(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
        }

        private void SetLastPage(object sender, RoutedEventArgs e)
        {
            CurrentPage = LastPage;
        }

        private void SetFirstPage(object sender, RoutedEventArgs e)
        {
            CurrentPage = 0;
        }

        private void SpecifyPage(object sender, KeyEventArgs e)
        {
            long page;

            if (long.TryParse((sender as TextBox).Text, out page))
            {
                CurrentPage = page;
            }
        }

        public void EntriesCountChanged(long entriesCount)
        {
            EntriesCount = entriesCount;
            CurrentPage = 0;
            OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, Controls));
        }

        public event EventHandler<PaginationOnRefreshEventArgs> OnRefresh;
        
    }
    class PaginationOnRefreshEventArgs : EventArgs
    {
        public long CurrentOffset;
        public string SearchQuery;
        public PaginationControls Controls;
        

        public PaginationOnRefreshEventArgs(long currentOffset, string searchQuery)
        {
            CurrentOffset = currentOffset;
            SearchQuery = searchQuery;
        }

        public PaginationOnRefreshEventArgs(long currentOffset, string searchQuery, PaginationControls controls)
        {
            CurrentOffset = currentOffset;
            SearchQuery = searchQuery;
            Controls = controls;
        }
    }
    class PaginationControls
    {
        public Button FirstPageButton, PreviousPageButton, NextPageButton, LastPageButton;
        public TextBox CurrentPageTextBox;

        public PaginationControls(Button firstPageButton, Button previousPageButton, Button nextPageButton, Button lastPageButton, TextBox currentPageTextBox)
        {
            FirstPageButton = firstPageButton;
            PreviousPageButton = previousPageButton;
            NextPageButton = nextPageButton;
            LastPageButton = lastPageButton;
            CurrentPageTextBox = currentPageTextBox;
        }

        public void SetEnabledState(bool isEnabled)
        {
            FirstPageButton.IsEnabled = isEnabled;
            PreviousPageButton.IsEnabled = isEnabled;
            NextPageButton.IsEnabled = isEnabled;
            LastPageButton.IsEnabled = isEnabled;
            CurrentPageTextBox.IsEnabled = isEnabled;
        }
    }
}
