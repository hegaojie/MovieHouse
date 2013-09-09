using System.Windows.Controls;

namespace MovieHouse
{
    public class MVItemsControl : ItemsControl
    {
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new MovieViewContainer();
        }
    }
}