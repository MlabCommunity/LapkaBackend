using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using MockQueryable.EntityFrameworkCore;

namespace LapkaBackend.Application.Tests.Helper;

public abstract class Mocker
{
    internal static Mock<DbSet<T>> MockDbSet<T>(ICollection<T> initialObject) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new AsyncQueryProvider<T>(initialObject.AsEnumerable()));
        
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(initialObject.AsQueryable().Expression);
        
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(initialObject.AsQueryable().ElementType);
        
        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(initialObject.GetEnumerator()));
        
        return mockSet;
    }
}