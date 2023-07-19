namespace HPlusSport.API.Models
{
    public class QueryParams
    {
        // for pagination
        private int _page = 1;
        private int _size = 50;
        private int _maxSize = 100;

        public int Page
        {
            get {
                return _page;
            }
            set
            {
                _page = value;
            }

        }

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = Math.Min(_maxSize, value);
            }
        }

        // for sorting
        public string SortBy { get; set; } = "Id";

        private string _sortOrder = "asc";
        public string SortOrder
        {
            get { return _sortOrder;  }
            set {
                if(value == "asc" || value == "desc")
                {
                    _sortOrder = value;
                }
            }
        }


    }
}
