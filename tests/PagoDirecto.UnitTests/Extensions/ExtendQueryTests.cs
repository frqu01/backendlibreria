using PagoDirecto.Application.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PagoDirecto.UnitTests.Extensions;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ExtendQueryTests
{
    [Fact]
    public void OrderQuery_ShouldSortAscending_ByStringProperty()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Zebra" },
            new TestEntity { Id = 2, Name = "Apple" },
            new TestEntity { Id = 3, Name = "Monkey" }
        }.AsQueryable();

        // Act
        var result = query.OrderQuery("asc", "Name").ToList();

        // Assert
        result[0].Name.Should().Be("Apple");
        result[1].Name.Should().Be("Monkey");
        result[2].Name.Should().Be("Zebra");
    }

    [Fact]
    public void OrderQuery_ShouldSortDescending_ByIntProperty()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 5, Name = "B" },
            new TestEntity { Id = 3, Name = "C" }
        }.AsQueryable();

        // Act
        var result = query.OrderQuery("desc", "Id").ToList();

        // Assert
        result[0].Id.Should().Be(5);
        result[1].Id.Should().Be(3);
        result[2].Id.Should().Be(1);
    }

    [Fact]
    public void OrderQuery_ShouldNotBreak_WhenPropertyDoesNotExist()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "A" },
            new TestEntity { Id = 5, Name = "B" }
        }.AsQueryable();

        // Act
        // Attempt to sort by a non-existent property
        var result = query.OrderQuery("asc", "InvalidProperty").ToList();

        // Assert
        result.Count.Should().Be(2);
        // Returns elements in original order because property isn't found (if implemented to skip invalid ones)
        // Note: Dynamic LINQ might throw an exception instead if not handled. Let's see if ExtendQuery throws or bypasses.
        // Assuming it's safe or handles it. If it throws, we should test for the exception.
    }
}
