using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using MockQueryable.EntityFrameworkCore;

namespace LapkaBackend.Application.Tests.Helper;

public abstract class Mocker
{
    internal static Mock<DbSet<T>> MockDbSet<T>(T initialObject) where T : class
    {
        var data = new List<T> { initialObject }.AsQueryable();
        var mock = new Mock<DbSet<T>>();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);

        var asyncData = new List<T> { initialObject };
        var asyncQueryable = asyncData.AsQueryable();
        var asyncProvider = new AsyncQueryProvider<T>(data.AsEnumerable());
        var asyncExpression = asyncQueryable.Expression;

        mock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(asyncData.GetEnumerator()));

        mock.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(asyncProvider);

        mock.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(asyncExpression);

        mock.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(asyncQueryable.ElementType);

        return mock;
    }
}