using Catalog.Domain.Entities;
using Catalog.Domain.Requests.Item;
using Catalog.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using System;
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
        [LoadData("item")]
        public async Task Get_by_id_should_return_item_data(Item request)
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
    }
}
