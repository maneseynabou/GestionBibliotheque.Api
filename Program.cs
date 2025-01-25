using Gestion_de_Bibliothéque;
using Gestion_de_Bibliothéque.DTOs;
using AutoMapper;
using Gestion_de_Bibliothéque.datas;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Configuration du context de la base de donnee
var ConnexionString = builder.Configuration.GetConnectionString("Bibliotheque");
builder.Services.AddSqlite<BookDbContext>(ConnexionString);

builder.Services.AddAutoMapper(typeof(Program)); 
builder.Services.AddControllersWithViews();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => "Hello World!"); 

#region CREATION DE LISTE DE LIVRES ET LES RECUPERER
var books = new List<BookDTOs>
{
    new BookDTOs
    {
        Id = 1,
        Title = "sous l'orage", 
        Author = "Mariama Ba", 
        PubDate = new DateOnly(2000,10,29)
    },
    new BookDTOs 
    { 
        Id = 2, 
        Title = "Une vie de boy", 
        Author = "Ferdinant Oyono", 
        PubDate = new DateOnly(2004,1,7)
    },
     new BookDTOs 
    { 
        Id = 3, 
        Title = "vol de nuit", 
        Author = "Entoine Saint Exprit", 
        PubDate = new DateOnly(2005,1,7)
    }

};
// EndPoint pour recuperer la liste de livres
app.MapGet("/books", async(BookDbContext dbContext) => {
    var book = await dbContext.Books.ToListAsync();
    return Results.Ok(book);
});
#endregion

#region CREATION DE ENDPONIT POUR RECUPERER A PARTIR DE L'ID
//app.MapGet("/books/{id}", (int id) => books.FirstOrDefault(book=>book.Id == id));
app.MapGet("/books/{id}", async(int id, BookDbContext dbContext)=>
{
    try{
        var book = await dbContext.Books!.FindAsync(id);
        return Results.Ok(book);
    }
    catch(Exception ex){
        return Results.NotFound(new {Message = $"Le book avec l'ID {id} n'existe pas !"});
    }
    
    //return book is null ?Results.NotFound(new {Message = $"Le book avec l'ID {id} n'existe pas !"}) : Results.Ok(book);
    //
    
    // var book = books.FirstOrDefault(book => book.Id == id);
    // if(book is null)
    // {
    //     return Results.NotFound(new {Message = $"Le book avec l'ID {id} n'existe pas !"});
    // }
    // return Results.Ok(book);
});
#endregion
                            
#region ENDPOINTE POUR AJOUTER 
// Creation d'un Endponit qui permet d'ajouter des livres
app.MapPost("/books", async (CreateBookDTOs newBookDTOS, IMapper mapper, BookDbContext dbContext) =>
{
    //int newId = books.Any() ? books.Max(book=> book.Id) +1 : 1;
    var newBook = mapper.Map<Book>(newBookDTOS);
    dbContext.Books!.Add(newBook);
    await dbContext.SaveChangesAsync();
    // var book = new CreateBookDTOs
    // {
    //     Title = newBookDTOS.Title,
    //     Author = newBookDTOS.Author,
    //     PubDate = newBookDTOS.PubDate,
    // };
    var bookReponse = mapper.Map<BookDTOs>(newBook);
    return Results.Created($"/book/{newBook.Id}", newBook);
});
#endregion

#region ENDPOINTE POUR MODIFIER

app.MapPut("/books/{id}", async (int id, UpdateBookDTOs newBook, IMapper mapper,  BookDbContext dbContext) =>
{
    //var bookToUpdate = books.FirstOrDefault(b => b.Id == id);
    var bookToUpdate = await dbContext.Books!.FindAsync(id);
    if(bookToUpdate is null)
    {
        return Results.NotFound(new {Message = $"le livre de id {id} n'existe pas !"});
    }
    bookToUpdate.Title = newBook.Title ?? bookToUpdate.Title;
    bookToUpdate.Author = newBook.Author ?? bookToUpdate.Author;
    bookToUpdate.PubDate = newBook.PubDate != default ? newBook.PubDate : bookToUpdate.PubDate;
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});
#endregion

#region ENDPOINTs POUR SUPPRIMER

app.MapDelete("/books/{id}",(int id) =>
{
    var book = books.FirstOrDefault(book => book.Id == id);
    if(book is null)
    {
        return Results.NotFound(new {Message = $"Le book avec l'ID {id} n'existe pas !"});
    }
    books.Remove(book);
    return Results.Ok(book);
});
#endregion

app.Run();













