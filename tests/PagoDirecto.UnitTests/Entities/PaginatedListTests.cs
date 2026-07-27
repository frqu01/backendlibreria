using PagoDirecto.Domain.Entities;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace PagoDirecto.UnitTests.Entities;

public class PaginatedListTests
{
    [Fact]
    public void Constructor_ShouldCalculateTotalPagesCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        int count = 15;
        int pageNumber = 1;
        int pageSize = 10;

        // Act
        var paginatedList = new PaginatedList<string>(items, count, pageNumber, pageSize);

        // Assert
        paginatedList.TotalPages.Should().Be(2);
    }

    [Fact]
    public void HasNextPage_ShouldBeTrue_WhenNotOnLastPage()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var paginatedList = new PaginatedList<string>(items, 5, 1, 2); // 5 items, page 1, size 2 -> 3 pages

        // Act & Assert
        paginatedList.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_ShouldBeFalse_WhenOnLastPage()
    {
        // Arrange
        var items = new List<string> { "item5" };
        var paginatedList = new PaginatedList<string>(items, 5, 3, 2); // 5 items, page 3, size 2 -> 3 pages

        // Act & Assert
        paginatedList.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_ShouldBeTrue_WhenNotOnFirstPage()
    {
        // Arrange
        var items = new List<string> { "item3" };
        var paginatedList = new PaginatedList<string>(items, 5, 2, 2);

        // Act & Assert
        paginatedList.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasPreviousPage_ShouldBeFalse_WhenOnFirstPage()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var paginatedList = new PaginatedList<string>(items, 5, 1, 2);

        // Act & Assert
        paginatedList.HasPreviousPage.Should().BeFalse();
    }
}
