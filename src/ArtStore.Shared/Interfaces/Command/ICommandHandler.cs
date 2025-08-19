namespace ArtStore.Shared.Interfaces.Command;

public interface ICommand<out TResult>
{
}

public interface ICommand : ICommand<IResult<object?>>
{
}

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}

// This version now supports any IResult<T> where T can be any type
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, IResult<object?>> where TCommand : ICommand
{
}