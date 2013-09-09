using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using System.Linq;

namespace MovieHouse
{
    public class AppViewModel : Conductor<object>, IHandle<Movie>
    {
        private readonly IWindowManager _windowManager;

        private readonly IEventAggregator _eventAggregator;

        private readonly MovieManager _mmanager;

        private readonly MoviePlayer _mplayer;

        private readonly MovieProcessQueue _queueManager;

        private int _rightNextIndex;
        private int _leftNextIndex;
        private int _total;
        private int _currentIndex;

        public AppViewModel(MovieManager mmanager, MoviePlayer mplayer, IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _windowManager = windowManager;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _mmanager = mmanager;
            _mmanager.LoadConfig();

            Total = _mmanager.Movies.Count;

            _mplayer = mplayer;

            _queueManager = new MovieProcessQueue(9);

            Initialize();
        }

        private void Initialize()
        {
            while (_queueManager.Queue.Count < _queueManager.Capacity)
            {
                if (_rightNextIndex >= _mmanager.Movies.Count) { break; }

                var mvm = new MovieViewModel(_mmanager.Movies[_rightNextIndex], _eventAggregator);


                _queueManager.InsertAtRight(mvm);

                _rightNextIndex++;
            }

            if (_queueManager.Queue.Count(m => !m.IsDummy) > 0)
            {
                CurrentIndex = 1;
            }
        }

        #region Binding Properties

        public ReadOnlyObservableCollection<MovieViewModel> Movies
        {
            get { return new ReadOnlyObservableCollection<MovieViewModel>(_queueManager.Queue); }
        }

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
            }
        }

        #endregion

        #region Binding Methods

        // TODO: should be moved to class MovieProcessQueue
        public void FindNext()
        {
            if (_queueManager.AlreadyReachRightEnd()) { return; }
            
            CurrentIndex++;

            if (!_queueManager.Queue[_queueManager.Queue.Count - 1].IsDummy)
            {
                _rightNextIndex = _queueManager.Queue[_queueManager.Queue.Count - 1].SequencialNo + 1;
            }

            var newMovie = _rightNextIndex < _mmanager.Movies.Count ? new MovieViewModel(_mmanager.Movies[_rightNextIndex], _eventAggregator) 
                : new MovieViewModel(new Movie(), _eventAggregator);

            _queueManager.InsertAtRight(newMovie);
            _queueManager.Animate();
        }

        // TODO: should be moved to class MovieProcessQueue
        public void FindPrevious()
        {
            if (_queueManager.AlreadyReachLeftEnd()) { return; }

            CurrentIndex--;

            if (!_queueManager.Queue[0].IsDummy)
            {
                _leftNextIndex = _queueManager.Queue[0].SequencialNo - 1;
            }

            var newMovie = _leftNextIndex >= 0 ? new MovieViewModel(_mmanager.Movies[_leftNextIndex], _eventAggregator) 
                : new MovieViewModel(new Movie(), _eventAggregator);

            _queueManager.InsertAtLeft(newMovie);
            _queueManager.Animate();
        }

        public void AddNewMovie()
        {
            _windowManager.ShowDialog(new MovieDetailViewModel(_eventAggregator));
            //_windowManager.ShowPopup(new MovieDetailViewModel(_eventAggregator));
        }

        public bool CanAddNewMovie()
        {
            return true;
        }

        public void RemoveNewMovie()
        {
        }

        public bool CanRemoveNewMovie()
        {
            return true;
        }

        #endregion

        public void Handle(Movie message)
        {
            _mplayer.Play(message.FileName);
        }
    }
}