using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public record UserEntity(string Username, string Password, string? Email, string? FavouriteGenre);
    public record MediaEntity(int Id, string? Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> Genres, string? MediaType, double? AverageStars);
    public record RatingEntity(int Id, string Username, int MediaId, string? Comment, int? Rating);
    public record FavouriteEntity(string Username, int MediaId);
    public record UpdateMedia(int Id, string? Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string>? Genres, string? MediaType);
    public record AddMedia(string Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> GenreNames, string? MediaType);
    public record AddRating(int MediaId, string Username, string? Comment, int? Stars);
    public record UpdateRating(int Id,string? Comment, int? Stars);
    public record MediaFilterDAL(string? Title, string? MediaType, int? ReleaseYear, string? Genre, int? Fsk, int? MinStars, string? SortBy);
    
}
