using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public record LoginDTO(string Username, string Password);
    public record MediaDTO(int Id, string? Title, string? Description, decimal? AverageRating, DateOnly? ReleaseDate, int? Fsk, List<string> Genres, string? MediaType, string CreatorName);
    public record RatingDTO(int Id, string Username, RatingMediaDTO Media, string? Comment, int? Stars, DateTime? Published);
    public record AddRatingDTO(string? Comment, int? Stars);
    public record RatingMediaDTO(int Id, string Medianame);
    public record FavouriteDTO(string Username, FavouriteMediaDTO Media);
    public record FavouriteMediaDTO(int Id, string? Title, DateOnly? ReleaseDate, string? MediaType);
    public record ProfileDTO(string? Email, string? FavouriteGenre);
    public record PostMediaDTO(string Title, string? Description, int? ReleaseYear, List<string>? Genres, int? AgeRestriction, string MediaType);
    public record UserRankDTO(int Placement, int Score, string Username);
    public record RecommendationDTO(MediaDTO media, decimal similarityScore);
}
