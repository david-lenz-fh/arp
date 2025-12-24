using BusinessLogic;
using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;

namespace Unit_Test
{
    public class UserServiceTest
    {
        private UserService _userService;
        [SetUp]
        public void Setup()
        {
            var mockDAL = new MockDAL();

            _userService = new UserService(mockDAL);
        }

        [Test]
        public async Task Find_Existing_Username()
        {
            string existingUsername = "TestUser";
            var result = await _userService.FindUserByName(existingUsername);
            Assert.That(result.Value != null);
            Assert.That(result.Value != null && result.Value.Username == existingUsername);
            Assert.That(result.Response.ResponseCode == BL_Response.OK);
        }

        [Test]
        public async Task Dont_Find_Non_Existing_Username()
        {
            string nonExistingUsername = "David";
            var result = await _userService.FindUserByName(nonExistingUsername);
            Assert.That(result.Value == null);
            Assert.That(result.Response.ResponseCode == BL_Response.NotFound);
        }
    }
}