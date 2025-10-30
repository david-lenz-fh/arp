using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public record UserEntity(string Username, string Password, string? Email, string? FavouriteGenre);
    public record MediaEntity(int Id, string? Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> Genres, string? MediaType, double? AverageRating);
    public record RatingEntity(int Id, string Username, int MediaId, string? Comment, int? Rating);
    public record FavouriteEntity(string Username, int MediaId);
    public record UpdateMedia(int Id, string? Title = null, string? Description = null, DateTime? ReleaseDate = null, int? Fsk = null, List<string>? Genres = null, string? MediaType = null);
    public record AddMedia(string Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> GenreNames, string? MediaType);
    public record AddRating(string Username, int MediaId, string Comment, int Rating);
    public record UpdateRating(int Id,string? Comment, int? Rating);
    public record MediaFilterDAL(string? Title, string? MediaType, int? ReleaseYear, string? Genre, int? Fsk, int? MinRating, string? SortBy);
    
}
