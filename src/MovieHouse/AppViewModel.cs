using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace MovieHouse
{
    public class AppViewModel : Conductor<object>, IHandle<AddMovieEvent>, IHandle<PlayMovieEvent>, IHandle<MoveMovieEvent>
    {
        private readonly IWindowManager _windowManager;

        private readonly IEventAggregator _eventAggregator;

        private readonly MovieManager _mmanager;

        private readonly MoviePlayer _mplayer;

        private int _total;
        private int _currentIndex;

        public AppViewModel(MovieManager mmanager, MoviePlayer mplayer, IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _windowManager = windowManager;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _mplayer = mplayer;

            _mmanager = mmanager;
            _mmanager.Initialize();

            Total = _mmanager.MovieCount;
            if (Total > 0) { CurrentIndex = 1; }
        }

        #region Binding Properties

        public ReadOnlyObservableCollection<MovieViewModel> Movies { get { return _mmanager.Movies; } }

        public int Total
        {
            get { return _total; }
            set
            {
                if (value == _total) return;
                _total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (value == _currentIndex) return;
                _currentIndex = value;
                NotifyOfPropertyChange(() => CurrentIndex);
                NotifyOfPropertyChange(() => CurrentMovieName);
                NotifyOfPropertyChange(() => CanFindNext);
                NotifyOfPropertyChange(() => CanFindPrevious);
            }
        }

        public string CurrentMovieName
        {
            get { return _mmanager.CurrentMovie.Name; }
        }

        #endregion

        #region Binding Methods

        public bool CanFindNext
        {
            get { return _mmanager.CanFindNext; }
        }

        public void FindNext()
        {
            CurrentIndex = _mmanager.FindNext();
        }

        public bool CanFindPrevious
        {
            get { return _mmanager.CanFindPrevious; }
        }

        public void FindPrevious()
        {
            CurrentIndex = _mmanager.FindPrevious();
        }

        public void AddNewMovie()
        {
            _windowManager.ShowDialog(new MovieDetailViewModel(_eventAggregator));
        }

        public bool CanAddNewMovie()
        {
            return true;
        }

        public void RemoveMovie()
        {
            _mmanager.DeleteCurrentMovie();

            Total--;
            
            if (!_mmanager.CanFindNext)
            {
                CurrentIndex--;
            }

            if (Total == 0)
            {
                CurrentIndex = 0;
            }
        }

        public bool CanRemoveNewMovie()
        {
            return true;
        }

        #endregion

        public void Handle(AddMovieEvent message)
        {
            var movie = message.Movie;
            _mmanager.AddMovie(movie);
            Total++;
            NotifyOfPropertyChange(() => Movies);
        }

        public void Handle(PlayMovieEvent message)
        {
            var movie = message.Movie.FileName;
            _mplayer.Play(movie);
        }

        public void Handle(MoveMovieEvent message)
        {
            var movie = message.Movie;
            CurrentIndex = _mmanager.MarkAsCurrentMovie(movie.Name);
            NotifyOfPropertyChange(() => CanFindNext);
            NotifyOfPropertyChange(() => CanFindPrevious);
        }
    }
}