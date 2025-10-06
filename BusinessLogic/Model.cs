using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public record Token(string token);
    public record User(string Username, string Password);
    public record Media(int Id, string Title, string Description, DateTime ReleaseDate, int Fsk, List<Genre> Genres, MediaType MediaType);
    public record Genre(int Id, string Name);
    public record MediaType(int Id, string Name);
    public record Review(int Id, User User, Media Media, string Comment, int Rating);
    public record Favourite(int UserId, int MediaId);
}
