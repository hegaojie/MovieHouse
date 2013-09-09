using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Caliburn.Micro;

namespace MovieHouse
{
    public class MovieViewModel : PropertyChangedBase
    {
        private readonly int AnimationMilliseconds = 5000;
        private readonly Movie _movie;

        public readonly double _originalWidth;
        public readonly double _originalHeight;

        private double _opacity;
        private double _newOpacity;
        private double _scale;
        private double _newScale;
        private int _zIndex;
        private double _centerX;
        private double _centerY;
        private int _newZIndex;
        private double _newCenterX;
        private double _newCenterY;
        private string _triggerAnimation;
        private bool _isCurrent;
        private readonly IEventAggregator _eventAggregator;

        public bool IsDummy { get; set; }

        public MovieViewModel(Movie movie, IEventAggregator eventAggregator)
        {
            if (string.IsNullOrEmpty(movie.Name))
            {
                IsDummy = true;
                _movie = movie;
                return;
            }

            _movie = movie;
            _movie.LoadPoster();

            _eventAggregator = eventAggregator;

            SequencialNo = _movie.SequencialNo;
            _originalWidth = _movie.Poster.Width;
            _originalHeight = _movie.Poster.Height;
        }

        public int SequencialNo { get; set; }
        
        public string Name
        {
            get { return _movie.Name; }
        }

        public double CenterX
        {
            get { return _centerX; }
            set
            {
                if (value.Equals(_centerX)) return;
                _centerX = value;
                NotifyOfPropertyChange(() => CenterX);
            }
        }

        public double CenterY
        {
            get { return _centerY; }
            set
            {
                if (value.Equals(_centerY)) return;
                _centerY = value;
                NotifyOfPropertyChange(() => CenterY);
            }
        }

        #region Binding Properties

        public double OffsetX
        {
            get { return _centerX - (_originalWidth * Scale) / 2; }
        }

        public double OffsetY
        {
            get { return _centerY - (_originalHeight * Scale) / 2; }
        }

        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (value.Equals(_opacity)) return;
                _opacity = value;
                NotifyOfPropertyChange(() => Opacity);
            }
        }

        public double Scale
        {
            get { return _scale; }
            set
            {
                if (value.Equals(_scale)) return;
                _scale = value;

                NotifyOfPropertyChange(() => Scale);
            }
        }

        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                if (value == _zIndex) return;
                _zIndex = value;
                NotifyOfPropertyChange(() => ZIndex);
            }
        }

        public double NewOpacity
        {
            get { return _newOpacity; }
            set
            {
                if (value.Equals(_newOpacity)) return;
                _newOpacity = value;
                NotifyOfPropertyChange(() => NewOpacity);
            }
        }

        public double NewScale
        {
            get { return _newScale; }
            set
            {
                if (value.Equals(_newScale)) return;
                _newScale = value;
                NotifyOfPropertyChange(() => NewScale);
            }
        }

        public int NewZIndex
        {
            get { return _newZIndex; }
            set
            {
                if (value == _newZIndex) return;
                _newZIndex = value;
                NotifyOfPropertyChange(() => NewZIndex);
            }
        }

        public double NewCenterX
        {
            get { return _newCenterX; }
            set
            {
                if (value.Equals(_newCenterX)) return;
                _newCenterX = value;
                NotifyOfPropertyChange(() => NewCenterX);
            }
        }

        public double NewCenterY
        {
            get { return _newCenterY; }
            set
            {
                if (value.Equals(_newCenterY)) return;
                _newCenterY = value;
                NotifyOfPropertyChange(() => NewCenterY);
            }
        }

        public double Width
        {
            get { return Scale * _originalWidth; }
        }

        public double Height
        {
            get { return Scale * _originalHeight; }
        }

        public string TriggerAnimation
        {
            get { return _triggerAnimation; }
            set
            {
                if (value == _triggerAnimation) return;
                _triggerAnimation = value;
                NotifyOfPropertyChange(() => TriggerAnimation);
            }
        }

        public bool IsCurrent
        {
            get { return _isCurrent; }
            set
            {
                if (value.Equals(_isCurrent)) return;
                _isCurrent = value;
                NotifyOfPropertyChange(() => IsCurrent);
            }
        }

        public int Year
        {
            get { return _movie.Year; }
            set
            {
                if (value.Equals(_movie.Year)) return;
                _movie.Year = value;
                NotifyOfPropertyChange(() => Year);
            }
        }

        public string Country { get { return _movie.Country; } }

        public string FileName { get { return _movie.FileName; } }

        public string PosterName { get { return _movie.PosterName; } }

        public string Details { get { return _movie.Details; } }

        public ImageSource Poster
        {
            get { return _movie.Poster; }
        }

        #endregion

        #region Binding Methods

        public void Play()
        {
            _eventAggregator.Publish(_movie);
        }

        public void ShowDetails(MovieView movieView)
        {
            StartScaleTransform(movieView.MovieBack, 1);
            StartScaleTransform(movieView.MovieFront, 0);
            StartScaleTransform(movieView.MovieBorder, 0);
        }

        public void Back(MovieView movieView)
        {
            StartScaleTransform(movieView.MovieFront, 1);
            StartScaleTransform(movieView.MovieBorder, 1);
            StartScaleTransform(movieView.MovieBack, 0);
        }

        #endregion

        private void StartScaleTransform(Border grid, int targetScale)
        {
            grid.SetValue(Canvas.ZIndexProperty, targetScale);
            var trans = new ScaleTransform();
            grid.RenderTransformOrigin = new Point(.5, .5);
            grid.RenderTransform = trans;

            var anim = new DoubleAnimation(1 - targetScale, targetScale, TimeSpan.FromMilliseconds(AnimationMilliseconds));
            trans.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            trans.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        public override bool Equals(object obj)
        {
            var movie = obj as MovieViewModel;
            return movie != null && string.Equals(Name, movie.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public void AnimateOpacity()
        {
            NewOpacity = 0.0;
        }
    }
}