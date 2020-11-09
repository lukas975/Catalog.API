using Catalog.API.ResponseModels;
using Catalog.Domain.Entities;
using Catalog.Domain.Requests.Item;
using Catalog.Domain.Responses;
using Catalog.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.API.Tests.Controllers
{
    public class ItemControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        private readonly InMemoryApplicationFactory<Startup> _factory;
        
        public ItemControllerTests()
        {
            _factory = new InMemoryApplicationFactory<Startup>();

        }

        [Theory]
        [InlineData("/api/items/")]
        public async Task Get_should_return_success(string url)
        {
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync(url);
            
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("/api/items/?pageSize=1&pageIndex=0", 1, 0)]
        [InlineData("/api/items/?pageSize=2&pageIndex=0", 2, 0)]
        [InlineData("/api/items/?pageSize=1&pageIndex=1", 1, 1)]
        public async Task get_should_return_paginated_data(string url, int pageSize, int pageIndex)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            var responseEntity = JsonConvert.DeserializeObject<PaginatedItemsResponseModel<ItemResponse>>(responseContent);
            responseEntity.PageIndex.ShouldBe(pageIndex);
            responseEntity.PageSize.ShouldBe(pageSize);
            responseEntity.Data.Count().ShouldBe(pageSize);
        }

        [Fact]
        public async Task Get_by_id_should_return_item_data()
        {
            const string id = "86bff4f7-05a7-46b6-ba73-d43e2c45840f";
            
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync($"/api/items/{id}");
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            var responseEntity = JsonConvert.DeserializeObject<Item>(responseContent);

            responseEntity.ShouldNotBeNull();
        }

        [Theory]
        [LoadData("item")]
        public async Task Add_should_create_new_record(AddItemRequest request)
        {
            var client = _factory.CreateClient();
            
            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/items", httpContent);
            
            response.EnsureSuccessStatusCode();
            
            response.Headers.Location.ShouldNotBeNull();
        }

        [Theory]
        [LoadData("item")]
        public async Task Update_should_modify_existing_item(EditItemRequest request)
        {
            var client = _factory.CreateClient();

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/items/{request.Id}", httpContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            
            var responseEntity = JsonConvert.DeserializeObject<Item>(responseContent);
            
            responseEntity.Name.ShouldBe(request.Name);
            responseEntity.Description.ShouldBe(request.Description);
            responseEntity.GenreId.ShouldBe(request.GenreId);
            responseEntity.ArtistId.ShouldBe(request.ArtistId);
        }

        [Theory]
        [LoadData("item")]
        public async Task Delete_should_returns_no_content_when_called_with_right_id(DeleteItemRequest request)
        {
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync($"/api/items/{request.Id}");

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_should_returns_not_found_when_called_with_not_existing_id()
        {
            var client = _factory.CreateClient();

            var response = await client.DeleteAsync($"/api/items/{Guid.NewGuid()}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
