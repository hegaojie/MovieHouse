using System.Windows;
using Caliburn.Micro;
using Microsoft.Win32;

namespace MovieHouse
{
    public class MovieDetailViewModel : PropertyChangedBase
    {
        private Movie _movie;
        private readonly IEventAggregator _eventAggregator;

        public MovieDetailViewModel(Movie movie, IEventAggregator eventAggregator)
        {
            _movie = movie ?? new Movie();

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
                _movie.IsPosterChanged = true;
                NotifyOfPropertyChange(() => PosterName);
            }
        }

        public bool IsPosterNameChanged { get { return _movie.IsPosterChanged; } }

        public Visibility CancelButtonVisibility { get { return string.IsNullOrEmpty(_movie.Name) ? Visibility.Visible : Visibility.Collapsed; } }

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
            var openFile = new OpenFileDialog();
            var ret = openFile.ShowDialog();
            if (ret == false)
                return;

            FileName = openFile.FileName;
        }

        public void SelectPoster()
        {
            var openFile = new OpenFileDialog();
            var ret = openFile.ShowDialog();
            if (ret == false)
                return;

            PosterName = openFile.FileName;
        }

        public void OK()
        {
            _eventAggregator.Publish(new CloseDetailEvent());
        }

        public void Cancel()
        {
            _eventAggregator.Publish(new CancelDetailEvent());
        }

        public Movie ToMovie()
        {
            return _movie;
        }

        public void RefreshPoster()
        {
            if (!IsPosterNameChanged) { return; }
            _movie.LoadPoster();
            _movie.IsPosterChanged = false;
        }

        public void SetMovie(Movie movie)
        {
            if (!_movie.Equals(movie))
            {
                _movie = movie;
            }
        }
    }
}