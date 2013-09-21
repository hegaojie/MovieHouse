using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MovieHouse
{
    public class MovieProcessQueue
    {
        private readonly ObservableCollection<MovieViewModel> _queue;
        private readonly int _capacity;
        private IList<AnimationFactor> _factorsMap;

        public ObservableCollection<MovieViewModel> Queue { get { return _queue; } }

        public int Capacity { get { return _capacity; } }

        public int QueueCount { get { return _queue.Count; } }

        public MovieProcessQueue(int capacity)
        {
            _capacity = capacity;
            _queue = new ObservableCollection<MovieViewModel>();

            InitializeFactorsMap();

            InitializeQueue();
        }

        public void Animate()
        {
            foreach (var mvm in _queue)
            {
                mvm.TriggerAnimation = Guid.NewGuid().ToString();
            }
        }

        public bool AlreadyReachRightEnd()
        {
            return (_queue.Count <= (_capacity + 1) / 2 || _queue[(_capacity + 1) / 2].IsDummy);
        }

        public bool AlreadyReachLeftEnd()
        {
            return _queue[(_capacity - 1) / 2 - 1].IsDummy;
        }

        public void InsertAtLeft(MovieViewModel movie)
        {
            movie.CenterX = _factorsMap[0].CenterX;
            movie.CenterY = _factorsMap[0].CenterY;
            movie.Scale = _factorsMap[0].Scale;
            movie.Opacity = _factorsMap[0].Opacity;
            movie.ZIndex = _factorsMap[0].ZIndex;

            for (var i = 0; i < _capacity - 1; i++)
            {
                _queue[i].NewCenterX = _factorsMap[i + 1].CenterX;
                _queue[i].NewCenterY = _factorsMap[i + 1].CenterY;
                _queue[i].NewScale = _factorsMap[i + 1].Scale;
                _queue[i].NewOpacity = _factorsMap[i + 1].Opacity;
                _queue[i].NewZIndex = _factorsMap[i + 1].ZIndex;
            }

            Insert(movie, 0, _capacity);
        }

        public void InsertAtRight(MovieViewModel newMovie)
        {
            if (_queue.Count < _capacity)
            {
                newMovie.CenterX = _factorsMap[_queue.Count].CenterX;
                newMovie.CenterY = _factorsMap[_queue.Count].CenterY;
                newMovie.Scale = _factorsMap[_queue.Count].Scale;
                newMovie.Opacity = _factorsMap[_queue.Count].Opacity;
                newMovie.ZIndex = _factorsMap[_queue.Count].ZIndex;
            }
            else
            {
                for (var i = _queue.Count - 1; i > 0; i--)
                {
                    _queue[i].NewCenterX = _factorsMap[i - 1].CenterX;
                    _queue[i].NewCenterY = _factorsMap[i - 1].CenterY;
                    _queue[i].NewScale = _factorsMap[i - 1].Scale;
                    _queue[i].NewOpacity = _factorsMap[i - 1].Opacity;
                    _queue[i].NewZIndex = _factorsMap[i - 1].ZIndex;
                }

                newMovie.CenterX = _factorsMap[_queue.Count - 1].CenterX;
                newMovie.CenterY = _factorsMap[_queue.Count - 1].CenterY;
                newMovie.Scale = _factorsMap[_queue.Count - 1].Scale;
                newMovie.Opacity = _factorsMap[_queue.Count - 1].Opacity;
                newMovie.ZIndex = _factorsMap[_queue.Count - 1].ZIndex;
            }

            Insert(newMovie, _queue.Count, 0);
        }

        public void InsertAtRightHalf(MovieViewModel newMovie)
        {
            if (_queue.Count < _capacity)
            {
                newMovie.CenterX = _factorsMap[_queue.Count].CenterX;
                newMovie.CenterY = _factorsMap[_queue.Count].CenterY;
                newMovie.Scale = _factorsMap[_queue.Count].Scale;
                newMovie.Opacity = _factorsMap[_queue.Count].Opacity;
                newMovie.ZIndex = _factorsMap[_queue.Count].ZIndex;
            }
            else
            {
                for (var i = _queue.Count - 1; i > _capacity/2; i--)
                {
                    _queue[i].NewCenterX = _factorsMap[i - 1].CenterX;
                    _queue[i].NewCenterY = _factorsMap[i - 1].CenterY;
                    _queue[i].NewScale = _factorsMap[i - 1].Scale;
                    _queue[i].NewOpacity = _factorsMap[i - 1].Opacity;
                    _queue[i].NewZIndex = _factorsMap[i - 1].ZIndex;

                    _queue[i].SequencialNo = _queue[i - 1].SequencialNo;
                }

                newMovie.CenterX = _factorsMap[_queue.Count - 1].CenterX;
                newMovie.CenterY = _factorsMap[_queue.Count - 1].CenterY;
                newMovie.Scale = _factorsMap[_queue.Count - 1].Scale;
                newMovie.Opacity = _factorsMap[_queue.Count - 1].Opacity;
                newMovie.ZIndex = _factorsMap[_queue.Count - 1].ZIndex;
            }

            _queue.RemoveAt(_capacity/2);

            Insert(newMovie, _queue.Count, 0);
        }

        public void RemoveCenter()
        {
            _queue.RemoveAt(_capacity/2);
        }

        private void Insert(MovieViewModel movie, int insertIndex, int removeIndex)
        {
            _queue.Insert(insertIndex, movie);

            if (_queue.Count > _capacity)
            {
                _queue.RemoveAt(removeIndex);
            }
        }

        private void InitializeQueue()
        {
            for (var i = 0; i < (_capacity - 1) / 2; i++)
            {
                InsertAtRight(new MovieViewModel(new Movie(), null));
            }
        }

        private void InitializeFactorsMap()
        {
            _factorsMap = new List<AnimationFactor>
                              {
                                  new AnimationFactor
                                      {CenterX = 140, CenterY = 250, Opacity = 1, Scale = 0.6, ZIndex = 0},       
                                  new AnimationFactor
                                      {CenterX = 100, CenterY = 250, Opacity = 1, Scale = 0.6, ZIndex = 1},
                                  new AnimationFactor
                                      {CenterX = 140, CenterY = 250, Opacity = 1, Scale = 0.7, ZIndex = 2},
                                  new AnimationFactor
                                      {CenterX = 210, CenterY = 250, Opacity = 1, Scale = 0.85, ZIndex = 3},
                                  
                                  // CENTER
                                  new AnimationFactor
                                      {CenterX = 320, CenterY = 250, Opacity = 1, Scale = 1.0, ZIndex = 4},
                                  
                                  new AnimationFactor
                                      {CenterX = 430, CenterY = 250, Opacity = 1, Scale = 0.85, ZIndex = 3},
                                  new AnimationFactor
                                      {CenterX = 500, CenterY = 250, Opacity = 1, Scale = 0.7, ZIndex = 2},
                                  new AnimationFactor
                                      {CenterX = 540, CenterY = 250, Opacity = 1, Scale = 0.6, ZIndex = 1},  
                                  new AnimationFactor
                                      {CenterX = 500, CenterY = 250, Opacity = 1, Scale = 0.6, ZIndex = 0}
                              };
        }

        public MovieViewModel Center
        {
            get 
            { 
                if (QueueCount > _capacity/2)
                {
                    return _queue[_capacity/2];
                }

                if (QueueCount > 0)
                {
                    return _queue[QueueCount - 1];
                }

                return new MovieViewModel(new Movie(), null);
            }
        }
    }
}