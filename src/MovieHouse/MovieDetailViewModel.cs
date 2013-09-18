using Caliburn.Micro;
using Microsoft.Win32;

namespace MovieHouse
{
    public class MovieDetailViewModel : Screen
    {
        private Movie _movie;

        private readonly IEventAggregator _eventAggregator;

        public MovieDetailViewModel(IEventAggregator eventAggregator)
            : this(eventAggregator, new Movie())
        {
        }

        public MovieDetailViewModel(IEventAggregator eventAggregator, Movie movie)
        {
            _eventAggregator = eventAggregator;
            _movie = movie;
        }

        public string Name
        {
            get { return _movie.Name; }
            set
            {
                _movie.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public int Year
        {
            get { return _movie.Year; }
            set
            {
                _movie.Year = value;
                NotifyOfPropertyChange(() => Year);
            }
        }

        public string Country
        {
            get { return _movie.Country; }
            set
            {
                _movie.Country = value;
                NotifyOfPropertyChange(() => Country);
            }
        }

        public string Details
        {
            get { return _movie.Details; }
            set
            {
                _movie.Details = value;
                NotifyOfPropertyChange(() => Details);
            }
        }

        public string FileName
        {
            get { return _movie.FileName; }
            set
            {
                _movie.FileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }

        public string PosterName
        {
            get { return _movie.PosterName; }
            set
            {
                _movie.PosterName = value;
                NotifyOfPropertyChange(() => PosterName);
            }
        }

        public void SearchVideoFile()
        {
            var openFile = new OpenFileDialog();
            var ret = openFile.ShowDialog();
            if (ret == false)
                return;

            FileName = openFile.FileName;
        }

        public void SearchPosterFile()
        {
            var openFile = new OpenFileDialog();
            var ret = openFile.ShowDialog();
            if (ret == false)
                return;

            PosterName = openFile.FileName;
        }

        public void Commit()
        {
            TryClose();
            var e = new AddMovieEvent()
                        {
                            Movie = _movie
                        };

            _eventAggregator.Publish(e);
        }

        public void Cancel()
        {
            TryClose();
        }
    }
}