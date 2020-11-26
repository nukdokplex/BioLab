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
        public long CurrentPage
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
                    if (EntriesPerPage != -1)
                    {
                        //OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, -1, Controls));
                    }
                    else
                    {
                       // OnRefresh(this, new PaginationOnRefreshEventArgs(0, CurrentSearchQuery, -1, Controls));
                    }
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

        public PaginationController(int entriesPerPage, long entriesCount, PaginationControls controls)
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
            if (EntriesPerPage == -1)
            {
                OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, -1, Controls));
            }
            else
            {
                OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, -1, Controls));
            }

        }

        public void EntriesPerPageCountChanged(int entriesPerPage)
        {
            EntriesPerPage = entriesPerPage;
            CurrentPage = 0;
            if (entriesPerPage == -1)
            {
                OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, -1, Controls));
            }
            else
            {
                OnRefresh(this, new PaginationOnRefreshEventArgs(CurrentOffset, CurrentSearchQuery, -1, Controls));
            }
            
        }

        public event EventHandler<PaginationOnRefreshEventArgs> OnRefresh;
        
    }
    class PaginationOnRefreshEventArgs : EventArgs
    {
        public long CurrentOffset;
        public string SearchQuery;
        public PaginationControls Controls;
        public long EntriesCount;
        

        public PaginationOnRefreshEventArgs(long currentOffset, string searchQuery, long entriesCount)
        {
            CurrentOffset = currentOffset;
            SearchQuery = searchQuery;
            EntriesCount = entriesCount;
        }

        public PaginationOnRefreshEventArgs(long currentOffset, string searchQuery, long entriesCount, PaginationControls controls)
        {
            CurrentOffset = currentOffset;
            SearchQuery = searchQuery;
            Controls = controls;
            EntriesCount = entriesCount;
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
