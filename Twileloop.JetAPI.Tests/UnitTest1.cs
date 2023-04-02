namespace Twileloop.JetAPI.Tests {
    public class UnitTest1 {
        [Fact]
        public async Task GET_Simple() {
            var response = await new JetRequest<dynamic>()
                            .ExecuteAsync("https://jsonplaceholder.typicode.com/posts/1");

            Assert.True(response.IsSuccessfull);
        }
    }
}