using Caliburn.Micro;

namespace MovieHouse
{
    public class MovieDetail2ViewModel : PropertyChangedBase
    {
        private readonly Movie _movie;
        private readonly IEventAggregator _eventAggregator;
        
        public MovieDetail2ViewModel(Movie movie, IEventAggregator eventAggregator)
        {
            _movie = movie;
            _eventAggregator = eventAggregator;
        }

        public string Name
        {
            get { return _movie.Name; }
            set
            {
                if (value == _movie.Name) return;
                _movie.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Country
        {
            get { return _movie.Country; }
            set
            {
                if (value == _movie.Country) return;
                _movie.Country = value;
                NotifyOfPropertyChange(() => Country);
            }
        }

        public string FileName
        {
            get { return _movie.FileName; }
            set
            {
                if (value == _movie.FileName) return;
                _movie.FileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }

        public string PosterName
        {
            get { return _movie.PosterName; }
            set
            {
                if (value == _movie.PosterName) return;
                _movie.PosterName = value;
                NotifyOfPropertyChange(() => PosterName);
            }
        }

        public int Year
        {
            get { return _movie.Year; }
            set
            {
                if (value == _movie.Year) return;
                _movie.Year = value;
                NotifyOfPropertyChange(() => Year);
            }
        }

        public string Details
        {
            get { return _movie.Details; }
            set
            {
                if (value == _movie.Details) return;
                _movie.Details = value;
                NotifyOfPropertyChange(() => Details);
            }
        }

        public void SelectMovie()
        {
            // TODO:
        }

        public void SelectPoster()
        {
            // TODO:
        }

        public void Close()
        {
            // TODO:
        }
    }
}