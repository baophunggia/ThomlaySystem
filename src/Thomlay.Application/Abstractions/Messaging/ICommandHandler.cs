namespace Thomlay.Application.Abstractions.Messaging
{
    // Xử lý các thao tác Đọc (Lấy danh sách vật phẩm, Xem trạng thái đơn)
    public interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}