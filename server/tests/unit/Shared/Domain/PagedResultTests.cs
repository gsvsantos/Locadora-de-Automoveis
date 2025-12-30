using LocadoraDeAutomoveis.Domain.Shared;

namespace LocadoraDeAutomoveis.Tests.Unit.Shared.Domain;

[TestClass]
[TestCategory("PagedResult Domain - Unit Tests")]
public sealed class PagedResultTests
{
    [TestMethod]
    public void PagedResultConstructor_Default_ShouldInitializeProperties()
    {
        // Arrange & Act
        PagedResult<string> pagedResult = new();

        // Assert
        Assert.IsNotNull(pagedResult.Items);
        Assert.AreEqual(0, pagedResult.Items.Count);
        Assert.AreEqual(0, pagedResult.CurrentPage);
        Assert.AreEqual(0, pagedResult.PageSize);
        Assert.AreEqual(0, pagedResult.TotalCount);
        Assert.AreEqual(0, pagedResult.TotalPages);
    }

    [TestMethod]
    public void PagedResultConstructor_Parameterized_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        List<string> items = ["Item 1", "Item 2"];
        int totalCount = 10;
        int currentPage = 1;
        int pageSize = 5;

        // Act
        PagedResult<string> result = new(items, totalCount, currentPage, pageSize);

        // Assert
        Assert.AreEqual(items, result.Items);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(totalCount, result.TotalCount);
        Assert.AreEqual(currentPage, result.CurrentPage);
        Assert.AreEqual(pageSize, result.PageSize);
    }

    [TestMethod]
    public void PagedResultConstructor_Parameterized_ShouldCalculateTotalPages_WhenExactDivision()
    {
        // Arrange
        List<string> items = [];
        int totalCount = 10;
        int currentPage = 1;
        int pageSize = 5;

        // Act
        PagedResult<string> result = new(items, totalCount, currentPage, pageSize);

        // Assert
        Assert.AreEqual(2, result.TotalPages);
    }

    [TestMethod]
    public void PagedResultConstructor_Parameterized_ShouldCalculateTotalPages_WhenPartialDivision()
    {
        // Arrange
        List<string> items = [];
        int totalCount = 10;
        int currentPage = 1;
        int pageSize = 3;

        // Act
        PagedResult<string> result = new(items, totalCount, currentPage, pageSize);

        // Assert
        Assert.AreEqual(4, result.TotalPages);
    }

    [TestMethod]
    public void PagedResultConstructor_Parameterized_ShouldCalculateTotalPages_WhenTotalCountIsZero()
    {
        // Arrange
        List<string> items = [];
        int totalCount = 0;
        int currentPage = 1;
        int pageSize = 10;

        // Act
        PagedResult<string> result = new(items, totalCount, currentPage, pageSize);

        // Assert
        Assert.AreEqual(0, result.TotalPages);
    }

    [TestMethod]
    public void PagedResultConstructor_Parameterized_ShouldCalculateTotalPages_WhenTotalCountIsLessThanPageSize()
    {
        // Arrange
        List<string> items = [];
        int totalCount = 5;
        int currentPage = 1;
        int pageSize = 10;

        // Act
        PagedResult<string> result = new(items, totalCount, currentPage, pageSize);

        // Assert
        Assert.AreEqual(1, result.TotalPages);
    }
}