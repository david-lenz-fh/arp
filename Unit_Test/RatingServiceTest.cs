using BusinessLogic;
using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;

namespace Unit_Test
{
    public class RatingServiceTest
    {
        private RatingService _ratingService;
        [SetUp]
        public void Setup()
        {
            var mockDAL = new MockDAL();
            var mockBL = new MockBL();
            var userService = mockBL.UserService;
            var mediaService = mockBL.MediaService;
            _ratingService = new RatingService(mockDAL, mediaService, userService);
        }
        [Test]
        public async Task Find_Rating_With_Existing_Id()
        {
            int existingId = 0;
            var result = await _ratingService.FindRatingById(existingId);
            Assert.That(result.Value != null);
            Assert.That(result.Value != null && result.Value.Id == existingId);
            Assert.That(result.Response.ResponseCode == BL_Response.OK);
        }

        [Test]
        public async Task Dont_Find_Rating_With_Non_Existing_Id()
        {
            int nonExistingId = 1;
            var result = await _ratingService.FindRatingById(nonExistingId);
            Assert.That(result.Value == null);
            Assert.That(result.Response.ResponseCode == BL_Response.NotFound);
        }
        [Test]
        public async Task Confirm_Comment_With_Existing_Id_And_Valid_Token()
        {
            int existingId = 0;
            string validToken = "token";
            var response = await _ratingService.ConfirmComment(validToken, existingId);
            Assert.That(response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Dont_Confirm_Comment_With_Non_Existing_Id_But_Valid_Token_Of_Owner()
        {
            int nonExistingId = 1;
            string ownersToken = "token";
            var response = await _ratingService.ConfirmComment(ownersToken, nonExistingId);
            Assert.That(response.ResponseCode == BL_Response.NotFound);
        }
        [Test]
        public async Task Dont_Confirm_Comment_With_Existing_Id_But_Invalid_Token()
        {
            int existingId = 0;
            string invalidToken = "asdtoken";
            var response = await _ratingService.ConfirmComment(invalidToken, existingId);
            Assert.That(response.ResponseCode == BL_Response.AuthenticationFailed);
        }
        [Test]
        public async Task Dont_Confirm_Comment_With_Existing_Id_And_Valid_Token_Of_Someone_Else_Than_Owner()
        {
            int existingId = 0;
            string someoneElsesToken = "token2";
            var response = await _ratingService.ConfirmComment(someoneElsesToken, existingId);
            Assert.That(response.ResponseCode == BL_Response.Unauthorized);
        }
        [Test]
        public async Task Dont_Confirm_Comment_With_Non_Existing_Id_And_Invalid_Token()
        {
            int nonExistingId = 1;
            string ownersToken = "tokasren";
            var response = await _ratingService.ConfirmComment(ownersToken, nonExistingId);
            Assert.That(response.ResponseCode == BL_Response.AuthenticationFailed);
        }
    }
}
