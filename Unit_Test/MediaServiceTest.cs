using BusinessLogic;
using BusinessLogic.Models;
using DataAccess;
using DataAccess.Entities;

namespace Unit_Test
{
    public class MediaServiceTest
    {
        private MediaService _mediaService;
        [SetUp]
        public void Setup()
        {
            var mockDAL = new MockDAL();
            var mockBL = new MockBL();
            var userservice = mockBL.UserService;

            _mediaService = new MediaService(mockDAL, userservice);
        }

        [Test]
        public async Task Find_Media_With_Existing_Id()
        {
            int existingId = 1;
            var result = await _mediaService.FindMediaById(existingId);
            Assert.That(result.Value != null);
            Assert.That(result.Value.Id == existingId);
            Assert.That(result.Response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Dont_Find_Media_With_Non_Existing_Id()
        {
            int nonExistingId = 4;
            var result = await _mediaService.FindMediaById(nonExistingId);
            Assert.That(result.Value == null);
            Assert.That(result.Response.ResponseCode == BL_Response.NotFound);
        }
        [Test]
        public async Task Delete_Media_Successfully_With_Existing_Id_And_Valid_Token()
        {
            int existingId = 1;
            string validToken = "token";
            var response = await _mediaService.DeleteMediaById(validToken, existingId);
            Assert.That(response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Dont_Delete_Media_With_Existing_Id_But_Invalid_Token()
        {
            int existingId = 1;
            string invalidToken = "autherizationToken";
            var response = await _mediaService.DeleteMediaById(invalidToken, existingId);
            Assert.That(response.ResponseCode == BL_Response.AuthenticationFailed);
        }

        [Test]
        public async Task Dont_Delete_Media_With_Non_Existing_Id_And_Invalid_Token()
        {
            int nonExistingId = 3;
            string invalidToken = "autherizationToken";
            var response = await _mediaService.DeleteMediaById(invalidToken, nonExistingId);
            Assert.That(response.ResponseCode == BL_Response.AuthenticationFailed);
        }

        [Test]
        public async Task Dont_Delete_Media_With_Non_Existing_Id_But_Invalid_Token()
        {
            int nonExistingId = 3;
            string validToken = "token";
            var response = await _mediaService.DeleteMediaById(validToken, nonExistingId);
            Assert.That(response.ResponseCode == BL_Response.NotFound);
        }
        [Test]
        public async Task Get_One_Element_With_Empty_Media_Filter()
        {
            var result = await _mediaService.GetMedia(null);
            Assert.That(result.Value != null);
            Assert.That(result.Value.Count == 1);
            Assert.That(result.Value.First().Id == 1);
            Assert.That(result.Response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Get_One_Element_With_A_Media_Filter()
        {
            MediaFilter filter = new MediaFilter("Test", null, 2000, "comedy", 0, 0, null);
            var result = await _mediaService.GetMedia(null);
            Assert.That(result.Value != null);
            Assert.That(result.Value.Count == 1);
            Assert.That(result.Value.First().Id == 1);
            Assert.That(result.Response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Add_A_Media_With_Valid_Token_And_Valid_Media()
        {
            PostMedia media = new PostMedia("TestNewMedia", null, null, null, new List<string>(), "Series");
            string validToken = "token";
            var result = await _mediaService.PostMedia(validToken, media);
            Assert.That(result.Value != null);
            Assert.That(result.Response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Dont_Add_A_Media_With_Valid_Token_But_Invalid_Media()
        {
            PostMedia media = new PostMedia("TestNewMedia", null, null, null, new List<string>(), "Serises");
            string validToken = "token";
            var result = await _mediaService.PostMedia(validToken, media);
            Assert.That(result.Value == null);
            Assert.That(result.Response.ResponseCode == BL_Response.BadParameters);
        }
        [Test]
        public async Task Dont_Add_A_Media_With_Invalid_Token_But_Valid_Media()
        {
            PostMedia media = new PostMedia("TestNewMedia", null, null, null, new List<string>(), "Movie");
            string invalidToken = "tokenasdasdhuof";
            var result = await _mediaService.PostMedia(invalidToken, media);
            Assert.That(result.Value == null);
            Assert.That(result.Response.ResponseCode == BL_Response.AuthenticationFailed);
        }
        [Test]
        public async Task Dont_Add_A_Media_With_Invalid_Token_And_Invalid_Media()
        {
            PostMedia media = new PostMedia("TestNewMedia", null, null, null, new List<string>(), "Serises");
            string invalidToken = "tosken";
            var result = await _mediaService.PostMedia(invalidToken, media);
            Assert.That(result.Value == null);
            Assert.That(result.Response.ResponseCode == BL_Response.AuthenticationFailed);
        }
        [Test]
        public async Task Update_Media_With_Valid_Token_Of_Media_Owner_And_Existing_Id()
        {
            PutMedia media = new PutMedia(1,"TestNewMedia", null, null, null, new List<string>(), null);
            string validTokenOfOwner = "token";
            var response = await _mediaService.PutMedia(validTokenOfOwner, media);
            Assert.That(response.ResponseCode == BL_Response.OK);
        }
        [Test]
        public async Task Update_Media_With_Valid_Token_Of_Media_Owner_But_Not_Existing_Id()
        {
            PutMedia media = new PutMedia(4, "TestNewMedia", null, null, null, new List<string>(), null);
            string validTokenOfOwner = "token";
            var response = await _mediaService.PutMedia(validTokenOfOwner, media);
            Assert.That(response.ResponseCode == BL_Response.NotFound);
        }
        [Test]
        public async Task Update_Media_With_Valid_Token_Of_Someone_Not_The_Media_Owner_And_Existing_Id()
        {
            PutMedia media = new PutMedia(1, "TestNewMedia", null, null, null, new List<string>(), null);
            string validTokenOfSomeoneElse = "token2";
            var response = await _mediaService.PutMedia(validTokenOfSomeoneElse, media);
            Assert.That(response.ResponseCode == BL_Response.Unauthorized);
        }
        [Test]
        public async Task Update_Media_With_Valid_Token_Of_Someone_Not_The_Media_Owner_And_Not_Existing_Id()
        {
            PutMedia media = new PutMedia(4, "TestNewMedia", null, null, null, new List<string>(), null);
            string validTokenOfSomeoneElse = "token2";
            var response = await _mediaService.PutMedia(validTokenOfSomeoneElse, media);
            Assert.That(response.ResponseCode == BL_Response.NotFound);
        }
        [Test]
        public async Task Update_Media_With_Invalid_Token_But_Existing_Id()
        {
            PutMedia media = new PutMedia(1, "TestNewMedia", null, null, null, new List<string>(), null);
            string validTokenOfSomeoneElse = "token21asad";
            var response = await _mediaService.PutMedia(validTokenOfSomeoneElse, media);
            Assert.That(response.ResponseCode == BL_Response.AuthenticationFailed);
        }
    }
}