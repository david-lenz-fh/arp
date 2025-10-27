using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public record Token(string token);
    public record User(string Username, string Password, string? Email, string? FavouriteGenre);
    public record Media(int Id, string? Title, string? Description, DateOnly? ReleaseDate, int? Fsk, List<string> Genres, string? MediaType);
    public record Login(string Username, string Password);
    public record MediaType(int Id, string Name);
    public record Review(int Id, User User, Media Media, string? Comment, int? Rating);
    public record Favourite(int UserId, int MediaId);
}
