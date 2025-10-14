using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public record UserEntity(string Username, string Password, string? Email, int? FavoriteGenreId);
    public record MediaEntity(int Id, string Title, string Description, DateTime ReleaseDate, int Fsk, List<GenreEntity> Genres, MediaTypeEntity MediaType);
    public record GenreEntity(int Id, string Name);
    public record MediaTypeEntity(int Id, string Name);
    public record RatingEntity(int Id, string Username, int MediaId, string Comment, int Rating);
    public record FavouriteEntity(string Username, int MediaId);
    public record UpdateMedia(int Id, string? Title = null, string? Description = null, DateTime? ReleaseDate = null, int? Fsk = null, List<GenreEntity>? Genres = null, MediaTypeEntity? MediaType = null);
    public record AddMedia(string Title, string Description, DateTime ReleaseDate, int Fsk, int[] GenreIds, int MediaTypeId);
    public record AddRating(string Username, int MediaId, string Comment, int Rating);
    public record UpdateRating(int Id,string? Comment, int? Rating);

}
