using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRP.model
{
    internal record User(int Id, string Username, string Password);
    internal record Media(int Id, string Title, string Description, DateTime ReleaseDate, int Fsk, List<Genre> Genres, MediaType MediaType);
    internal record Genre(int Id, string Name);
    internal record MediaType(int Id, string Name);
    internal record Review(int Id, User User, Media media, string Comment, int rating);
    internal record Favourite(User User, Media Media);
}
