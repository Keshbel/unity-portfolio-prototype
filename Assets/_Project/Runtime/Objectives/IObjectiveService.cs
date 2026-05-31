using System;
using R3;

namespace ExtractionRoom.Objectives
{
    public interface IObjectiveService : IDisposable
    {
        ObjectiveState CurrentState { get; }

        ReadOnlyReactiveProperty<string> CurrentObjectiveTextObservable { get; }

        ReadOnlyReactiveProperty<ObjectiveProgress> ProgressObservable { get; }
    }
}
