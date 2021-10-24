using System.ComponentModel;

namespace Crono.Service
{
    public interface IAnimationService
    {
        bool IsLoading { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}