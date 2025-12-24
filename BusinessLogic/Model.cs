using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public record User(string Username, string Password, string? Email, string? FavouriteGenre);
    public record Profile(string? Email, string? FavouriteGenre);
    public record Media(int Id, string? Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> Genres, string MediaType, decimal? AverageStars, User Creator);
    public record PostMedia(string Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> Genres, string? MediaType);
    public record PutMedia(int Id, string? Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string>? Genres, string? MediaType);
    public record Login(string Username, string Password);
    public record MediaType(int Id, string Name);
    public record Rating(int Id, User User, Media Media, string? Comment, int Stars, DateTime? Timestamp);
    public record PutRating(int RatingId, string? Comment, int? Stars);
    public record PostRating(int MediaId, string? Comment, int Stars);
    public record Favourite(User user, Media media);
    public record MediaFilter(string? Title, string? MediaType, int? ReleaseYear, string? Genre, int? Fsk, int? MinStars, string? SortBy);
    public record UserRank(int Placement, int Score, string Username);
    public record Result<T>(T? Value, ResultResponse Response);
    public record ResultResponse(BL_Response ResponseCode, string? Message);
    public enum BL_Response {
        NotFound,
        InternalError,
        OK,
        AuthenticationFailed,
        Unauthorized,
        BadParameters,
        CorruptedData
    }
}
