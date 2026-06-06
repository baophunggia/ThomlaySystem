namespace Thomlay.Application.Abstractions.Messaging
{
    // Xử lý các thao tác Đọc (Lấy danh sách vật phẩm, Xem trạng thái đơn)
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}