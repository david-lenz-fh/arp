using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public record LoginDTO(string Username, string Password);
    public record MediaDTO(int Id, string Title, string Description, DateTime ReleaseDate, int Fsk, List<GenreDTO> Genres, MediaTypeDTO MediaType);
    public record GenreDTO(int Id, string Name);
    public record MediaTypeDTO(int Id, string Name);
    public record ReviewDTO(int Id, LoginDTO User, MediaDTO Media, string Comment, int Rating);
    public record FavouriteDTO(int UserId, int MediaId);
    public record ProfileDTO(string? Email, string? FavouriteGenre);
}
